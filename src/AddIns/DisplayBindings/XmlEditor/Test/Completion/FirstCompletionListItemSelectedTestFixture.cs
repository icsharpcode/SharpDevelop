// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.TextEditor;
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
		XmlCompletionDataProvider provider;
		ICompletionItem selectedCompletionData;
		ICompletionItemList completionDataItems;
		XmlSchemaCompletionData defaultSchema;
		IList<ICompletionItem> completionList;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			defaultSchema = new XmlSchemaCompletionData(ResourceManager.GetXhtmlStrictSchema());
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			schemas.Add(defaultSchema);
			provider = new XmlCompletionDataProvider(schemas, defaultSchema, String.Empty);
			completionDataItems = provider.GenerateCompletionData(String.Empty, '<');
			selectedCompletionData = completionDataItems.SuggestedItem;
			completionList = (IList<ICompletionItem>)completionDataItems.Items;
		}
		
		/// <summary>
		/// Sanity check to make sure that we actually have some completion
		/// data items from the xml completion data provider.
		/// </summary>
		[Test]
		public void GeneratedCompletionItemsHasMoreThanOneItem()
		{
			Assert.IsTrue(completionList.Count > 1);
		}
		
		[Test]
		public void SelectedCompletionItemMatchesFirstItemInCompletionList()
		{
			Assert.AreSame(completionList[0], selectedCompletionData);
		}
	}
}
