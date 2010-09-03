// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Document
{
	/// <summary>
	/// Tests that the WixDialog requests the bitmap from the project.
	/// </summary>
	[TestFixture]
	public class BinaryFileNameFromProjectTestFixture : DialogLoadingTestFixtureBase, ITextFileReader
	{
		string projectDirectory;
		bool hasImage;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			BitmapFileNamesRequested.Clear();
			CreatedComponents.Clear();
			
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			projectDirectory = p.Directory;
			p.Name = "MySetup";
			
			FileProjectItem item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "Setup.wxs";
			string docFileName = item.FileName;
			ProjectService.AddProjectItem(p, item);
			
			item = new FileProjectItem(p, ItemType.Compile);
			item.Include = "Fragment.wxs";
			ProjectService.AddProjectItem(p, item);

			WixDocument doc = new WixDocument(p, this);
			doc.FileName = docFileName;
			doc.LoadXml(GetMainWixXml());
			
			WixDialog wixDialog = doc.CreateWixDialog("WelcomeDialog", this);
			using (Form dialog = wixDialog.CreateDialog(this)) {
				PictureBox pictureBox = (PictureBox)dialog.Controls[0];
				hasImage = (pictureBox.Image != null);
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
			string expectedFileName = Path.Combine(projectDirectory, "Bitmaps/DialogBitmap.bmp");
			Assert.AreEqual(expectedFileName, BitmapFileNamesRequested[0]);
		}
		
		public TextReader Create(string fileName)
		{
			fileName = Path.GetFileName(fileName);
			if (fileName == "Fragment.wxs") {
				return new StringReader(GetWixFragmentXml());
			}
			return null;
		}
		
		string GetMainWixXml()
		{
			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='DialogBitmap' Type='Bitmap' X='10' Y='15' Width='50' Height='50' Text='DialogBitmap.bmp'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
		
		string GetWixFragmentXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<Binary Id='DialogBitmap.bmp' SourceFile='Bitmaps/DialogBitmap.bmp'/>\r\n" +
				"\t\t<Binary Id='Browse' src='Bitmaps/Browse.bmp'/>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
