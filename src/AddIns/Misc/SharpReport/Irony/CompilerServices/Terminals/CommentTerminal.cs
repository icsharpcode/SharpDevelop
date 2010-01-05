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

  public class CommentTerminal : Terminal {
    public CommentTerminal(string name, string startSymbol, params string[] endSymbols) : base(name, TokenCategory.Comment) {
      this.StartSymbol = startSymbol;
      this.EndSymbols = new StringList();
      EndSymbols.AddRange(endSymbols);
      Priority = Terminal.HighestPriority; //assign max priority
    }

    public string StartSymbol;
    public StringList EndSymbols;
    private char[] _endSymbolsFirsts;
    private bool _isLineComment; //true if NewLine is one of EndSymbols; if yes, EOF is also considered a valid end symbol


    #region overrides
    public override void Init(GrammarData grammarData) {
      base.Init(grammarData);
      //_endSymbolsFirsts char array is used for fast search for end symbols using String's method IndexOfAny(...)
      _endSymbolsFirsts = new char[EndSymbols.Count];
      for (int i = 0; i < EndSymbols.Count; i++) {
        string sym = EndSymbols[i];
        _endSymbolsFirsts[i] = sym[0];
        _isLineComment |= sym.Contains("\n");
        if (!_isLineComment)
          SetOption(TermOptions.IsMultiline); 
      }
      if (this.EditorInfo == null) {
        TokenType ttype = _isLineComment ? TokenType.LineComment : TokenType.Comment;
        this.EditorInfo = new TokenEditorInfo(ttype, TokenColor.Comment, TokenTriggers.None);
      }
    }

    public override Token TryMatch(CompilerContext context, ISourceStream source) {
      Token result;
      if (context.ScannerState.Value != 0) {
        // we are continuing in line mode - restore internal env (none in this case)
        context.ScannerState.Value = 0;
      } else {
        //we are starting from scratch
        if (!BeginMatch(context, source)) return null;
      }
      result = CompleteMatch(context, source);
      if (result != null) return result;
      //if it is LineComment, it is ok to hit EOF without final line-break; just return all until end.
      if (_isLineComment) 
        return new Token(this, source.TokenStart, source.GetLexeme(), null);
      if (context.Mode == CompileMode.VsLineScan)
        return CreateIncompleteToken(context, source);
      return context.CreateErrorTokenAndReportError(source.TokenStart, string.Empty, "Unclosed comment block");
    }

    private Token CreateIncompleteToken(CompilerContext context, ISourceStream source) {
      source.Position = source.Text.Length;
      Token result = new Token(this, source.TokenStart, source.GetLexeme(), null);
      result.Flags |= TokenFlags.IsIncomplete;
      context.ScannerState.TerminalIndex = this.MultilineIndex;
      return result; 
    }

    private bool BeginMatch(CompilerContext context, ISourceStream source) {
      //Check starting symbol
      if (!source.MatchSymbol(StartSymbol, !OwnerGrammar.CaseSensitive)) return false;
      source.Position += StartSymbol.Length;
      return true; 
    }
    private Token CompleteMatch(CompilerContext context, ISourceStream source) {
      //Find end symbol
      while (!source.EOF()) {
        int firstCharPos;
        if (EndSymbols.Count == 1)
          firstCharPos = source.Text.IndexOf(EndSymbols[0], source.Position);
        else 
          firstCharPos = source.Text.IndexOfAny(_endSymbolsFirsts, source.Position);
        if (firstCharPos < 0) {
          source.Position = source.Text.Length;
          return null; //indicating error
        }
        //We found a character that might start an end symbol; let's see if it is true.
        source.Position = firstCharPos;
        foreach (string endSymbol in EndSymbols) {
          if (source.MatchSymbol(endSymbol, !OwnerGrammar.CaseSensitive)) {
            //We found end symbol; eat end symbol only if it is not line comment.
            // For line comment, leave LF symbol there, it might be important to have a separate LF token
            if (!_isLineComment)
              source.Position += endSymbol.Length;
            return new Token(this, source.TokenStart, source.GetLexeme(), null);
          }//if
        }//foreach endSymbol
        source.Position++; //move to the next char and try again    
      }//while
      return null; //might happen if we found a start char of end symbol, but not the full endSymbol
    }//method

    public override IList<string> GetFirsts() {
      return new string[] { StartSymbol };
    }
    #endregion
  }//CommentTerminal class


}
