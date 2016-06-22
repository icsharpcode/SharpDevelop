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

using System;
using ICSharpCode.SharpDevelop;
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
			SD.InitializeForUnitTests();
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
