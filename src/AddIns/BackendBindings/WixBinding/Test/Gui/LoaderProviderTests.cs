// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.ComponentModel.Design.Serialization;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	[TestFixture]
	public class LoaderProviderTests : IWixDialogDesigner
	{
		[Test]
		public void NoDesignerSpecified()
		{
			WixDialogDesignerLoaderProvider provider = new WixDialogDesignerLoaderProvider();
			WixDialogDesignerGenerator generator = new WixDialogDesignerGenerator();
			try {
				DesignerLoader loader = provider.CreateLoader(generator);
				Assert.Fail("Expected an argument exception before this line.");
			} catch (ArgumentException ex) {
				Assert.AreEqual("designer", ex.ParamName);
			}
		}
		
		[Test]
		public void DialogIdAndTextEditorSpecified()
		{
			WixDialogDesignerLoaderProvider provider = new WixDialogDesignerLoaderProvider();
			provider.Designer = this;
			WixDialogDesignerGenerator generator = new WixDialogDesignerGenerator();
			WixDialogDesignerLoader loader = (WixDialogDesignerLoader)provider.CreateLoader(generator);
			Assert.IsTrue(loader is WixDialogDesignerLoader);
			Assert.AreSame(this, loader.Designer);
			Assert.AreSame(generator, loader.Generator);
		}
		
		string IWixDialogDesigner.DialogId {
			get { return "WelcomeDialog"; }
		}
		
		string IWixDialogDesigner.DocumentFileName {
			get { return String.Empty; }
		}
		
		WixProject IWixDialogDesigner.Project {
			get { return WixBindingTestsHelper.CreateEmptyWixProject(); }
		}
		
		string IWixDialogDesigner.GetDocumentXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<UI>\r\n" +
				"\t\t\t<Dialog Id=\"WelcomeDialog\" Height=\"100\" Width=\"200\"/>\r\n" +
				"\t\t</UI>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
