// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Resources;
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
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			WixBindingTestsHelper.RegisterResourceStringsWithSharpDevelopResourceManager();
		}
		
		[Test]
		[ExpectedException(typeof(FormsDesignerLoadException), ExpectedMessage = "No setup dialog selected in Wix document. Please move the cursor inside a dialog element or use the Setup Dialogs window to open a dialog.")]
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
