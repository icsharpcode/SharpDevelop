// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class XmlCompletionItemComparableTests
	{
		[Test]
		public void CompareToReturnsZeroIfCompletionTextIsTheSame()
		{
			XmlCompletionItem lhs = new XmlCompletionItem("a");
			XmlCompletionItem rhs = new XmlCompletionItem("a");
			
			Assert.AreEqual(0, lhs.CompareTo(rhs));
		}
		
		[Test]
		public void CompareToReturnsOneIfLhsCompletionItemTextIsAfterRhsCompletionItemText()
		{
			XmlCompletionItem lhs = new XmlCompletionItem("b");
			XmlCompletionItem rhs = new XmlCompletionItem("a");
			
			Assert.AreEqual(1, lhs.CompareTo(rhs));
		}
		
		[Test]
		public void CompareToReturnsMinusOneIfLhsCompletionItemTextIsBeforeRhsCompletionItemText()
		{
			XmlCompletionItem lhs = new XmlCompletionItem("a");
			XmlCompletionItem rhs = new XmlCompletionItem("b");
			
			Assert.AreEqual(-1, lhs.CompareTo(rhs));
		}		
	}
}
