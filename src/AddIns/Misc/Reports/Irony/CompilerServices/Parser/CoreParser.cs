#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using Irony.Diagnostics;


namespace Irony.CompilerServices {
  // CoreParser class implements NLALR parser automaton. Its behavior is controlled by the state transition graph
  // with root in Data.InitialState. Each state contains a dictionary of parser actions indexed by input 
  // element (terminal or non-terminal). 
  public partial class CoreParser {

    #region Constructors
    public CoreParser(ParserData parserData, Scanner scanner) {
      Data = parserData;
      _grammar = parserData.Grammar;
      _scanner = scanner;
      
    }
    #endregion

    #region Properties and fields: _grammar, Data, Stack, _context, Input, CurrentState, LineCount, TokenCount
    Grammar _grammar;
    CompilerContext _context;
    public readonly ParserData Data;
    public readonly ParserStack Stack = new ParserStack();
    readonly ParserStack InputStack = new ParserStack();
    Scanner _scanner;
    bool _traceOn;
    
    //"current" stuff
    public ParserState CurrentState {
      get { return _currentState; }
    } ParserState _currentState;

    public ParseTreeNode CurrentInput {
      get { return _currentInput; }
    }  ParseTreeNode _currentInput;

    private ParserTraceEntry _currentTraceEntry; 
    #endregion

    #region Parse method
    public void Parse(CompilerContext context) {
      _context = context;
      _traceOn = _context.OptionIsSet(CompilerOptions.TraceParser);
      _currentInput = null;
      InputStack.Clear();
      Stack.Clear();
      _currentState = Data.InitialState; //set the current state to InitialState
      Stack.Push(new ParseTreeNode(Data.InitialState));
      //main loop
      while (ExecuteAction()) {}
    }//Parse
    #endregion

    #region reading input
    private void ReadInput() {
      if (InputStack.Count > 0)
        InputStack.Pop(); 
      if (InputStack.Count == 0)
        FetchToken();
      _currentInput = InputStack.Top;
    }

    private void FetchToken() {
      Token token;
      do {
        token = _scanner.GetToken();
      } while (token.Terminal.IsSet(TermOptions.IsNonGrammar) && token.Terminal != _grammar.Eof);  
      _currentInput = new ParseTreeNode(token);
      InputStack.Push(_currentInput);
      if (_currentInput.IsError)
        TryRecover(); 
    }
    #endregion

    #region execute actions
    private bool ExecuteAction() {
      if (_currentInput == null)
        ReadInput();
      //Trace current state if tracing is on
      if (_traceOn)
        _currentTraceEntry = _context.AddParserTrace(_currentState, Stack.Top, _currentInput);
      //Try getting action
      ParserAction action = GetAction();
      if (action == null) {
          ReportParseError();
          return TryRecover();
      }
      //write trace
      if (_currentTraceEntry != null)
        _currentTraceEntry.SetDetails(action.ToString(), _currentState);
      //Execute it
      switch (action.ActionType) {
        case ParserActionType.Shift: ExecuteShift(action.NewState); break;
        case ParserActionType.Operator: ExecuteOperatorAction(action.NewState, action.ReduceProduction); break;
        case ParserActionType.Reduce: ExecuteReduce(action.ReduceProduction); break;
        case ParserActionType.Code: ExecuteConflictAction (action); break;
        case ParserActionType.Jump: ExecuteNonCanonicalJump(action); break;
        case ParserActionType.Accept: ExecuteAccept(action); return false; 
      }
      //add info to trace
      return true; 
    }

    private ParserAction GetAction() {
      if (_currentState.DefaultReduceAction != null)
        return _currentState.DefaultReduceAction;
      ParserAction action;
      //First try as Symbol; 
      Token inputToken = _currentInput.Token;
      if (inputToken != null && inputToken.AsSymbol != null) {
        var asSym = inputToken.AsSymbol;
        if (CurrentState.Actions.TryGetValue(asSym, out action)) {
          #region comments
          // Ok, we found match as a symbol
          // Backpatch the token's term. For example in most cases keywords would be recognized as Identifiers by Scanner.
          // Identifier would also check with SymbolTerms table and set AsSymbol field to SymbolTerminal if there exist
          // one for token content. So we first find action by Symbol if there is one; if we find action, then we 
          // patch token's main terminal to AsSymbol value.  This is important for recognizing keywords (for colorizing), 
          // and for operator precedence algorithm to work when grammar uses operators like "AND", "OR", etc. 
          //TODO: This is not quite correct action, and we can run into trouble with some languages that have keywords that 
          // are not reserved words. But proper implementation would require substantial addition to parser code: 
          // when running into errors, we need to check the stack for places where we made this "interpret as Symbol"
          // decision, roll back the stack and try to reinterpret as identifier
          #endregion
          inputToken.SetTerminal(asSym);
          _currentInput.Term = asSym;
          _currentInput.Precedence = asSym.Precedence;
          _currentInput.Associativity = asSym.Associativity;
          return action;
        }
      }

      //Try to get by main Terminal, only if it is not the same as symbol
      if (_currentState.Actions.TryGetValue(_currentInput.Term, out action))
        return action;
      //for non-canonical methods, we may encounter reduced lookaheads while the state does not expect it.
      // the reduced lookahead was "created" for other state with canonical conflict, helped resolved it, 
      // but remained on the stack, and now gets as lookahead state that does not expect it. In this case,
      // if we don't find action by reduced term, we should retry it by first child, recursively
      if (Data.ParseMethod != ParseMethod.Lalr) {
        action = GetActionFromChildRec(_currentInput);
        if (action != null)
          return action;
      }
      //Return JumpAction or null if it is not defined
      return _currentState.JumpAction;
    }

    //For NLALR, when non-canonical lookahead had been already reduced, the action for the input in some state
    // might be still for its child term.
    private ParserAction GetActionFromChildRec(ParseTreeNode input) {
      var firstChild = input.FirstChild;
      if (firstChild == null) return null;
      ParserAction action;
      if (_currentState.Actions.TryGetValue(firstChild.Term, out action)) {
        if (action.ActionType == ParserActionType.Reduce) //it applies only to reduce actions
          return action;
      }
      action = GetActionFromChildRec(firstChild);
      return action; 
    }

    private void ExecuteShift(ParserState newState) {
      Stack.Push(_currentInput, newState);
      _currentState = newState;
      if (_traceOn) SetTraceDetails("Shift", _currentState); 
      ReadInput();
    }

    #region ExecuteReduce
    private void ExecuteReduce(Production reduceProduction) {
      var newNode = CreateParseTreeNodeForReduce(reduceProduction);
      //Prepare switching to the new state. First read the state from top of the stack 
      _currentState = Stack.Top.State;
      //write to trace
      if (_traceOn)
        SetTraceDetails("Reduce on '" + reduceProduction.ToString() + "'", _currentState);
      // Shift to new state (LALR) or push new node into input stack(NLALR, NLALRT)
      if (Data.ParseMethod == ParseMethod.Lalr) {
        //execute shift over non-terminal
        var action = _currentState.Actions[reduceProduction.LValue];
        Stack.Push(newNode, action.NewState);
        _currentState = action.NewState;
      } else {
        //NLALR - push it back into input stack
        InputStack.Push(newNode);
        _currentInput = newNode;
      }
    }
    
    private ParseTreeNode CreateParseTreeNodeForReduce(Production reduceProduction) {
      int childCount = reduceProduction.RValues.Count;
      ParseTreeNode newNode; 
      //Check if it is an existing list node
      if (CheckReducingExistingList(reduceProduction, out newNode)) 
        return newNode; 
      //"normal" node
      newNode = new ParseTreeNode(reduceProduction);
      newNode.Span = ComputeNewNodeSpan(childCount);
      if (childCount == 0) 
        return newNode; 
      newNode.FirstChild = Stack[Stack.Count - childCount]; //remember first child; it might be thrown away later if it's punctuation
      //copy precedence field if there's one child only
      if (childCount == 1 && newNode.FirstChild.Precedence != BnfTerm.NoPrecedence) {
        newNode.Precedence = newNode.FirstChild.Precedence;
        newNode.Associativity = newNode.FirstChild.Associativity;
      }
      //Pop child nodes one-by-one
      foreach(var rvalue in reduceProduction.RValues) {
        PopChildNode(newNode, ListAddMode.Start); 
      }//for i
      return newNode;
    }
    
    private SourceSpan ComputeNewNodeSpan(int childCount) {
      if (childCount == 0)
        return new SourceSpan(_currentInput.Span.Start, 0);
      var first = Stack[Stack.Count - childCount];
      var last = Stack.Top;
      return new SourceSpan(first.Span.Start, last.Span.EndPos - first.Span.Start.Position);
    }

    private bool CheckReducingExistingList(Production reduceProduction, out ParseTreeNode listNode) {
      listNode = null;
      var childCount = reduceProduction.RValues.Count;
      bool isExistingList = childCount > 0 && reduceProduction.LValue.IsSet(TermOptions.IsList) &&
          reduceProduction.RValues[0] == reduceProduction.LValue;
      if (!isExistingList) return false;
      listNode = Stack[Stack.Count - childCount]; //get the list already created - it is first child node
      listNode.Span = ComputeNewNodeSpan(childCount);
      PopChildNode(listNode, ListAddMode.End); //new list element sits on top, so add it to the end of child list
      //now pop the rest one or two nodes
      var poppedNode = Stack.Pop();
      while (poppedNode != listNode)
        poppedNode = Stack.Pop();
      return true; 
    }

    // 
    private void PopChildNode(ParseTreeNode addToParent, ListAddMode mode) {
      var poppedNode = Stack.Pop();
      poppedNode.State = null; //clear the State field, we need only when node is in the stack
      if (poppedNode.Term.IsSet(TermOptions.IsPunctuation)) return;
      if (poppedNode.Term.IsSet(TermOptions.IsTransient)) {
        addToParent.ChildNodes.Add(poppedNode.ChildNodes, mode);
      } else {
        //TODO: make it possible to create AST nodes for terminals (Number, StringLiteral).
        if (_grammar.FlagIsSet(LanguageFlags.CreateAst))
          SafeCreateAstNode(poppedNode);
        addToParent.ChildNodes.Add(poppedNode, mode);
      }
    }
    
    private void SafeCreateAstNode(ParseTreeNode parseNode) {
      try {
        _grammar.CreateAstNode(_context, parseNode);
        if (parseNode.AstNode != null && parseNode.Term != null)
          parseNode.Term.OnNodeCreated(parseNode);
      } catch (Exception ex) {
        _context.ReportError(parseNode.Span.Start, "Failed to create AST node for non-terminal [{0}], error: " + ex.Message, parseNode.Term.Name); 
      }
    }
    #endregion

    private void ExecuteConflictAction(ParserAction action) {
      var args = new ConflictResolutionArgs(_context, action);
      _grammar.OnResolvingConflict(args);
      switch(args.Result) {
        case ParserActionType.Reduce:
          ExecuteReduce(args.ReduceProduction);
          break; 
        case ParserActionType.Operator:
          ExecuteOperatorAction(action.NewState, args.ReduceProduction);
          break;
        case ParserActionType.Shift:
        default:
          ExecuteShift(action.NewState); 
          break; 
      }
    }


    private void ExecuteNonCanonicalJump(ParserAction action) {
      _currentState = action.NewState;
    }
    private void ExecuteAccept(ParserAction action) {
      //AST nodes are created when we pop them from the stack to add to parent's child list
      // for top node we do it here
      var rootNode = Stack.Pop();
      if (_grammar.FlagIsSet(LanguageFlags.CreateAst))
        SafeCreateAstNode(rootNode);
      _context.CurrentParseTree.Root = rootNode; 
    }

    private void ExecuteOperatorAction(ParserState newShiftState, Production reduceProduction) {
      var realActionType = GetActionTypeForOperation();
      switch (realActionType) {
        case ParserActionType.Shift: ExecuteShift(newShiftState); break;
        case ParserActionType.Reduce: ExecuteReduce(reduceProduction); break; 
      }//switch
      if (_currentTraceEntry != null) {
        _currentTraceEntry.Message = "(Operator)" + _currentTraceEntry.Message;
      }

    }

    private ParserActionType GetActionTypeForOperation() {
      for (int i = Stack.Count - 1; i >= 0; i--) {
        var  prev = Stack[i];
        if (prev == null) continue; 
        if (prev.Precedence == BnfTerm.NoPrecedence) continue;
        ParserActionType result;
        //if previous operator has the same precedence then use associativity
        if (prev.Precedence == _currentInput.Precedence)
          result = _currentInput.Associativity == Associativity.Left ? ParserActionType.Reduce : ParserActionType.Shift;
        else
          result = prev.Precedence > _currentInput.Precedence ? ParserActionType.Reduce : ParserActionType.Shift;
        return result;
      }
      //If no operators found on the stack, do simple shift
      return ParserActionType.Shift;
    }
    #endregion

    #region Tracing
    private void AddTraceEntry(string message, ParserState newState) {
      if (!_traceOn) return;
      AddTraceEntry();
      SetTraceDetails(message, newState); 
    }
    private void AddTraceEntry() {
      if (!_traceOn) return; 
      _currentTraceEntry = _context.AddParserTrace(_currentState, Stack.Top, _currentInput);
    }
    private void SetTraceDetails(string message, ParserState newState) {
      if (_currentTraceEntry != null)
        _currentTraceEntry.SetDetails(message, newState); 
    }

    #endregion 
  }//class



}//namespace
