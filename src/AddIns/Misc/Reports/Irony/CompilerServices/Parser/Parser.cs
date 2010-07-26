using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.CompilerServices {
  //Parser class represents combination of scanner and LALR parser (CoreParser)
  public class Parser {
    public readonly LanguageData Language; 
    public readonly Scanner Scanner;
    public readonly CoreParser CoreParser;

    public Parser(LanguageData language) {
      Language = language; 
      Scanner = new Scanner(Language.ScannerData);
      CoreParser = new CoreParser(Language.ParserData, Scanner); 
    }

    public ParseTree Parse(CompilerContext context, string sourceText, string fileName) {
      context.CurrentParseTree = new ParseTree(sourceText, fileName);
      Scanner.SetSource(sourceText);
      Scanner.BeginScan(context);
      CoreParser.Parse(context);
      if (context.CurrentParseTree.Errors.Count > 0)
        context.CurrentParseTree.Errors.Sort(SyntaxErrorList.ByLocation);
      return context.CurrentParseTree;
    }
  
  }//class
}//namespace
