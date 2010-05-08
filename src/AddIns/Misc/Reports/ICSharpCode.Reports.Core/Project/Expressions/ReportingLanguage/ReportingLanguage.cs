/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 14.06.2009
 * Zeit: 18:47
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using Irony.CompilerServices;
using Irony.Scripting.Ast;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
	/// <summary>
	/// Description of ReportingLanguage.
	/// </summary>
	[Language("ReportingLanguage", "1.0", "ExpressionEvaluator for SharpReporting")]
	public class ReportingLanguage : Grammar {
		public ReportingLanguage():base(false)
		{
			// 1. Terminals

			var numberLiteral = TerminalFactory.CreateCSharpNumber("Number");
			
			var boolean = new ConstantTerminal("Boolean");
            boolean.Add("true", true);
            boolean.Add("false", false);
            boolean.Priority = 10;
            
            
            var nil = new ConstantTerminal("Null");
            nil.Add("null", null);
            nil.Add("nothing", null);
            nil.Priority = 10;
            
			var identifier = new IdentifierTerminal("Identifier");
         
			var stringLiteral = new StringLiteral("String","'",StringFlags.AllowsDoubledQuote);
			stringLiteral.AddStartEnd("\"",StringFlags.AllowsAllEscapes);
			
			Terminal dot = Symbol(".", "dot");
            Terminal less = Symbol("<");
            Terminal greater = Symbol(">");
            Terminal LCb = Symbol("(");
            Terminal RCb = Symbol(")");
            Terminal RFb = Symbol("}");
            Terminal LFb = Symbol("{");
            Terminal comma = Symbol(",");
//            Terminal LSb = Symbol("[");
//            Terminal RSb = Symbol("]");
              var exclamationMark = Symbol("!");
              
             Terminal and = Symbol("and");
            and.Priority = 10;

            Terminal or = Symbol("or");
            or.Priority = 10;
            
			var UserSection = Symbol("User");
			var GlobalSection = Symbol("Globals");
			var ParameterSection = Symbol("Parameters");
			var FieldsSection = Symbol("Fields");
			
			// 2. Non-terminals
			
			var FieldRef = new NonTerminal("FieldRef");
			var userSectionStmt = new NonTerminal("UserSectionStmt");
			var globalSectionStmt = new NonTerminal("GlobalSectionStmt");
			var parameterSectionStmt = new NonTerminal("ParameterSectionStmt");
			var fieldsSectionStmt = new NonTerminal("FieldsSectionStmt");
			
			var QualifiedName = new NonTerminal("QualifiedName");
			var FunctionExpression = new NonTerminal("FunctionExpression");
			
			var Condition = new NonTerminal("Condition");
            var Conditional = new NonTerminal("IfThen");  
			  
			var Expr = new NonTerminal("Expr");
			 
			var BinOp = new NonTerminal("BinOp");
			var LUnOp = new NonTerminal("LUnOp");
			 
			 
			var ExprList = new NonTerminal("ExprList");
			var BinExpr = new NonTerminal("BinExpr", typeof(BinExprNode));	
				
			var ProgramLine = new NonTerminal("ProgramLine");
			var Program = new NonTerminal("Program", typeof(StatementListNode));
			
			// 3. BNF rules
			 
			#region Reporting
			userSectionStmt.Rule = UserSection + exclamationMark + Symbol("UserId")
				|UserSection + exclamationMark + Symbol("Language");

			globalSectionStmt.Rule = GlobalSection + exclamationMark + Symbol("PageNumber")
				| GlobalSection + exclamationMark + Symbol("TotalPages")
				| GlobalSection + exclamationMark + Symbol("ExecutionTime")
				| GlobalSection + exclamationMark + Symbol("ReportFolder")
				| GlobalSection + exclamationMark + Symbol("ReportName");
			
			
			parameterSectionStmt.Rule = ParameterSection + exclamationMark + identifier;
			
			fieldsSectionStmt.Rule = FieldsSection + exclamationMark + identifier;
			#endregion
			
			Expr.Rule = Symbol("null")
				| boolean
				| nil
				| stringLiteral
				| numberLiteral
				| QualifiedName
				| FunctionExpression
				| LCb + Expr + RCb
				| LFb + QualifiedName + RFb
				| Conditional
				| BinExpr
				//| Expr + BinOp + Expr
				//| LUnOp + Expr
				
				| parameterSectionStmt
				| globalSectionStmt
				| userSectionStmt
				| fieldsSectionStmt;
			
		
			
			ExprList.Rule = MakePlusRule(ExprList, comma, Expr);
			
			BinOp.Rule = Symbol("+") | "-" | "*" | "%" | "^" | "&" | "|" | "/"
                         | "&&" | "||" | "==" | "!=" | greater | less
                         | ">=" | "<=" | "is" | "<>"
                         | "=" //| "+=" | "-="
                         | "." | and | or;
			
			LUnOp.Rule = Symbol("-")
                              | "!";
			   
			FunctionExpression.Rule = QualifiedName + LCb + ExprList.Q() + RCb;
			
			QualifiedName.Rule = identifier
				| QualifiedName + dot + identifier 
				| parameterSectionStmt + "!" + identifier 
				| "#" + identifier ;
			
			Condition.Rule = LCb + Expr + RCb;

            Conditional.Rule = "if" + Condition + "then" + Expr |
                               "if" + Condition + "then" + Expr + "else" + Expr |
                               "if" + Condition + "then" + Expr + "otherwise" + Expr;
			
			
		
			BinExpr.Rule =  Expr + BinOp + Expr
				| LUnOp + Expr;
			
			ProgramLine.Rule = Expr + NewLine;
			
			Program.Rule = MakeStarRule(Program, ProgramLine);
			this.Root = Program;       // Set grammar root

			#region 5. Operators precedence
            RegisterOperators(1, "is", "=", "==", "!=", "<>", ">", "<", ">=", "<=");
            RegisterOperators(2, "+", "-");
            RegisterOperators(3, "*", "/", "%");
            RegisterOperators(4, Associativity.Right, "^");
            RegisterOperators(5, "|", "||", "or");
            RegisterOperators(6, "&", "&&", "and");
            RegisterOperators(7, "!");

            #endregion

			RegisterPunctuation("(", ")", "[", "]", "{", "}", ",", ";");
			MarkTransient( Expr, BinOp);
			
			//automatically add NewLine before EOF so that our BNF rules work correctly when there's no final line break in source
			this.SetLanguageFlags(LanguageFlags.NewLineBeforeEOF
			                      | LanguageFlags.SupportsInterpreter 
			                      | LanguageFlags.AutoDetectTransient 
			                      |LanguageFlags.CreateAst);
		}
	}
}
