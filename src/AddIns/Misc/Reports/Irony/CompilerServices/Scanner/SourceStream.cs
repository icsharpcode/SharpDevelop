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
//using System.Text.RegularExpressions;

namespace Irony.CompilerServices {

  public interface ISourceStream {

    int Position { get;set;}
    char CurrentChar { get;} //char at Position
    char NextChar { get;}    //preview char at Position+1
    bool MatchSymbol(string symbol, bool ignoreCase);

    string Text { get; set; } //returns entire text buffer
    //returns substring from TokenStart.Position till (Position - 1)
    string GetLexeme();
    SourceLocation TokenStart { get; set;}
    int TabWidth { get;}
    bool EOF();
  }

  #region SourceFile class
  public class SourceStream : ISourceStream {

    public const int DefaultTabWidth = 8;
    
    public SourceStream(string text) : this(text, 0, DefaultTabWidth) { }
    public SourceStream(string text, int offset) : this(text, offset, DefaultTabWidth) { }
    public SourceStream(string text, int offset, int tabWidth) {
      _text = text;
      _tabWidth = tabWidth;
      _position = offset;
      TokenStart = new SourceLocation(); 
    }
    public int TabWidth {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _tabWidth; }
      set {_tabWidth = value;}
    } int  _tabWidth; // = 8;

    #region ISourceStream Members
    public int Position {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _position; }
      set { _position = value; }
    } int _position;

    [System.Diagnostics.DebuggerStepThrough]
    public bool EOF() {
      return _position >= Text.Length;
    }
#if DEBUG
    //Slower versions with boundary checking
    public char CurrentChar {
      [System.Diagnostics.DebuggerStepThrough]
      get {
        if (_position >= Text.Length) return '\0';
        return _text[_position];
      }
    }

    public char NextChar {
      [System.Diagnostics.DebuggerStepThrough]
      get {
        if (_position + 1 >= Text.Length) return '\0';
        return _text[_position + 1];
      }
    }
#else
    //Fast versions without explicit bounds check - remember, try/catch costs nothing at runtime if there's no exception
    public char CurrentChar {
      [System.Diagnostics.DebuggerStepThrough]
      get {
        try {
          return _text[_position];
        } catch { return '\0'; }
      }
    }

    public char NextChar {
      [System.Diagnostics.DebuggerStepThrough]
      get {
        try {
          return _text[_position + 1];
        } catch { return '\0'; }
      }
    }
#endif
    public bool MatchSymbol(string symbol, bool ignoreCase) {
      try {
        int cmp = string.Compare(_text, _position, symbol, 0, symbol.Length, ignoreCase);
        return cmp == 0;
      } catch { 
        //exception may be thrown if Position + symbol.length > text.Length; 
        // this happens not often, only at the very end of the file, so we don't check this explicitly
        //but simply catch the exception and return false. Remember, try/catch block is free (no overhead)
        // if exception is not thrown. 
        return false;
      }
    }

    public string Text {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _text; }
      set { _text = value; }
    }  string _text;

    //returns substring from TokenStart.Position till (Position - 1)
    [System.Diagnostics.DebuggerStepThrough]
    public string GetLexeme() {
      string text = _text.Substring(_tokenStart.Position, _position - _tokenStart.Position);
      return text;
    }
    public SourceLocation TokenStart {
      [System.Diagnostics.DebuggerStepThrough]
      get { return _tokenStart; }
      set { _tokenStart = value; }
    } SourceLocation  _tokenStart;

    #endregion

    public override string ToString() {
      return SourceToString(this);
    }

    internal static string SourceToString(ISourceStream source) {
      try {
        string text = source.Text;
        var pos = source.Position;
        //show just 30 chars from current position
        if (pos + 30 < text.Length)
          return text.Substring(pos, 30);
        else
          return text.Substring(pos);
      } catch (Exception) {
        return "Scanner, current =" + source.CurrentChar;
      }
    }

  }//class
  #endregion

}//namespace
