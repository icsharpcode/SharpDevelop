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
using System.Text;
using System.Reflection;

namespace Irony.CompilerServices {

  public enum TermOptions {
    None = 0,
    IsOperator = 0x01,
    UsePrecedence = 0x02, //allows using precedence on symbol; by default is set together with IsOperator flag; maybe overwritten
                          // by RestrictPrecedence method
    IsOpenBrace = 0x04,
    IsCloseBrace = 0x08,
    IsBrace = IsOpenBrace | IsCloseBrace,
    IsConstant = 0x10,
    IsPunctuation = 0x20,
    IsDelimiter = 0x40,
    IsList = 0x80,
    IsMemberSelect = 0x100,    
    IsNonGrammar = 0x0200,  // if set, parser would eliminate the token from the input stream; terms in Grammar.NonGrammarTerminals have this flag set
    IsTransient = 0x0400,  // Transient non-terminal - should be removed from the AST tree. 
    IsNullable = 0x0800,
    IsVisible = 0x1000,
    IsKeyword = 0x2000,
    IsReservedWord = 0x04000,
    IsMultiline = 0x08000,
    AllowConvertToSymbol = 0x10000, // Allows to convert to symbol (typically keyword) is parser is not able to match by Term type
    // Identifier terminal is an example
  }

  public delegate void AstNodeCreator(CompilerContext context, ParseTreeNode parseNode);

  //Basic Backus-Naur Form element. Base class for Terminal, NonTerminal, BnfExpression, GrammarHint
  public abstract class BnfTerm {
    #region consructors
    public BnfTerm(string name) : this(name, name) { }
    public BnfTerm(string name, string displayName) {
      Name = name;
      DisplayName = displayName;
    }
    public BnfTerm(string name, string displayName, Type nodeType) : this(name, displayName) {
      NodeType = nodeType;
    }
    public BnfTerm(string name, string displayName, AstNodeCreator nodeCreator) : this(name, displayName) {
      NodeCreator = nodeCreator;  
    }
    #endregion


    public virtual void Init(GrammarData grammarData) {
      GrammarData = grammarData;
      OwnerGrammar = grammarData.Grammar;
    }

    public override string ToString() {
      if (!string.IsNullOrEmpty(DisplayName)) 
          return DisplayName;
      return Name;
    }
    public override int GetHashCode() {
      if (Name == null) return 0;
      return Name.GetHashCode();
    }

    public const int NoPrecedence = 0;

    #region properties: Name, DisplayName, Key, Options
    public string Name;
  
    //DisplayName is used in error reporting, e.g. "Syntax error, expected <list-of-display-names>". 
    public string DisplayName;
    public TermOptions Options;
    protected GrammarData GrammarData;
    protected Grammar OwnerGrammar; 
    public int Precedence = NoPrecedence;
    public Associativity Associativity = Associativity.Neutral;

    [System.Diagnostics.DebuggerStepThrough]
    public bool IsSet(TermOptions option) {
      return (Options & option) != 0;
    }
    [System.Diagnostics.DebuggerStepThrough]
    public void SetOption(TermOptions option) {
      SetOption(option, true);
    }
    [System.Diagnostics.DebuggerStepThrough]
    public void SetOption(TermOptions option, bool value) {
      if (value)
        Options |= option;
      else
        Options &= ~option;
    }

    #endregion

    #region AST node creations: NodeType, NodeCreator, NodeCreated
    public Type NodeType;
    public AstNodeCreator NodeCreator;
    public event EventHandler<NodeCreatedEventArgs> NodeCreated;

    protected internal void OnNodeCreated(ParseTreeNode parseNode) {
      if (NodeCreated == null) return;
      NodeCreatedEventArgs args = new NodeCreatedEventArgs(parseNode);
      NodeCreated(this, args);
    }
    #endregion


    #region Kleene operators: Q(), Plus(), Star()
    NonTerminal _plus, _star;
    public BnfExpression Q() {
      BnfExpression q = Grammar.CurrentGrammar.Empty | this;
      q.Name = this.Name + "?";
      return q; 
    }
    public NonTerminal Plus() {
      if (_plus != null) return _plus;
      string name = this.Name + "+";
      _plus = new NonTerminal(name);
      _plus.SetOption(TermOptions.IsList);
      _plus.Rule = this | _plus + this;
      return _plus;
    }
    public NonTerminal Star() {
      if (_star != null) return _star;
      string name = this.Name + "*";
      _star = new NonTerminal(name);
      _star.SetOption(TermOptions.IsList);
      _star.Rule = Grammar.CurrentGrammar.Empty | _star + this;
      return _star;
    }
    #endregion

    #region Operators: +, |, implicit
    public static BnfExpression operator +(BnfTerm term1, BnfTerm term2) {
      return Op_Plus(term1, term2);
    }
    public static BnfExpression operator +(BnfTerm term1, string symbol2) {
      return Op_Plus(term1, Grammar.CurrentGrammar.Symbol(symbol2));
    }
    public static BnfExpression operator +( string symbol1, BnfTerm term2) {
      return Op_Plus(Grammar.CurrentGrammar.Symbol(symbol1), term2);
    }

    //Alternative 
    public static BnfExpression operator |(BnfTerm term1, BnfTerm term2) {
      return Op_Pipe(term1, term2);
    }
    public static BnfExpression operator |(BnfTerm term1, string symbol2) {
      return Op_Pipe(term1, Grammar.CurrentGrammar.Symbol(symbol2));
    }
    public static BnfExpression operator |(string symbol1, BnfTerm term2) {
      return Op_Pipe(Grammar.CurrentGrammar.Symbol(symbol1), term2);
    }

    //BNF operations implementation -----------------------
    // Plus/sequence
    internal static BnfExpression Op_Plus(BnfTerm term1, BnfTerm term2) {
      //Check term1 and see if we can use it as result, simply adding term2 as operand
      BnfExpression expr1 = term1 as BnfExpression;
      if (expr1 == null || expr1.Data.Count > 1) //either not expression at all, or Pipe-type expression (count > 1)
        expr1 = new BnfExpression(term1);
      expr1.Data[expr1.Data.Count - 1].Add(term2);
      return expr1;
    }

    //Pipe/Alternative
    internal static BnfExpression Op_Pipe(BnfTerm term1, BnfTerm term2) {
      //Check term1 and see if we can use it as result, simply adding term2 as operand
      BnfExpression expr1 = term1 as BnfExpression;
      if (expr1 == null) //either not expression at all, or Pipe-type expression (count > 1)
        expr1 = new BnfExpression(term1);
      //Check term2; if it is an expression and is simple sequence (Data.Count == 1) then add this sequence directly to expr1
      BnfExpression expr2 = term2 as BnfExpression;
      //1. term2 is a simple expression
      if (expr2 != null && expr2.Data.Count == 1) { // if it is simple sequence (plus operation), add it directly
        expr1.Data.Add(expr2.Data[0]);
        return expr1;
      }
      //2. term2 is not a simple expression
      expr1.Data.Add(new BnfTermList()); //add a list for a new OR element (new "plus" sequence)
      expr1.Data[expr1.Data.Count - 1].Add(term2); // and put  term2 there if it is not Empty pseudo-element
      return expr1;
    }

    #endregion

  }//class

  public class BnfTermList : List<BnfTerm> { }
  public class BnfTermSet : HashSet<BnfTerm> {
    public override string ToString() {
      var sb = new StringBuilder();
      foreach (var term in this) {
        sb.Append(term.ToString());
        sb.Append(" ");
      }
      return sb.ToString().Trim();
    }
    public string ToErrorString() {
      var sb = new StringBuilder();
      foreach (var term in this) {
        sb.Append(term.DisplayName);
        sb.Append(",");
      }
      return sb.ToString().Trim();
    }
  }



}//namespace

