// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn
{
	public static class AstEvaluator
	{
		/// <returns> Returned value or null for statements </returns>
		public static Value Evaluate(string code, SupportedLanguage language, StackFrame context)
		{
			SnippetParser parser = new SnippetParser(language);
			INode astRoot = parser.Parse(code);
			if (parser.SnippetType == SnippetType.Expression ||
			    parser.SnippetType == SnippetType.Statements) {
				if (parser.Errors.Count > 0) {
					throw new GetValueException(parser.Errors.ErrorOutput);
				}
				try {
					EvaluateAstVisitor visitor = new EvaluateAstVisitor(context);
					
					return astRoot.AcceptVisitor(visitor, null) as Value;
				} catch (NotImplementedException e) {
					throw new GetValueException("Language feature not implemented: " + e.Message);
				}
			}
			throw new GetValueException("Code must be expression or statement");
		}
		
		public static string FormatValue(Value val)
		{
			if (val == null) {
				return null;
			} if (val.IsNull) {
				return "null";
			} else if (val.Type.IsArray) {
				StringBuilder sb = new StringBuilder();
				sb.Append(val.Type.Name);
				sb.Append(" {");
				bool first = true;
				foreach(Value item in val.GetArrayElements()) {
					if (!first) sb.Append(", ");
					first = false;
					sb.Append(FormatValue(item));
				}
				sb.Append("}");
				return sb.ToString();
			} else if (val.Type.IsPrimitive) {
				return val.PrimitiveValue.ToString();
			} else {
				return val.InvokeToString();
			}
		}
	}
}
