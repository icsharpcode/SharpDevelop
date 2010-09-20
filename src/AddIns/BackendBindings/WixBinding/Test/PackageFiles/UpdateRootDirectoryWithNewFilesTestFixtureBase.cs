// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	public abstract class UpdateRootDirectoryWithNewFilesTestFixtureBase
	{
		protected MockTextEditor textEditor;
		protected MockWorkbench workbench;
		protected WixDocument document;
		protected PackageFilesView packageFilesView;
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockTextEditor();
			MockTextEditorViewContent viewContent = new MockTextEditorViewContent();
			viewContent.TextEditor = textEditor;
			viewContent.SetFileName(@"d:\projects\test\file.wxs");
			
			workbench = new MockWorkbench();
			workbench.ViewContentCollection.Add(viewContent);
			
			MockTextEditorOptions textEditorOptions = new MockTextEditorOptions();
			MockXmlTextWriter xmlTextWriter = new MockXmlTextWriter(textEditorOptions);
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			document = new WixDocument(project, new DefaultFileLoader());
			document.LoadXml(GetWixXml());
			document.FileName = @"d:\projects\test\File.wxs";
			textEditor.Document.Text = GetWixXml();
			
			MockWixPackageFilesControl packageFilesControl = new MockWixPackageFilesControl();
			packageFilesView = new PackageFilesView(project, workbench, packageFilesControl, xmlTextWriter);
			
			packageFilesControl.IsDirty = true;
			AddNewChildElementsToDirectory();
			packageFilesView.Write(document);
		}
		
		protected abstract string GetWixXml();
		
		protected abstract void AddNewChildElementsToDirectory();
	}
}
