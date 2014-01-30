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
	/// Tests the loading of a simple Wix dialog that contains a bitmap.
	/// </summary>
	[TestFixture]
	public class BitmapBinaryTestFixture : DialogLoadingTestFixtureBase
	{
		bool hasImage;
		PictureBoxSizeMode sizeMode;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			SD.InitializeForUnitTests();
			
			BitmapFileNamesRequested.Clear();
			CreatedComponents.Clear();
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			WixDocument doc = new WixDocument(project, this);
			doc.LoadXml(GetWixXml());
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", new MockTextFileReader());
			using (Form dialog = wixDialog.CreateDialog(this)) {
				PictureBox pictureBox = (PictureBox)dialog.Controls[0];
				hasImage = (pictureBox.Image != null);
				sizeMode = pictureBox.SizeMode;
			}
		}
		
		[Test]
		public void PictureBoxHasImage()
		{
			Assert.IsTrue(hasImage);
		}
		
		[Test]
		public void OneBinaryFileNameRequested()
		{
			Assert.AreEqual(1, BitmapFileNamesRequested.Count);
		}
		
		[Test]
		public void BinaryFileNameRequested()
		{
			Assert.AreEqual("Bitmaps/DialogBitmap.bmp", BitmapFileNamesRequested[0]);
		}
		
		[Test]
		public void BitmapShouldBeCentred()
		{
			Assert.AreEqual(PictureBoxSizeMode.CenterImage, sizeMode);
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='DialogBitmap' Type='Bitmap' X='10' Y='15' Width='50' Height='50' Text='DialogBitmap.bmp'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t\t<Binary Id='DialogBitmap.bmp' SourceFile='Bitmaps/DialogBitmap.bmp'/>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
