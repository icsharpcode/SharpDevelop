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
	/// Adds two new items to the list view.
	/// </summary>
	[TestFixture]
	public class ListViewItemAddedTestFixture : DialogLoadingTestFixtureBase
	{
		int itemCount;
		string item1Text;
		string item2Text;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {

				ListView listView = (ListView)dialog.Controls[0];
				listView.Items.Add("New item1");
				listView.Items.Add("New item2");
				
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				XmlElement listViewElement = (XmlElement)dialogElement.SelectSingleNode("//w:ListView[@Property='ListViewProperty']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				
				itemCount = listViewElement.ChildNodes.Count;
				XmlElement item1Element = (XmlElement)listViewElement.ChildNodes[0];
				item1Text = item1Element.GetAttribute("Text");
				XmlElement item2Element = (XmlElement)listViewElement.ChildNodes[1];
				item2Text = item2Element.GetAttribute("Text");
			}
		}
		
		[Test]
		public void TwoListViewItems()
		{
			Assert.AreEqual(2, itemCount);
		}

		[Test]
		public void ListViewItem1Text()
		{
			Assert.AreEqual("New item1", item1Text);
		}
		
		[Test]
		public void ComboBoxItem2Text()
		{
			Assert.AreEqual("New item2", item2Text);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='ListView1' Type='ListView' X='20' Y='187' Width='330' Height='40' Property='ListViewProperty'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t\t<ListView Property='ListViewProperty'>\r\n" +
				"\t\t\t</ListView>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
