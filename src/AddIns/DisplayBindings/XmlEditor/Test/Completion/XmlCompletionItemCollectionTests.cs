// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class XmlCompletionItemCollectionTests
	{
		XmlCompletionItem firstItem;
		XmlCompletionItem secondItem;
		XmlCompletionItemCollection itemCollection;
		
		[SetUp]
		public void Init()
		{
			firstItem = new XmlCompletionItem("text", "desc-1", XmlCompletionItemType.XmlElement);
			secondItem = new XmlCompletionItem("text2", "desc-2", XmlCompletionItemType.XmlAttribute);
			
			itemCollection = new XmlCompletionItemCollection();
			itemCollection.Add(firstItem);
			itemCollection.Add(secondItem);
		}

		[Test]
		public void CollectionToArrayReturnsExpectedArray()
		{
			List<XmlCompletionItem> expectedArray = new List<XmlCompletionItem>();
			expectedArray.Add(firstItem);
			expectedArray.Add(secondItem);
			
			Assert.AreEqual(expectedArray.ToArray(), itemCollection.ToArray());
		}
		
		[Test]
		public void CollectionContainsCompletionItemWithSpecifiedName()
		{			
			Assert.IsTrue(itemCollection.Contains("text"));
		}
		
		[Test]
		public void CollectionDoesNotContainCompletionItemWithUnknownName()
		{
			Assert.IsFalse(itemCollection.Contains("unknown"));
		}		
		
		[Test]
		public void CollectionContainsCompletionItemWithSpecifiedNameAndDescription()
		{
			Assert.IsTrue(itemCollection.ContainsDescription("text", "desc-1"));
		}
		
		[Test]
		public void CollectionDoesNotContainCompletionItemWithKnownNameAndUnknownDescription()
		{
			Assert.IsFalse(itemCollection.ContainsDescription("text", "unknown-description"));
		}		
		
		[Test]
		public void CollectionDoesNotContainCompletionItemWithUnknownNameKnownDescription()
		{
			Assert.IsFalse(itemCollection.ContainsDescription("unknown", "desc-1"));
		}
		
		[Test]
		public void CollectionGetOccurrencesFindsOnlyOneOccurrenceOfCompletionItemWithSpecifiedName()
		{
			Assert.AreEqual(1, itemCollection.GetOccurrences("text"));
		}
		
		[Test]
		public void CollectionOccurrencesFindsOnlyTwoOccurrencesOfDuplicateCompletionItemWithSpecifiedName()
		{
			itemCollection.Add(new XmlCompletionItem("text", "desc-not-checked"));
			Assert.AreEqual(2, itemCollection.GetOccurrences("text"));
		}
		
		[Test]
		public void CollectionFindsNoOccurrencesOfUnknownCompletionItemName()
		{
			Assert.AreEqual(0, itemCollection.GetOccurrences("unknown"));
		}
		
		[Test]
		public void CollectionHasItemsReturnsFalseIfCollectionEmpty()
		{
			itemCollection = new XmlCompletionItemCollection();
			Assert.IsFalse(itemCollection.HasItems);
		}
		
		[Test]
		public void CollectionHasItemsReturnsTrueIfCollectionHasItems()
		{
			Assert.IsTrue(itemCollection.HasItems);
		}

		[Test]
		public void CollectionCanSortItemsByCompletionItemText()
		{
			itemCollection.Add(new XmlCompletionItem("a"));
			itemCollection.Sort();
			
			Assert.AreEqual("a", itemCollection[0].Text);
		}
		
	}
}
