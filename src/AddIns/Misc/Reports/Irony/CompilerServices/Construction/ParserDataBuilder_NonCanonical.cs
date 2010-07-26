using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Irony.CompilerServices.Construction {
  //Non-canonical extensions for LALR
  internal partial class ParserDataBuilder {
    LR0ItemSet _coresToAddWrapTailHint = new LR0ItemSet(); 

    private void SwitchConflictingStatesToNonCanonicalLookaheads() {
      //we do not allow merging non-canonical states into canonical, so we just clear state hash here, effectively 
      // excluding all canonical states created previously from matching state search;
      _stateHash.Clear();
      //Lookaheads and conflicts are already computed in LALR for LALR states.
      var statesWithConflicts = GetStatesWithConflicts(0);
      while (statesWithConflicts.Count > 0) {

        foreach (var conflictState in statesWithConflicts) {
          SwitchStateToNLalrLookaheads(conflictState);
          //We just added non-canonical state at the end of the list, let's generate all successor states
          GenerateNewParserStatesThruShifts(Data.States.Count - 1, "SN");
        }
        PropagateTransitionsIncludes(Data.LalrStateCount);
        RecomputeLookaheadsAndResolveConflicts(Data.LalrStateCount);
        statesWithConflicts = GetStatesWithConflicts(Data.LalrStateCount);
        if (Data.States.Count > 3000) { //protect against infinite looping
          _language.Errors.Add("NLALR process is in indefinite loop, number of states exceeded 3000.");
          return; 
        }
      }
      if (_coresToAddWrapTailHint.Count > 0) 
        ReportCoresToAddWrapTailHint(); 
    }//method

    private ParserStateList GetStatesWithConflicts(int startIndex) {
      var conflictStates = new ParserStateList();
      for (int i = startIndex; i < Data.States.Count; i++) {
        var state = Data.States[i];
        if (!state.BuilderData.IsInadequate || state.BuilderData.Conflicts.Count == 0) continue;
        //if(state.BuilderData.Conflicts.Count > 0)
          conflictStates.Add(state); 
      }//for i
      return conflictStates; 
    }

    private void RecomputeLookaheadsAndResolveConflicts(int startIndex) {
      for (int i = startIndex; i < Data.States.Count; i++) {
        var state = Data.States[i];
        if (state.BuilderData.InitialLookaheadsComputed)
          RecomputeLookaheads(state);
        else
          ComputeLookaheads(state);//if it is the first time, then use full compute
        if (state.BuilderData.IsInadequate) {
          RecomputeAndResolveConflicts(state);
          if (state.BuilderData.Conflicts.Count > 0) {
            DetectConflictsUnresolvableByRestructuring(state);
            DetectNlalrFixableConflicts(state);
          }
        }//if
      }//for i
    }

    #region The game plan
    /*
    Initial condition: we have state S with conflicts on lookaheads in Sc.Conflicts. 
     Each reduce item Ir in S has a set of lookahead sources Ir.ReducedLookaheadSources. 
     Our goal is to a create non-canonical state ncState with proper lookaheds, create jumps to this state 
     on jump lookaheads from S to ncState, remove these jump lookaheads from lookahead sets of reduce items 
     and replace them with non-canonical non-terminal lookaheads.
    1. Compute all jump lookaheads jL and non-canonical lookaheads ncL for state Sc. 
    2. Collect relevant lookahead sources in lkhSources set. For each lookahead source lkhSrc in all reduce items, 
       if lkhSrc.Current.Firsts includes a jump lookahead in jL, include it into lkhSources. 
    3. Collect item cores for non-canonical state into ncCores. For each production Prod in productions 
       of current non-terminal of all items in lkhSources, if Prod.Firsts contains a jump lookahead, then add initial LR0 item of Prod 
       to ncCores.
    4. Add to ncCores all shift items in state Sc that have Current term in jump lookaheads. We need to include 
       such shift items from original state into non-canonical state to allow proper resolution of shift-reduce
       conflicts. We let shift items in current state "compete" with possible reductions to non-canonical lookaheads 
       inside non-canonical state. 
    5. Create (or find existing) non-canonical state Sn from ncCores. 
    6. Assign lookbacks to items in ncState. For each item Src in lkhSources, for each production Prod 
       in Src.Current.Productions, if Prod.DirectFirsts contains jump lookahead, then: 
       find LR item I in Sn with I.Core == Prod.LR0Items[0]; do the following: I.Lookbacks.Add(Src.Transition).
    7. For state S for each reduce item I adjust I.Lookaheads:  remove jump lookaheads from I.Lookaheads, 
         and add those non-canonical lookaheads that are in I.AllLookaheads
*/
    //TODO: one thing to look at - see if all items in ncState should lead to reduce item in some state. 
    //     There may be items (most likely top ones, expansions of original reduced lookahead) that never get 
    //     to full reduce, because we switch back to canonical state on reduction of some "child" non-terminal and
    //     continue through canonical states from there. 
    //     So we don't need to generate target transition states for these items (unreachable states). 
    //     The main trouble is that unreachable state may introduce conflicts that in fact are never realized. 
    #endregion
    private void SwitchStateToNLalrLookaheads(ParserState state) {
      //1. Compute minimal (most expanded) non-canonical lookaheads that resolve all conflicts
      ComputeStateNonCanonicalLookaheads(state);
      var stateData = state.BuilderData;
      //2. Collect reduced lookahead sources and non-terminal lookaheads
      var lkhSources = new LRItemSet();
      var ntSet = new NonTerminalSet(); //All non-terminals in current positions of lkhSources
      foreach(var reduceItem in stateData.ReduceItems)
        foreach (var lkhSource in reduceItem.ReducedLookaheadSources) {
          var ntLkh = lkhSource.Core.Current as NonTerminal;
          if (ntLkh == null) continue; 
          if (!ntLkh.Firsts.Overlaps(stateData.JumpLookaheads))continue;
          lkhSources.Add(lkhSource);
          ntSet.Add(ntLkh); 
        }
      //2. Collect core set for non-canonical state
      var ncCoreSet = new LR0ItemSet();
      foreach(var ntLkh in ntSet) 
        foreach(var prod in ntLkh.Productions)
          if (prod.Firsts.Overlaps(stateData.JumpLookaheads))
            ncCoreSet.Add(prod.LR0Items[0]); 
      //4. Add shift items
      foreach (var shiftItem in stateData.ShiftItems)
        if (stateData.JumpLookaheads.Contains(shiftItem.Core.Current))
          ncCoreSet.Add(shiftItem.Core); 
      //5. Find or create non-canonical state
      var oldStateCount = Data.States.Count;
      var ncState = FindOrCreateState(ncCoreSet, "SN"); //if not found, state is created and added to state list and state hash
      bool ncStateIsNew = Data.States.Count > oldStateCount;
      stateData.JumpTarget = ncState; 
      //6. Setup appropriate lookbacks in items in ncState; 
      // first set lookbacks for items originated from lookaheads of reduce items in original state.
      foreach(var lkhSource in lkhSources) {
        var ntLkh = lkhSource.Core.Current as NonTerminal;
        foreach (var prod in ntLkh.Productions)
          if (prod.Firsts.Overlaps(stateData.JumpLookaheads)) {
            var ncItem = ncState.BuilderData.AllItems.FindByCore(prod.LR0Items[0]);
            ncItem.Lookbacks.Add(lkhSource.Transition);
          }//if 
      }//foreach lkhSource
      //Now items orginated from shift items in original state in step 4 above
      // just copy lookbacks
      foreach (var shiftItem in stateData.ShiftItems)
        if (stateData.JumpLookaheads.Contains(shiftItem.Core.Current)) {
          var ncItem = ncState.BuilderData.ShiftItems.FindByCore(shiftItem.Core);
          shiftItem.ShiftedItem = ncItem; 
          ncItem.Lookbacks.UnionWith(shiftItem.Lookbacks);
          if (ncItem.Transition != null)
          ncItem.Transition.Include(shiftItem.Transition.Includes);
        }
      PropagateLookbacksAndTransitionsThruShifts(ncState); 
      //7. Adjust reduce items lookaheads in original state
      foreach (var reduceItem in stateData.ReduceItems) {
        foreach(var jumpLkh in stateData.JumpLookaheads)
          if (reduceItem.Lookaheads.Contains(jumpLkh))
            reduceItem.Lookaheads.Remove(jumpLkh);
        foreach (var ncLkh in stateData.NonCanonicalLookaheads)
          if (reduceItem.AllLookaheads.Contains(ncLkh))
            reduceItem.Lookaheads.Add(ncLkh);
      }//foreach reduceItem
      // 8. Create jump action to non-canonical state, remove shifts on jump lookaheads
      state.JumpAction = ParserAction.CreateJump(ncState);
      foreach (var jumpTerm in state.BuilderData.JumpLookaheads)
        if (state.Actions.ContainsKey(jumpTerm))
          state.Actions.Remove(jumpTerm); 
      //9. Complete generating states
      state.BuilderData.Conflicts.ExceptWith(state.BuilderData.JumpLookaheads);
    }//method

    #region some explanations
    //Computes  non-canonical lookaheads and jump lookaheads - those that cause jump
    // to non-canonical state
    // We are doing it top-down way, starting from most reduced lookaheads - they are not conflicting. 
    // (If there were conflicting reduced lookaheads in a state initially, the grammar transformation algorithm 
    // should have already wrapped them into non-conflicting "tail" non-terminals.) 
    // We want to eliminate reduced lookaheads as much as possible, and replace them with expanded "child" 
    // terms, to have only those non-canonical lookaheads that are absolutely necessary. 
    // So for each reduced lookahead we check if we can replace it with its expanded, "child" terms 
    // (from DirectFirsts set). We do it only if lookaheads child terms are all non-conflicting as lookaheads in 
    // the state. If however, at least one child is conflicting, the reduced parent should stay. 
    // What if we have some children conflicting and some not? We leave the parent reduced lookahead in state, 
    // to cover (hide) the conflicting children, but we also add non-conflicting children as well, to allow 
    // the parser automaton to use them (in canonical state) as soon as they are recognized, without need 
    // to reduce the parent and switch back to canonical state. 
    #endregion 
    private void ComputeStateNonCanonicalLookaheads(ParserState state) {
      var stateData = state.BuilderData;
      //rename for shorter code
      var jumps = stateData.JumpLookaheads; // conflicting lookaheads, that must result in jump to non-canonical state         
      var valids = stateData.NonCanonicalLookaheads; // valid non-canonical lookaheads, non-terminals only
      jumps.Clear(); 
      valids.Clear(); 
      var alreadyChecked = new BnfTermSet();
      var toCheck = new BnfTermSet();   //terms to check for expansion
      //1. precompute initial set to check
      foreach (var reduceItem in stateData.ReduceItems) 
        toCheck.UnionWith(reduceItem.ReducedLookaheads);
      toCheck.RemoveWhere(t => t is Terminal); //we are interested in non-terminals only
      //2. Try to expand all initial (reduced) lookaheads, and replace original lookaheads with expanded versions
      while (toCheck.Count > 0) { // do until no terms to check left
        var lkhInCheck = toCheck.First() as NonTerminal;
        toCheck.Remove(lkhInCheck);
        //to prevent repeated checking of mutually recursive terms 
        if (alreadyChecked.Contains(lkhInCheck)) continue; 
        alreadyChecked.Add(lkhInCheck);
        //Now check children for conflicts; go through all direct firsts of lkhInCheck and check them for conflicts
        bool hasJumpChild = false; 
        foreach (var lkhChild in lkhInCheck.DirectFirsts) {
          if (lkhChild == lkhInCheck) continue;
          if (jumps.Contains(lkhChild)) {
            hasJumpChild = true;
            continue;
          }
          var ntChild = lkhChild as NonTerminal;
          if (ntChild != null && valids.Contains(ntChild))   continue;
          //the child has not been tested yet; check if it is a conflict in current state
          var occurCount = GetLookaheadOccurenceCount(state, lkhChild);
          if (occurCount > 1) {            
            //possible conflict, check precedence
            if (lkhChild.IsSet(TermOptions.UsePrecedence)) {
              if (ntChild != null) {
                valids.Add(ntChild); //if it is terminal, it is valid;
                if (!alreadyChecked.Contains(lkhChild))  toCheck.Add(ntChild); 
              } //if ntChild
            } else {
              //conflict!
              hasJumpChild = true;
              jumps.Add(lkhChild);
              //if it is non-terminal, add its Firsts to conflict as well
              if (ntChild != null) {
                jumps.UnionWith(ntChild.Firsts);
                //valids.ExceptWith(ntChild.Firsts); 
              }
            }//if IsSet... else...

          } else { //occurCount == 1
            //no conflict: if it is non-terminal, add it to toCheck set to check in the future 
            if (ntChild != null && !alreadyChecked.Contains(ntChild))
              toCheck.Add(ntChild); //if nonterminal and not checked yet, add it to toCheck for further checking
          }//if ...else...
        }//foreach lkhChild
        //Ok, we finished checking all direct children; if at least one of them has conflict, 
        // then lkhInCheck (parent) must stay as a lookahead - we cannot fully expand it replacing by all children
        if (hasJumpChild)
          valids.Add(lkhInCheck);         
      }//while toCheck.Count > 0
      //remove conflicts
      stateData.Conflicts.Clear();
    }//method

    private int GetLookaheadOccurenceCount(ParserState state, BnfTerm lookahead) {
      var result = 0;
      foreach (var reduceItem in state.BuilderData.ReduceItems)
        if (reduceItem.AllLookaheads.Contains(lookahead))
          result++;
      //add 1 if it is shift term
      if (state.BuilderData.ShiftTerms.Contains(lookahead))
        result++;
      return result; 
    }

    //Detect conflicts that cannot be handled by non-canonical NLALR method directly, by may be fixed by grammar transformation
    private void DetectNlalrFixableConflicts(ParserState state) {
      var stateData = state.BuilderData;
      //compute R-R and S-R conflicting lookaheads
      var reduceLkhds = new BnfTermSet();
      var rrConflicts = new BnfTermSet();
      var srConflicts = new BnfTermSet();
      foreach (var reduceItem in state.BuilderData.ReduceItems) {
        foreach (var lkh in reduceItem.ReducedLookaheads) {
          if (stateData.ShiftTerms.Contains(lkh)) {
            if (!lkh.IsSet(TermOptions.UsePrecedence))
              srConflicts.Add(lkh); //S-R conflict
          } else if (reduceLkhds.Contains(lkh))
            rrConflicts.Add(lkh); //R-R conflict
          reduceLkhds.Add(lkh);
        }//foreach lkh
      }//foreach item
      if (srConflicts.Count == 0 && rrConflicts.Count == 0) return; 
      //Collect all cores to recommend for adding WrapTail hint.
      var allConflicts = new BnfTermSet();
      allConflicts.UnionWith(srConflicts);
      allConflicts.UnionWith(rrConflicts); 
      foreach (var conflict in allConflicts) {
        var conflictingShiftItems = state.BuilderData.ShiftItems.SelectByCurrent(conflict);
        foreach (var item in conflictingShiftItems)
          if (!item.Core.IsInitial) //only non-initial
          _coresToAddWrapTailHint.Add(item.Core);
        foreach (var reduceItem in state.BuilderData.ReduceItems) {
          var conflictingSources = reduceItem.ReducedLookaheadSources.SelectByCurrent(conflict);
          foreach (var source in conflictingSources)
            _coresToAddWrapTailHint.Add(source.Core); 
        }
      }
      //still report them as conflicts
      ReportParseConflicts(state, srConflicts, rrConflicts);
      //create default actions and remove conflicts from list so we don't deal with them anymore
      foreach (var conflict in rrConflicts) {
        var reduceItems = stateData.ReduceItems.SelectByReducedLookahead(conflict);
        var action = ParserAction.CreateReduce(reduceItems.First().Core.Production);
        state.Actions[conflict] = action;
      }
      //Update ResolvedConflicts and Conflicts sets
      stateData.ResolvedConflicts.UnionWith(srConflicts);
      stateData.ResolvedConflicts.UnionWith(rrConflicts);
      stateData.Conflicts.ExceptWith(stateData.ResolvedConflicts);
    }//method

    /* Detecting conflicts that cannot be resolved by tail wrapping 
         1. Shift-reduce conflicts. If inadequate state S has shift item based on the same core as source 
             of one of reduced lookaheads of reduce item, then the conflict is unresolvable - 
             no wrapping of lookahead would resolve ambiguity
         2. Reduce-reduce conflict. If reduce items in inadequate state have reduced lookaheads 
            with sources having the same core (LR0 item) then we have unresolvable conflict. Wrapping of the item tail would produce
            the same new non-terminal as lookahead in both conflicting reduce items. 
    */
    private void DetectConflictsUnresolvableByRestructuring(ParserState state) {
      //compute R-R and S-R conflicting lookaheads
      var rrConflicts = new BnfTermSet();
      var srConflicts = new BnfTermSet();
      var conflictSources = new LR0ItemSet();
      foreach (var conflict in state.BuilderData.Conflicts) {
        var nonConflictingSourceCores = new LR0ItemSet();
        foreach (var reduceItem in state.BuilderData.ReduceItems) {
          foreach (var source in reduceItem.ReducedLookaheadSources) {
            if (source.Core.Current != conflict) continue;
            if (state.BuilderData.Cores.Contains(source.Core)) {  //we have unresolvable shift-reduce
              srConflicts.Add(source.Core.Current);
              conflictSources.Add(source.Core);
            } else if (nonConflictingSourceCores.Contains(source.Core)) {        //unresolvable reduce-reduce
              rrConflicts.Add(source.Core.Current);
              conflictSources.Add(source.Core);
            } else
              nonConflictingSourceCores.Add(source.Core);
          }//foreach source
        }//foreach item
      }//foreach conflict
      if (srConflicts.Count > 0)
        ReportParseConflicts("Ambiguous grammar, unresolvable shift-reduce conflicts.", state, srConflicts);
      if (rrConflicts.Count > 0)
        ReportParseConflicts("Ambiguous grammar, unresolvable reduce-reduce conflicts.", state, rrConflicts);
      //create default actions and remove them from conflict list, so we don't deal with them anymore
      CreateDefaultActionsForConflicts(state, srConflicts, rrConflicts); 
    } // method


    //We use this method to recompute lookaheads after we add some non-canonical states, so existing states can get 
    // more lookaheads as a result of merging into existing states on shift transitions. We must be careful here
    // to not destroy previously created non-canonical lookahead sets; for efficiency we cancel computations
    // as soon as it is clear that nothing changed. 
    private void RecomputeLookaheads(ParserState state) {
      var stateData = state.BuilderData;
      bool sourcesChanged = false; 
      foreach (var reduceItem in stateData.ReduceItems) {
        //ReducedLookaheadSources
        var oldCount = reduceItem.ReducedLookaheadSources.Count;
        foreach (var trans in reduceItem.Lookbacks) {
          reduceItem.ReducedLookaheadSources.UnionWith(trans.LookaheadSources);
          foreach (var inclTrans in trans.Includes)
            reduceItem.ReducedLookaheadSources.UnionWith(inclTrans.LookaheadSources);
        }
        if (reduceItem.ReducedLookaheadSources.Count == oldCount) continue;
        sourcesChanged = true; 
        //ReducedLookaheads
        oldCount = reduceItem.ReducedLookaheads.Count;
        foreach (var lkhSource in reduceItem.ReducedLookaheadSources) {
          reduceItem.ReducedLookaheads.Add(lkhSource.Core.Current);
        }
        if (reduceItem.ReducedLookaheads.Count == oldCount) continue;
        //AllLookaheads 
        reduceItem.AllLookaheads.UnionWith(reduceItem.ReducedLookaheads);
        foreach (var lkh in reduceItem.ReducedLookaheads) {
          var ntLkhd = lkh as NonTerminal;
          if (ntLkhd != null)
            reduceItem.AllLookaheads.UnionWith(ntLkhd.Firsts);
        }
      }//foreach reduce item
      //If we have state with jump already created, and sources change, then we have to wipe out jump and recreate it
      if (sourcesChanged && state.JumpAction != null) {
        state.JumpAction = null;
        stateData.JumpLookaheads.Clear(); 
      }
      //For each item fill in the lookaheads list
      foreach (var reduceItem in stateData.ReduceItems) {
        //Copy terminals that are not jump lookaheads into item's Lookaheads and to state.ExpectedTerminals
        foreach (var lkh in reduceItem.AllLookaheads) {
          if (lkh is Terminal && !stateData.JumpLookaheads.Contains(lkh))
            reduceItem.Lookaheads.Add(lkh);
        }
        //sanity check
        if (reduceItem.Lookaheads.Count == 0)
          AddError("ParserBuilder error: inadequate state {0}, reduce item '{1}' has no lookaheads.", state.Name, reduceItem.Core.Production);
      }
    }//method

    private void ReportCoresToAddWrapTailHint() {
      StringBuilder sb = new StringBuilder();
      foreach (var core in _coresToAddWrapTailHint) 
        sb.Append("" + core.ToString() + "\r\n");
      AddError("\r\nParser builder detected parsing conflicts that can be resolved by restructuring.\r\n" + 
               "Add WrapTail() hint method in place of '.' to the following productions in original grammar: \r\n" + 
               sb.ToString()); 
    }

  }//class
}//namespace
