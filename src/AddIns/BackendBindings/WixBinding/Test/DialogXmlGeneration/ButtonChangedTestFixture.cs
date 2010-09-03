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
	/// Tests that the button's changes are added to the dialog xml. 
	/// </summary>
	[TestFixture]
	public class ButtonChangedTestFixture
	{
		XmlElement dialogElement;
		XmlElement nextButtonElement;
		XmlElement cancelButtonElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());

			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog()) {
				Button nextButton = (Button)dialog.Controls[0];
				nextButton.Left = 200;
				nextButton.Top = 220;
				nextButton.Width = 50;
				nextButton.Height = 10;
				nextButton.Text = "Changed text";
				dialog.AcceptButton = null;
				
				Button cancelButton = (Button)dialog.Controls[1];
				cancelButton.Text = "New cancel button text";
				dialog.CancelButton = null;
				
				dialogElement = wixDialog.UpdateDialogElement(dialog);
				nextButtonElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='Next']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				cancelButtonElement= (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='Cancel']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
			}
		}
		
		[Test]
		public void NextButtonHeight()
		{
			int expectedHeight = Convert.ToInt32(10 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedHeight.ToString(), nextButtonElement.GetAttribute("Height"));
		}
		
		[Test]
		public void NextButtonWidth()
		{
			int expectedWidth = Convert.ToInt32(50 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedWidth.ToString(), nextButtonElement.GetAttribute("Width"));			
		}
		
		[Test]
		public void NextButtonX()
		{
			int expectedX = Convert.ToInt32(200 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedX.ToString(), nextButtonElement.GetAttribute("X"));
		}
		
		[Test]
		public void NextButtonY()
		{
			int expectedY = Convert.ToInt32(220 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedY.ToString(), nextButtonElement.GetAttribute("Y"));			
		}
		
		[Test]
		public void NextButtonText()
		{
			Assert.AreEqual("Changed text", nextButtonElement.GetAttribute("Text"));
		}
		
		[Test]
		public void CancelButtonText()
		{
			XmlElement textElement = (XmlElement)cancelButtonElement.SelectSingleNode("w:Text", new WixNamespaceManager(cancelButtonElement.OwnerDocument.NameTable));
			Assert.AreEqual("New cancel button text", textElement.InnerText);
		}
		
		[Test]
		public void DialogHasNoCancelButton()
		{
			Assert.AreEqual("no", cancelButtonElement.GetAttribute("Cancel"));
		}
		
		[Test]
		public void DialogHasNoAcceptButton()
		{
			Assert.AreEqual("no", nextButtonElement.GetAttribute("Default"));
		}
		
		[Test]
		public void CancelButtonHasNoTextAttribute()
		{
			Assert.IsFalse(cancelButtonElement.HasAttribute("Text"));
		}

		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Next' Type='PushButton' X='236' Y='243' Width='60' Height='20' Default='yes' Text='Next'>\r\n" +
				"\t\t\t\t</Control>\r\n" +
				"\t\t\t\t<Control Id='Cancel' Type='PushButton' X='304' Y='243' Width='56' Height='17' Cancel='yes'>\r\n" +
				"\t\t\t\t\t<Text>Cancel</Text>\r\n" +
				"\t\t\t\t</Control>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
