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

namespace Irony.CompilerServices {

  public class SymbolTerminalTable : Dictionary<string, SymbolTerminal> {
    public SymbolTerminalTable(StringComparer comparer) : base(100, comparer) { 
    }
  }
  public class SymbolTerminalList : List<SymbolTerminal> { }

  //Represents a fixed symbol. 
  public class SymbolTerminal : Terminal {
    public SymbolTerminal(string symbol, string name)  : base(name) {
      _symbol = symbol;
      base.DisplayName = _symbol;

      #region comments
      // Priority - determines the order in which multiple terminals try to match input for a given current char in the input.
      // For a given input char the scanner looks up the collection of terminals that may match this input symbol. It is the order
      // in this collection that is determined by Priority value - the higher the priority, the earlier the terminal gets a chance 
      // to check the input. 
      //Symbols found in grammar by default have lowest priority to allow other terminals (like identifiers)to check the input first.
      // Additionally, longer symbols have higher priority, so symbols like "+=" should have higher priority value than "+" symbol. 
      // As a result, Scanner would first try to match "+=", longer symbol, and if it fails, it will try "+". 
      #endregion
      base.Priority = LowestPriority + symbol.Length;
    }

    public string Symbol {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _symbol; }
    }  string _symbol;

    public SymbolTerminal IsPairFor;

    #region overrides: TryMatch, GetPrefixes(), ToString() 
    public override Token TryMatch(CompilerContext context, ISourceStream source) {
      if (!source.MatchSymbol(_symbol, !OwnerGrammar.CaseSensitive))
        return null;
      source.Position += _symbol.Length;
      Token tkn = new Token(this, source.TokenStart, Symbol, null);
      return tkn;
    }
    public override IList<string> GetFirsts() {
      return new string[] { _symbol };
    }
    #endregion

    public override void Init(GrammarData grammarData) {
      base.Init(grammarData);
      if (this.EditorInfo != null) return;
      TokenType tknType = TokenType.Identifier;
      if (IsSet(TermOptions.IsOperator))
        tknType |= TokenType.Operator; 
      else if (IsSet(TermOptions.IsDelimiter | TermOptions.IsPunctuation))
        tknType |= TokenType.Delimiter;
      TokenTriggers triggers = TokenTriggers.None;
      if (this.IsSet(TermOptions.IsBrace))
        triggers |= TokenTriggers.MatchBraces;
      if (this.IsSet(TermOptions.IsMemberSelect))
        triggers |= TokenTriggers.MemberSelect;
      TokenColor color = TokenColor.Text; 
      if (IsSet(TermOptions.IsKeyword))
        color = TokenColor.Keyword;
      this.EditorInfo = new TokenEditorInfo(tknType, color, triggers);
    }

    [System.Diagnostics.DebuggerStepThrough]
    public override bool Equals(object obj) {
      return base.Equals(obj);
    }

    [System.Diagnostics.DebuggerStepThrough]
    public override int GetHashCode() {
      return _symbol.GetHashCode();
    }

  }//class


}
