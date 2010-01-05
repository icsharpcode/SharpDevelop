using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.CompilerServices;

namespace Irony.Diagnostics {
  public class ParserTraceEntry {
    public ParserState State;
    public ParseTreeNode StackTop;
    public ParseTreeNode Input;
    public string Message;
    public ParserState NewState;
    public bool IsError;

    public ParserTraceEntry(ParserState state, ParseTreeNode stackTop, ParseTreeNode input) {
      State = state;
      StackTop = stackTop;
      Input = input;
    }
    public void SetDetails(string message, ParserState newState) {
      Message = message;
      NewState = newState;
    }
  }//class

  public class ParserTrace : List<ParserTraceEntry> { }

  public class ParserTraceEventArgs : EventArgs {
    public ParserTraceEventArgs(ParserTraceEntry entry) {
      Entry = entry; 
    }

    public readonly ParserTraceEntry Entry;

    public override string ToString() {
      return Entry.ToString(); 
    }
  }//class



}//namespace
