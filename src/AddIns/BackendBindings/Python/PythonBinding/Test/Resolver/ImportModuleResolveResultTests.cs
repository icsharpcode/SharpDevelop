// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ImportModuleResolveResultTests
	{
		[Test]
		public void Name_ExpressionIsImportFollowedByName_MatchesNameAfterImport()
		{
			PythonImportExpression expression = new PythonImportExpression("import abc");
			PythonImportModuleResolveResult result = new PythonImportModuleResolveResult(expression);
			string expectedName = "abc";
			Assert.AreEqual(expectedName, result.Name);
		}
	
		[Test]
		public void GetCompletionData_WhenImportNameIsEmptyString_ReturnsStandardMathPythonModule()
		{
			PythonImportExpression expression = new PythonImportExpression(String.Empty);
			PythonImportModuleResolveResult result = new PythonImportModuleResolveResult(expression);
			MockProjectContent projectContent = new MockProjectContent();
			
			List<ICompletionEntry> completionItems = result.GetCompletionData(projectContent);
			NamespaceEntry mathNamespaceCompletionItem = new NamespaceEntry("math");
			Assert.Contains(mathNamespaceCompletionItem, completionItems);
		}
		
		[Test]
		public void GetCompletionData_ClonedPythonModuleResult_ReturnsSameCompletionItems()
		{
			PythonImportExpression expression = new PythonImportExpression(String.Empty);
			PythonImportModuleResolveResult result = new PythonImportModuleResolveResult(expression);
			ResolveResult clonedResult = result.Clone();
			MockProjectContent projectContent = new MockProjectContent();
			
			List<ICompletionEntry> completionItems = clonedResult.GetCompletionData(projectContent);
			NamespaceEntry mathNamespaceCompletionItem = new NamespaceEntry("math");
			Assert.Contains(mathNamespaceCompletionItem, completionItems);
		}
	}
}
