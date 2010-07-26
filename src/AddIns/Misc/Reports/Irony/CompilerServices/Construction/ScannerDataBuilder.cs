using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.CompilerServices.Construction {
  internal class ScannerDataBuilder {
    LanguageData _language;
    Grammar _grammar;

    internal ScannerDataBuilder(LanguageData language) {
      _language = language;
      _grammar = _language.Grammar;
    }

    internal void Build() {
      var data = _language.ScannerData = new ScannerData(_language.GrammarData);
      data.LineTerminators = _grammar.LineTerminators.ToCharArray();
      data.ScannerRecoverySymbols = _grammar.WhitespaceChars + _grammar.Delimiters;
      InitMultilineTerminalsList(data);
      BuildTerminalsLookupTable(data);
      InitTokenFilters(data);
    }

    private static void InitMultilineTerminalsList(ScannerData data) {
      foreach (var terminal in data.GrammarData.Terminals) {
        if (terminal.IsSet(TermOptions.IsMultiline)) {
          data.MultilineTerminals.Add(terminal);
          terminal.MultilineIndex = (byte)(data.MultilineTerminals.Count);
        }
      }
    }

    private static void BuildTerminalsLookupTable(ScannerData data) {
      var grammar = data.GrammarData.Grammar;
      data.TerminalsLookup.Clear();
      data.FallbackTerminals.AddRange(grammar.FallbackTerminals);
      foreach (Terminal term in data.GrammarData.Terminals) {
        IList<string> prefixes = term.GetFirsts();
        if (prefixes == null || prefixes.Count == 0) {
          if (!data.FallbackTerminals.Contains(term))
            data.FallbackTerminals.Add(term);
          continue; //foreach term
        }
        //Go through prefixes one-by-one
        foreach (string prefix in prefixes) {
          if (string.IsNullOrEmpty(prefix)) continue;
          //Calculate hash key for the prefix
          char hashKey = prefix[0];
          if (!grammar.CaseSensitive)
            hashKey = char.ToLower(hashKey);
          TerminalList currentList;
          if (!data.TerminalsLookup.TryGetValue(hashKey, out currentList)) {
            //if list does not exist yet, create it
            currentList = new TerminalList();
            data.TerminalsLookup[hashKey] = currentList;
          }
          //add terminal to the list
          if (!currentList.Contains(term))
            currentList.Add(term);
        }
      }//foreach term
      //Sort all terminal lists by reverse priority, so that terminal with higher priority comes first in the list
      foreach (TerminalList list in data.TerminalsLookup.Values)
        if (list.Count > 1)
          list.Sort(Terminal.ByPriorityReverse);
    }//method

    private static void InitTokenFilters(ScannerData data) {
      data.TokenFilters.AddRange(data.GrammarData.Grammar.TokenFilters);
      //check if we need brace match token filter
      bool needBraceMatchFilter = false;
      foreach(var term in data.GrammarData.Terminals)
        if (term.IsSet(TermOptions.IsBrace)) {
          needBraceMatchFilter = true;
          break; 
        }
      if (needBraceMatchFilter)
        EnsureBraceMatchFilter(data); 
      //initialize filters
      foreach (var filter in data.TokenFilters)
        filter.Init(data.GrammarData);
    }
    private static void EnsureBraceMatchFilter(ScannerData data) {
      foreach (TokenFilter filter in data.TokenFilters)
        if (filter is BraceMatchFilter) return;
      data.TokenFilters.Add(new BraceMatchFilter());
    }

  }//class

}
