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

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Tests that the button text is taken from the Control's Text element if it exists otherwise
	/// it defaults to using the Control's Text attribute.
	/// </summary>
	[TestFixture]
	public class ButtonTextTestFixture : DialogLoadingTestFixtureBase
	{
		string nextButtonText;
		string cancelButtonText;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				Button nextButton = (Button)dialog.Controls[0];
				nextButtonText = nextButton.Text;			
				Button cancelButton = (Button)dialog.Controls[1];
				cancelButtonText = cancelButton.Text;
			}
		}
		
		[Test]
		public void CancelButtonText()
		{
			Assert.AreEqual("Cancel", cancelButtonText);
		}
		
		[Test]
		public void NextButtonText()
		{
			Assert.AreEqual("Next", nextButtonText);
		}
	
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Next' Type='PushButton' X='236' Y='243' Width='60' Height='20' Default='yes' Text='Next'>\r\n" +
				"\t\t\t\t\t<Text>Next</Text>\r\n" +
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
