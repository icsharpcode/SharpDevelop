// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using Irony.Interpreter;
using Irony.Parsing;

namespace ICSharpCode.Reporting.Expressions.Irony
{
	/// <summary>
	/// Description of ReportingLanguageEvaluator.
	/// </summary>
	public class ReportingExpressionEvaluator
	{
	public InterpretedLanguageGrammar Grammar {get; private set;}
    public Parser Parser {get; private set;} 
    public LanguageData Language {get; private set;}
    public LanguageRuntime Runtime {get; private set;} 
    public ScriptApp App {get; private set;}

    public IDictionary<string, object> Globals {
      get { return App.Globals; }
    }


    //Default constructor, creates default evaluator 
    public ReportingExpressionEvaluator(InterpretedLanguageGrammar grammar) {
      Grammar = grammar;
      Language = new LanguageData(Grammar);
      Parser = new Parser(Language);
      Runtime = Grammar.CreateRuntime(Language);
      App = new ScriptApp(Runtime);
    }

    public object Evaluate(string script) {
      var result = App.Evaluate(script);
      return result; 
    }

    public object Evaluate(ParseTree parsedScript) {
      var result = App.Evaluate(parsedScript);
      return result;
    }

    //Evaluates again the previously parsed/evaluated script
    public object Evaluate() {
      return App.Evaluate(); 
    }
	/*
    public void ClearOutput() {
      App.ClearOutputBuffer(); 
    }
    public string GetOutput() {
      return App.GetOutput(); 
    }
    */
	}
}
