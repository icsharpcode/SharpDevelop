// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using UnitTesting.Tests.Utils;
using ScriptingUtils = ICSharpCode.Scripting.Tests.Utils;

namespace PythonBinding.Tests.Utils
{
	public class PythonResolverTestsHelper
	{
		public ScriptingUtils.MockProjectContent ProjectContent;
		public	DefaultCompilationUnit CompilationUnit;
		public ParseInformation ParseInfo;
		public PythonResolverContext Context;
		public PythonResolver Resolver;
		public ResolveResult ResolveResult;
		
		public PythonResolverTestsHelper(string code)
		{
			ProjectContent = new ScriptingUtils.MockProjectContent();
			PythonParser parser = new PythonParser();
			string fileName = @"test.py";
			CompilationUnit = parser.Parse(ProjectContent, fileName, code) as DefaultCompilationUnit;
			
			ParseInfo = new ParseInformation(CompilationUnit);
			Context = new PythonResolverContext(ParseInfo);
			
			Resolver = new PythonResolver();
		}
		
		public PythonResolverTestsHelper()
			: this(String.Empty)
		{
		}
		
		public ResolveResult Resolve(string expression)
		{
			ExpressionResult expressionResult = new ExpressionResult(expression);
			ResolveResult = Resolver.Resolve(Context, expressionResult);
			return ResolveResult;
		}
		
		public MemberResolveResult MemberResultResult {
			get { return ResolveResult as MemberResolveResult; }
		}
		
		public MockClass CreateClass(string name)
		{
			return new MockClass(ProjectContent, name);
		}
	}
}
