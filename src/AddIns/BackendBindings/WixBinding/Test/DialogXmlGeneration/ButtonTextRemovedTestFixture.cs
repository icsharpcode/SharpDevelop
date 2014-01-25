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

using System;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogXmlGeneration
{
	/// <summary>
	/// If a control has no text then Wix requires the Text attribute to be removed
	/// otherwise the compiler returns errors.
	/// </summary>
	[TestFixture]
	public class ButtonTextRemovedTestFixture
	{
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
				nextButton.Text = String.Empty;
				
				Button cancelButton = (Button)dialog.Controls[1];
				cancelButton.Text = String.Empty;
				
				XmlElement dialogElement = wixDialog.UpdateDialogElement(dialog);
				nextButtonElement = (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='Next']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
				cancelButtonElement= (XmlElement)dialogElement.SelectSingleNode("w:Control[@Id='Cancel']", new WixNamespaceManager(dialogElement.OwnerDocument.NameTable));
			}
		}

		[Test]
		public void NextButtonTextAttributeRemoved()
		{
			Assert.IsFalse(nextButtonElement.HasAttribute("Text"));
		}
		
		/// <summary>
		/// If the text is removed from a control, but the Wix Control element 
		/// has a child Text element we do not need to remove the Text element just 
		/// set its inner text to be an empty string.
		/// </summary>
		[Test]
		public void CancelButtonText()
		{
			XmlElement textElement = (XmlElement)cancelButtonElement.SelectSingleNode("w:Text", new WixNamespaceManager(cancelButtonElement.OwnerDocument.NameTable));
			Assert.AreEqual(String.Empty, textElement.InnerText);
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
