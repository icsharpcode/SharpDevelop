// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class XmlCompletionItemCollectionTests
	{
		[Test]
		public void CollectionToArrayReturnsExpectedArray()
		{
			XmlCompletionItem firstItem = new XmlCompletionItem("text", "desc", XmlCompletionDataType.XmlElement);
			XmlCompletionItem secondItem = new XmlCompletionItem("text2", "desc", XmlCompletionDataType.XmlAttribute);

			List<XmlCompletionItem> items = new List<XmlCompletionItem>();
			items.Add(firstItem);
			items.Add(secondItem);
			
			XmlCompletionItemCollection itemCollection = new XmlCompletionItemCollection();
			itemCollection.Add(firstItem);
			itemCollection.Add(secondItem);
			
			Assert.AreEqual(items.ToArray(), itemCollection.ToArray());
		}
	}
}
