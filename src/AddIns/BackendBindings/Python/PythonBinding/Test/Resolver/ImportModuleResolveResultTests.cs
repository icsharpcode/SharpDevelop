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
