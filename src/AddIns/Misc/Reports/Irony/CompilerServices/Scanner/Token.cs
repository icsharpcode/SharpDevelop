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
using System.Xml;

namespace Irony.CompilerServices {

  public enum TokenFlags {
    IsIncomplete = 0x01,
  }

  public enum TokenCategory {
    Literal,
    Content,
    Outline, //newLine, indent, dedent
    Comment,
    Error,
  }

  public class TokenList : List<Token> {}
  //Tokens are produced by scanner and fed to parser, optionally passing through Token filters in between. 
  public class Token  {
    private Terminal _terminal; 
    public readonly SourceLocation Location; 
    public readonly string Text;
    
    public object Value {
      get { return _value; }
    } object _value;

    public string ValueString {
      get { return _valueString; }
    } string _valueString; 

    public object Details;
    public TokenFlags Flags; 
    public TokenEditorInfo EditorInfo;
    public SymbolTerminal AsSymbol;

    public Token(Terminal term, SourceLocation location, string text, object value)  {
      SetTerminal(term);
      this.AsSymbol = term as SymbolTerminal;
      Location = location;
      Text = text;
      SetValue(value); 
    }
    public void SetValue(object value) {
      _value = value;
      _valueString = (value == null ? string.Empty : value.ToString());
    }

    public Terminal Terminal {
      get { return _terminal; }
    }
    public void SetTerminal(Terminal terminal) {
      _terminal = terminal; 
      this.EditorInfo = Terminal.EditorInfo;  //set to term's EditorInfo by default
    }

    public bool IsSet(TokenFlags flag) {
      return (Flags & flag) != 0;
    }
    public TokenCategory Category  {
      get { return Terminal.Category; }
    }

    public bool IsError() {
      return Category == TokenCategory.Error;
    }

    public int Length {
      get { return Text == null ? 0 : Text.Length; }
    }

    public Token OtherBrace {  //matching opening/closing brace
      get { return _otherBrace; }
    } Token _otherBrace;

    public static void LinkMatchingBraces(Token openingBrace, Token closingBrace) {
      openingBrace._otherBrace = closingBrace;
      closingBrace._otherBrace = openingBrace;
    }

    public short ScannerState; //Scanner state after producing token 

    [System.Diagnostics.DebuggerStepThrough]
    public override string ToString() {
      string result = string.Empty;
      if (Terminal is SymbolTerminal)
        result = Text + " [Symbol]";
      else if (Terminal.IsSet(TermOptions.IsKeyword))
        result = Text + " " + "[Keyword]";
      else if (_valueString == null)
        result = Text + " (" + Terminal.ToString() + ")";
      else
        result = _valueString + " (" + Terminal.ToString() + ")";
      return result; 
    }

/*
    protected override void XmlSetAttributes(XmlElement thisElement) {
      base.XmlSetAttributes(thisElement);
      thisElement.SetAttribute("Value", Text); //Adds value string
      if (Value != null)
        thisElement.SetAttribute("ValueType", this.Value.GetType().FullName); //Adds value string

    }
*/

  }//class

  //Some terminals may need to return a bunch of tokens in one call to TryMatch; MultiToken is a container for these tokens
  public class MultiToken : Token {
    public TokenList ChildTokens;
    public MultiToken(Terminal term, SourceLocation location, TokenList childTokens) : base(term, location, string.Empty, null) {
      ChildTokens = childTokens;
    }
  }//class

}//namespace
