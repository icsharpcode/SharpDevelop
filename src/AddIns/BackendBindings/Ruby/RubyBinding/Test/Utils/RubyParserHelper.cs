// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
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
			SourceUnitTree unit = parser.CreateAst(@"snippet.rb", new StringTextBuffer(code));
			return unit.Statements;
		}
	}
}
