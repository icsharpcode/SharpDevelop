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
	/// Adds two new items to the combo box.
	/// </summary>
	[TestFixture]
	public class ComboBoxItemAddedTestFixture : DialogLoadingTestFixtureBase
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

				ComboBox comboBox = (ComboBox)dialog.Controls[0];
				comboBox.Items.Add("New item1");
				comboBox.Items.Add("New item2");
				
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				XmlElement comboBoxElement = (XmlElement)dialogElement.SelectSingleNode("//w:ComboBox[@Property='ComboBoxProperty']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				
				itemCount = comboBoxElement.ChildNodes.Count;
				XmlElement item1Element = (XmlElement)comboBoxElement.ChildNodes[0];
				item1Text = item1Element.GetAttribute("Text");
				XmlElement item2Element = (XmlElement)comboBoxElement.ChildNodes[1];
				item2Text = item2Element.GetAttribute("Text");
			}
		}
		
		[Test]
		public void TwoComboBoxItems()
		{
			Assert.AreEqual(2, itemCount);
		}

		[Test]
		public void ComboBoxItem1Text()
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
				"\t\t\t\t<Control Id='ComboBox1' Type='ComboBox' X='20' Y='187' Width='330' Height='40' Property='ComboBoxProperty'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t\t<ComboBox Property='ComboBoxProperty'>\r\n" +
				"\t\t\t</ComboBox>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
