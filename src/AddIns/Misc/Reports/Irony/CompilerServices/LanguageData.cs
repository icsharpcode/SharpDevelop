using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.CompilerServices {
  public class LanguageData {
    public readonly Grammar Grammar;
    public GrammarData GrammarData; 
    public ParserData ParserData;
    public ScannerData ScannerData;
    public readonly StringSet Errors = new StringSet();

    public LanguageData(Grammar grammar) {
      Grammar = grammar; 
    }
  }//class
}//namespace
