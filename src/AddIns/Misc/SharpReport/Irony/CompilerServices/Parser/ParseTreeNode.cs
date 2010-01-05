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
using System.Linq;
using System.Text;

namespace Irony.CompilerServices {

  /* 
    A node for so-called parse tree (concrete syntax tree) - initial tree that parser produces; it contains all 
    syntax elements of the input text, each element represented by generic node ParseTreeNode. 
    The parse tree is converted into abstract syntax tree (AST) which contains custom nodes. The conversion might 
    happen on-the-fly: as parser creates the parse tree nodes it can create the AST nodes and puts them into AstNode field. 
    Alternatively it might happen as a separate step, after completing the parse tree. 
    AST node might optinally implement IAstNode interface, so Irony parser can initialize the node providing it
    with all relevant information. Also Irony code analysis process uses IScriptNode interface. 
    Nodes in Irony Compiler.Ast namespace implement this interface and can be used to implement scripting languages. 
    The ParseTreeNode also works as stack element in parser stack, so it has extra State property to carry 
    the pushed state while it sits in the stack. 
  */
  public class ParseTreeNode {
    public object AstNode;
    public Token Token; 
    public BnfTerm Term;
    public int Precedence;
    public Associativity Associativity;
    public SourceSpan Span;
    public Production ReduceProduction;
    public ParseTreeNodeList ChildNodes = new ParseTreeNodeList();
    /* Used by NLALR parser to search for action based on "expanded" version of the lookahead
      when actual lookahead is non-canonical (reduced non-terminal) and action for it does not exist.
     This might happen in non-canonical parser. Not necessarily the same as ChildNodes[0], because 
     it can be punctuation symbol, so it might be removed from ChildNodes, so we need to keep it in a separate field.*/
    public ParseTreeNode FirstChild; 
    public bool IsError;
    internal ParserState State;      //used by parser to store current state when node is pushed into the parser stack

    public ParseTreeNode(object node) {
      AstNode = node;
    }
    public ParseTreeNode(BnfTerm term) {
      Term = term;
    }
    public ParseTreeNode(Token token) {
      Token = token;
      Term = token.Terminal;
      Precedence = Term.Precedence;
      Associativity = token.Terminal.Associativity;
      Span = new SourceSpan(token.Location, token.Length);
      IsError = token.IsError(); 
    }
    public ParseTreeNode(ParserState initialState) {
      State = initialState;
    }
    public ParseTreeNode(Production reduceProduction) {
      ReduceProduction = reduceProduction;
      Term = ReduceProduction.LValue;
      Precedence = Term.Precedence;
    }
    public ParseTreeNode(object node, BnfTerm term, int precedence, Associativity associativity, SourceSpan span) {
      AstNode = node;
      Term = term;
      Precedence = precedence;
      Associativity = associativity;
    }
    public override string ToString() {
      if (Term == null) //special case for initial node pushed into the stack at parser start
        return "(INITIAL STATE)";
      if (Token != null)
        return Token.ToString();
      if (AstNode == null) return 
        Term.ToString();
      return AstNode.ToString();
    }//method

    public bool HasChildNodes() {
      return ChildNodes != null && ChildNodes.Count > 0;
    }
  }

  public enum ListAddMode {
    Start, 
    End
  }

  public class ParseTreeNodeList : List<ParseTreeNode> {
    public void Add(ParseTreeNode node, ListAddMode mode) {
      if (mode == ListAddMode.Start)
        base.Insert(0, node);
      else
        base.Add(node);

    }
    public void Add(ParseTreeNodeList nodes, ListAddMode mode) {
      if (mode == ListAddMode.Start)
        base.InsertRange(0, nodes);
      else
        base.AddRange(nodes);

    }
  }


}
