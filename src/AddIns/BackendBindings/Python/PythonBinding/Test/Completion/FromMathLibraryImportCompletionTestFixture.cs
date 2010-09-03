// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
