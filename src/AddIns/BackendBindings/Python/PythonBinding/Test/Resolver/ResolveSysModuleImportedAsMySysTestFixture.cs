// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveSysModuleImportedAsMySysTestFixture : ResolveTestFixtureBase
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
			ArrayList items = GetCompletionItems();
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("exit", items);
			Assert.IsNotNull(method);
		}
		
		ArrayList GetCompletionItems()
		{
			return resolveResult.GetCompletionData(projectContent);
		}
	}
}
