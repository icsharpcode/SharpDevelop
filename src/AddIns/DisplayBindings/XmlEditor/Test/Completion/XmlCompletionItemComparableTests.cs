// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
