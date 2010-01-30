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
	public class ResolveSystemImportedAsMySystemTestFixture : ResolveTestFixtureBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			ArrayList namespaceItems = new ArrayList();
			DefaultClass consoleClass = new DefaultClass(compilationUnit, "System.Console");
			namespaceItems.Add(consoleClass);
			projectContent.AddExistingNamespaceContents("System", namespaceItems);
			
			return new ExpressionResult("MySystem", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"import System as MySystem\r\n" +
				"MySystem.\r\n" +
				"\r\n";
		}
				
		[Test]
		public void ResolveResultContainsConsoleClass()
		{
			ArrayList items = GetCompletionItems();
			IClass consoleClass = PythonCompletionItemsHelper.FindClassFromCollection("Console", items);
			Assert.IsNotNull(consoleClass);
		}
		
		ArrayList GetCompletionItems()
		{
			return resolveResult.GetCompletionData(projectContent);
		}
		
		[Test]
		public void NamespaceResolveResultNameIsSystem()
		{
			NamespaceResolveResult namespaceResolveResult = resolveResult as NamespaceResolveResult;
			Assert.AreEqual("System", namespaceResolveResult.Name);
		}
		
		[Test]
		public void MockProjectContentSystemNamespaceContentsIncludesConsoleClass()
		{
			ArrayList items = projectContent.GetNamespaceContents("System");
			IClass consoleClass = PythonCompletionItemsHelper.FindClassFromCollection("Console", items);
			Assert.IsNotNull(consoleClass);
		}
		
		[Test]
		public void MockProjectContentNamespaceExistsReturnsTrueForSystem()
		{
			Assert.IsTrue(projectContent.NamespaceExists("System"));
		}
	}
}
