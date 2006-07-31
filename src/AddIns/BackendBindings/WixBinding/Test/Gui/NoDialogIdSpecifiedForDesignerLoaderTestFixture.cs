// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.FormsDesigner;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests that a FormsDesignerLoadException is thrown when the WixDialogDesignerLoader
	/// is used when no dialog id is set.
	/// </summary>
	[TestFixture]
	public class NoDialogIdSpecifiedForDesignerLoaderTestFixture : IWixDialogDesigner
	{
		[Test]
		[ExpectedException(typeof(FormsDesignerLoadException), "No setup dialog selected in Wix document. Please move the cursor inside a dialog element or use the Setup Dialogs window to open a dialog.")]
		public void NoDialogIdSpecified()
		{
			WixDialogDesignerLoader loader = new WixDialogDesignerLoader(this, new WixDialogDesignerGenerator());
			loader.BeginLoad(new MockDesignerLoaderHost());
		}
		
		[Test]
		public void NoGeneratorSpecified()
		{
			try {
				WixDialogDesignerLoader loader = new WixDialogDesignerLoader(this, null);
				Assert.Fail("Expected an argument exception before this line.");
			} catch (ArgumentException ex) {
				Assert.AreEqual("generator", ex.ParamName);
			}
		}
		
		string IWixDialogDesigner.DialogId {
			get {
				return null;
			}
		}
		
		string IWixDialogDesigner.GetDocumentXml()
		{
			return String.Empty;
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
	}
}
