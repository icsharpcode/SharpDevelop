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
using System.Diagnostics;

namespace Irony.CompilerServices {

  public class Compiler {

    public Compiler(Grammar grammar) : this(grammar, grammar.ParseMethod) { }

    public Compiler(Grammar grammar, ParseMethod parseMethod) {
      var builder = new Construction.LanguageDataBuilder(grammar);
      builder.Build(parseMethod);
      this.Language = builder.Language;
      Parser = new Parser(Language);
    }
    public Compiler(LanguageData language) {
      this.Language = language;
      Parser = new Parser(Language);
    }

    //Used in unit tests
    public static Compiler CreateDummy() {
      Compiler compiler = new Compiler(new Grammar());
      return compiler;
    }

    #region properties: Language, Scanner, Parser
    public readonly LanguageData Language;
    public readonly Parser Parser; //combination of Scanner (with token filters inside) and CoreParser
    #endregion 

    public ParseTree Parse(string source) {
      return Parse(new CompilerContext(this), source, "<Source>");
    }

    public ParseTree Parse(CompilerContext context, string sourceText, string fileName) {
      int start = Environment.TickCount;
      Parser.Parse(context, sourceText, fileName);
      context.CurrentParseTree.ParseTime = Environment.TickCount - start;
      return context.CurrentParseTree;
    }//method

    public ParseTree ScanOnly(string sourceText, string fileName) {
      var context = new CompilerContext(this);
      context.CurrentParseTree = new ParseTree(sourceText, fileName);
      Parser.Scanner.SetSource(sourceText);
      Parser.Scanner.BeginScan(context);
      while (true) {
        var token = Parser.Scanner.GetToken();
        if (token == null || token.Terminal == Language.Grammar.Eof) break;
      }
      return context.CurrentParseTree;
    }


  }//class

}//namespace
