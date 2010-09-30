// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveSysModuleImportedAsMySysTestFixture : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("mysys", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"import sys as mysys\r\n" +
				"mysys\r\n" +
				"\r\n";
		}
		
		[Test]
		public void ResolveResultContainsExitMethod()
		{
			List<ICompletionEntry> items = GetCompletionItems();
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("exit", items);
			Assert.IsNotNull(method);
		}
		
		List<ICompletionEntry> GetCompletionItems()
		{
			return resolveResult.GetCompletionData(projectContent);
		}
	}
}
