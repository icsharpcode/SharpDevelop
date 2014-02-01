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
	/// Tests the updating of the dialog xml from a modified Wix dialog.
	/// </summary>
	[TestFixture]
	public class SimpleDialogTestFixture
	{
		XmlElement dialogElement;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDocument doc = new WixDocument();
			doc.LoadXml(GetWixXml());

			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog()) {
				dialog.Text = "New dialog title";
				dialog.ClientSize = new Size(200, 100);
				dialog.MinimizeBox = false;
				dialogElement = wixDialog.UpdateDialogElement(dialog);
			}
		}
		
		[Test]
		public void DialogTitle()
		{
			Assert.AreEqual("New dialog title", dialogElement.GetAttribute("Title"));
		}
		
		[Test]
		public void DialogHeight()
		{
			int expectedHeight = Convert.ToInt32(100 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedHeight.ToString(), dialogElement.GetAttribute("Height"));
		}
		
		[Test]
		public void DialogWidth()
		{
			int expectedWidth = Convert.ToInt32(200 / WixDialog.InstallerUnit);
			Assert.AreEqual(expectedWidth.ToString(), dialogElement.GetAttribute("Width"));
		}
		
		[Test]
		public void DialogMinimizeBox()
		{
			Assert.AreEqual("yes", dialogElement.GetAttribute("NoMinimize"));
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370' Title='Welcome Dialog Title'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
