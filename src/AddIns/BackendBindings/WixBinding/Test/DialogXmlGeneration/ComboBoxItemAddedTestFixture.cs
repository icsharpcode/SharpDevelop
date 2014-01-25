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
