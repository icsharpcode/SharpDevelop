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
