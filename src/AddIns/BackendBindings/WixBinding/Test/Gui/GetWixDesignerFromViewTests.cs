// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
