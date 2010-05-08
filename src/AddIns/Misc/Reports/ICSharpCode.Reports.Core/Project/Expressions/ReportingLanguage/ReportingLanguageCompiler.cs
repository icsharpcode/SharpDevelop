/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 14.06.2009
 * Zeit: 18:27
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using ICSharpCode.Reports.Core.Interfaces;
using System;
using System.Collections.Generic;
using Irony.CompilerServices;
using SimpleExpressionEvaluator;
using SimpleExpressionEvaluator.Compilation;
using SimpleExpressionEvaluator.Compilation.Functions;
using SimpleExpressionEvaluator.Utilities;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
	/// <summary>
	/// Description of ReportingLanguageCompiler.
	/// </summary>
	public class ReportingLanguageCompiler:SimpleExpressionLanguageCompiler
	{
		private ISinglePage singlePage;
		private ReportingLanguage reportingLanguage;
		private Compiler compiler;
		
		public ReportingLanguageCompiler():base()
		{
			this.reportingLanguage = new ReportingLanguage();
			this.compiler = new Compiler(this.reportingLanguage);
		}
		
		
		//Don't call base
		public override IExpression<T> CompileExpression<T>(string expression)
		{
			if (String.IsNullOrEmpty(expression)) {
				return null;
			}
			
			string delim = "=";
			string str = expression.Trim();
			if (String.IsNullOrEmpty(str)) {
				return null;
			}
			
			string cleaned = str.TrimStart (delim.ToCharArray());
			if (!String.IsNullOrEmpty(cleaned)) {
				ParseTree node = this.compiler.Parse(cleaned);
				if (node.Root == null) {
					return null;
				}
				return CompileExpression<T>(node.Root);
			}
			return null;
		}
		
		
		// Don't call base
		protected override IExpression<T> CompileExpression<T>(ParseTreeNode root)
		{
			ParseTreeNode expr = root.ChildNodes[0];
			IExpression rootNode = CompileExpressionNode(ExpressionNodeFactory,expr.ChildNodes[0]);
			return new Expression<T>(rootNode);
		}
		
		// don't call base
		protected override IExpression CompileExpressionNode(IExpressionNodeFactory factory,ParseTreeNode astNode)
		{
			switch (astNode.Term.Name)
			{
				case "Number":
					var num = TypeNormalizer.EnsureType<double>(astNode.Token.Value);
					return factory.CreateLiteral(num);
				case "String":
					var str = TypeNormalizer.EnsureType<string>(astNode.Token.Value);
					return factory.CreateLiteral(str);
				case "Boolean":
					var bln = TypeNormalizer.EnsureType<bool>(astNode.Token.Value);
					return factory.CreateLiteral(bln);
				case "Identifier":
					var variable = astNode.Token.Value;
					if (variable != null)
						return factory.CreateVariable(variable.ToString());
					break;
				case "Symbol" :
					var str_1 = TypeNormalizer.EnsureType<string>(astNode.Token.Value);
					return factory.CreateLiteral(str_1);
				case "UserSectionStmt":
					return factory.CreateFunction(astNode.ChildNodes[2].Token.Text,null);
				case "GlobalSectionStmt" :
					return factory.CreateLiteral(this.SinglePage.EvaluatePropertyPath(astNode.ChildNodes[2].Token.Text));
				case "ParameterSectionStmt" :
					return CompileExpressionNode(factory, astNode.ChildNodes[2]);
				case "FieldsSectionStmt" :
					return CompileExpressionNode (factory,astNode.ChildNodes[2]);
				case "ParExpr" :
					return CompileExpressionNode(factory, astNode.ChildNodes[0]);
				case "BinExpr":
					if (astNode.ChildNodes.Count == 3 &&
					    astNode.ChildNodes[1].Term is SymbolTerminal)
					{
						IExpression left = CompileExpressionNode(factory, astNode.ChildNodes[0]);
						IExpression right = CompileExpressionNode(factory, astNode.ChildNodes[2]);
						return factory.CreateBinaryOperator(astNode.ChildNodes[1].Term.Name, left, right);
					}
					if (astNode.ChildNodes.Count == 2 &&
					    astNode.ChildNodes[0].Term is SymbolTerminal)
					{
						IExpression arg = CompileExpressionNode(factory, astNode.ChildNodes[1]);
						return factory.CreateUnaryOperator(astNode.ChildNodes[0].Term.Name, arg);
					}
					if (astNode.ChildNodes.Count == 2 &&
					    astNode.ChildNodes[1].Term is SymbolTerminal)
					{
						IExpression arg = CompileExpressionNode(factory, astNode.ChildNodes[0]);
						return factory.CreateUnaryOperator(astNode.ChildNodes[1].Term.Name, arg);
					}
					break;
					
				case "QualifiedName":
					var parts = new List<string>();

					if (astNode.ChildNodes.Count == 2)
						return new QualifiedName(new[] {astNode.ChildNodes[1].Token.ValueString});
					/*
                    //Condition ought to be impossible
                    if (astNode.ChildNodes.Count != 3 )
                        throw new Exception("Malformed QualifiedName - should have 3 child nodes");
					 */
					if (astNode.ChildNodes.Count == 1) {
						return CompileExpressionNode(factory, astNode.ChildNodes[0]);
					}
					SimpleExpressionLanguageCompiler.ExtractQualifiedName(astNode, parts);
					
					return new QualifiedName(parts.ToArray());
					
				case "FunctionExpression":
					IExpression [] args = null;
					string functionName = (astNode.ChildNodes[0].ChildNodes[0].Token.ValueString);
					if (astNode.ChildNodes.Count == 1) {
						args = new IExpression[astNode.ChildNodes[0].ChildNodes[0].ChildNodes.Count];
					} else {
						args = new IExpression[astNode.ChildNodes[1].ChildNodes.Count];
						for (int i = 0; i < astNode.ChildNodes[1].ChildNodes.Count;i++)
						{
							args[i] = CompileExpressionNode(factory, astNode.ChildNodes[1].ChildNodes[i]);
						}
					}
					return factory.CreateFunction(functionName, args);
					
					
				case "IfThen":
					IExpression condition = CompileExpressionNode(factory,astNode.ChildNodes[1].ChildNodes[0]);
					IExpression trueExpr = CompileExpressionNode(factory, astNode.ChildNodes[3]);
					IExpression falseExpr = null;
					if (astNode.ChildNodes.Count == 6)
						falseExpr = CompileExpressionNode(factory, astNode.ChildNodes[5]);
					var func = new IfThen();
					if (falseExpr != null)
						func.AcceptArguments(condition, trueExpr, falseExpr);
					else
						func.AcceptArguments(condition, trueExpr);
					return func;
					
					/*
                case "ArrayExpression":
                    IExpression context = CompileExpressionNode(factory, astNode.ChildNodes[0]);
                    IExpression index = CompileExpressionNode(factory, astNode.ChildNodes[1]);
                    var indexer = new ItemAtIndex();
                    indexer.AcceptArguments(context, index);
                    return indexer;
				
					 */
			}
			return null;
		}
		
		
		
		// don't call base
		protected  IExpression old_CompileExpressionNode(IExpressionNodeFactory factory,ParseTreeNode astNode)
//		protected override IExpression old_CompileExpressionNode(IExpressionNodeFactory factory,ParseTreeNode astNode)
		{
			switch (astNode.Term.Name)
			{
				case "Number":
					var num = TypeNormalizer.EnsureType<double>(astNode.Token.Value);
					return factory.CreateLiteral(num);
				case "String":
					var str = TypeNormalizer.EnsureType<string>(astNode.Token.Value);
					return factory.CreateLiteral(str);
				case "Boolean":
					var bln = TypeNormalizer.EnsureType<bool>(astNode.Token.Value);
					return factory.CreateLiteral(bln);
				case "Identifier":
					var variable = astNode.Token.Value;
					if (variable != null)
						return factory.CreateVariable(variable.ToString());
					break;
				case "Symbol" :
					var str_1 = TypeNormalizer.EnsureType<string>(astNode.Token.Value);
					return factory.CreateLiteral(str_1);
				case "UserSectionStmt":
					return factory.CreateFunction(astNode.ChildNodes[2].Token.Text,null);
				case "GlobalSectionStmt" :
					return factory.CreateLiteral(this.SinglePage.EvaluatePropertyPath(astNode.ChildNodes[2].Token.Text));
				case "ParameterSectionStmt" :
					return CompileExpressionNode(factory, astNode.ChildNodes[2]);
					
				case "ParExpr" :
					return CompileExpressionNode(factory, astNode.ChildNodes[0]);
					
				case "BinExpr":
					if (astNode.ChildNodes.Count == 3 &&
					    astNode.ChildNodes[1].Term is SymbolTerminal)
					{
						IExpression left = CompileExpressionNode(factory, astNode.ChildNodes[0]);
						IExpression right = CompileExpressionNode(factory, astNode.ChildNodes[2]);
						return factory.CreateBinaryOperator(astNode.ChildNodes[1].Term.Name, left, right);
					}
					if (astNode.ChildNodes.Count == 2 &&
					    astNode.ChildNodes[0].Term is SymbolTerminal)
					{
						IExpression arg = CompileExpressionNode(factory, astNode.ChildNodes[1]);
						return factory.CreateUnaryOperator(astNode.ChildNodes[0].Term.Name, arg);
					}
					if (astNode.ChildNodes.Count == 2 &&
					    astNode.ChildNodes[1].Term is SymbolTerminal)
					{
						IExpression arg = CompileExpressionNode(factory, astNode.ChildNodes[0]);
						return factory.CreateUnaryOperator(astNode.ChildNodes[1].Term.Name, arg);
					}
					break;
					
				case "QualifiedName":
					var parts = new List<string>();

					if (astNode.ChildNodes.Count == 2)
						return new QualifiedName(new[] {astNode.ChildNodes[1].Token.ValueString});
					/*
                    //Condition ought to be impossible
                    if (astNode.ChildNodes.Count != 3 )
                        throw new Exception("Malformed QualifiedName - should have 3 child nodes");
					 */
					if (astNode.ChildNodes.Count == 1) {
						return CompileExpressionNode(factory, astNode.ChildNodes[0]);
					}
					SimpleExpressionLanguageCompiler.ExtractQualifiedName(astNode, parts);
					
					return new QualifiedName(parts.ToArray());
					
				case "FunctionExpression":
					string functionName = (astNode.ChildNodes[0].ChildNodes[0].Token.ValueString);
				
					var args = new IExpression[astNode.ChildNodes[0].ChildNodes[0].ChildNodes.Count];
					for (int i = 0; i < astNode.ChildNodes[0].ChildNodes[0].ChildNodes.Count;i++)
					{
						args[i] = CompileExpressionNode(factory, astNode.ChildNodes[1].ChildNodes[i]);
					}

					return factory.CreateFunction(functionName, args);
					
					
				case "IfThen":
					IExpression condition = CompileExpressionNode(factory,astNode.ChildNodes[1].ChildNodes[0]);
					IExpression trueExpr = CompileExpressionNode(factory, astNode.ChildNodes[3]);
					IExpression falseExpr = null;
					if (astNode.ChildNodes.Count == 6)
						falseExpr = CompileExpressionNode(factory, astNode.ChildNodes[5]);
					var func = new IfThen();
					if (falseExpr != null)
						func.AcceptArguments(condition, trueExpr, falseExpr);
					else
						func.AcceptArguments(condition, trueExpr);
					return func;
					
					/*
                case "ArrayExpression":
                    IExpression context = CompileExpressionNode(factory, astNode.ChildNodes[0]);
                    IExpression index = CompileExpressionNode(factory, astNode.ChildNodes[1]);
                    var indexer = new ItemAtIndex();
                    indexer.AcceptArguments(context, index);
                    return indexer;
				
					 */
			}
			return null;
		}
		
		
		public ISinglePage SinglePage {
			get { return singlePage; }
			set { singlePage = value; }
		}
		
	}
}
