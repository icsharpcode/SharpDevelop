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
	/// Tests the dialog xml updating when a new radio group has been added to the form.
	/// </summary>
	[TestFixture]
	public class RadioButtonGroupAddedTestFixture : DialogLoadingTestFixtureBase
	{
		XmlElement controlElement;
		XmlElement radioButtonGroupElement;
		XmlElement acceptRadioButtonElement;
		XmlElement declineRadioButtonElement;
		
		[SetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("AcceptLicenseDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {

				RadioButtonGroupBox radioButtonGroup = new RadioButtonGroupBox();
				radioButtonGroup.Left = 30;
				radioButtonGroup.Top = 100;
				radioButtonGroup.Name = "AcceptLicenseRadioButtonGroup";
				radioButtonGroup.PropertyName = "AcceptLicense";
				dialog.Controls.Add(radioButtonGroup);
				
				RadioButton acceptRadioButton = new RadioButton();
				acceptRadioButton.Left = 0;
				acceptRadioButton.Top = 5;
				acceptRadioButton.Width = 100;
				acceptRadioButton.Height = 50;
				acceptRadioButton.Text = "Accept";
				acceptRadioButton.Name = "AcceptLicenseRadioButton1";
				radioButtonGroup.Controls.Add(acceptRadioButton);

				RadioButton declineRadioButton = new RadioButton();
				declineRadioButton.Left = 10;
				declineRadioButton.Top = 20;
				declineRadioButton.Width = 200;
				declineRadioButton.Height = 30;
				declineRadioButton.Text = "Decline";
				declineRadioButton.Name = "AcceptLicenseRadioButton2";
				radioButtonGroup.Controls.Add(declineRadioButton);
				
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				controlElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='AcceptLicenseRadioButtonGroup']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				radioButtonGroupElement = (XmlElement)dialogElement.SelectSingleNode("//w:RadioButtonGroup[@Property='AcceptLicense']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				XmlNodeList radioButtonElements = radioButtonGroupElement.SelectNodes("//w:RadioButtonGroup/w:RadioButton", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				acceptRadioButtonElement = (XmlElement)radioButtonElements[0];
				declineRadioButtonElement = (XmlElement)radioButtonElements[1];
			}
		}
		
		[Test]
		public void RadioButtonGroupName()
		{
			Assert.AreEqual("AcceptLicenseRadioButtonGroup", controlElement.GetAttribute("Id"));
		}
		
		[Test]
		public void RadioButtonProperty()
		{
			Assert.AreEqual("AcceptLicense", radioButtonGroupElement.GetAttribute("Property"));
		}
		
		[Test]
		public void RadioButtonGroupX()
		{
			int expectedX = Convert.ToInt32(30 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), controlElement.GetAttribute("X"));
		}
		
		[Test]
		public void RadioButtonGroupY()
		{
			int expectedY = Convert.ToInt32(100 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), controlElement.GetAttribute("Y"));
		}
		
		[Test]
		public void AcceptRadioButtonX()
		{
			int expectedX = Convert.ToInt32(0 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), acceptRadioButtonElement.GetAttribute("X"));
		}

		[Test]
		public void DeclineRadioButtonX()
		{
			int expectedX = Convert.ToInt32(10 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), declineRadioButtonElement.GetAttribute("X"));
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='AcceptLicenseDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
