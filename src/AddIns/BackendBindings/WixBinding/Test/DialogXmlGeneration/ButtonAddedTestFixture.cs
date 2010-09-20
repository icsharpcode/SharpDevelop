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
	/// Tests that a new button added to the dialog is also added to the dialog xml.
	/// </summary>
	[TestFixture]
	public class ButtonAddedTestFixture
	{
		XmlElement dialogElement;
		XmlElement nextButtonElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());

			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog()) {
				Button nextButton = new Button();
				nextButton.Left = 200;
				nextButton.Top = 220;
				nextButton.Width = 50;
				nextButton.Height = 10;
				nextButton.Text = "[Button_Next]";
				nextButton.Name = "Next";
				dialog.Controls.Add(nextButton);
				
				dialogElement = wixDialog.UpdateDialogElement(dialog);
				nextButtonElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='Next']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));				
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
			Assert.AreEqual("[Button_Next]", nextButtonElement.GetAttribute("Text"));
		}
		
		[Test]
		public void NextButtonControlType()
		{
			Assert.AreEqual("PushButton", nextButtonElement.GetAttribute("Type"));
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
