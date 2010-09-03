// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class XmlCompletionItemIsEqualTests
	{
		[Test]
		public void TwoItemsAreEqualIfTextAndXmlCompletionDataTypeAreEqual()
		{
			XmlCompletionItem lhs = new XmlCompletionItem("text", "description", XmlCompletionItemType.XmlElement);
			XmlCompletionItem rhs = new XmlCompletionItem("text", "description", XmlCompletionItemType.XmlElement);
			
			Assert.IsTrue(lhs.Equals(rhs));
		}
		
		[Test]
		public void TwoItemsAreNotEqualIfTextIsDifferent()
		{
			XmlCompletionItem lhs = new XmlCompletionItem("text", "description", XmlCompletionItemType.XmlElement);
			XmlCompletionItem rhs = new XmlCompletionItem("different-text", "description", XmlCompletionItemType.XmlElement);
			
			Assert.IsFalse(lhs.Equals(rhs));
		}		
		
		[Test]
		public void TwoItemsAreNotEqualIfXmlCompletionDataTypeIsDifferent()
		{
			XmlCompletionItem lhs = new XmlCompletionItem("text", "description", XmlCompletionItemType.XmlElement);
			XmlCompletionItem rhs = new XmlCompletionItem("text", "description", XmlCompletionItemType.XmlAttribute);
			
			Assert.IsFalse(lhs.Equals(rhs));
		}	
		
		[Test]
		public void NullIsNotEqualToCompletionItem()
		{
			XmlCompletionItem lhs = new XmlCompletionItem("text");
			Assert.IsFalse(lhs.Equals(null));
		}
	}
}
