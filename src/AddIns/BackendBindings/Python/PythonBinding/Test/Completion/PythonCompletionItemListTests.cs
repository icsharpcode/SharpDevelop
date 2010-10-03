// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using NUnit.Framework;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class PythonCompletionItemListTests
	{
		PythonCompletionItemList completionItemList;
		
		void CreatePythonCompletionItemList()
		{
			completionItemList = new PythonCompletionItemList();
		}
		
		[Test]
		public void ProcessInput_KeyIsAsterisk_ReturnsNormalKey()
		{
			CreatePythonCompletionItemList();
			CompletionItemListKeyResult result = completionItemList.ProcessInput('*');
			CompletionItemListKeyResult expectedResult = CompletionItemListKeyResult.NormalKey;
			
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void ProcessInput_KeyIsOpenParenthesis_ReturnsNormalKey()
		{
			CreatePythonCompletionItemList();
			CompletionItemListKeyResult result = completionItemList.ProcessInput('(');
			CompletionItemListKeyResult expectedResult = CompletionItemListKeyResult.NormalKey;
			
			Assert.AreEqual(expectedResult, result);
		}
	}
}
