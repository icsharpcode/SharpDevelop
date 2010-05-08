using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Error handling methods of CoreParser class
namespace Irony.CompilerServices {
  public partial class CoreParser {

    private bool TryRecover() {
      _context.ParserIsRecovering = true;
      try {
        if (_traceOn)
          AddTraceEntry("*** RECOVERING - searching for state with error shift ***", _currentState); //add new trace entry
        var result = TryRecoverImpl();
        if (_traceOn) {
          string msg = (result ? "*** RECOVERED ***" : "*** FAILED TO RECOVER ***");
          AddTraceEntry(msg, _currentState); //add new trace entry
        }//if
        return result;
      } finally {
        _context.ParserIsRecovering = false;
      }
    }

    private bool TryRecoverImpl() {
      //1. We need to find a state in the stack that has a shift item based on error production (with error token), 
      // and error terminal is current. This state would have a shift action on error token. 
      ParserAction errorShiftAction = FindErrorShiftActionInStack();
      if (errorShiftAction == null) return false; //we failed to recover
      //2. Shift error token - execute shift action
      if (_traceOn) AddTraceEntry();
      ExecuteShift(errorShiftAction.NewState);
      //4. Now we need to go along error production until the end, shifting tokens that CAN be shifted and ignoring others.
      //   We shift until we can reduce
      while (_currentInput.Term != _grammar.Eof) {
        //Check if we can reduce
        var action = GetReduceActionInCurrentState();
        if (action != null) {
          //Now reset scanner's position to current input position and clear all token queues; do it before ExecuteReduce
          // as reset would clear the input stack
          ResetSourceLocation(_currentInput.Span.Start, _currentInput.Span.EndPos);
          ExecuteReduce(action.ReduceProduction);
          return true; //we recovered 
        }
        //No reduce action in current state. Try to shift current token or throw it away or reduce
        action = GetShiftActionInCurrentState();
        if (action != null)
          ExecuteShift(action.NewState); //shift input token
        else
          ReadInput(); //throw away input token
      }
      return false;
    }//method

    public void ResetSourceLocation(SourceLocation tokenStart, int position) {
      _currentInput = null;
      InputStack.Clear();
      _scanner.SetSourceLocation(tokenStart, position);
    }

    private ParserAction FindErrorShiftActionInStack() {
      while (Stack.Count >= 1) {
        ParserAction errorShiftAction;
        if (_currentState.Actions.TryGetValue(_grammar.SyntaxError, out errorShiftAction) && errorShiftAction.ActionType == ParserActionType.Shift)
          return errorShiftAction;
        //pop next state from stack
        if (Stack.Count == 1)
          return null; //don't pop the initial state
        Stack.Pop();
        _currentState = Stack.Top.State;
      }
      return null;
    }

    private ParserAction GetReduceActionInCurrentState() {
      if (_currentState.DefaultReduceAction != null) return _currentState.DefaultReduceAction;
      foreach (var action in _currentState.Actions.Values)
        if (action.ActionType == ParserActionType.Reduce)
          return action;
      return null;
    }

    private ParserAction GetShiftActionInCurrentState() {
      ParserAction result = null;
      if (_currentState.Actions.TryGetValue(_currentInput.Term, out result) ||
         _currentInput.Token != null && _currentInput.Token.AsSymbol != null &&
             _currentState.Actions.TryGetValue(_currentInput.Token.AsSymbol, out result))
        if (result.ActionType == ParserActionType.Shift)
          return result;
      return null;
    }

    private void ReportParseError() {
      string msg;
      if (_currentInput.Term == _grammar.Eof)
        msg = "Unexpected end of file.";
      else {
        if (_currentState.ReportedExpectedSet == null)
          ComputeReportedExpectedSet(_currentState); 
        msg = _grammar.GetSyntaxErrorMessage(_context, _currentState, _currentInput);
        if (msg == null)
          msg = "Syntax error" + (_currentState.ReportedExpectedSet.Count == 0 ? "." :
            ", expected: " + _currentState.ReportedExpectedSet.ToErrorString());
      }
      _context.ReportError(_currentState, _currentInput.Span.Start, msg);
      if (_currentTraceEntry != null) {
        _currentTraceEntry.Message = msg;
        _currentTraceEntry.IsError = true;
      }
    }

    private void ComputeReportedExpectedSet(ParserState state) {
      //2. Compute reduced expected terms - to be used in error reporting
      //2.1. Scan Expected terms, add non-terminals with non-empty DisplayName to reduced set, and collect all their firsts
      var reducedSet = state.ReportedExpectedSet = new BnfTermSet();
      var allFirsts = new BnfTermSet();
      foreach (var term in state.ExpectedTerms) {
        var nt = term as NonTerminal;
        if (nt == null) continue;
        if (!reducedSet.Contains(nt) && !string.IsNullOrEmpty(nt.DisplayName) && !allFirsts.Contains(nt)) {
          reducedSet.Add(nt);
          allFirsts.UnionWith(nt.Firsts);
        }
      }
      //2.2. Now go thru all expected terms and add only those that are NOT in the allFirsts set.
      foreach (var term in state.ExpectedTerms) {
        if (!reducedSet.Contains(term) && !allFirsts.Contains(term) && (term is Terminal || !string.IsNullOrEmpty(term.DisplayName)))
          reducedSet.Add(term);
      }
      //Clean-up reduced set, remove pseudo terms
      if (reducedSet.Contains(_grammar.Eof)) reducedSet.Remove(_grammar.Eof);
      if (reducedSet.Contains(_grammar.SyntaxError)) reducedSet.Remove(_grammar.SyntaxError);

    
    }


  }//class
}//namespace
