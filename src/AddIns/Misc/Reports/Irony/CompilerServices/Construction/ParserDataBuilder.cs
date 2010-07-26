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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Irony.CompilerServices.Construction {
  
  // Methods constructing LALR automaton
  internal partial class ParserDataBuilder {
    LanguageData _language;
    internal ParserData Data;
    Grammar _grammar;
    ParserStateHash _stateHash = new ParserStateHash();


    internal ParserDataBuilder(LanguageData language) {
      _language = language;
      _grammar = _language.Grammar;
    }

    public void Build(ParseMethod method) {
      _stateHash.Clear();
      Stopwatch sw = new Stopwatch();
      sw.Start(); 
      var i1 = sw.ElapsedMilliseconds;
      Data = _language.ParserData = new ParserData(_language.Grammar, method);
      CheckPrecedenceSettings(_language.GrammarData, method); 
      var i2 = sw.ElapsedMilliseconds;
      var i3 = sw.ElapsedMilliseconds;
      CreateLalrParserStates(); 
      var i4 = sw.ElapsedMilliseconds;
      //TODO: move all the following to a single method
      //ComputeTransitionIncludesAndItemLookbacks();  //5 ms
      var i5 = sw.ElapsedMilliseconds;
      PropagateTransitionsIncludes(0);               //220 ms
      var i6 = sw.ElapsedMilliseconds;
      //ComputeTransitionsSources(0); 
      var i7 = sw.ElapsedMilliseconds;
      ComputeLookaheads(); 
      var i8 = sw.ElapsedMilliseconds;
      var i9 = sw.ElapsedMilliseconds;
      ComputeAndResolveConflicts();
      var i10 = sw.ElapsedMilliseconds;
      var i11 = sw.ElapsedMilliseconds;
      var i12 = sw.ElapsedMilliseconds;
      if (Data.ParseMethod == ParseMethod.Nlalr) {
        SwitchConflictingStatesToNonCanonicalLookaheads();
      }
      var i13 = sw.ElapsedMilliseconds;
      ReportAndSetDefaultActionsForRemainingConflicts();
      CreateReduceActions();
      ComputeStateExpectedLists(); 
    }//method

    private void CleanupStateData() {
      foreach (var state in Data.States)
        state.ClearData();
    }

    #region Building grammar data
    #endregion 

    #region Building scanner data

    #endregion

  
    #region Creating parser states
    private void CheckPrecedenceSettings(GrammarData data, ParseMethod method) {
      if(!_grammar.UsePrecedenceRestricted) {
        // All terms registered with RegisterOperator method already have IsOperator and UsePrecedence flags set. 
        // What we need to do is detect non-terminals (like BinOp) that also should be treated as operators 
        // in the parser input, and set the UsePrecedence flag on them.
        // We find all non-terminals having all productions either empty or consisting of a single term which is operator
        // It will cover situations when we define non-terminal like 'BinOp.Rule = "+" | "-" | "*" | "/";'
        //  After reducing lookaheads in NLALR BinOp may become a lookahead, and it must be treated as operator
        foreach (NonTerminal nt in data.NonTerminals) {
          var isOp = true;
          foreach (var prod in nt.Productions) {
            isOp &= prod.RValues.Count == 0 || prod.RValues.Count == 1 && prod.RValues[0].IsSet(TermOptions.IsOperator);
            if (!isOp) break;
          }//foreac prod
          if (isOp)
            nt.SetOption(TermOptions.UsePrecedence);
        }//foreach 

      }//else
    }//method
  

    private void CreateLalrParserStates() {
      CreateInitialState();
      GenerateNewParserStatesThruShifts(0, "S"); //S is prefix for canonical LALR states
      SetFinalStateAndCreateAcceptAction();
      Data.LalrStateCount = Data.States.Count; 
    }

    private void CreateInitialState() {
      //there is always just one initial production "Root' -> .Root", and we're interested in LR item at 0 index
      var iniItemSet = new LR0ItemSet();
      iniItemSet.Add(_language.GrammarData.AugmentedRoot.Productions[0].LR0Items[0]);
      Data.InitialState = CreateState(iniItemSet, "S");
    }

    private void SetFinalStateAndCreateAcceptAction() {
      //Find final state: find final shift from initial state over grammar root; create accept action in final state on EOF
      var lastShiftAction = Data.InitialState.Actions[_grammar.Root];
      Data.FinalState = lastShiftAction.NewState;
      Data.FinalState.Actions[_grammar.Eof] = ParserAction.CreateAccept();
    }

    private void GenerateNewParserStatesThruShifts(int startIndex, string statePrefix) {
      // Iterate through states (while new ones are created) and create shift transitions and new states 
      for (int index = startIndex; index < Data.States.Count; index++) {
        var state = Data.States[index];
        //Get all possible shifts
        foreach (var term in state.BuilderData.ShiftTerms) {
          var shiftItems = state.BuilderData.ShiftItems.SelectByCurrent(term);
          //the following constructor assigns Transition prop of items, and adds to state.ShiftTransitions
          var trans = new ShiftTransition(state, term, shiftItems); 
          //Get set of shifted cores and find/create target state
          var shiftedCoreItems = shiftItems.GetShiftedCores(); 
          trans.ToState = FindOrCreateState(shiftedCoreItems, statePrefix);
          //Create shift action
          var newAction = ParserAction.CreateShift(trans.ToState);
          state.Actions[term] = newAction;
          //Link items in old/new states
          foreach (var shiftItem in shiftItems) {
            shiftItem.ShiftedItem = trans.ToState.BuilderData.AllItems.FindByCore(shiftItem.Core.ShiftedItem);
          }//foreach shiftItem
        }//foreach term
        //Setup transitions includes and item lookbacks; we have to do it after the "term loop", when all transitions have been 
        // already created and are properly added to state items' lookbacks
      } //for index
      //Now for all newly created states and their transitions, compute transition lookahead sources
      // and setup lookbacks and transition includes
      ComputeTransitionsLookaheadSources(startIndex);
      ComputeItemLookbacksAndTransitionIncludes(startIndex);
    }//method

    private void ComputeTransitionsLookaheadSources(int startStateIndex) {
      for (int i = startStateIndex; i < Data.States.Count; i++) {
        var state = Data.States[i];
        foreach (var trans in state.BuilderData.ShiftTransitions) {
          foreach (var shiftItem in trans.ShiftItems) {
            var source = shiftItem.ShiftedItem;
            while (source.Core.Current != null) {
              trans.LookaheadSources.Add(source);
              if (!source.Core.Current.IsSet(TermOptions.IsNullable)) break;
              source = source.ShiftedItem;
            }//while
          }//foreach shiftItem
        } //foreach trans
      }//for i
    }//method

    private void ComputeItemLookbacksAndTransitionIncludes(int startStateIndex) {
      for (int i = startStateIndex; i < Data.States.Count; i++) {
        var state = Data.States[i];
        var stateData = state.BuilderData;
        //1. Setup initial lookbacks
        foreach (var trans in stateData.ShiftTransitions) {
          var ntTerm = trans.OverTerm as NonTerminal;
          if (ntTerm != null) {
            var expItems = state.BuilderData.AllItems.SelectByLValue(ntTerm);
            foreach (var expItem in expItems)
              expItem.Lookbacks.Add(trans);
          }//if 
        }
        //2. Setup Transition includes using initial lookbacks
        foreach (var trans in stateData.ShiftTransitions) {
          //For each shift item, if item's tail is nullable, then include item's lookbacks
          foreach (var shiftItem in trans.ShiftItems)
            if (shiftItem.Core.TailIsNullable)
              trans.Include(shiftItem.Lookbacks);
        }
        //3. Propagate lookbacks and transition includes
        //PropagateLookbacksAndTransitionsThruShifts(state); 
        foreach (var shiftItem in stateData.ShiftItems) {
          var currItem = shiftItem.ShiftedItem;
          while (currItem != null) {
            currItem.Lookbacks.UnionWith(shiftItem.Lookbacks);
            if (currItem.Transition != null && currItem.Core.TailIsNullable) {
              // if target item's tail is nullable, its transition must include all shiftItem's lookbacks  
              currItem.Transition.Include(shiftItem.Lookbacks);
            }
            currItem = currItem.ShiftedItem;
          }
        }//foreach shiftItem      
      }//for i
    }//method

    private void PropagateLookbacksAndTransitionsThruShifts(ParserState state) {
      foreach (var shiftItem in state.BuilderData.ShiftItems) {
        var currItem = shiftItem.ShiftedItem;
        while (currItem != null) {
          currItem.Lookbacks.UnionWith(shiftItem.Lookbacks);
          if (currItem.Transition != null && currItem.Core.TailIsNullable) {
            // if target item's tail is nullable, its transition must include all shiftItem's lookbacks  
            currItem.Transition.Include(shiftItem.Lookbacks);
          }
          currItem = currItem.ShiftedItem;
        }
      }//foreach shiftItem      
    }

    private ParserState FindOrCreateState(LR0ItemSet coreItems, string statePrefix) {
      string key = ComputeLR0ItemSetKey(coreItems);
      ParserState result;
      if (_stateHash.TryGetValue(key, out result))
        return result;
      return CreateState(coreItems, key, statePrefix); 
    }

    private ParserState CreateState(LR0ItemSet coreItems, string statePrefix) {
      string key = ComputeLR0ItemSetKey(coreItems);
      return CreateState(coreItems, key, statePrefix); 
    }
   
    private ParserState CreateState(LR0ItemSet coreItems, string key, string statePrefix) {
      var result = new ParserState(statePrefix + Data.States.Count);
      result.BuilderData = new ParserStateData(result, coreItems, key);
      Data.States.Add(result);
      _stateHash[key] = result;
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
    public static string ComputeLR0ItemSetKey(LR0ItemSet items) {
      if (items.Count == 0) return "";
      //Copy non-initial items to separate list, and then sort it
      LR0ItemList itemList = new LR0ItemList();
      foreach (var item in items)
        itemList.Add(item);
      //quick shortcut
      if (itemList.Count == 1)
        return itemList[0].ID.ToString();
      itemList.Sort(CompareLR0Items); //Sort by ID
      //now build the key
      StringBuilder sb = new StringBuilder(255);
      foreach (LR0Item item in itemList) {
        sb.Append(item.ID);
        sb.Append(",");
      }//foreach
      return sb.ToString();
    }
    private static int CompareLR0Items(LR0Item x, LR0Item y) {
      if (x.ID < y.ID) return -1;
      if (x.ID == y.ID) return 0;
      return 1;
    }


    #endregion

    #region Computing lookaheads - processing transitions
    //VERY inefficient - should be reimplemented using SCC (strongly-connected components) algorithm for efficient computation
    // of transitive closures
    private void PropagateTransitionsIncludes(int startStateIndex) {
      int time = 0;
      //initial recompute set includes all transitons over non-terminals
      var recomputeSet = new ShiftTransitionSet();
      for (int i = startStateIndex; i < Data.States.Count; i++) {
        var state = Data.States[i]; 
        foreach (var trans in state.BuilderData.ShiftTransitions)
          if ((trans.OverTerm is NonTerminal)) {
            trans._lastChanged = 0; //reset time fields
            trans._lastChecked = 0;
            recomputeSet.Add(trans);
          }
      }

      var newIncludes = new ShiftTransitionSet(); //temp set
      while (recomputeSet.Count > 0) {
        var newRecomputeSet = new ShiftTransitionSet();
        foreach (var trans in recomputeSet) {
          newIncludes.Clear();
          foreach (var child in trans.Includes)
            if (child._lastChanged >= trans._lastChecked)
              newIncludes.UnionWith(child.Includes);
          trans._lastChecked = time++;
          var oldCount = trans.Includes.Count;
          trans.Includes.UnionWith(newIncludes);
          if (trans.Includes.Count > oldCount) {
            trans._lastChanged = time;
            newRecomputeSet.UnionWith(trans.IncludedBy);
          }
        }//foreach trans
        recomputeSet = newRecomputeSet;
      }//while recomputeSet.Count > 0
    }

    #endregion

    #region Computing lookaheads proper
    private void ComputeLookaheads() {
      foreach (var state in Data.States) {
        ComputeLookaheads(state);
      }//foreach state
    }

    // Initial lookahead computation
    private void ComputeLookaheads(ParserState state) {
      var stateData = state.BuilderData;

        //if (!stateData.IsInadequate) return;
      stateData.InitialLookaheadsComputed = true; 
      foreach (var reduceItem in stateData.ReduceItems) {
        //ReducedLookaheadSources
        foreach (var trans in reduceItem.Lookbacks) {
          reduceItem.ReducedLookaheadSources.UnionWith(trans.LookaheadSources);
          foreach (var inclTrans in trans.Includes)
            reduceItem.ReducedLookaheadSources.UnionWith(inclTrans.LookaheadSources);
        }
        //ReducedLookaheads
        foreach (var lkhSource in reduceItem.ReducedLookaheadSources) {
          reduceItem.ReducedLookaheads.Add(lkhSource.Core.Current);
        }
        //AllLookaheads 
        reduceItem.AllLookaheads.UnionWith(reduceItem.ReducedLookaheads);
        foreach (var lkh in reduceItem.ReducedLookaheads) {
          var ntLkhd = lkh as NonTerminal;
          if (ntLkhd != null)
            reduceItem.AllLookaheads.UnionWith(ntLkhd.Firsts);
        }
        //initialize Lookaheads set with terminals from AllLookaheads; 
        foreach (var lkh in reduceItem.AllLookaheads)
          if (lkh is Terminal) {
            reduceItem.Lookaheads.Add(lkh);
          }
        //Sanity check
        if (reduceItem.Lookaheads.Count == 0 && reduceItem.Core.Production.LValue != _language.GrammarData.AugmentedRoot)
          AddError("ParserBuilder error: inadequate state {0}, reduce item '{1}' has no lookaheads.", state.Name, reduceItem.Core.Production);
      }
    }

    private void ComputeStateExpectedLists() {
      foreach (var state in Data.States) {
        ComputeStateExpectedLists(state);
      }//foreach state
    }

    private void ComputeStateExpectedLists(ParserState state) {
      //1. First compute ExpectedTerms
      var stateData = state.BuilderData;
      //1.1 add shift terms to state.ExpectedTerminals and state.ExpectedNonTerminals
      foreach (var term in stateData.ShiftTerms)
        state.ExpectedTerms.Add(term);
      //1.2 Add lookaheads from reduce items
      foreach (var reduceItem in stateData.ReduceItems) 
        foreach(var term in reduceItem.AllLookaheads)
          state.ExpectedTerms.Add(term);
    }//method

    #endregion

    #region Analyzing and resolving conflicts
    private void ComputeAndResolveConflicts() {
      foreach (var state in Data.States) 
        if (state.BuilderData.IsInadequate)
          RecomputeAndResolveConflicts(state);
    }

    private void RecomputeAndResolveConflicts(ParserState state) {
      if (!state.BuilderData.IsInadequate) return;
      state.BuilderData.Conflicts.Clear();
      var allLkhds = new BnfTermSet();
      //reduce/reduce
      foreach (var item in state.BuilderData.ReduceItems) {
        foreach (var lkh in item.Lookaheads) {
          if (allLkhds.Contains(lkh)) {
            state.BuilderData.Conflicts.Add(lkh);
          }
          allLkhds.Add(lkh);
        }//foreach lkh
      }//foreach item
      //shift/reduce
      foreach (var term in state.BuilderData.ShiftTerms)
        if (allLkhds.Contains(term) && !state.BuilderData.JumpLookaheads.Contains(term)) {
          state.BuilderData.Conflicts.Add(term);
        }
      state.BuilderData.Conflicts.ExceptWith(state.BuilderData.ResolvedConflicts);
      state.BuilderData.Conflicts.ExceptWith(state.BuilderData.JumpLookaheads);
      //Now resolve conflicts by hints and precedence
      ResolveConflictsByHints(state);
      ResolveConflictsByPrecedence(state);
    }//method

    private void ResolveConflictsByPrecedence(ParserState state) {
      var stateData = state.BuilderData;
      var oldCount = stateData.ResolvedConflicts.Count; 
      foreach (var conflict in stateData.Conflicts) {
        ResolveConflictByPrecedence(state, conflict);
      }
      if (stateData.ResolvedConflicts.Count > oldCount)
        stateData.Conflicts.ExceptWith(stateData.ResolvedConflicts); 
    }
    private bool ResolveConflictByPrecedence(ParserState state, BnfTerm conflict) {
      var stateData = state.BuilderData;
      if (!conflict.IsSet(TermOptions.UsePrecedence)) return false;
      if (!stateData.ShiftTerms.Contains(conflict)) return false; //it is not shift-reduce
      //first find reduce items
      var reduceItems = stateData.ReduceItems.SelectByLookahead(conflict);
      if (reduceItems.Count > 1) return false; // if it is reduce-reduce conflict, we cannot fix it by precedence
      var reduceItem = reduceItems.First();
      //remove shift action and replace it with operator action
      var oldAction = state.Actions[conflict];
      var action = ParserAction.CreateOperator(oldAction.NewState, reduceItem.Core.Production);
      state.Actions[conflict] = action;
      stateData.ResolvedConflicts.Add(conflict);
      return true; 
    }

    private void ResolveConflictsByHints(ParserState state) {
      var stateData = state.BuilderData;
      var oldCount = stateData.ResolvedConflicts.Count;
      foreach (var conflict in stateData.Conflicts)
        ResolveConflictByHints(state, conflict);
      if (stateData.ResolvedConflicts.Count > oldCount)
        stateData.Conflicts.ExceptWith(stateData.ResolvedConflicts);
    }

    private void ResolveConflictByHints(ParserState state, BnfTerm conflict) {
      if (TryResolveConflictByHints(state, conflict))
        state.BuilderData.ResolvedConflicts.Add(conflict);
    }

    private bool TryResolveConflictByHints(ParserState state, BnfTerm conflict) {
      var stateData = state.BuilderData;
      //reduce hints
      var reduceItems = stateData.ReduceItems.SelectByLookahead(conflict);
      foreach(var reduceItem in reduceItems) 
        if (reduceItem.Core.Hints != null) 
          foreach (var hint in reduceItem.Core.Hints) 
            if (hint.HintType == HintType.ResolveToReduce) {
              var newAction = ParserAction.CreateReduce(reduceItem.Core.Production);
              state.Actions[conflict] = newAction; //replace/add reduce action
              return true; 
            }          
      
      //Shift hints
      var shiftItems = stateData.ShiftItems.SelectByCurrent(conflict);
      foreach (var shiftItem in shiftItems)
        if (shiftItem.Core.Hints != null)
          foreach (var hint in shiftItem.Core.Hints)
            if (hint.HintType == HintType.ResolveToShift) {
              //shift action is already there
              return true;
            }

      //code hints
      // first prepare data for conflict action: reduceProduction (for possible reduce) and newState (for possible shift)
      var reduceProduction = reduceItems.First().Core.Production; //take first of reduce productions
      ParserState newState = (state.Actions.ContainsKey(conflict) ? state.Actions[conflict].NewState : null); 
      // Get all items that might contain hints; first take all shift items and reduce items in conflict;
      // we should also add lookahead sources of reduce items. Lookahead source is an LR item that produces the lookahead, 
      // so if it contains a code hint right before the lookahead term, then it applies to this conflict as well. 
      var allItems = new LRItemList();
      allItems.AddRange(shiftItems);
      foreach (var reduceItem in reduceItems) {
        allItems.Add(reduceItem);
        allItems.AddRange(reduceItem.ReducedLookaheadSources);
      }
      // Scan all items and try to find hint with resolution type Code
      foreach (var item in allItems)
        if (item.Core.Hints != null)
          foreach (var hint in item.Core.Hints)
            if (hint.HintType == HintType.ResolveInCode) {
              //found hint with resolution type "code" - this is instruction to use custom code here to resolve the conflict
              // create new ConflictAction and place it into Actions table
              var newAction = ParserAction.CreateCodeAction(newState, reduceProduction);
              state.Actions[conflict] = newAction; //replace/add reduce action
              return true;
            }
      return false; 
    }

    private void ReportAndSetDefaultActionsForRemainingConflicts() {
      foreach (var state in Data.States) {
        var stateData = state.BuilderData;
        if (stateData.Conflicts.Count == 0) continue;
        //Shift-reduce
        var srConflicts = stateData.GetShiftReduceConflicts(); 
        if (srConflicts.Count > 0)
          ReportParseConflicts("Shift-reduce conflict.", state, srConflicts);
        //Reduce-reduce
        var rrConflicts = stateData.GetReduceReduceConflicts(); 
        if (rrConflicts.Count > 0)  
          ReportParseConflicts("Reduce-reduce conflict.",  state, rrConflicts);
        CreateDefaultActionsForConflicts(state, srConflicts, rrConflicts); 
      }

    }//method

    private void CreateDefaultActionsForConflicts(ParserState state, 
                                                   BnfTermSet shiftReduceConflicts, BnfTermSet reduceReduceConflicts) {
      var stateData = state.BuilderData;
      //Create default actions for these conflicts. For shift-reduce, default action is shift, and shift action already
      // exist for all shifts from the state, so we don't need to do anything. For reduce-reduce create reduce actions
      // for the first reduce item (whatever comes first in the set). 
      foreach (var conflict in reduceReduceConflicts) {
        var reduceItems = stateData.ReduceItems.SelectByLookahead(conflict);
        var action = ParserAction.CreateReduce(reduceItems.First().Core.Production);
        state.Actions[conflict] = action;
      }
      //Update ResolvedConflicts and Conflicts sets
      stateData.ResolvedConflicts.UnionWith(shiftReduceConflicts);
      stateData.ResolvedConflicts.UnionWith(reduceReduceConflicts);
      stateData.Conflicts.ExceptWith(stateData.ResolvedConflicts);
    }

    #endregion

    #region Creating reduce actions
    private void CreateReduceActions() {
      foreach (var state in Data.States) {
        var stateData = state.BuilderData;
        if (stateData.ShiftItems.Count == 0 && stateData.ReduceItems.Count == 1) {
          state.DefaultReduceAction = ParserAction.CreateReduce(stateData.ReduceItems.First().Core.Production);
          continue; 
        } 
        //now create actions
        foreach (var item in state.BuilderData.ReduceItems) {
          //create actions
          foreach (var lkh in item.Lookaheads) {
            if (state.Actions.ContainsKey(lkh)) continue;
            state.Actions[lkh] = ParserAction.CreateReduce(item.Core.Production);
          }
        }//foreach item
      }//foreach state
    }
    #endregion

    #region error reporting
    private void ReportParseConflicts(ParserState state, BnfTermSet shiftReduceConflicts, BnfTermSet reduceReduceConflicts) {
      if (shiftReduceConflicts.Count > 0)
        ReportParseConflicts("Shift-reduce conflict.", state, shiftReduceConflicts);
      if (reduceReduceConflicts.Count > 0)
        ReportParseConflicts("Reduce-reduce conflict.", state, reduceReduceConflicts);
    }

    private void ReportParseConflicts(string description, ParserState state, BnfTermSet lookaheads) {
      string msg = description + " State " + state.Name + ", lookaheads: " + lookaheads.ToString();
      AddError(msg);
    }

    private void AddError(string message, params object[] args) {
      if (args != null && args.Length > 0)
        message = string.Format(message, args);
      _language.Errors.Add(message);
    }

    #endregion
  }//class


}//namespace


