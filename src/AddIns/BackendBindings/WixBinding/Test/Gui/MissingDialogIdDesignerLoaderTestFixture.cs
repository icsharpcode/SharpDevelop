// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Resources;
using System.Windows.Forms;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests that a WixDialogDesignerLoader reports that the dialog id cannot be found
	/// in the Wix document. 
	/// </summary>
	[TestFixture]
	public class MissingDialogIdDesignerLoaderTestFixture : IWixDialogDesigner
	{
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			ResourceManager rm = new ResourceManager("WixBinding.Tests.Strings", GetType().Assembly);
			ResourceService.RegisterNeutralStrings(rm);
		}
		[Test]
		[ExpectedException(typeof(FormsDesignerLoadException), ExpectedMessage = "Unable to find dialog with an id of 'MissingDialog'.")]
		public void LoadMissingDialog()
		{
			WixDialogDesignerLoader loader = new WixDialogDesignerLoader(this, new WixDialogDesignerGenerator());
			MockDesignerLoaderHost loaderHost = new MockDesignerLoaderHost();
			loader.BeginLoad(loaderHost);
		}
		
		string IWixDialogDesigner.DialogId {
			get {
				return "MissingDialog";
			}
		}
		
		string IWixDialogDesigner.GetDocumentXml()
		{
			return GetWixXml();
		}
		
		public string DocumentFileName {
			get {
				return String.Empty;
			}
		}
		
		public WixProject Project {
			get {
				return WixBindingTestsHelper.CreateEmptyWixProject();
			}
		}
		
		string GetWixXml()
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
