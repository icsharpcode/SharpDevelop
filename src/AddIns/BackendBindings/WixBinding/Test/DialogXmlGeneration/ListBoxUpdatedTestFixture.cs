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
	/// Updates the list box items text.
	/// </summary>
	[TestFixture]
	public class ListBoxUpdatedTestFixture : DialogLoadingTestFixtureBase
	{
		string listBoxItem1Text;
		string listBoxItem2Text;
		
		[SetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {

				ListBox listBox = (ListBox)dialog.Controls[0];
				listBox.Items[0] = "Updated item1 text";
				listBox.Items[1] = "Updated item2 text";
				
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				XmlElement listBoxElement = (XmlElement)dialogElement.SelectSingleNode("//w:ListBox[@Property='ListBoxProperty']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				
				XmlElement listBoxItem1Element = (XmlElement)listBoxElement.ChildNodes[0];
				listBoxItem1Text = listBoxItem1Element.GetAttribute("Text");
				XmlElement listBoxItem2Element = (XmlElement)listBoxElement.ChildNodes[1];
				listBoxItem2Text = listBoxItem2Element.GetAttribute("Text");
			}
		}

		[Test]
		public void ListBoxItem1Text()
		{
			Assert.AreEqual("Updated item1 text", listBoxItem1Text);
		}
		
		[Test]
		public void ListBoxItem2Text()
		{
			Assert.AreEqual("Updated item2 text", listBoxItem2Text);
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='ListBox1' Type='ListBox' X='20' Y='187' Width='330' Height='40' Property='ListBoxProperty'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t\t<ListBox Property='ListBoxProperty'>\r\n" +
				"\t\t\t\t<ListItem Text='first'/>\r\n" +
				"\t\t\t\t<ListItem Text='second'/>\r\n" +
				"\t\t\t</ListBox>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
