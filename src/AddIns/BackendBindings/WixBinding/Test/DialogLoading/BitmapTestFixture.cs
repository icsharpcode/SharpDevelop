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
