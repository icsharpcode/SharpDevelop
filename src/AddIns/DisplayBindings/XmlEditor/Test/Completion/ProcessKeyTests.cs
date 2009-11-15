// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			Assert.AreEqual(CompletionItemListKeyResult.NormalKey, completionItems.ProcessInput(' '));
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
