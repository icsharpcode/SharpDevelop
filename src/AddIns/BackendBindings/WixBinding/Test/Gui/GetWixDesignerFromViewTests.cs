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

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using Rhino.Mocks;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests that the WixDialogDesigner.GetDesigner method returns the 
	/// Wix dialog designer attached to the primary view.
	/// </summary>
	[TestFixture]
	public class GetWixDesignerFromViewTests
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			SD.InitializeForUnitTests();
			SD.Services.AddService(typeof(IWinFormsService), MockRepository.GenerateStub<IWinFormsService>());
			SD.WinForms.Stub(s => s.CreateWindowsFormsHost()).IgnoreArguments().Return(new CustomWindowsFormsHost());
			SD.Services.AddService(typeof(IWorkbench), new MockWorkbench());
			SD.Services.AddService(typeof(IFileService), MockRepository.GenerateStub<IFileService>());
			SD.Services.AddService(typeof(IProjectService), MockRepository.GenerateStub<IProjectService>());
		}
		
		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			SD.TearDownForUnitTests();
		}
		
		[Test]
		public void WixDesignerAttached()
		{
			MockTextEditorViewContent view = new MockTextEditorViewContent();
			using (WixDialogDesigner designerAdded = new WixDialogDesigner(view)) {
				view.SecondaryViewContents.Add(designerAdded);
				Assert.IsNotNull(WixDialogDesigner.GetDesigner(view));
			}
		}
		
		[Test]
		public void NoWixDesignerAttached()
		{
			MockViewContent view = new MockViewContent();
			Assert.IsNull(WixDialogDesigner.GetDesigner(view));
		}
	}
}
