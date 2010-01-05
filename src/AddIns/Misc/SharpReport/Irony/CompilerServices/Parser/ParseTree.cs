using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.CompilerServices {
  public class ParseTree {
    public readonly string SourceText;
    public readonly string FileName; 
    public readonly TokenList Tokens = new TokenList();
    public readonly TokenList OpenBraces = new TokenList(); 
    public ParseTreeNode Root;
    public readonly SyntaxErrorList Errors = new SyntaxErrorList();
    public int ParseTime;

    public ParseTree(string sourceText, string fileName) {
      SourceText = sourceText;
      FileName = fileName; 
    }
  }

}
