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

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop;
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
			SD.InitializeForUnitTests();
			WixBindingTestsHelper.RegisterResourceStringsWithSharpDevelopResourceManager();
		}
		
		[Test]
		[ExpectedException(typeof(FormsDesignerLoadException), ExpectedMessage = "No setup dialog selected in Wix document. Please move the cursor inside a dialog element or use the Setup Dialogs window to open a dialog.")]
		public void NoDialogIdSpecified()
		{
			WixDialogDesignerLoader loader = new WixDialogDesignerLoader(this);
			loader.BeginLoad(new MockDesignerLoaderHost());
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
		
		ICSharpCode.SharpDevelop.Editor.ITextEditor IWixDialogDesigner.PrimaryViewContentTextEditor {
			get {
				throw new NotImplementedException();
			}
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
