// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.RubyBinding;
using IronRuby.Compiler.Ast;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// Description of RubyParserHelper.
	/// </summary>
	public class RubyParserHelper
	{
		/// <summary>
		/// Parses the code and returns the first statement as an assignment express.
		/// </summary>
		public static SimpleAssignmentExpression GetSimpleAssignmentExpression(string code)
		{
			return GetFirstExpression(code) as SimpleAssignmentExpression;
		}
		
		/// <summary>
		/// Parses the code and returns the first statement's expression as a method call.
		/// </summary>		
		public static MethodCall GetMethodCall(string code)
		{
			return GetFirstExpression(code) as MethodCall;
		}
		
		public static Expression GetLastExpression(string code)
		{
			return Parse(code).Last;			
		}
		
		static Expression GetFirstExpression(string code)
		{
			return Parse(code).First;
		}
		
		static Statements Parse(string code)
		{
			RubyParser parser = new RubyParser();
			SourceUnitTree unit = parser.CreateAst(@"snippet.rb", code);
			return unit.Statements;
		}
	}
}
