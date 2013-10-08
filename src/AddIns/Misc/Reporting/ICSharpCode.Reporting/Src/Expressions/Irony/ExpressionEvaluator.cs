// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using Irony.Interpreter;
using Irony.Interpreter.Evaluator;
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

    public void ClearOutput() {
      App.ClearOutputBuffer(); 
    }
    public string GetOutput() {
      return App.GetOutput(); 
    }
	}
}
