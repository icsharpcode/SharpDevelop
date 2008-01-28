// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
		ICompletionData selectedCompletionData;
		ICompletionData[] completionDataItems;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			Form parentForm = new Form();
			parentForm.CreateControl();
			
			XmlSchemaCompletionData schema = new XmlSchemaCompletionData(ResourceManager.GetXhtmlStrictSchema());
			XmlSchemaCompletionDataCollection schemas = new XmlSchemaCompletionDataCollection();
			schemas.Add(schema);
			provider = new XmlCompletionDataProvider(schemas, schema, String.Empty);
			TextEditorControl textEditor = new TextEditorControl();
			completionDataItems = provider.GenerateCompletionData(@"C:\Test.xml", textEditor.ActiveTextAreaControl.TextArea, '<');
			using (CodeCompletionWindow completionWindow = CodeCompletionWindow.ShowCompletionWindow(parentForm, textEditor, @"C:\Test.xml", provider, '<')) {
				CodeCompletionListView listView = (CodeCompletionListView)completionWindow.Controls[0];
				selectedCompletionData = listView.SelectedCompletionData;
				completionWindow.Close();
			}
		}
		
		/// <summary>
		/// Sanity check to make sure that we actually have some completion
		/// data items from the xml completion data provider.
		/// </summary>
		[Test]
		public void HasGeneratedCompletionDataItems()
		{
			Assert.IsNotNull(completionDataItems);
			Assert.IsTrue(completionDataItems.Length > 0);
		}
		
		/// <summary>
		/// Default index should be zero so that the first item in the
		/// list view is selected.
		/// </summary>
		[Test]
		public void DefaultIndex()
		{
			Assert.AreEqual(0, provider.DefaultIndex);
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
			List<ICompletionData> items = new List<ICompletionData>(completionDataItems);
			items.Sort(DefaultCompletionData.Compare);
			Assert.AreEqual(items[0].Text, selectedCompletionData.Text);
		}
	}
}
