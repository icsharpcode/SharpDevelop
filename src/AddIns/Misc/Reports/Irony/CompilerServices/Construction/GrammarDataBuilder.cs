using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.CompilerServices.Construction {

  internal class GrammarDataBuilder {
    LanguageData _language;
    Grammar _grammar;
    GrammarData _grammarData;
    int _unnamedCount; //internal counter for generating names for unnamed non-terminals
    internal int _lastItemId; //each LR0Item gets its unique ID, last assigned (max) Id is kept in this field

    internal GrammarDataBuilder(LanguageData language) {
      _language = language;
      _grammar = _language.Grammar;
    }

    internal void Build() {
      _grammarData = _language.GrammarData = new GrammarData(_language);
      _grammarData.AugmentedRoot = new NonTerminal(_grammar.Root.Name + "'");
      _grammarData.AugmentedRoot.Rule = _grammar.Root + _grammar.Eof;
      CollectTermsFromGrammar();
      InitTermLists(_grammarData);
      CreateProductions();
      ComputeNonTerminalsNullability(_grammarData);
      ComputeTailsNullability(_grammarData);
      ComputeFirsts(_grammarData);
      if (_grammar.FlagIsSet(LanguageFlags.AutoDetectTransient))
        DetectTransientNonTerminals(_grammarData);
      ValidateGrammar(); 
    }

    private void CollectTermsFromGrammar() {
      _unnamedCount = 0;
      _grammarData.AllTerms.Clear();
      //Start with NonGrammarTerminals, and set IsNonGrammar flag
      foreach (Terminal t in _grammarData.Grammar.NonGrammarTerminals) {
        t.SetOption(TermOptions.IsNonGrammar);
        _grammarData.AllTerms.Add(t);
      }
      CollectTermsRecursive(_grammarData.AugmentedRoot);
    }

    private void CollectTermsRecursive(BnfTerm term) {
      // Do not add pseudo terminals defined as static singletons in Grammar class (Empty, Eof, etc)
      //  We will never see these terminals in the input stream.
      //   Filter them by type - their type is exactly "Terminal", not derived class. 
      if (term.GetType() == typeof(Terminal)) return;
      if (_grammarData.AllTerms.Contains(term)) return;
      _grammarData.AllTerms.Add(term);
      NonTerminal nt = term as NonTerminal;
      if (nt == null) return;

      if (nt.Name == null) {
        if (nt.Rule != null && !string.IsNullOrEmpty(nt.Rule.Name))
          nt.Name = nt.Rule.Name;
        else
          nt.Name = "NT" + (_unnamedCount++);
      }
      if (nt.Rule == null) {
        _language.Errors.Add("Non-terminal " + nt.Name + " has uninitialized Rule property.");
        return;
      }
      //check all child elements
      foreach (BnfTermList elemList in nt.Rule.Data)
        for (int i = 0; i < elemList.Count; i++) {
          BnfTerm child = elemList[i];
          if (child == null) {
            _language.Errors.Add("Rule for NonTerminal " + nt.Name + " contains null as an operand in position " + i.ToString() + " in one of productions.");
            continue; //for i loop 
          }
          //Check for nested expression - convert to non-terminal
          BnfExpression expr = child as BnfExpression;
          if (expr != null) {
            child = new NonTerminal(null, expr);
            elemList[i] = child;
          }
          CollectTermsRecursive(child);
        }//for i
    }//method

    private static void InitTermLists(GrammarData data) {
      //Collect terminals and NonTerminals
      foreach (BnfTerm term in data.AllTerms) {  //remember - we may have hints, so it's not only terminals and non-terminals
        if (term is NonTerminal) data.NonTerminals.Add((NonTerminal)term);
        if (term is Terminal) data.Terminals.Add((Terminal)term);
      }
      data.Terminals.Sort(Terminal.ByName);
      //Mark keywords - any "word" symbol directly mentioned in the grammar
      if (data.Grammar.FlagIsSet(LanguageFlags.AutoDetectKeywords)) 
        foreach (var term in data.Terminals) {
          var symTerm = term as SymbolTerminal;
          if (symTerm == null) continue;
          if (symTerm.Symbol.Length > 0 && char.IsLetter(symTerm.Symbol[0]))
            symTerm.SetOption(TermOptions.IsKeyword); 
        }//foreach term
      //Init all terms
      foreach (BnfTerm term in data.AllTerms)
        term.Init(data);
    }//method

    private void CreateProductions() {
      _lastItemId = 0;
      //CheckWrapTailHints() method may add non-terminals on the fly, so we have to use for loop here (not foreach)
      for (int i = 0; i < _grammarData.NonTerminals.Count; i++) {
        var nt = _grammarData.NonTerminals[i];
        nt.Productions.Clear();
        //Get data (sequences) from both Rule and ErrorRule
        BnfExpressionData allData = new BnfExpressionData();
        allData.AddRange(nt.Rule.Data);
        if (nt.ErrorRule != null)
          allData.AddRange(nt.ErrorRule.Data);
        //actually create productions for each sequence
        foreach (BnfTermList prodOperands in allData) {
          CheckWrapTailHints(_grammarData, nt, prodOperands);
          Production prod = CreateProduction(nt, prodOperands);
          nt.Productions.Add(prod);
        }//foreach prodOperands
      }
    }

    private static void CheckWrapTailHints(GrammarData data, NonTerminal nonTerminal, BnfTermList operands) {
      //WrapTail hint doesn't make sense in last position, so we start with Count-2
      for (int i = operands.Count - 2; i >= 0; i--) {
        var hint = operands[i] as GrammarHint;
        if (hint == null || hint.HintType != HintType.WrapTail) continue;
        //we have WrapTail hint; wrap all operands after this into new non-terminal
        var wrapNt = new NonTerminal(nonTerminal.Name + "_tail" + nonTerminal._tailCount++);
        wrapNt.SetOption(TermOptions.IsTransient);
        wrapNt.Rule = new BnfExpression();
        for (int j = i + 1; j < operands.Count; j++) {
          wrapNt.Rule.Data[0].Add(operands[j]);
        }
        operands.RemoveRange(i, operands.Count - i);
        operands.Add(wrapNt);
        data.AllTerms.Add(wrapNt);
        data.NonTerminals.Add(wrapNt);
      }//for i
    }

    private Production CreateProduction(NonTerminal lvalue, BnfTermList operands) {
      Production prod = new Production(lvalue);
      GrammarHintList hints = null;
      //create RValues list skipping Empty terminal and collecting grammar hints
      foreach (BnfTerm operand in operands) {
        if (operand == Grammar.CurrentGrammar.Empty)
          continue;
        //Collect hints as we go - they will be added to the next non-hint element
        GrammarHint hint = operand as GrammarHint;
        if (hint != null) {
          if (hints == null) hints = new GrammarHintList();
          hints.Add(hint);
          continue;
        }
        //Check if it is a Terminal or Error element
        Terminal t = operand as Terminal;
        if (t != null) {
          prod.Flags |= ProductionFlags.HasTerminals;
          if (t.Category == TokenCategory.Error) prod.Flags |= ProductionFlags.IsError;
        }
        //Add the operand info and LR0 Item
        LR0Item item = new LR0Item(_lastItemId++, prod, prod.RValues.Count, hints);
        prod.LR0Items.Add(item);
        prod.RValues.Add(operand);
        hints = null;
      }//foreach operand
      //set the flags
      if (prod.RValues.Count == 0)
        prod.Flags |= ProductionFlags.IsEmpty;
      //Add final LRItem
      prod.LR0Items.Add(new LR0Item(_lastItemId++, prod, prod.RValues.Count, hints));
      return prod;
    }
    private static void ComputeNonTerminalsNullability(GrammarData data) {
      NonTerminalList undecided = data.NonTerminals;
      while (undecided.Count > 0) {
        NonTerminalList newUndecided = new NonTerminalList();
        foreach (NonTerminal nt in undecided)
          if (!ComputeNullability(nt))
            newUndecided.Add(nt);
        if (undecided.Count == newUndecided.Count) return;  //we didn't decide on any new, so we're done
        undecided = newUndecided;
      }//while
    }

    private static bool ComputeNullability(NonTerminal nonTerminal) {
      foreach (Production prod in nonTerminal.Productions) {
        if (prod.RValues.Count == 0) {
          nonTerminal.SetOption(TermOptions.IsNullable);
          return true; //decided, Nullable
        }//if 
        //If production has terminals, it is not nullable and cannot contribute to nullability
        if (prod.IsSet(ProductionFlags.HasTerminals)) continue;
        //Go thru all elements of production and check nullability
        bool allNullable = true;
        foreach (BnfTerm child in prod.RValues) {
          allNullable &= child.IsSet(TermOptions.IsNullable);
        }//foreach child
        if (allNullable) {
          nonTerminal.SetOption(TermOptions.IsNullable);
          return true;
        }
      }//foreach prod
      return false; //cannot decide
    }

    private static void ComputeTailsNullability(GrammarData data) {
      foreach (var nt in data.NonTerminals) {
        foreach (var prod in nt.Productions) {
          var count = prod.LR0Items.Count;
          for (int i = count - 1; i >= 0; i--) {
            var item = prod.LR0Items[i];
            item.TailIsNullable = true;
            if (item.Current == null) continue;
            if (!item.Current.IsSet(TermOptions.IsNullable))
              break; //for i
          }//for i
        }//foreach prod
      }
    }

    //computes DirectFirsts, Firsts for non-terminals and productions
    private static void ComputeFirsts(GrammarData data) {
      //compute prod direct firsts and initialize NT.Firsts
      foreach (var nt in data.NonTerminals) {
        foreach (var prod in nt.Productions) {
          foreach (var term in prod.RValues) {
            prod.DirectFirsts.Add(term);
            nt.DirectFirsts.Add(term);
            nt.Firsts.Add(term);
            if (!term.IsSet(TermOptions.IsNullable)) break; //foreach term
          }
        }
      }//foreach nt

      //propagate NT.Firsts
      int time = 0;
      var done = false;
      var newSet = new BnfTermSet();
      while (!done) {
        done = true;
        foreach (var nt in data.NonTerminals) {
          newSet.Clear();
          foreach (var first in nt.Firsts) {
            var ntFirst = first as NonTerminal;
            if (ntFirst != null && ntFirst._lastChanged >= nt._lastChecked)
              newSet.UnionWith(ntFirst.Firsts);
          }
          nt._lastChecked = time++;
          var oldCount = nt.Firsts.Count;
          nt.Firsts.UnionWith(newSet);
          if (nt.Firsts.Count > oldCount) {
            done = false;
            nt._lastChanged = time;
          }
        }//foreach nt
      }//while

      //compute prod.Firsts
      foreach (var nt in data.NonTerminals) {
        foreach (var prod in nt.Productions) {
          prod.Firsts.UnionWith(prod.DirectFirsts);
          foreach (var directFirst in prod.DirectFirsts) {
            var ntDirectFirst = directFirst as NonTerminal;
            if (ntDirectFirst != null)
              prod.Firsts.UnionWith(ntDirectFirst.Firsts);
          }//foreach directFirst
        }//foreach prod
      }//foreach nt
    }//method


    private static void DetectTransientNonTerminals(GrammarData data) {
      foreach (NonTerminal nt in data.NonTerminals) {
        var transient = true;
        foreach (var prod in nt.Productions)
          if (prod.RValues.Count > 1) {
            transient = false;
            break;
          }
        if (transient)
          nt.SetOption(TermOptions.IsTransient);
      }
    }

    #region Grammar Validation
    private void ValidateGrammar() {
      //Check CreateAst flag and give a warning if this flag is not set, but node types or NodeCreator methods are assigned
      // in any of non-terminals
      if (!_grammar.FlagIsSet(LanguageFlags.CreateAst)) {
        var ntSet = new BnfTermSet();
        foreach (var nt in _grammarData.NonTerminals)
          if (nt.NodeCreator != null || nt.NodeType != null)
            ntSet.Add(nt); 
        if (ntSet.Count > 0)
          this._language.Errors.Add("Warning: LanguageFlags.CreateAst flag is not set in grammar's Flags, but there are" +
            " non-terminals that have NodeType or NodeCreator property set. If you want Irony to construct AST tree during parsing," +
            " set CreateAst flag in Grammar. Non-terminals: " + ntSet.ToString()); 
      }
    }//method
    #endregion

  }//class
}
