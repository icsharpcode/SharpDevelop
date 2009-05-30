// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 3490 $</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Completion
{
	/// <summary>
	/// Tests that the first item in the completion list view is selected by
	/// default. When coding in C# the default is to not selected the
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
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			XmlSchemaCompletionData schema = new XmlSchemaCompletionData(ResourceManager.GetXhtmlStrictSchema());
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			schemas.Add(schema);
			provider = new XmlCompletionDataProvider(schemas, schema, String.Empty);
			TextEditorControl textEditor = new TextEditorControl();
			completionDataItems = provider.GenerateCompletionData("", '<');
			selectedCompletionData = completionDataItems.SuggestedItem;
		}
		
		/// <summary>
		/// Sanity check to make sure that we actually have some completion
		/// data items from the xml completion data provider.
		/// </summary>
		[Test]
		public void HasGeneratedCompletionDataItems()
		{
			Assert.IsNotNull(completionDataItems);
			Assert.IsTrue(completionDataItems.Items.ToArray().Length > 0);
		}
		
		/// <summary>
		/// Default index should be zero so that the first item in the
		/// list view is selected.
		/// </summary>
		[Test]
		public void DefaultIndex()
		{
			Assert.True(completionDataItems.Items.FirstOrDefault() == selectedCompletionData);
		}
		
		[Test]
		public void SelectedCompletionDataExists()
		{
			Assert.IsNotNull(selectedCompletionData);
		}
		
		/// <summary>
		/// First item returned from completion list view should correspond
		/// to the first completion item returned from the xml completion 
		/// data provider after those items have been sorted.
		/// </summary>
		[Test]
		public void SelectedCompletionDataMatches()
		{
			List<ICompletionItem> items = completionDataItems.Items.OrderBy(item => item.Text).ToList();
			Assert.AreEqual(items[0].Text, selectedCompletionData.Text);
		}
	}
}
