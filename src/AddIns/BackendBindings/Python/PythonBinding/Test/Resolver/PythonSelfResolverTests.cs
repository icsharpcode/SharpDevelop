// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class PythonSelfResolverTests
	{
		PythonSelfResolver resolver;
		ExpressionResult expression;
		ParseInformation parseInfo;
		PythonResolverContext context;
		
		[Test]
		public void Resolve_NoCallingClass_ReturnsNull()
		{
			CreatePythonSelfResolver();
			CreateParseInfo();
			CreatePythonResolverContext();
			
			ResolveResult result = resolver.Resolve(context);
			Assert.IsNull(result);
		}
		
		void CreatePythonSelfResolver()
		{
			resolver = new PythonSelfResolver();
		}
		
		void CreateParseInfo()
		{
			MockProjectContent projectContent = new MockProjectContent();
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			parseInfo = new ParseInformation(unit);
		}
		
		void CreatePythonResolverContext()
		{
			expression = new ExpressionResult("self");
			context = new PythonResolverContext(parseInfo, expression, String.Empty);
		}
	}
}
