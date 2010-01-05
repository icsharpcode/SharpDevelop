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


namespace Irony.Compiler.Nlalr {
  // ParserControlData is a container for all information used by LALR Parser in input processing.
  // The state graph entry is InitialState state; the state graph encodes information usually contained 
  // in what is known in literature as transiton/goto tables.
  // The graph is built from the language grammar by ParserControlDataBuilder instance. 
  // See Dragon book or other book on compilers on details of LALR parsing and parsing tables construction. 

  public class ParserControlData {
    public Grammar Grammar;
    public NonTerminal AugmentedRoot;
    public ParserState InitialState;
    public ParserState FinalState;
    public readonly ParserStateList States = new ParserStateList();
    
    public bool AnalysisCanceled;  //True if grammar analysis was canceled due to errors

    public ParserControlData(Grammar grammar) {
      Grammar = grammar;
    }
  }


  public enum ParserActionType {
    Shift,
    Resolve,   //Like reduce, only new node is pushed into input stack
    Operator,  //shift or resolve/reduce depending on operator associativity and precedence
  }

  public partial class ParserState {
    public readonly string Name;
    public readonly ActionRecordTable Actions = new ActionRecordTable();
    public readonly LRItemList Items = new LRItemList();

    public ParserState(string name, LRItem item) {
      Name = name;
      Items.Add(item);
    }
    public ParserState(string name, LR0ItemList coreItems) {
      Name = name;
      foreach (LR0Item coreItem in coreItems)
        Items.Add(new LRItem(this, coreItem));
    }
    public override string ToString() {
      return Name;
    }
  }//class

  public class ParserStateList : List<ParserState> { }
  public class ParserStateTable : Dictionary<string, ParserState> { } //hash table

  public class ActionRecord {
    public string Key;
    public ParserActionType ActionType = ParserActionType.Shift;
    public ParserState NewState;
    public readonly ProductionList ReduceProductions = new ProductionList(); //may be more than one, in case of conflict
    public readonly LRItemList ShiftItems = new LRItemList();
    public bool ConflictResolved; 

    internal ActionRecord(string key, ParserActionType type, ParserState newState, Production reduceProduction) {
      this.Key = key;
      this.ActionType = type;
      this.NewState = newState; 
      if (reduceProduction != null)
        ReduceProductions.Add(reduceProduction);
    }
    public ActionRecord CreateDerived(ParserActionType type, Production reduceProduction) {
      return new ActionRecord(this.Key, type, this.NewState, reduceProduction);
    }

    public Production Production { 
      get {return ReduceProductions.Count > 0? ReduceProductions[0] : null;}
    }
    public NonTerminal NonTerminal {
      get { return Production == null? null: Production.LValue; }
    }
    public int PopCount {
      get { return Production.RValues.Count;}
    }
    public bool HasConflict() {
      // This function is used by parser to determine if it needs to call OnActionConflict method in Grammar.
      // Even if conflict is resolved, we still need to return true to force parser to invoke method.
      // This is necessary to make parser work properly in situation like this: in c#, the "<" symbol is 
      // both operator symbol and opening brace for type parameter. When used purely as operator symbol, 
      //  it is involved in shift/reduced conflict resolved by operator precedence. Still, before parser starts 
      // acting based on precedence, a custom grammar method should decide - is it really an operator or type parameter
      // bracket.
      //if (ConflictResolved) return false; -- this should be commented out
      return ShiftItems.Count > 0 && ReduceProductions.Count > 0 ||
        ReduceProductions.Count > 1; 
    }
    public override string ToString() {
      string result = ActionType.ToString();
      if (ActionType == ParserActionType.Resolve && ReduceProductions.Count > 0)
        result += " on " + ReduceProductions[0];
      return result;
    }

  }//class ActionRecord

  public class ActionRecordTable : Dictionary<string, ActionRecord> { }

  public class LRItem {
    public readonly ParserState State;
    public readonly LR0Item Core;
    public readonly LRItemList PropagateTargets = new LRItemList(); //used for lookaheads propagation
    public readonly StringSet Lookaheads = new StringSet();
    public readonly StringSet NewLookaheads = new StringSet();
    public LRItem(ParserState state, LR0Item core) {
      State = state;
      Core = core;
    }
    public override string ToString() {
      return Core.ToString() + "  LOOKAHEADS: " + TextUtils.Cleanup(Lookaheads.ToString(" "));
    }
  }//LRItem class

  public class LRItemList : List<LRItem> { }


}//namespace
