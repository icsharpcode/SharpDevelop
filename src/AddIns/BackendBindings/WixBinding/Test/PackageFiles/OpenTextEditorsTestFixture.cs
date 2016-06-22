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
using ICSharpCode.SharpDevelop.Editor;
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
			SD.InitializeForUnitTests();
			existingTextEditor = new MockTextEditor();
			var viewContent = new MockTextEditorViewContent();
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
		
		[Test]
		public void FindTextEditorForDocument_FirstViewContentHasNoTextEditorAndNoPrimaryFileName_DoesNotThrowNullReferenceException()
		{
			var viewContent = new MockViewContent();
			viewContent.PrimaryFile = null;
			workbench.ViewContentCollection.Add(viewContent);
			var unknownDocument = new WixDocument();
			unknownDocument.FileName = @"d:\unknown-file.wxs";
			
			ITextEditor textEditor = openEditors.FindTextEditorForDocument(unknownDocument);
			
			Assert.IsNull(textEditor);
		}
	}
}
