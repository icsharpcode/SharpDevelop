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
			WixDialogDesignerLoader loader = new WixDialogDesignerLoader(this, fileLoader: this);
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
		
		ICSharpCode.SharpDevelop.Editor.ITextEditor IWixDialogDesigner.PrimaryViewContentTextEditor {
			get {
				throw new NotImplementedException();
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
