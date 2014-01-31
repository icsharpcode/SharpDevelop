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

using System;
using ICSharpCode.Reporting.Expressions.Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace ICSharpCode.Reporting.Expressions.Irony
{
	/// <summary>
	/// Description of ReportingLanguageGrammer.
	/// </summary>
	public class ReportingLanguageGrammer:InterpretedLanguageGrammar
	{
		private const string exclamationMark = "!";
		public ReportingLanguageGrammer() :base(caseSensitive : false) {
			
      this.GrammarComments = 
@"Irony expression evaluator. Case-insensitive. Supports big integers, float data types, variables, assignments,
arithmetic operations, augmented assignments (+=, -=), inc/dec (++,--), strings with embedded expressions; 
bool operations &,&&, |, ||; ternary '?:' operator." ;
      
      // 1. Terminals
      var number = new NumberLiteral("number");
      //Let's allow big integers (with unlimited number of digits):
      number.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };
      var identifier = new IdentifierTerminal("identifier");
      var comment = new CommentTerminal("comment", "#", "\n", "\r"); 
      //comment must be added to NonGrammarTerminals list; it is not used directly in grammar rules,
      // so we add it to this list to let Scanner know that it is also a valid terminal. 
      base.NonGrammarTerminals.Add(comment);
      var comma = ToTerm(",");

      //String literal with embedded expressions  ------------------------------------------------------------------
      var stringLit = new StringLiteral("string", "\"", StringOptions.AllowsAllEscapes | StringOptions.IsTemplate);
      stringLit.AddStartEnd("'", StringOptions.AllowsAllEscapes | StringOptions.IsTemplate); 
      stringLit.AstConfig.NodeType = typeof(StringTemplateNode);
      var Expr = new NonTerminal("Expr"); //declare it here to use in template definition 
      var templateSettings = new StringTemplateSettings(); //by default set to Ruby-style settings 
      templateSettings.ExpressionRoot = Expr; //this defines how to evaluate expressions inside template
      this.SnippetRoots.Add(Expr);
      stringLit.AstConfig.Data = templateSettings;
      //--------------------------------------------------------------------------------------------------------

      // 2. Non-terminals
      var Term = new NonTerminal("Term");
      var BinExpr = new NonTerminal("BinExpr", typeof(BinaryOperationNode));
      var ParExpr = new NonTerminal("ParExsumpr");
      var UnExpr = new NonTerminal("UnExpr", typeof(UnaryOperationNode));
      var TernaryIfExpr = new NonTerminal("TernaryIf", typeof(IfNode));
      var ArgList = new NonTerminal("ArgList", typeof(ExpressionListNode));
      var FunctionCall = new NonTerminal("FunctionCall", typeof(FunctionCallNode));
      
      // SharpReporting
      var ParametersSection = new NonTerminal("ParametersCall",typeof(ParametersCallNode));
      var FieldsSection  = new NonTerminal("FieldsCall",typeof(FieldsNode));
      var GlobalSection = new NonTerminal("GlobalCall",typeof(GlobalsNode));
      
      // end of SharpReporting
      
      var MemberAccess = new NonTerminal("MemberAccess", typeof(MemberAccessNode));
      
      
      var IndexedAccess = new NonTerminal("IndexedAccess", typeof(IndexedAccessNode));
      var ObjectRef = new NonTerminal("ObjectRef"); // foo, foo.bar or f['bar']
      var UnOp = new NonTerminal("UnOp");
      var BinOp = new NonTerminal("BinOp", "operator");
      var PrefixIncDec = new NonTerminal("PrefixIncDec", typeof(IncDecNode));
      var PostfixIncDec = new NonTerminal("PostfixIncDec", typeof(IncDecNode));
      var IncDecOp = new NonTerminal("IncDecOp");
      var AssignmentStmt = new NonTerminal("AssignmentStmt", typeof(AssignmentNode));
      var AssignmentOp = new NonTerminal("AssignmentOp", "assignment operator");
      var Statement = new NonTerminal("Statement");
      var Program = new NonTerminal("Program", typeof(StatementListNode));

      // 3. BNF rules
      Expr.Rule = Term | UnExpr | BinExpr | PrefixIncDec | PostfixIncDec | TernaryIfExpr  
      			| ParametersSection
      			| GlobalSection
      			| FieldsSection;
      
      Term.Rule = number | ParExpr | stringLit | FunctionCall | identifier | MemberAccess | IndexedAccess;
      
      ParExpr.Rule = "(" + Expr + ")";
      UnExpr.Rule = UnOp + Term + ReduceHere();
      UnOp.Rule = ToTerm("+") | "-" | "!"; 
      BinExpr.Rule = Expr + BinOp + Expr;
      BinOp.Rule = ToTerm("+") | "-" | "*" | "/" | "**" | "==" | "<" | "<=" | ">" | ">=" | "!=" | "&&" | "||" | "&" | "|";
      PrefixIncDec.Rule = IncDecOp + identifier;
      PostfixIncDec.Rule = identifier + PreferShiftHere() + IncDecOp;
      IncDecOp.Rule = ToTerm("++") | "--";
      TernaryIfExpr.Rule = Expr + "?" + Expr + ":" + Expr;
      MemberAccess.Rule = Expr + PreferShiftHere() + "." + identifier; 
      AssignmentStmt.Rule = ObjectRef + AssignmentOp + Expr;
      AssignmentOp.Rule = ToTerm("=") | "+=" | "-=" | "*=" | "/=";
      Statement.Rule = AssignmentStmt | Expr | Empty;
      ArgList.Rule = MakeStarRule(ArgList, comma, Expr);
      FunctionCall.Rule = Expr + PreferShiftHere() + "(" + ArgList + ")";
      
      // SharpReporting
      
      ParametersSection.Rule = ToTerm("Parameters") + exclamationMark + identifier;
      FieldsSection.Rule  = ToTerm("Fields") + exclamationMark + identifier;
      
      GlobalSection.Rule = ToTerm("Globals") + exclamationMark + identifier;

      // end of SharpReporting
      
      FunctionCall.NodeCaptionTemplate = "call #{0}(...)";
      
      ObjectRef.Rule = identifier | MemberAccess | IndexedAccess;
      IndexedAccess.Rule = Expr + PreferShiftHere() + "[" + Expr + "]";

      Program.Rule = MakePlusRule(Program, NewLine, Statement);

      this.Root = Program;       // Set grammar root

      // 4. Operators precedence
      RegisterOperators(10, "?");
      RegisterOperators(15, "&", "&&", "|", "||");
      RegisterOperators(20, "==", "<", "<=", ">", ">=", "!=");
      RegisterOperators(30, "+", "-");
      RegisterOperators(40, "*", "/");
      RegisterOperators(50, Associativity.Right, "**");
      RegisterOperators(60, "!");
      // For precedence to work, we need to take care of one more thing: BinOp. 
      //For BinOp which is or-combination of binary operators, we need to either 
      // 1) mark it transient or 2) set flag TermFlags.InheritPrecedence
      // We use first option, making it Transient.  

      // 5. Punctuation and transient terms
      MarkPunctuation("(", ")", "?", ":", "[", "]");
      RegisterBracePair("(", ")");
      RegisterBracePair("[", "]");
      MarkTransient(Term, Expr, Statement, BinOp, UnOp, IncDecOp, AssignmentOp, ParExpr, ObjectRef);

      // 7. Syntax error reporting
      MarkNotReported("++", "--");
      AddToNoReportGroup("(", "++", "--");
      AddToNoReportGroup(NewLine);
      AddOperatorReportGroup("operator");
      AddTermsReportGroup("assignment operator", "=", "+=", "-=", "*=", "/=");

      //8. Console
      ConsoleTitle = "Irony Expression Evaluator";
      ConsoleGreeting =
@"Irony Expression Evaluator 

  Supports variable assignments, arithmetic operators (+, -, *, /),
    augmented assignments (+=, -=, etc), prefix/postfix operators ++,--, string operations. 
  Supports big integer arithmetics, string operations.
  Supports strings with embedded expressions : ""name: #{name}""

Press Ctrl-C to exit the program at any time.
";
      ConsolePrompt = "?";
      ConsolePromptMoreInput = "?";

      //9. Language flags. 
      // Automatically add NewLine before EOF so that our BNF rules work correctly when there's no final line break in source
      this.LanguageFlags = LanguageFlags.NewLineBeforeEOF | LanguageFlags.CreateAst | LanguageFlags.SupportsBigInt;
    }
		
		
		public override LanguageRuntime CreateRuntime(LanguageData language)
		{
			return new ReportingLanguageRuntime(language);
		}
	}
}
