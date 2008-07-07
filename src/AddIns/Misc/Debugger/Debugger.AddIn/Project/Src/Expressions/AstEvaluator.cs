// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.NRefactory.Ast;
using System;
using Debugger;
using Debugger.AddIn.TreeModel;
using Debugger.Expressions;
using ICSharpCode.NRefactory;

namespace Debugger.AddIn
{
	public static class AstEvaluator
	{
		public static Debugger.AddIn.TreeModel.AbstractNode Evaluate(string code, SupportedLanguage language, StackFrame context)
		{
			SnippetParser parser = new SnippetParser(language);
			INode astRoot = parser.Parse(code);
			if (parser.SnippetType == SnippetType.Expression ||
			    parser.SnippetType == SnippetType.Statements) {
				if (parser.Errors.Count == 0) {
					try {
						EvaluateAstVisitor visitor = new EvaluateAstVisitor(context);
						Value result = (Value)astRoot.AcceptVisitor(visitor, null);
						return new ValueNode(result);
					} catch (NotImplementedException) {
						return null;
					} catch (GetValueException e) {
						return new ErrorNode(new EmptyExpression(), e);
					}
				}
			}
			return null;
		}
	}
}
