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
	/// Tests the loading of a simple Wix dialog that has just an id, height and
	/// width.
	/// </summary>
	[TestFixture]
	public class SimpleDialogTestFixture
	{
		string dialogName;
		FormBorderStyle borderStyle;
		Size clientSize;
		bool maximizeBox;
		bool minimizeBox;
		WixDocument doc;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			doc = new WixDocument();
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form simpleDialog = wixDialog.CreateDialog()) {
				dialogName = simpleDialog.Name;
				borderStyle = simpleDialog.FormBorderStyle;
				clientSize = simpleDialog.ClientSize;
				minimizeBox = simpleDialog.MinimizeBox;
				maximizeBox = simpleDialog.MaximizeBox;
			}
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual("WelcomeDialog", dialogName);
		}
		
		[Test]
		public void FixedDialogBorder()
		{
			Assert.AreEqual(FormBorderStyle.FixedDialog, borderStyle);
		}
		
		[Test]
		public void ClientSizeHeight()
		{
			int expectedHeight = Convert.ToInt32(270 * WixDialog.InstallerUnit);
			Assert.AreEqual(expectedHeight, clientSize.Height);
		}
		
		[Test]
		public void ClientSizeWidth()
		{
			int expectedWidth = Convert.ToInt32(370 * WixDialog.InstallerUnit);
			Assert.AreEqual(expectedWidth, clientSize.Width);
		}
		
		[Test]
		public void MinimizeBox()
		{
			Assert.IsTrue(minimizeBox);
		}
		
		[Test]
		public void MaximizeBox()
		{
			Assert.IsFalse(maximizeBox);
		}
		
		[Test]
		public void DialogIdWithSingleQuote()
		{
			Assert.IsNull(doc.CreateWixDialog("Test'Id", new MockTextFileReader()));
		}
				
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
