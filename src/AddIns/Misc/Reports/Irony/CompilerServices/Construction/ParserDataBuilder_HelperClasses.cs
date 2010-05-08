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


namespace Irony.CompilerServices.Construction {

  internal class ParserStateData {
    public readonly ParserState State;
    public readonly LR0ItemSet Cores = new LR0ItemSet();
    public readonly LRItemSet AllItems = new LRItemSet();
    public readonly LRItemSet ShiftItems = new LRItemSet();
    public readonly LRItemSet ReduceItems = new LRItemSet();
    public readonly BnfTermSet ShiftTerms = new BnfTermSet();
    public readonly ShiftTransitionSet ShiftTransitions = new ShiftTransitionSet();  
    public readonly BnfTermSet Conflicts = new BnfTermSet();
    public readonly BnfTermSet ResolvedConflicts = new BnfTermSet();
    public readonly NonTerminalSet NonCanonicalLookaheads = new NonTerminalSet();
    public readonly BnfTermSet JumpLookaheads = new BnfTermSet();
    public ParserState JumpTarget;
    public bool InitialLookaheadsComputed;

    public string Key;
    public readonly bool IsNonCanonical;

    //used for creating canonical states from core set
    public ParserStateData(ParserState state, LR0ItemSet kernelCores, string coresKey) {
      State = state;
      Key = coresKey;
      foreach (var core in kernelCores)
        AddItem(core);
    }
    //Used for creating non-canonical states
    public ParserStateData(ParserState state, LRItemSet items) {
      State = state;
      IsNonCanonical = true;
      foreach (var item in items)
        AddItem(item);
    }//method

    public LRItem AddItem(LR0Item core) {
      var item = AllItems.FindByCore(core);
      if (item != null) return item;
      item = new LRItem(State, core);
      AddItem(item);
      //If current term is non-terminal, expand it
      var currNt = core.Current as NonTerminal;
      if (currNt == null) return item;
      foreach (var prod in currNt.Productions) {
        var expItem = AddItem(prod.LR0Items[0]);
        item.Expansions.Add(expItem);
      }
      return item;
    }//method

    public void AddItem(LRItem item) {
      if (AllItems.Contains(item)) return;
      AllItems.Add(item);
      Cores.Add(item.Core);
      if (item.Core.IsFinal)
        ReduceItems.Add(item);
      else {
        ShiftItems.Add(item);
        ShiftTerms.Add(item.Core.Current);
      }
    }
    public bool IsInadequate {
      get { return ReduceItems.Count > 1 || ReduceItems.Count == 1 && ShiftItems.Count > 0; } //reduce/reduce or shift/reduce
    }

    public BnfTermSet GetShiftReduceConflicts() {
      var result = new BnfTermSet();
      result.UnionWith(Conflicts);
      result.IntersectWith(ShiftTerms);
      return result;
    }
    public BnfTermSet GetReduceReduceConflicts() {
      var result = new BnfTermSet();
      result.UnionWith(Conflicts);
      result.ExceptWith(ShiftTerms);
      return result;
    }

  }//class

  //An object representing inter-state shifts. Defines Includes, IncludedBy sets that 
  // are used for efficient lookahead computation (see Grune, Jacobs "Parsing Techniques" 2nd ed, section 9.7, p. 309)
  // The "efficient" part (transitive closure using SCC) is not implemented yet
  internal class ShiftTransition {
    public readonly ParserState FromState;
    public ParserState ToState;
    public readonly BnfTerm OverTerm;
    //shift items associated with transition, such that item.current = OverNonTerminal
    public readonly LRItemSet ShiftItems;
    //shifted items of ShiftItems plus shifts over nulls
    public readonly LRItemSet LookaheadSources = new LRItemSet();

    public readonly ShiftTransitionSet Includes = new ShiftTransitionSet();
    public readonly ShiftTransitionSet IncludedBy = new ShiftTransitionSet();

    int _hashCode;
    internal int _lastChecked;
    internal int _lastChanged;

    public ShiftTransition(ParserState fromState, BnfTerm overTerm, LRItemSet shiftItems) {
      FromState = fromState;
      OverTerm = overTerm;
      ShiftItems = shiftItems;
      _hashCode = unchecked(fromState.GetHashCode() - overTerm.GetHashCode());
      //Add self to state's set of transitions
      fromState.BuilderData.ShiftTransitions.Add(this);
      //Assign self to Transition field of all shift items
      foreach (var shiftItem in ShiftItems)
        shiftItem.Transition = this;
    }//constructor


    public bool Include(ShiftTransition other) {
      bool result = Includes.Add(other);
      if (!result) return false;
      other.IncludedBy.Add(this);
      return result;
    }

    public bool Include(ShiftTransitionSet includeTransitions) {
      bool result = false; 
      foreach (var trans in includeTransitions)
        result |= Include(trans);
      return result; 
    }

    public override string ToString() {
      return FromState.Name + "->" + ToState.Name + "/" + OverTerm.Name;
    }
    public override int GetHashCode() {
      return _hashCode;
    }
  }//class

  internal class ShiftTransitionSet : HashSet<ShiftTransition> { }

  internal class LRItem {
    public readonly ParserState State;
    public readonly LR0Item Core;
    //these helper fields are used in lookahead computations
    public LRItem ShiftedItem;
    public ShiftTransition Transition; 
    public readonly ShiftTransitionSet Lookbacks = new ShiftTransitionSet();
    public readonly LRItemSet Expansions = new LRItemSet(); //Only direct expansions
    public string Key;

    public readonly LRItemSet ReducedLookaheadSources = new LRItemSet();
    public readonly BnfTermSet ReducedLookaheads = new BnfTermSet(); //fully-reduced lookaheads
    public readonly BnfTermSet AllLookaheads = new BnfTermSet();   //all lookaheads, canonical and non-canonical
    public readonly BnfTermSet Lookaheads = new BnfTermSet(); //actual active lookaheads

    public LRItem(ParserState state, LR0Item core) {
      State = state;
      Core = core;
      Key = state.Name + "/" + core.ID;
    }
    public override string ToString() {
      string s = Core.ToString();
      if (this.Core.IsFinal) 
        s += " [" + Lookaheads.ToString() + "]";
      return s; 
    }
    public string ToString(BnfTermSet exceptLookaheads) {
      string s = Core.ToString();
      if (!this.Core.IsFinal) return s;
      var lkhds = new BnfTermSet();
      lkhds.UnionWith(Lookaheads);
      lkhds.ExceptWith(exceptLookaheads);
      s += " [" + lkhds.ToString() + "]";
      return s;
    }
    //direct expansions plus expansions of expansions; computed on demand
    public LRItemSet AllExpansions {
      get {
        if (_allExpansions == null)
          ComputeAllExpansions(); 
        return _allExpansions;
      }
    } LRItemSet _allExpansions;

    private void ComputeAllExpansions() {
      _allExpansions = new LRItemSet();
      _allExpansions.UnionWith(Expansions);
      var newItems = new LRItemSet();
      bool done = false;
      while (!done) {
        newItems.Clear();
        foreach (var expItem in _allExpansions)
          newItems.UnionWith(expItem.Expansions);
        var oldCount = _allExpansions.Count;
        _allExpansions.UnionWith(newItems);
        done = (_allExpansions.Count == oldCount);
      }
    }
    public override int GetHashCode() {
      return Key.GetHashCode();
    }
  }//LRItem class

  internal class LRItemList : List<LRItem> { }

  internal class LRItemSet : HashSet<LRItem> {
    public LRItem FindByCore(LR0Item core) {
      foreach (LRItem item in this)
        if (item.Core == core) return item;
      return null;
    }
    public LRItemSet SelectByLValue(NonTerminal lvalue) {
      var result = new LRItemSet();
      foreach (var item in this)
        if (item.Core.Production.LValue == lvalue)
          result.Add(item);
      return result; 
    }
    public LRItemSet SelectByCurrent(BnfTerm current) {
      var result = new LRItemSet();
      foreach (var item in this)
        if (item.Core.Current == current)
          result.Add(item);
      return result;
    }
    public LR0ItemSet GetCores() {
      var result = new LR0ItemSet();
      foreach (var item in this)
        result.Add(item.Core);
      return result;
    }
    public LR0ItemSet GetShiftedCores() {
      var result = new LR0ItemSet();
      foreach (var item in this)
        if (item.Core.ShiftedItem != null) 
          result.Add(item.Core.ShiftedItem);
      return result;
    }
    public LRItemSet SelectByLookahead(BnfTerm lookahead) {
      var result = new LRItemSet();
      foreach (var item in this)
        if (item.Lookaheads.Contains(lookahead))
          result.Add(item);
      return result;
    }
    public LRItemSet SelectByReducedLookahead(BnfTerm lookahead) {
      var result = new LRItemSet();
      foreach (var item in this)
        if (item.ReducedLookaheads.Contains(lookahead))
          result.Add(item);
      return result;
    }
    public LRItemSet SelectNonFinal() {
      var result = new LRItemSet();
      foreach (var item in this)
        if (item.Core.Current != null)
          result.Add(item);
      return result;
    }
    public GrammarHintList GetHints(HintType hintType) {
      var result = new GrammarHintList();
      foreach (var item in this)
        if (item.Core.HasHints())
          foreach(var hint in item.Core.Hints)
            if (hint.HintType == hintType)
          result.Add(hint);
      return result;
    }
  }

  public partial class LR0Item {
    public Production Production;
    public int Position;
    public bool TailIsNullable;
    public GrammarHintList Hints;

    //automatically generated IDs - used for building keys for lists of kernel LR0Items
    // which in turn are used to quickly lookup parser states in hash
    internal readonly int ID;

    public LR0Item(int id, Production production, int position) : this(id, production, position, null)  { }

    public LR0Item(int id, Production production, int position, GrammarHintList hints) {
      ID = id;
      Production = production;
      Position = position;
      Hints = hints; 
      _hashCode = ID.ToString().GetHashCode();
    }
    //The after-dot element
    public BnfTerm Current {
      get {
        if (Position < Production.RValues.Count)
          return Production.RValues[Position];
        else
          return null;
      }
    }
    public LR0Item ShiftedItem {
      get {
        if (Position >= Production.LR0Items.Count - 1)
          return null;
        else
          return Production.LR0Items[Position + 1];
      }
    }
    public bool IsKernel {
      get { return Position > 0; }
    }
    public bool IsInitial {
      get { return Position == 0; }
    }
    public bool IsFinal {
      get { return Position == Production.RValues.Count; }
    }
    public bool HasHints() {
      return Hints != null && Hints.Count > 0;
    }
    public override string ToString() {
      return Production.ProductionToString(Production, Position);
    }
    public override int GetHashCode() {
      return _hashCode;
    } int _hashCode;

  }//LR0Item

  internal class LR0ItemList : List<LR0Item> { }
  internal class LR0ItemSet : HashSet<LR0Item> { }



}//namespace
