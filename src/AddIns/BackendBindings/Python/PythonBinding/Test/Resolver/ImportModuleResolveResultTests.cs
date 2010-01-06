// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			Assert.Contains("math", result.GetCompletionData(projectContent));
		}
		
		[Test]
		public void ClonedPythonModuleResultReturnsSameCompletionItems()
		{
			PythonImportExpression expression = new PythonImportExpression(String.Empty);
			PythonImportModuleResolveResult result = new PythonImportModuleResolveResult(expression);
			ResolveResult clonedResult = result.Clone();
			MockProjectContent projectContent = new MockProjectContent();
			Assert.Contains("math", clonedResult.GetCompletionData(projectContent));
		}
	}
}
