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
	/// Tests the dialog xml updating when a radio group has been changed in the
	/// designer.
	/// </summary>
	[TestFixture]
	public class RadioButtonGroupChangedTestFixture : DialogLoadingTestFixtureBase
	{
		XmlElement radioButtonGroupElement;
		XmlElement acceptRadioButtonElement;
		XmlElement declineRadioButtonElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("AcceptLicenseDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {

				Panel radioButtonGroup = (Panel)dialog.Controls[0];
				radioButtonGroup.Left = 30;
				radioButtonGroup.Top = 100;
				
				RadioButton acceptRadioButton = (RadioButton)radioButtonGroup.Controls[0];
				acceptRadioButton.Left = 0;
				acceptRadioButton.Top = 5;
				acceptRadioButton.Width = 100;
				acceptRadioButton.Height = 50;
				acceptRadioButton.Text = "Accept";

				RadioButton declineRadioButton = (RadioButton)radioButtonGroup.Controls[1];
				declineRadioButton.Left = 10;
				declineRadioButton.Top = 20;
				declineRadioButton.Width = 200;
				declineRadioButton.Height = 30;
				declineRadioButton.Text = String.Empty;
				
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				radioButtonGroupElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='Buttons']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				
				XmlNodeList radioButtonElements = radioButtonGroupElement.SelectNodes("//w:RadioButtonGroup/w:RadioButton", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				acceptRadioButtonElement = (XmlElement)radioButtonElements[0];
				declineRadioButtonElement = (XmlElement)radioButtonElements[1];
			}
		}
		
		[Test]
		public void RadioButtonGroupX()
		{
			int expectedX = Convert.ToInt32(30 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), radioButtonGroupElement.GetAttribute("X"));
		}
		
		[Test]
		public void RadioButtonGroupY()
		{
			int expectedY = Convert.ToInt32(100 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), radioButtonGroupElement.GetAttribute("Y"));
		}
		
		[Test]
		public void AcceptRadioButtonX()
		{
			int expectedX = Convert.ToInt32(0 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), acceptRadioButtonElement.GetAttribute("X"));
		}
		
		[Test]
		public void AcceptRadioButtonY()
		{
			int expectedY = Convert.ToInt32(5 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), acceptRadioButtonElement.GetAttribute("Y"));
		}
		
		[Test]
		public void AcceptRadioButtonHeight()
		{
			int expectedHeight = Convert.ToInt32(50 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedHeight.ToString(), acceptRadioButtonElement.GetAttribute("Height"));
		}
		
		[Test]
		public void AcceptRadioButtonWidth()
		{
			int expectedWidth = Convert.ToInt32(100 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedWidth.ToString(), acceptRadioButtonElement.GetAttribute("Width"));
		}
		
		[Test]
		public void AcceptRadioButtonText()
		{
			Assert.AreEqual("Accept", acceptRadioButtonElement.GetAttribute("Text"));
		}
		
		[Test]
		public void DeclineRadioButtonX()
		{
			int expectedX = Convert.ToInt32(10 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), declineRadioButtonElement.GetAttribute("X"));
		}
		
		[Test]
		public void DeclineRadioButtonY()
		{
			int expectedY = Convert.ToInt32(20 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), declineRadioButtonElement.GetAttribute("Y"));
		}
		
		[Test]
		public void DeclineRadioButtonHeight()
		{
			int expectedHeight = Convert.ToInt32(30 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedHeight.ToString(), declineRadioButtonElement.GetAttribute("Height"));
		}
		
		[Test]
		public void DeclineRadioButtonWidth()
		{
			int expectedWidth = Convert.ToInt32(200 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedWidth.ToString(), declineRadioButtonElement.GetAttribute("Width"));
		}
		
		[Test]
		public void DeclineRadioButtonText()
		{
			Assert.IsFalse(declineRadioButtonElement.HasAttribute("Text"));
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='AcceptLicenseDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Buttons' Type='RadioButtonGroup' X='20' Y='187' Width='330' Height='40' Property='AcceptLicense'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t\t<RadioButtonGroup Property='AcceptLicense'>\r\n" +
				"\t\t\t\t<RadioButton Text='I accept' X='5' Y='0' Width='300' Height='15' Value='Yes'/>\r\n" +
				"\t\t\t\t<RadioButton Text='I do not accept' X='5' Y='20' Width='300' Height='15'  Value='No'/>\r\n" +
				"\t\t\t</RadioButtonGroup>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
