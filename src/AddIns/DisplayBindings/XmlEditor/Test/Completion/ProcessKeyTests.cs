// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class CompletionListProcessKeyTests
	{
		XmlCompletionItemCollection completionItems;
		
		[SetUp]
		public void Init()
		{
			completionItems = new XmlCompletionItemCollection();
		}
		
		[Test]
		public void ProcessInputWithSpaceCharReturnsNormalKey()
		{
			Assert.AreEqual(CompletionItemListKeyResult.InsertionKey, completionItems.ProcessInput(' '));
		}
		
		[Test]
		public void ProcessInputWithTabCharReturnsInsertionKey()
		{
			Assert.AreEqual(CompletionItemListKeyResult.InsertionKey, completionItems.ProcessInput('\t'));
		}		

		[Test]
		public void ProcessInputWithColonCharReturnsNormalKey()
		{
			Assert.AreEqual(CompletionItemListKeyResult.NormalKey, completionItems.ProcessInput(':'));
		}
		
		[Test]
		public void ProcessInputWithDotCharReturnsNormalKey()
		{
			Assert.AreEqual(CompletionItemListKeyResult.NormalKey, completionItems.ProcessInput('.'));
		}
		
		[Test]
		public void ProcessInputWithUnderscoreCharReturnsNormalKey()
		{
			Assert.AreEqual(CompletionItemListKeyResult.NormalKey, completionItems.ProcessInput('_'));
		}	
	}
}
