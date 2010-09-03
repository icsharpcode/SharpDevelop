// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests that a WixDialogDesignerLoader creates bitmaps specified in the Wix XML. 
	/// </summary>
	[TestFixture]
	public class BitmapCreatedByDesignerLoaderTestFixture : DialogLoadingTestFixtureBase, IWixDialogDesigner
	{
		Form formCreated;
		MockDesignerLoaderHost loaderHost;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixDialogDesignerLoader loader = new WixDialogDesignerLoader(this, new WixDialogDesignerGenerator(), this);
			loaderHost = new MockDesignerLoaderHost();
			loader.BeginLoad(loaderHost);
			IComponent rootComponent = loaderHost.RootComponent;
			formCreated = (Form)rootComponent;
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			if (formCreated != null) {
				formCreated.Dispose();
			}
		}
				
		[Test]
		public void DialogBitmapFileNameRequested()
		{
			Assert.AreEqual(@"C:\Projects\Setup\Bitmaps\DialogBitmap.bmp", BitmapFileNamesRequested[0]);
		}
		
		[Test]
		public void BannerBitmapFileNameRequested()
		{
			Assert.AreEqual(@"C:\Projects\Setup\Bitmaps\BannerBitmap.bmp", BitmapFileNamesRequested[1]);
		}
		
		string IWixDialogDesigner.DialogId {
			get {
				return "WelcomeDialog";
			}
		}
		
		string IWixDialogDesigner.GetDocumentXml()
		{
			return GetWixXml();
		}
		
		string IWixDialogDesigner.DocumentFileName {
			get {
				return @"C:\Projects\Setup\Setup.wxs";
			}
		}
		
		WixProject IWixDialogDesigner.Project {
			get {
				WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
				project.SetProperty("DefineConstants", "DATADIR=Bitmaps");
				return project;
			}
		}
		
		string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id='WelcomeDialog' Height='270' Width='370'>\r\n" +
				"\t\t\t\t<Control Id='DialogBitmap' Type='Bitmap' X='10' Y='15' Width='50' Height='50' Text='DialogBitmap.bmp'/>\r\n" +
				"\t\t\t\t<Control Id='BannerBitmap' Type='Bitmap' X='10' Y='15' Width='50' Height='50' Text='BannerBitmap.bmp'/>\r\n" +
				"\t\t\t</Dialog>\r\n" +
				"\t\t</UI>\r\n" +
				"\t\t<Binary Id='DialogBitmap.bmp' SourceFile='Bitmaps\\DialogBitmap.bmp'/>\r\n" +
				"\t\t<Binary Id='BannerBitmap.bmp' SourceFile='$(var.DATADIR)\\BannerBitmap.bmp'/>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
