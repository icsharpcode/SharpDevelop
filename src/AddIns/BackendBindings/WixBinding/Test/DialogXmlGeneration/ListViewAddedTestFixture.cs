// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using WixBinding;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogXmlGeneration
{
	/// <summary>
	/// Adds a new list view to the dialog.
	/// </summary>
	[TestFixture]
	public class ListViewAddedTestFixture : DialogLoadingTestFixtureBase
	{
		int listViewItemCount;
		string controlName;
		string controlType;
		string listViewItem1Text;
		string listViewItem2Text;
		
		[SetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {

				ListView listView = new ListView();
				listView.Name = "NewListView";
				listView.Items.Add("New item1");
				listView.Items.Add("New item2");
				dialog.Controls.Add(listView);
				
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				XmlElement controlElement = (XmlElement)dialogElement.ChildNodes[0];
				controlName = controlElement.GetAttribute("Id");
				controlType = controlElement.GetAttribute("Type");
				
				XmlElement listViewElement = (XmlElement)controlElement.ChildNodes[0];
				
				listViewItemCount = listViewElement.ChildNodes.Count;
				XmlElement listViewItem1Element = (XmlElement)listViewElement.ChildNodes[0];
				listViewItem1Text = listViewItem1Element.GetAttribute("Text");
				XmlElement listViewItem2Element = (XmlElement)listViewElement.ChildNodes[1];
				listViewItem2Text = listViewItem2Element.GetAttribute("Text");
			}
		}
		
		[Test]
		public void TwoListViewItems()
		{
			Assert.AreEqual(2, listViewItemCount);
		}

		[Test]
		public void ControlName()
		{
			Assert.AreEqual("NewListView", controlName);
		}
		
		[Test]
		public void ControlType()
		{
			Assert.AreEqual("ListView", controlType);
		}
		
		[Test]
		public void ListViewItem1Text()
		{
			Assert.AreEqual("New item1", listViewItem1Text);
		}
		
		[Test]
		public void ListViewItem2Text()
		{
			Assert.AreEqual("New item2", listViewItem2Text);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
