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
