using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.CompilerServices {

  //GrammarData is a container for all basic info about the grammar
  // GrammarData is a field in LanguageData object. 
  public class GrammarData {
    public readonly LanguageData Language; 
    public readonly Grammar Grammar;
    public NonTerminal AugmentedRoot;
    public readonly BnfTermSet AllTerms = new BnfTermSet();
    public readonly TerminalList Terminals = new TerminalList();
    public readonly NonTerminalList NonTerminals = new NonTerminalList();

    public GrammarData(LanguageData language) {
      Language = language;
      Grammar = language.Grammar;
    }

  }//class

  [Flags]
  public enum LanguageFlags {
    None = 0,

    //Compilation options
    //Be careful - use this flag ONLY if you use NewLine terminal in grammar explicitly!
    // - it happens only in line-based languages like Basic.
    NewLineBeforeEOF = 0x01,
    //Automatically detect transient non-terminals - whose rules are just OR of other single terms
    // nodes for these terminals would be eliminated from parse tree. Formerly this stuff was called "node bubbling"
    AutoDetectTransient = 0x02,
    AutoDetectKeywords = 0x04, //automatically mark all words found in grammar as keywords
    CreateAst = 0x08, //create AST nodes 

    //Runtime
    SupportsInterpreter = 0x0100,
    SupportsConsole = 0x0200,
    TailRecursive = 0x0400, //Tail-recursive language - Scheme is one example

    //Default value
    Default = AutoDetectTransient | AutoDetectKeywords,
  }

  public enum ParseMethod {
    Lalr,    //canonical LALR
    Nlalr,   //non-canonical LALR
  }

  //Operator associativity types
  public enum Associativity {
    Left,
    Right,
    Neutral  //don't know what that means 
  }

}//namespace
