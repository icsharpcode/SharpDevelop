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
