// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Completion
{
	/// <summary>
	/// Tests that the first item in the completion list view is selected by
	/// default. When coding in C# the default is to not select the
	/// first item in the completion list. With XML there tends to not
	/// be very many items in the completion list so selecting the first
	/// one can often make editing quicker.
	/// </summary>
	[TestFixture]
	public class FirstCompletionListItemSelectedTestFixture
	{
		ICompletionItem selectedCompletionItem;
		XmlCompletionItemCollection completionItems;
		XmlSchemaCompletion defaultSchema;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			defaultSchema = new XmlSchemaCompletion(ResourceManager.ReadXhtmlStrictSchema());
			completionItems = new XmlCompletionItemCollection(defaultSchema.GetRootElementCompletion());
			selectedCompletionItem = completionItems.SuggestedItem;
		}
		
		/// <summary>
		/// Sanity check to make sure that we actually have some completion
		/// data items from the xml completion data provider.
		/// </summary>
		[Test]
		public void GeneratedCompletionItemsHasMoreThanOneItem()
		{
			Assert.IsTrue(completionItems.Count > 1);
		}
		
		[Test]
		public void SelectedCompletionItemMatchesFirstItemInCompletionList()
		{
			Assert.AreSame(completionItems[0], selectedCompletionItem);
		}
	}
}
