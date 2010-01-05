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
using System.Globalization;

namespace Irony.CompilerServices {
  #region notes
  //Identifier terminal. Matches alpha-numeric sequences that usually represent identifiers and keywords.
  // c#: @ prefix signals to not interpret as a keyword; allows \u escapes
  // 

  #endregion

  public enum IdFlags : short {
    None = 0,
    AllowsEscapes = 0x01,
    CanStartWithEscape = 0x03,  
    
    IsNotKeyword = 0x10,
    NameIncludesPrefix = 0x20,

    HasEscapes = 0x100,     
  }
  public class UnicodeCategoryList : List<UnicodeCategory> { }

  public class IdentifierTerminal : CompoundTerminalBase {


    //Note that extraChars, extraFirstChars are used to form AllFirstChars and AllChars fields, which in turn 
    // are used in QuickParse. Only if QuickParse fails, the process switches to full version with checking every
    // char's category
    #region constructors and initialization
    public IdentifierTerminal(string name) : this(name, IdFlags.None) {
    }
    public IdentifierTerminal(string name, IdFlags flags) : this(name, "_", "_") {
      Flags = flags; 
    }
    public IdentifierTerminal(string name, string extraChars, string extraFirstChars): base(name) {
      AllFirstChars = Strings.AllLatinLetters + extraFirstChars;
      AllChars = Strings.AllLatinLetters + Strings.DecimalDigits + extraChars;
      SetOption(TermOptions.AllowConvertToSymbol);
    }

    public void AddPrefix(string prefix, IdFlags flags) {
      base.AddPrefixFlag(prefix, (short)flags);
    }
    #endregion

    #region properties: ExtraChars, ExtraFirstChars
    //Used in QuickParse only!
    public string AllChars;
    public string AllFirstChars;
    private string _terminators;
    public TokenEditorInfo KeywordEditorInfo = new TokenEditorInfo(TokenType.Keyword, TokenColor.Keyword, TokenTriggers.None);
    public IdFlags Flags; //flags for the case when there are no prefixes

    public readonly UnicodeCategoryList StartCharCategories = new UnicodeCategoryList(); //categories of first char
    public readonly UnicodeCategoryList CharCategories = new UnicodeCategoryList();      //categories of all other chars
    public readonly UnicodeCategoryList CharsToRemoveCategories = new UnicodeCategoryList(); //categories of chars to remove from final id, usually formatting category
    #endregion

    #region overrides
    public override void Init(GrammarData grammarData) {
      base.Init(grammarData);
      _terminators = OwnerGrammar.WhitespaceChars + OwnerGrammar.Delimiters;
      if (this.StartCharCategories.Count > 0 && !OwnerGrammar.FallbackTerminals.Contains(this))
        OwnerGrammar.FallbackTerminals.Add(this);
      if (this.EditorInfo == null) 
        this.EditorInfo = new TokenEditorInfo(TokenType.Identifier, TokenColor.Identifier, TokenTriggers.None);
    }
    //TODO: put into account non-Ascii aplhabets specified by means of Unicode categories!
    public override IList<string> GetFirsts() {
      StringList list = new StringList();
      list.AddRange(Prefixes);
      if (string.IsNullOrEmpty(AllFirstChars))
        return list;
      char[] chars = AllFirstChars.ToCharArray();
      foreach (char ch in chars)
        list.Add(ch.ToString());
      if ((Flags & IdFlags.CanStartWithEscape) != 0)
        list.Add(this.EscapeChar.ToString());
      return list;
    }
    protected override void InitDetails(CompilerContext context, CompoundTokenDetails details) {
      base.InitDetails(context, details);
      details.Flags = (short)Flags;
    }

    //Override to assign IsKeyword flag to keyword tokens
    protected override Token CreateToken(CompilerContext context, ISourceStream source, CompoundTokenDetails details) {
      if (details.IsSet((short)IdFlags.NameIncludesPrefix) && !string.IsNullOrEmpty(details.Prefix))
        details.Value = details.Prefix + details.Body;
      Token token = base.CreateToken(context, source, details);
      if (details.IsSet((short)IdFlags.IsNotKeyword))
        return token;
      //check if it is keyword
      CheckReservedWord(token);
      return token; 
    }
    private void CheckReservedWord(Token token) {
      SymbolTerminal symbolTerm;
      if (OwnerGrammar.SymbolTerms.TryGetValue(token.Text, out symbolTerm)) {
        token.AsSymbol = symbolTerm;
        //if it is reserved word, then overwrite terminal
        if (symbolTerm.IsSet(TermOptions.IsReservedWord))
          token.SetTerminal(symbolTerm); 
      }
    }

    protected override Token QuickParse(CompilerContext context, ISourceStream source) {
      if (AllFirstChars.IndexOf(source.CurrentChar) < 0) 
        return null;
      source.Position++;
      while (AllChars.IndexOf(source.CurrentChar) >= 0 && !source.EOF())
        source.Position++;
      //if it is not a terminator then cancel; we need to go through full algorithm
      if (_terminators.IndexOf(source.CurrentChar) < 0) return null; 
      string text = source.GetLexeme();
      Token token = new Token(this, source.TokenStart, text, text);
      CheckReservedWord(token);
      return token; 
    }

    protected override bool ReadBody(ISourceStream source, CompoundTokenDetails details) {
      int start = source.Position;
      bool allowEscapes = details.IsSet((short)IdFlags.AllowsEscapes);
      CharList outputChars = new CharList();
      while (!source.EOF()) {
        char current = source.CurrentChar;
        if (_terminators.IndexOf(current) >= 0) break;
        if (allowEscapes && current == this.EscapeChar) {
          current = ReadUnicodeEscape(source, details);
          //We  need to back off the position. ReadUnicodeEscape sets the position to symbol right after escape digits.  
          //This is the char that we should process in next iteration, so we must backup one char, to pretend the escaped
          // char is at position of last digit of escape sequence. 
          source.Position--; 
          if (details.Error != null) 
            return false;
        }
        //Check if current character is OK
        if (!CharOk(current, source.Position == start)) 
          break; 
        //Check if we need to skip this char
        UnicodeCategory currCat = char.GetUnicodeCategory(current); //I know, it suxx, we do it twice, fix it later
        if (!this.CharsToRemoveCategories.Contains(currCat))
          outputChars.Add(current); //add it to output (identifier)
        source.Position++;
      }//while
      if (outputChars.Count == 0)
        return false;
      //Convert collected chars to string
      details.Body =  new string(outputChars.ToArray());
      return !string.IsNullOrEmpty(details.Body); 
    }

    private bool CharOk(char ch, bool first) {
      //first check char lists, then categories
      string all = first? AllFirstChars : AllChars;
      if(all.IndexOf(ch) >= 0) return true; 
      //check categories
      UnicodeCategory chCat = char.GetUnicodeCategory(ch);
      UnicodeCategoryList catList = first ? StartCharCategories : CharCategories;
      if (catList.Contains(chCat)) return true;
      return false; 
    }

    private char ReadUnicodeEscape(ISourceStream source, CompoundTokenDetails details) {
      //Position is currently at "\" symbol
      source.Position++; //move to U/u char
      int len;
      switch (source.CurrentChar) {
        case 'u': len = 4; break;
        case 'U': len = 8; break; 
        default:
          details.Error = "Invalid escape symbol, expected 'u' or 'U' only.";
          return '\0'; 
      }
      if (source.Position + len > source.Text.Length) {
        details.Error = "Invalid escape symbol";
        return '\0';
      }
      source.Position++; //move to the first digit
      string digits = source.Text.Substring(source.Position, len);
      char result = (char)Convert.ToUInt32(digits, 16);
      source.Position += len;
      details.Flags |= (int) IdFlags.HasEscapes;
      return result;
    }

    protected override bool ConvertValue(CompoundTokenDetails details) {
      if (details.IsSet((short)IdFlags.NameIncludesPrefix))
        details.Value = details.Prefix + details.Body;
      else
        details.Value = details.Body;
      return true; 
    }

    #endregion 

  }//class


} //namespace
