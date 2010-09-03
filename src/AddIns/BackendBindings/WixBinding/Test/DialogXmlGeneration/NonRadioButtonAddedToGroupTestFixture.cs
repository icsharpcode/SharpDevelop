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
	/// A non radio button control is added to a panel containing radio buttons.
	/// This test fixture checks that the non-radio button control is ignored.
	/// </summary>
	[TestFixture]
	public class NonRadioButtonAddedToGroupTestFixture : DialogLoadingTestFixtureBase
	{
		XmlElement acceptRadioButtonElement;
		int controlElementCount;
		int radioButtonElementCount;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			CreatedComponents.Clear();
			WixDialog wixDialog = doc.CreateWixDialog("AcceptLicenseDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {

				RadioButtonGroupBox radioButtonGroup = (RadioButtonGroupBox)dialog.Controls[0];
				Label label1 = new Label();
				label1.Left = 100;
				label1.Top = 30;
				radioButtonGroup.Controls.Add(label1);
				radioButtonGroup.Controls.SetChildIndex(label1, 0);
			
				Label label2 = new Label();
				label2.Left = 100;
				label2.Top = 30;
				radioButtonGroup.Controls.Add(label2);
				
				// Add a panel to the dialog controls.
				Panel panel = new Panel();
				panel.Left = 100;
				panel.Top = 30;
				dialog.Controls.Add(panel);
								
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				XmlElement radioButtonGroupElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='Buttons']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				acceptRadioButtonElement = (XmlElement)radioButtonGroupElement.SelectSingleNode("//w:RadioButtonGroup/w:RadioButton", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				
				controlElementCount = dialogElement.SelectNodes("w:Control", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable)).Count;
				radioButtonElementCount = radioButtonGroupElement.SelectNodes("//w:RadioButtonGroup//w:RadioButton", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable)).Count;				
			}
		}
		
		[Test]
		public void OneControlElement()
		{
			Assert.AreEqual(1, controlElementCount);
		}
		
		[Test]
		public void TwoRadioButtonElements()
		{
			Assert.AreEqual(2, radioButtonElementCount);
		}
		
		[Test]
		public void AcceptRadioButtonX()
		{
			Assert.AreEqual("5", acceptRadioButtonElement.GetAttribute("X"));
		}
		
		[Test]
		public void AcceptRadioButtonY()
		{
			Assert.AreEqual("0", acceptRadioButtonElement.GetAttribute("Y"));
		}
		
		[Test]
		public void AcceptRadioButtonHeight()
		{
			Assert.AreEqual("15", acceptRadioButtonElement.GetAttribute("Height"));
		}
		
		[Test]
		public void AcceptRadioButtonWidth()
		{
			Assert.AreEqual("300", acceptRadioButtonElement.GetAttribute("Width"));
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
