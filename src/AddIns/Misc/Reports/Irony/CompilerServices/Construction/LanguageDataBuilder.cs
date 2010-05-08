using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.CompilerServices.Construction {
  internal class LanguageDataBuilder {

    internal LanguageData Language;
    Grammar _grammar;
    private ParserStateHash _stateHash = new ParserStateHash();

    public LanguageDataBuilder(Grammar grammar) {
      _grammar = grammar;
      Language = new LanguageData(grammar);
    }

    public void Build(ParseMethod method) {
      if (_grammar.Root == null) {
        Language.Errors.Add("Root property of the grammar is not set.");
        return;
      }
      var gbld = new GrammarDataBuilder(Language);
      gbld.Build();
      //Just in case grammar author wants to customize something...
      _grammar.OnGrammarDataConstructed(Language);
      var sbld = new ScannerDataBuilder(Language);
      sbld.Build();
      var pbld = new ParserDataBuilder(Language);
      pbld.Build(method);
      Validate(); 
      //call grammar method, a chance to tweak the automaton
      _grammar.OnParserDataConstructed(Language); 
    }

    #region Language Data Validation
    private void Validate() {

    }//method
    #endregion

  
  }//class
}
