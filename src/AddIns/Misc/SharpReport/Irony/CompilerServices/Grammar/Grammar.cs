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
using Irony.Scripting.Runtime;

namespace Irony.CompilerServices {

  public partial class Grammar {

    #region properties
    /// <summary>
    /// Gets case sensitivity of the grammar. Read-only, true by default. 
    /// Can be set to false only through a parameter to grammar constructor.
    /// </summary>
    public readonly bool CaseSensitive = true;
    
    public ParseMethod ParseMethod = ParseMethod.Lalr;

    //List of chars that unambigously identify the start of new token. 
    //used in scanner error recovery, and in quick parse path in Number literals 
    public string Delimiters = ",;[](){}";

    public string WhitespaceChars = " \t\r\n\v";
    
    //Used for line counting in source file
    public string LineTerminators = "\n\r\v";

    #region Language Flags
    public LanguageFlags LanguageFlags {
      get { return _languageFlags; }
    } LanguageFlags _languageFlags = LanguageFlags.Default;

    public void SetLanguageFlags(LanguageFlags flag) {
      SetLanguageFlags(flag, true); 
    }
    public void SetLanguageFlags(LanguageFlags flag, bool value) {
      if (value)
        _languageFlags |= flag;
      else
        _languageFlags &= ~flag;
    }
    public bool FlagIsSet(LanguageFlags flag) {
      return (LanguageFlags & flag) != 0;
    }
    public void ResetFlags() {
      _languageFlags = LanguageFlags.None;
    }
    #endregion

    //Terminals not present in grammar expressions and not reachable from the Root
    // (Comment terminal is usually one of them)
    // Tokens produced by these terminals will be ignored by parser input. 
    public readonly TerminalSet NonGrammarTerminals = new TerminalSet();
    //public readonly TerminalList NonGrammarTerminals = new TerminalList();

    //Terminals that either don't have explicitly declared Firsts symbols, or can start with chars not covered by these Firsts 
    // For ex., identifier in c# can start with a Unicode char in one of several Unicode classes, not necessarily latin letter.
    //  Whenever terminals with explicit Firsts() cannot produce a token, the Scanner would call terminals from this fallback 
    // collection to see if they can produce it. 
    // Note that IdentifierTerminal automatically add itself to this collection if its StartCharCategories list is not empty, 
    // so programmer does not need to do this explicitly
    public readonly TerminalList FallbackTerminals = new TerminalList();

    //Default node type; if null then GenericNode type is used. 
    public Type DefaultNodeType;


    public NonTerminal Root;
    
    public readonly TokenFilterList TokenFilters = new TokenFilterList();

    public string GrammarComments; //shown in Grammar Errors page

    #endregion 

    #region constructors
    
    public Grammar() : this(true) { } //case sensitive by default

    public Grammar(bool caseSensitive) {
      this.CaseSensitive = caseSensitive;
      bool ignoreCase =  !this.CaseSensitive;
      StringComparer comparer = StringComparer.Create(System.Globalization.CultureInfo.InvariantCulture, ignoreCase);
      SymbolTerms = new SymbolTerminalTable(comparer);
      _currentGrammar = this;
      NewLinePlus = CreateNewLinePlus();
    }
    #endregion
    
    #region Reserved words handling
    //Reserved words handling 
    public void MarkReservedWords(params string[] reservedWords) {
      foreach (var word in reservedWords) {
        var wdTerm = Symbol(word);
        wdTerm.SetOption(TermOptions.IsReservedWord);
        //Reserved words get the highest priority, so they get to be tried before identifiers
        wdTerm.Priority = 1000 + word.Length;
      }
    }
    #endregion 

    #region Register methods
    public void RegisterPunctuation(params string[] symbols) {
      foreach (string symbol in symbols) {
        SymbolTerminal term = Symbol(symbol);
        term.SetOption(TermOptions.IsPunctuation);
      }
    }
    
    public void RegisterPunctuation(params BnfTerm[] elements) {
      foreach (BnfTerm term in elements) 
        term.SetOption(TermOptions.IsPunctuation);
    }

    public void RegisterOperators(int precedence, params string[] opSymbols) {
      RegisterOperators(precedence, Associativity.Left, opSymbols);
    }

    public void RegisterOperators(int precedence, Associativity associativity, params string[] opSymbols) {
      foreach (string op in opSymbols) {
        SymbolTerminal opSymbol = Symbol(op);
        opSymbol.SetOption(TermOptions.IsOperator);
        if (!UsePrecedenceRestricted)
          opSymbol.SetOption(TermOptions.UsePrecedence); 
        opSymbol.Precedence = precedence;
        opSymbol.Associativity = associativity;
      }
    }//method

    public void RegisterOperators(int precedence, params BnfTerm[] opTerms) {
      RegisterOperators(precedence, Associativity.Left, opTerms);
    }
    public void RegisterOperators(int precedence, Associativity associativity, params BnfTerm[] opTerms) {
      foreach (var term in opTerms) {
        term.SetOption(TermOptions.IsOperator);
        if (!UsePrecedenceRestricted)
          term.SetOption(TermOptions.UsePrecedence); 
        term.Precedence = precedence;
        term.Associativity = associativity;
      }
    }

    // Restricts use of precedence rule to certain non-terminals
    // In languages like c# the symbols "<" and ">" are used as comparison operators and as brackets for type parameters
    // We must avoid applying precedence rule to these symbols in the second case. We can achieve this by doing the following.
    // We register "<" and ">" as operators along with other operator symbols; we  define BinOp non-terminal with the rule 
    //  that includes OR of all operators including "<" and ">"; we define type_arguments non-terminal using "<" and ">" as well. 
    // We then instruct parser to apply precedence rule only to BinOp non-terminal. The parser then will apply precedence 
    // only after reducing the BinOp - the precedence value in this case is derived from the underlying symbol. 
    // If this set is empty, then Irony tries to detect automatically operator non-terminals like BinOp.
    // This restriction can be used only in NLALR/NLALR-T methods.
    internal bool UsePrecedenceRestricted;
    public void RestrictPrecedence(params BnfTerm[] onlyTerms) {
      UsePrecedenceRestricted = true; 
      foreach (var symbol in SymbolTerms.Values)
        symbol.SetOption(TermOptions.UsePrecedence, false); 
      foreach(var term in onlyTerms)
        term.SetOption(TermOptions.UsePrecedence); 
    }

    public void RegisterBracePair(string openBrace, string closeBrace) {
      SymbolTerminal openS = Symbol(openBrace);
      SymbolTerminal closeS = Symbol(closeBrace);
      openS.SetOption(TermOptions.IsOpenBrace);
      openS.IsPairFor = closeS;
      closeS.SetOption(TermOptions.IsCloseBrace);
      closeS.IsPairFor = openS;
    }
    public void MarkTransient(params NonTerminal[] nonTerminals) {
      foreach (NonTerminal nt in nonTerminals)
        nt.Options |= TermOptions.IsTransient;
    }
    //MemberSelect are symbols invoking member list dropdowns in editor; for ex: . (dot), ::
    public void MarkMemberSelect(params string[] symbols) {
      foreach (var symbol in symbols)
        Symbol(symbol).SetOption(TermOptions.IsMemberSelect);
    }
    #endregion

    #region virtual methods: TryMatch, CreateNode, GetSyntaxErrorMessage, CreateRuntime
    //This method is called if Scanner failed to produce token; it offers custom method a chance    
    public virtual Token TryMatch(CompilerContext context, ISourceStream source) {
      return null;
    }

    public virtual void CreateAstNode(CompilerContext context, ParseTreeNode nodeInfo) {
      var term = nodeInfo.Term;
      if (term.NodeCreator != null) {
        term.NodeCreator(context, nodeInfo);
        //We assume that Node creator method creates node and initializes it, so parser does not need to call 
        // IAstNodeInit.InitNode() method on node object.
        return;
      }
      Type nodeType = term.NodeType ?? this.DefaultNodeType;
      if (nodeType == null) return; 
      nodeInfo.AstNode =  Activator.CreateInstance(nodeType);
      //Initialize node
      var iInit = nodeInfo.AstNode as IAstNodeInit;
      if (iInit != null)
        iInit.Init(context, nodeInfo); 
    }

    /// <summary>
    /// Override this method to provide custom conflict resolution; for example, custom code may decide proper shift or reduce
    /// action based on preview of tokens ahead. 
    /// </summary>
    public virtual void OnResolvingConflict(ConflictResolutionArgs args) {
      //args.Result is Shift by default
    }

    //The method is called after GrammarData is constructed 
    public virtual void OnGrammarDataConstructed(LanguageData language) {
    }

    public virtual void OnParserDataConstructed(LanguageData language) {
    }

    public virtual string GetSyntaxErrorMessage(CompilerContext context, ParserState state, ParseTreeNode currentInput) {
      return null; //Irony then would construct default message
    }
    
    public virtual LanguageRuntime CreateRuntime() {
      return new LanguageRuntime();
    }

    #endregion

    #region MakePlusRule, MakeStarRule methods
    public static BnfExpression MakePlusRule(NonTerminal listNonTerminal, BnfTerm listMember) {
      return MakePlusRule(listNonTerminal, null, listMember);
    }
    public static BnfExpression MakePlusRule(NonTerminal listNonTerminal, BnfTerm delimiter, BnfTerm listMember) {
      listNonTerminal.SetOption(TermOptions.IsList);
      if (delimiter == null)
        listNonTerminal.Rule = listMember | listNonTerminal + listMember;
      else
        listNonTerminal.Rule = listMember | listNonTerminal + delimiter + listMember;
      return listNonTerminal.Rule;
    }
    public static BnfExpression MakeStarRule(NonTerminal listNonTerminal, BnfTerm listMember) {
      return MakeStarRule(listNonTerminal, null, listMember);
    }
    public static BnfExpression MakeStarRule(NonTerminal listNonTerminal, BnfTerm delimiter, BnfTerm listMember) {
      if (delimiter == null) {
        //it is much simpler case
        listNonTerminal.SetOption(TermOptions.IsList);
        listNonTerminal.Rule = _currentGrammar.Empty | listNonTerminal + listMember;
        return listNonTerminal.Rule;
      }
      //Note that deceptively simple version of the star-rule 
      //       Elem* -> Empty | Elem | Elem* + delim + Elem
      //  does not work when you have delimiters. This simple version allows lists starting with delimiters -
      // which is wrong. The correct formula is to first define "Elem+"-list, and then define "Elem*" list 
      // as "Elem* -> Empty|Elem+" 
      NonTerminal tmp = new NonTerminal(listMember.Name + "+");
      tmp.SetOption(TermOptions.IsTransient); //important - mark it as Transient so it will be eliminated from AST tree
      MakePlusRule(tmp, delimiter, listMember);
      listNonTerminal.Rule = _currentGrammar.Empty | tmp;
      //listNonTerminal.SetOption(TermOptions.IsStarList);
      return listNonTerminal.Rule;
    }
    #endregion

    #region Hint utilities
    //[Obsolete("Deprecated. Use ResolveBy(ResolutionType.Shift) method instead")]
    protected GrammarHint PreferShiftHere() {
      return new GrammarHint(HintType.ResolveToShift, null); 
    }
    protected GrammarHint ReduceHere() {
      return new GrammarHint(HintType.ResolveToReduce, null);
    }
    protected GrammarHint ResolveInCode() {
      return new GrammarHint(HintType.ResolveInCode, null); 
    }
    protected GrammarHint WrapTail() {
      return new GrammarHint(HintType.WrapTail, null);
    }
    #endregion

    #region Standard terminals: EOF, Empty, NewLine, Indent, Dedent
    // Empty object is used to identify optional element: 
    //    term.Rule = term1 | Empty;
    public readonly Terminal Empty = new Terminal("EMPTY");
    // The following terminals are used in indent-sensitive languages like Python;
    // they are not produced by scanner but are produced by CodeOutlineFilter after scanning
    public readonly NewLineTerminal NewLine = new NewLineTerminal("LF");
    public readonly Terminal Indent = new Terminal("INDENT", TokenCategory.Outline);
    public readonly Terminal Dedent = new Terminal("DEDENT", TokenCategory.Outline);
    // Identifies end of file
    // Note: using Eof in grammar rules is optional. Parser automatically adds this symbol 
    // as a lookahead to Root non-terminal
    public readonly Terminal Eof = new Terminal("EOF", TokenCategory.Outline);

    //End-of-Statement terminal
    public readonly Terminal Eos = new Terminal("EOS", TokenCategory.Outline);

    public readonly Terminal SyntaxError = new Terminal("SYNTAX_ERROR", TokenCategory.Error);

    public NonTerminal NewLinePlus;

    private NonTerminal CreateNewLinePlus() {
      NewLine.SetOption(TermOptions.IsTransient); 
      var result = new NonTerminal("LF+");
      result.SetOption(TermOptions.IsList);
      result.Rule = NewLine | result + NewLine;
      return result;
    }


    #endregion

    #region Symbol terminals support
    public SymbolTerminalTable SymbolTerms;

    public SymbolTerminal Symbol(string symbol) {
      return Symbol(symbol, symbol);
    }
    public SymbolTerminal Symbol(string symbol, string name) {
      SymbolTerminal term;
      if (SymbolTerms.TryGetValue(symbol, out term)) {
        //update name if it was specified now and not before
        if (string.IsNullOrEmpty(term.Name) && !string.IsNullOrEmpty(name))
          term.Name = name;
        return term; 
      }
      //create new term
      if (!CaseSensitive)
        symbol = symbol.ToLower();
      string.Intern(symbol); 
      term = new SymbolTerminal(symbol, name);
      SymbolTerms[symbol] = term;
      return term; 
    }

    #endregion

    #region CurrentGrammar static field
    //Static per-thread instance; Grammar constructor sets it to self (this). 
    // This field/property is used by operator overloads (which are static) to access Grammar's predefined terminals like Empty,
    //  and SymbolTerms dictionary to convert string literals to symbol terminals and add them to the SymbolTerms dictionary
    [ThreadStatic]
    private static Grammar _currentGrammar;
    public static Grammar CurrentGrammar {
      get { return _currentGrammar; }
    }
    internal static void ClearCurrentGrammar() {
      _currentGrammar = null; 
    }
    #endregion

  }//class

}//namespace
