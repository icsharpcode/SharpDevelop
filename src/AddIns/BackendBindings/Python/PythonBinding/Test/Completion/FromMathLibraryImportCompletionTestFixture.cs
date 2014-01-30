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

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class FromMathLibraryImportCompletionTestFixture
	{
		PythonImportExpression importExpression;
		PythonImportModuleResolveResult resolveResult;
		MockProjectContent projectContent;
		List<ICompletionEntry> completionItems;
		
		[SetUp]
		public void Init()
		{
			string code = "from math import";
			importExpression = new PythonImportExpression(code);
			resolveResult = new PythonImportModuleResolveResult(importExpression);
			
			projectContent = new MockProjectContent();
			completionItems = resolveResult.GetCompletionData(projectContent);
		}
		
		[Test]
		public void CompletionItemsContainsCosMethodFromMathLibrary()
		{
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("cos", completionItems);
			Assert.IsNotNull(method);
		}
		
		[Test]
		public void CompletionItemsContainsPiPropertyFromMathLibrary()
		{
			IField field = PythonCompletionItemsHelper.FindFieldFromCollection("pi", completionItems);
			Assert.IsNotNull(field);
		}
		
		[Test]
		public void CompletionItemsDoesNotContainNonStaticToStringMethod()
		{
			IMethod method = PythonCompletionItemsHelper.FindMethodFromCollection("ToString", completionItems);
			Assert.IsNull(method);			
		}
		
		[Test]
		public void CompletionItemsContain__name__()
		{
			IField field = PythonCompletionItemsHelper.FindFieldFromCollection("__name__", completionItems);
			Assert.IsNotNull(field);	
		}
		
		[Test]
		public void CompletionItemsContain__package__()
		{
			IField field = PythonCompletionItemsHelper.FindFieldFromCollection("__package__", completionItems);
			Assert.IsNotNull(field);	
		}
	}
}
