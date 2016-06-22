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
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.DialogLoading
{
	/// <summary>
	/// Tests the loading of a simple Wix dialog that contains a bitmap but the
	/// WixDialog cannot find the bitmap binary.
	/// </summary>
	[TestFixture]
	public class MissingBitmapBinaryTestFixture : DialogLoadingTestFixtureBase
	{
		string name;
		string text;
		Point location;
		Size size;
		bool hasImage;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			BitmapFileNamesRequested.Clear();
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			WixDocument doc = new WixDocument(project, this);
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				// Should be the last control added to the dialog appears behind all 
				// the other controls. This is what happens when you call SendToBack 
				// on a control.
				PictureBox pictureBox = (PictureBox)dialog.Controls[dialog.Controls.Count - 1];
				name = pictureBox.Name;
				text = pictureBox.Text;
				location = pictureBox.Location;
				size = pictureBox.Size;
				hasImage = (pictureBox.Image != null);
			}
		}
		
		[Test]
		public void Name()
		{
			Assert.AreEqual("ControlId", name);
		}
		
		[Test]
		public void NoBitmapFilesRequested()
		{
			Assert.AreEqual(0, BitmapFileNamesRequested.Count);
		}
		
		[Test]
		public void Location()
		{
			int expectedX = Convert.ToInt32(10 * WixDialog.InstallerUnit);
			int expectedY = Convert.ToInt32(15 * WixDialog.InstallerUnit);
			Point expectedPoint = new Point(expectedX, expectedY);
			Assert.AreEqual(expectedPoint, location);
		}
		
		[Test]
		public void Size()
		{
			int expectedWidth = (int)Math.Round(50 * WixDialog.InstallerUnit);
			int expectedHeight = (int)Math.Round(50 * WixDialog.InstallerUnit);
			Size expectedSize = new Size(expectedWidth, expectedHeight);
			
			Assert.AreEqual(expectedSize, size);
		}
		
		[Test]
		public void PictureBoxHasNoImage()
		{
			Assert.IsFalse(hasImage);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='Cancel' Type='PushButton' X='100' Y='150' Width='30' Height='20'/>\r\n" +
				"\t\t\t\t<Control Id='ControlId' Type='Bitmap' X='10' Y='15' Width='50' Height='50'/>\r\n" +
				"\t\t\t\t<Control Id='Next' Type='PushButton' X='100' Y='150' Width='30' Height='20'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
