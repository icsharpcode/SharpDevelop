// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class OpenTextEditorsTestFixture
	{
		OpenTextEditors openEditors;
		MockTextEditor existingTextEditor;
		MockWorkbench workbench;
		WixDocument document;
		
		[SetUp]
		public void Init()
		{
			existingTextEditor = new MockTextEditor();
			MockTextEditorViewContent viewContent = new MockTextEditorViewContent();
			viewContent.TextEditor = existingTextEditor;
			viewContent.SetFileName(@"d:\projects\test\file.wxs");
			
			workbench = new MockWorkbench();
			workbench.ViewContentCollection.Add(new MockViewContent());
			workbench.ViewContentCollection.Add(viewContent);
			
			document = new WixDocument();
			document.FileName = @"d:\Projects\Test\File.wxs";
			
			openEditors = new OpenTextEditors(workbench);
		}
		
		[Test]
		public void CanFindTextEditorForWixDocumentCurrentlyOpenInWorkbench()
		{
			Assert.AreSame(existingTextEditor, openEditors.FindTextEditorForDocument(document));
		}
		
		[Test]
		public void CannotFindTextEditorForUnknownWixDocumentFileName()
		{
			WixDocument unknownDocument = new WixDocument();
			unknownDocument.FileName = @"d:\unknown-file.wxs";
			Assert.IsNull(openEditors.FindTextEditorForDocument(unknownDocument));
		}
	}
}
