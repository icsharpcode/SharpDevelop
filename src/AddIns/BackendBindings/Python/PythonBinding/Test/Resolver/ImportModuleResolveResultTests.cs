// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ImportModuleResolveResultTests
	{
		[Test]
		public void NamePropertyMatchesTextPassedToConstructor()
		{
			PythonImportExpression expression = new PythonImportExpression("import abc");
			PythonImportModuleResolveResult result = new PythonImportModuleResolveResult(expression);
			Assert.AreEqual("abc", result.Name);
		}
	
		[Test]
		public void GetCompletionDataReturnsStandardMathPythonModuleWhenImportNameIsEmptyString()
		{
			PythonImportExpression expression = new PythonImportExpression(String.Empty);
			PythonImportModuleResolveResult result = new PythonImportModuleResolveResult(expression);
			MockProjectContent projectContent = new MockProjectContent();
			Assert.Contains(new NamespaceEntry("math"), result.GetCompletionData(projectContent));
		}
		
		[Test]
		public void ClonedPythonModuleResultReturnsSameCompletionItems()
		{
			PythonImportExpression expression = new PythonImportExpression(String.Empty);
			PythonImportModuleResolveResult result = new PythonImportModuleResolveResult(expression);
			ResolveResult clonedResult = result.Clone();
			MockProjectContent projectContent = new MockProjectContent();
			Assert.Contains(new NamespaceEntry("math"), clonedResult.GetCompletionData(projectContent));
		}
	}
}
