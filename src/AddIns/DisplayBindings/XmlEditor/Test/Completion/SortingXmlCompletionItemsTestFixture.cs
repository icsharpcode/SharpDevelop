// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class SortingXmlCompletionItemsTestFixture
	{
		XmlCompletionItemCollection completionItems;
		XmlCompletionItem[] expectedCompletionItems;
		
		[SetUp]
		public void Init()
		{
			XmlCompletionItem lastItem = new XmlCompletionItem("cc", XmlCompletionItemType.XmlElement);
			XmlCompletionItem secondItem = new XmlCompletionItem("bb", XmlCompletionItemType.XmlElement);
			XmlCompletionItem firstItem = new XmlCompletionItem("aa", XmlCompletionItemType.XmlElement);
			
			XmlCompletionItemCollection collection = new XmlCompletionItemCollection();
			collection.Add(lastItem);
			collection.Add(secondItem);
			collection.Add(firstItem);
			
			expectedCompletionItems = new XmlCompletionItem[] { firstItem, secondItem, lastItem };
			
			completionItems = new XmlCompletionItemCollection(collection);
			completionItems.Sort();
		}
		
		[Test]
		public void CompletionItemListIsSortedAndContainsTheCorrectCompletionItems()
		{
			Assert.AreEqual(expectedCompletionItems, completionItems.ToArray());
		}
		
		[Test]
		public void SuggestedItemIsFirstItemInCompletionList()
		{
			Assert.AreSame(expectedCompletionItems[0], completionItems.SuggestedItem);
		}
	}
}
