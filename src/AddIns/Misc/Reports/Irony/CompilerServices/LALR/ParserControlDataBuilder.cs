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

namespace Irony.Compiler.Lalr {
  
  // This class contains all complex logic of constructing LALR parser tables and other control information
  // from the language grammar. 
  // Warning: unlike other classes in this project, understanding what's going on here requires some knowledge of 
  // LR/LALR parsing algorithms. For this I refer you to the Dragon book or any other book on compiler/parser construction.
  public class ParserControlDataBuilder {
    class ShiftTable : Dictionary<string, LR0ItemList> { }
    private ParserStateTable _stateHash;
    public readonly ParserControlData Data;
    Grammar _grammar;
    private int _itemID; //used to assign unique IDs to LR0Items


    public ParserControlDataBuilder(Grammar grammar) {
      _grammar = grammar;
      Data = new ParserControlData(grammar);
      Data.Grammar = _grammar;
    }
    public ParserControlData Build() {
      try {
        if (_grammar.Root == null) 
          Cancel("Root property of the grammar is not set.");
        if (!_grammar.Initialized)
          _grammar.Init();

        //Create the augmented root for the grammar
        CreateAugmentedRootForGrammar();
        //Create productions and LR0Items 
        CreateProductions();
        //Calculate nullability, Firsts and TailFirsts collections of all non-terminals
        CalculateNullability();
        CalculateFirsts();
        CalculateTailFirsts();
        //Create parser states list, including initial and final states 
        CreateParserStates();
        //Propagate Lookaheads
        PropagateLookaheads();
        //Debug.WriteLine("Time of PropagateLookaheads: " + time);
        //now run through all states and create Reduce actions
        CreateReduceActions();
        //finally check for conflicts and detect Operator-based actions
        CheckActionConflicts();
        //Validate
        ValidateAll();
      } catch (GrammarErrorException e) {
        _grammar.Errors.Add(e.Message);
        Data.AnalysisCanceled = true; 
      }
      return Data;
    }//method

    private void Cancel(string msg) {
      if (msg == null) msg = "Grammar analysis canceled.";
      throw new GrammarErrorException(msg);
    }

    private void CreateAugmentedRootForGrammar() {
      Data.AugmentedRoot = new NonTerminal(_grammar.Root.Name + "'", new BnfExpression(_grammar.Root));
      _grammar.NonTerminals.Add(Data.AugmentedRoot);
    }

    #region Creating Productions
    private void CreateProductions() {
      //each LR0Item gets its unique ID, last assigned (max) Id is kept in static field
      _itemID = 0;
      foreach(NonTerminal nt in _grammar.NonTerminals) {
        nt.Productions.Clear();
        //Get data (sequences) from both Rule and ErrorRule
        BnfExpressionData allData = new BnfExpressionData();
        allData.AddRange(nt.Rule.Data);
        if (nt.ErrorRule != null) 
          allData.AddRange(nt.ErrorRule.Data);
        //actually create productions for each sequence
        foreach (BnfTermList prodOperands in allData) {
          Production prod = CreateProduction(nt, prodOperands);
          //Add the production to non-terminal's list and to global list 
          nt.Productions.Add(prod);
        }//foreach prodOperands
      }
    }
    private Production CreateProduction(NonTerminal lvalue, BnfTermList operands) {
      Production prod = new Production(lvalue);
      //create RValues list skipping Empty terminal and collecting grammar hints
      foreach (BnfTerm operand in operands) {
        if (operand == Grammar.Empty) 
          continue;
        //Collect hints as we go - they will be added to the next non-hint element
        GrammarHint hint = operand as GrammarHint;
        if (hint != null) {
          hint.Position = prod.RValues.Count;
          prod.Hints.Add(hint);
          continue;
        }
        //Check if it is a Terminal or Error element
        Terminal t = operand as Terminal;
        if (t != null) {
          prod.Flags |= ProductionFlags.HasTerminals;
          if (t.Category == TokenCategory.Error) prod.Flags |= ProductionFlags.IsError; 
        }
        //Add the operand info and LR0 Item
        _itemID++;
        LR0Item item = new LR0Item(prod, prod.RValues.Count, _itemID);
        prod.LR0Items.Add(item);
        prod.RValues.Add(operand);
      }//foreach operand
      //set the flags
      if (lvalue == Data.AugmentedRoot)
        prod.Flags |= ProductionFlags.IsInitial;
      if (prod.RValues.Count == 0)
        prod.Flags |= ProductionFlags.IsEmpty;
      //Add final LRItem
      _itemID++;
      prod.LR0Items.Add(new LR0Item(prod, prod.RValues.Count, _itemID));
      return prod; 
    }
    #endregion

    #region Nullability calculation
    private void CalculateNullability() {
      NonTerminalList undecided = _grammar.NonTerminals;
      while(undecided.Count > 0) {
        NonTerminalList newUndecided = new NonTerminalList();
        foreach(NonTerminal nt in undecided)
          if (!CalculateNullability(nt, undecided))
           newUndecided.Add(nt);
        if (undecided.Count == newUndecided.Count) return;  //we didn't decide on any new, so we're done
        undecided = newUndecided;
      }//while
    }

    private bool CalculateNullability(NonTerminal nonTerminal, NonTerminalList undecided) {
      foreach (Production prod in nonTerminal.Productions) {
        //If production has terminals, it is not nullable and cannot contribute to nullability
        if (prod.IsSet(ProductionFlags.HasTerminals))   continue;
        if (prod.IsSet(ProductionFlags.IsEmpty)) {
          nonTerminal.Nullable = true;
          return true; //Nullable
        }//if 
        //Go thru all elements of production and check nullability
        bool allNullable = true;
        foreach (BnfTerm  child in prod.RValues) {
          NonTerminal childNt = child as NonTerminal;
          if (childNt != null)
            allNullable &= childNt.Nullable;
        }//foreach nt
        if (allNullable) {
          nonTerminal.Nullable = true;
          return true;
        }
      }//foreach prod
      return false; //cannot decide
    }
    #endregion

    #region Calculating Firsts
    private void CalculateFirsts() {
      //1. Calculate PropagateTo lists and put initial terminals into Firsts lists
      foreach (NonTerminal nt in _grammar.NonTerminals) {
        foreach (Production prod in nt.Productions) {
          //NtData lvData = prod.LValue.ParserData as NtData;
          foreach (BnfTerm term in prod.RValues) {
            if (term is Terminal) { //it is terminal, so add it to Firsts and that's all with this production
              prod.LValue.Firsts.Add(term.Key); // Add terminal to Firsts (note: Add ignores repetitions)
              break; //from foreach term
            }//if
            NonTerminal ntChild = term as NonTerminal;
            if (!ntChild.PropagateFirstsTo.Contains(prod.LValue))
              ntChild.PropagateFirstsTo.Add(prod.LValue); //ignores repetitions
            if (!ntChild.Nullable) break; //if not nullable we're done
          }//foreach oper
        }//foreach prod
      }//foreach nt
      
      //2. Propagate all firsts thru all dependencies
      NonTerminalList workList = _grammar.NonTerminals;
      while (workList.Count > 0) {
        NonTerminalList newList = new NonTerminalList();
        foreach (NonTerminal nt in workList) {
          foreach (NonTerminal toNt in nt.PropagateFirstsTo) {
            foreach (string symbolKey in nt.Firsts) {
              if (!toNt.Firsts.Contains(symbolKey)) {
                toNt.Firsts.Add(symbolKey);
                if (!newList.Contains(toNt))
                  newList.Add(toNt);
              }//if
            }//foreach symbolKey
          }//foreach toNt
        }//foreach nt in workList
        workList = newList;
      }//while
    }//method

    #endregion

    #region Calculating Tail Firsts
    private void CalculateTailFirsts() {
      foreach (NonTerminal nt in _grammar.NonTerminals) {
        foreach (Production prod in nt.Productions) {
          StringSet accumulatedFirsts = new StringSet();
          bool allNullable = true;
          //We are going backwards in LR0Items list
          for (int i = prod.LR0Items.Count - 1; i >= 0; i--) {
            LR0Item item = prod.LR0Items[i];
            if (i >= prod.LR0Items.Count - 2) {
              //Last and before last items have empty tails
              item.TailIsNullable = true;
              item.TailFirsts.Clear();
              continue;
            }
            BnfTerm nextTerm = prod.RValues[i + 1];  //Element after-after-dot; remember we're going in reverse direction
            //if (ntElem == null) continue; //it is not NonTerminal
            NonTerminal nextNt = nextTerm as NonTerminal;
            bool notNullable = nextTerm is Terminal || nextNt != null && !nextNt.Nullable;
            if (notNullable) { //next term is not nullable  (a terminal or non-nullable NonTerminal)
              //term is not nullable, so we clear all old firsts and add this term
              accumulatedFirsts.Clear();
              allNullable = false;
              item.TailIsNullable = false;
              if (nextTerm is Terminal) {
                item.TailFirsts.Add(nextTerm.Key);//term is terminal so add its key
                accumulatedFirsts.Add(nextTerm.Key);
              } else if (nextNt != null) { //it is NonTerminal
                item.TailFirsts.AddRange(nextNt.Firsts); //nonterminal
                accumulatedFirsts.AddRange(nextNt.Firsts);
              }
              continue;
            }
            //if we are here, then ntElem is a nullable NonTerminal. We add 
            accumulatedFirsts.AddRange(nextNt.Firsts);
            item.TailFirsts.AddRange(accumulatedFirsts);
            item.TailIsNullable = allNullable;
          }//for i
        }//foreach prod
      }//foreach nt
    }//method

    #endregion

    #region Creating parser states
    private void CreateInitialAndFinalStates() {
      //there is always just one initial production "Root' -> .Root", and we're interested in LR item at 0 index
      LR0ItemList itemList = new LR0ItemList();
      itemList.Add(Data.AugmentedRoot.Productions[0].LR0Items[0]);
      Data.InitialState = FindOrCreateState(itemList); //it is actually create
      Data.InitialState.Items[0].NewLookaheads.Add(Grammar.Eof.Key);
      #region comment about FinalState
      //Create final state - because of the way states construction works, it doesn't create the final state automatically. 
      // We need to create it explicitly and assign it to _data.FinalState property
      // The final executed reduction is "Root' -> Root.". This jump is executed as follows: 
      //   1. parser creates Root' node 
      //   2. Parser pops the state from stack - that would be initial state
      //   3. Finally, parser tries to find the transition in state.Actions table by the key of [Root'] element. 
      // We must create the final state, and create the entry in transition table
      // The final state is based on the same initial production, but different LRItem - the one with dot AFTER the root nonterminal.
      // it is item at index 1.
      #endregion
      itemList.Clear();
      itemList.Add(Data.AugmentedRoot.Productions[0].LR0Items[1]);
      Data.FinalState = FindOrCreateState(itemList); //it is actually create
      //Create shift transition from initial to final state
      string rootKey = Data.AugmentedRoot.Key;
      Data.InitialState.Actions[rootKey] = new ActionRecord(rootKey, ParserActionType.Shift, Data.FinalState, null);
    }

    private void CreateParserStates() {
      Data.States.Clear();
      _stateHash = new ParserStateTable();
      CreateInitialAndFinalStates();

      string augmRootKey = Data.AugmentedRoot.Key;
      // Iterate through states (while new ones are created) and create shift transitions and new states 
      for (int index = 0; index < Data.States.Count; index++) {
        ParserState state = Data.States[index];
        AddClosureItems(state);
        //Get keys of all possible shifts
        ShiftTable shiftTable = GetStateShifts(state);
        //Each key in shifts dict is an input element 
        // Value is LR0ItemList of shifted LR0Items for this input element.
        foreach (string input in shiftTable.Keys) {
          LR0ItemList shiftedCoreItems = shiftTable[input];
          ParserState newState = FindOrCreateState(shiftedCoreItems);
          ActionRecord newAction = new ActionRecord(input, ParserActionType.Shift, newState, null);
          state.Actions[input] = newAction;
          //link original LRItems in original state to derived LRItems in newState
          foreach (LR0Item coreItem in shiftedCoreItems) {
            LRItem fromItem = FindItem(state, coreItem.Production, coreItem.Position - 1);
            LRItem toItem = FindItem(newState, coreItem.Production, coreItem.Position);
            if (!fromItem.PropagateTargets.Contains(toItem))
              fromItem.PropagateTargets.Add(toItem);
            //copy hints from core items into the newAction
            newAction.ShiftItems.Add(fromItem); 
          }//foreach coreItem
        }//foreach input
      } //for index
      Data.FinalState = Data.InitialState.Actions[augmRootKey].NewState;
    }//method

    private string AdjustCase(string key) {
      return _grammar.CaseSensitive ? key : key.ToLower();
    }
    private LRItem TryFindItem(ParserState state, LR0Item core) {
      foreach (LRItem item in state.Items)
        if (item.Core == core)
          return item;
      return null;
    }//method

    private LRItem FindItem(ParserState state, Production production, int position) {
      foreach(LRItem item in state.Items) 
        if (item.Core.Production == production && item.Core.Position == position)
          return item;
      string msg = string.Format("Failed to find an LRItem in state {0} by production [{1}] and position {2}. ",
        state, production.ToString(), position.ToString());
      throw new CompilerException(msg);
    }//method

    private ShiftTable GetStateShifts(ParserState state) {
      ShiftTable shifts = new ShiftTable();
      LR0ItemList list;
      foreach (LRItem item in state.Items) {
        BnfTerm term = item.Core.Current;
        if (term == null)  continue;
        LR0Item shiftedItem = item.Core.Production.LR0Items[item.Core.Position + 1];
        if (!shifts.TryGetValue(term.Key, out list))
          shifts[term.Key] = list = new LR0ItemList();
        list.Add(shiftedItem);
      }//foreach
      return shifts;
    }//method

    private ParserState FindOrCreateState(LR0ItemList lr0Items) {
      string key = CalcItemListKey(lr0Items);
      ParserState result;
      if (_stateHash.TryGetValue(key, out result))
        return result; 
      result = new ParserState("S" + Data.States.Count, lr0Items);
      Data.States.Add(result);
      _stateHash[key] = result;
      return result;
    }

    //Creates closure items with "spontaneously generated" lookaheads
    private bool AddClosureItems(ParserState state) {
      bool result = false;
      //note that we change collection while we iterate thru it, so we have to use "for i" loop
      for(int i = 0; i < state.Items.Count; i++) {
        LRItem item = state.Items[i];
        BnfTerm currTerm = item.Core.Current;
        if (currTerm == null || !(currTerm is NonTerminal))  
          continue;
        //1. Add normal closure items
        NonTerminal currNt = currTerm as NonTerminal;
        foreach (Production prod in currNt.Productions) {
          LR0Item core = prod.LR0Items[0]; //item at zero index is the one that starts with dot
          LRItem newItem = TryFindItem(state, core);
          if (newItem == null) {
            newItem = new LRItem(state, core);
            state.Items.Add(newItem);
            result = true;
          }
          #region Comments on lookaheads processing
          // The general idea of generating ("spontaneously") the lookaheads is the following:
          // Let's the original item be in the form 
          //   [A -> alpha . B beta , lset]
          //     where <B> is a non-terminal and <lset> is a set of lookaheads, 
          //      <beta> is some string (B's tail in our terminology)
          // Then the closure item on non-teminal B is an item
          //   [B -> x, firsts(beta + lset)]
          //      (the lookahead set is expression after the comma).
          // To generate lookaheads on a closure item, we simply take "firsts" 
          //   from the tail <beta> of the NonTerminal <B>. 
          //   Normally if tail <beta> is nullable we would add ("propagate") 
          //   the <lset> lookaheads from <A> to <B>.
          //   We dont' do it right here - we simply add a propagation link.
          //   We propagate all lookaheads later in a separate process.
          #endregion
          newItem.NewLookaheads.AddRange(item.Core.TailFirsts);
          if (item.Core.TailIsNullable && !item.PropagateTargets.Contains(newItem))
            item.PropagateTargets.Add(newItem);
        }//foreach prod
      }//for i (LRItem)
      return result;
    }


    #region comments
    //Parser states are distinguished by the subset of kernel LR0 items. 
    // So when we derive new LR0-item list by shift operation, 
    // we need to find out if we have already a state with the same LR0Item list.
    // We do it by looking up in a state hash by a key - [LR0 item list key]. 
    // Each list's key is a concatenation of items' IDs separated by ','.
    // Before producing the key for a list, the list must be sorted; 
    //   thus we garantee one-to-one correspondence between LR0Item sets and keys.
    // And of course, we count only kernel items (with dot NOT in the first position).
    #endregion
    private string CalcItemListKey(LR0ItemList items) {
      items.Sort(ById); //Sort by ID
      if (items.Count == 0) return "";
      //quick shortcut
      if (items.Count == 1 && items[0].IsKernel) 
        return items[0].ID.ToString();
      StringBuilder sb = new StringBuilder(1024);
      foreach (LR0Item item in items) {
        if (item.IsKernel) {
          sb.Append(item.ID);
          sb.Append(",");
        }
      }//foreach
      return sb.ToString();
    }
    private static int ById(LR0Item x, LR0Item y) {
      if (x.ID < y.ID) return -1;
      if (x.ID == y.ID) return 0;
      return 1;
    }

    #endregion

    #region Lookaheads propagation
    private void PropagateLookaheads() {
      LRItemList currentList = new LRItemList();
      //first collect all items
      foreach (ParserState state in Data.States)
        currentList.AddRange(state.Items);
      //Main loop - propagate until done
      while (currentList.Count > 0) {
        LRItemList newList = new LRItemList();
        foreach (LRItem item in currentList) {
          if (item.NewLookaheads.Count == 0) continue;
          int oldCount = item.Lookaheads.Count;
          item.Lookaheads.AddRange(item.NewLookaheads);
          if (item.Lookaheads.Count != oldCount) {
            foreach (LRItem targetItem in item.PropagateTargets) {
              targetItem.NewLookaheads.AddRange(item.NewLookaheads);
              newList.Add(targetItem);
            }//foreach targetItem
          }//if
          item.NewLookaheads.Clear();
        }//foreach item
        currentList = newList;
      }//while         
    }//method
    #endregion

    #region Final actions: createReduceActions
    private void CreateReduceActions() {
      foreach(ParserState state in Data.States) {
        foreach (LRItem item in state.Items) {
          //we are interested only in "dot  at the end" items
          if (item.Core.Current != null)   continue;
          foreach (string lookahead in item.Lookaheads) {
            ActionRecord action;
            if (state.Actions.TryGetValue(lookahead, out action)) 
              action.ReduceProductions.Add(item.Core.Production);
            else
              state.Actions[lookahead] = new ActionRecord(lookahead, ParserActionType.Reduce, null, item.Core.Production);
          }//foreach lookahead
        }//foreach item
      }// foreach state
    } //method
    
    #endregion

    #region Check for shift-reduce conflicts
    private void CheckActionConflicts() {
      StringDictionary errorTable = new StringDictionary();
      foreach (ParserState state in Data.States) {
        foreach (ActionRecord action in state.Actions.Values) {
          //1. Pure shift
          if (action.ShiftItems.Count > 0 && action.ReduceProductions.Count == 0) {
            action.ActionType = ParserActionType.Shift; 
            continue; 
          }
          //2. Pure reduce
          if (action.ShiftItems.Count == 0 && action.ReduceProductions.Count == 1) {
            action.ActionType = ParserActionType.Reduce; 
            continue;
          } 
          //3. Shift-reduce and reduce-reduce conflicts
          if (action.ShiftItems.Count > 0 && action.ReduceProductions.Count > 0) {
            if (CheckConflictResolutionByPrecedence(action))   continue; 
            if (CheckShiftHint(action))     continue;
            if (CheckReduceHint(action))    continue; 
            //if we are here, we couldn't resolve the conflict, so post a grammar error(actually warning)
            AddErrorForInput(errorTable, action.Key, "Shift-reduce conflict in state {0}, reduce production: {1}",
                state, action.ReduceProductions[0]);
          }//Shift-reduce conflict

          //4. Reduce-reduce conflicts
          if (action.ReduceProductions.Count > 1) {
            if (CheckReduceHint(action)) continue;
            //if we are here, we reduce-reduce conflict, so post a grammar error
            AddErrorForInput(errorTable, action.Key, "Reduce-reduce conflict in state {0} in productions: {1} ; {2}",
                state, action.ReduceProductions[0], action.ReduceProductions[1]);
          }
        }//foreach action
      }//foreach state
      //copy errors to Errors collection; In errorTable keys are error messages, values are inputs for this message 
      foreach (string msg in errorTable.Keys) {
        _grammar.Errors.Add(msg + " on inputs: " + errorTable[msg]);
      }
    }//methods

    private bool CheckConflictResolutionByPrecedence(ActionRecord action) {
      SymbolTerminal opTerm = SymbolTerminal.GetSymbol(action.Key);
      if (opTerm != null && opTerm.IsSet(TermOptions.IsOperator)) {
        action.ActionType = ParserActionType.Operator;
        action.ConflictResolved = true;
        return true;
      }
      return false;
    }
    //Checks  shift items for PreferShift grammar hint. Hints are associated with a particular position
    // inside production, which is in fact an LR0 item. The LR0 item is available thru shiftItem.Core property. 
    // If PreferShift hint found, moves the hint-owning shiftItem to the beginning of the list and returns true.
    private bool CheckShiftHint(ActionRecord action) {
      foreach(LRItem shiftItem in action.ShiftItems) {
        GrammarHint shiftHint = GetHint(shiftItem.Core.Production, shiftItem.Core.Position, HintType.PreferShift);
        if (shiftHint != null) {
            action.ActionType = ParserActionType.Shift;
            action.ShiftItems.Remove(shiftItem);
            action.ShiftItems.Insert(0, shiftItem);
            action.ConflictResolved = true; 
            return true;
        }//if
      }//foreach shiftItem
      return false; 
    }//method

    //Checks Reduce productions of an action for a ReduceThis hint. If found, the production is moved to the beginning of the list.
    private bool CheckReduceHint(ActionRecord action) {
      foreach (Production prod in action.ReduceProductions) {
        GrammarHint reduceHint = GetHint(prod, prod.RValues.Count, HintType.ReduceThis);
        if (reduceHint != null) {
          action.ReduceProductions.Remove(prod);
          action.ReduceProductions.Insert(0, prod);
          action.ActionType = ParserActionType.Reduce;
          action.ConflictResolved = true;
          return true; 
        }//if
      }//foreach prod
      return false; 
    }//method

    private GrammarHint GetHint(Production production, int position, HintType hintType) {
      foreach (GrammarHint hint in production.Hints)
        if (hint.Position == position && hint.HintType == hintType)
          return hint;
      return null;
    }
    //Aggregate error messages for different inputs (lookaheads) in errors dictionary
    private void AddErrorForInput(StringDictionary errors, string input, string template, params object[] args) {
      string msg = string.Format(template, args);
      string tmpInputs;
      errors.TryGetValue(msg, out tmpInputs);
      errors[msg] = tmpInputs + input + " ";
    }

    #endregion

    private void ValidateAll() {
      //Check rule on all non-terminals
      StringSet ntList = new StringSet();
      foreach(NonTerminal nt in _grammar.NonTerminals) {
        if (nt == Data.AugmentedRoot) continue; //augm root does not count
        BnfExpressionData data = nt.Rule.Data;
        if (data.Count == 1 && data[0].Count == 1 && data[0][0] is NonTerminal)
          ntList.Add(nt.Name);
      }//foreach
      if (ntList.Count > 0) {
        string slist =  TextUtils.Cleanup(ntList.ToString(", "));
        AddError("Warning: Possible non-terminal duplication. The following non-terminals have rules containing a single non-terminal: \r\n {0}. \r\n" +
         "Consider merging two non-terminals; you may need to use 'nt1 = nt2;' instead of 'nt1.Rule=nt2'.", slist);
      }
      //Check constructors of all nodes referenced in Non-terminals that don't use NodeCreator delegate
      var ctorArgTypes = new Type[] {typeof(NodeArgs)};
      foreach (NonTerminal nt in _grammar.NonTerminals) {
        if (nt.NodeCreator == null && nt.NodeType != null) {
          object ci = nt.NodeType.GetConstructor(ctorArgTypes);
          if (ci == null)
            AddError(
@"AST Node class {0} referenced by non-terminal {1} does not have a constructor for automatic node creation. 
Provide a constructor with a single NodeArgs parameter, or use NodeCreator delegate property in NonTerminal.", 
  nt.NodeType, nt.Name);

        }//if
      }//foreach ntInfo

    }//method

    #region error handling: AddError
    private void AddError(string message, params object[] args) {
      if (args != null && args.Length > 0)
        message = string.Format(message, args);
      _grammar.Errors.Add(message);
    }
    #endregion

  }//class


}//namespace
