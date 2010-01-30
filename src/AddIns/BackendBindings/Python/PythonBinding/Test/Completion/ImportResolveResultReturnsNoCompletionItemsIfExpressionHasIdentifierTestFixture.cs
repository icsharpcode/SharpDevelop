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

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class ImportResolveResultReturnsNoCompletionItemsIfExpressionHasIdentifierTestFixture
	{
		PythonImportExpression importExpression;
		PythonImportModuleResolveResult resolveResult;
		MockProjectContent projectContent;
		
		[SetUp]
		public void Init()
		{
			string code = "from System import Console";
			importExpression = new PythonImportExpression(code);
			resolveResult = new PythonImportModuleResolveResult(importExpression);
			
			projectContent = new MockProjectContent();
			DefaultCompilationUnit unit = new DefaultCompilationUnit(projectContent);
			DefaultClass c = new DefaultClass(unit, "Test");
			ArrayList namespaceItems = new ArrayList();
			namespaceItems.Add(c);
			projectContent.AddExistingNamespaceContents("System", namespaceItems);
		}
		
		[Test]
		public void ProjectContentGetNamespaceReturnsOneItem()
		{
			Assert.AreEqual(1, projectContent.GetNamespaceContents("System").Count);
		}
		
		[Test]
		public void NoCompletionItemsReturned()
		{
			ArrayList items = resolveResult.GetCompletionData(projectContent);
			Assert.AreEqual(0, items.Count);
		}
	}
}
