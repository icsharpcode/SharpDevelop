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

//using System;
//using System.Collections.Generic;
//using ICSharpCode.FormsDesigner;
//using ICSharpCode.SharpDevelop;
//using ICSharpCode.WixBinding;
//using NUnit.Framework;
//using WixBinding.Tests.Utils;
//
//namespace WixBinding.Tests.Gui
//{
//	[TestFixture]
//	public class DialogDesignerGetSourceFilesTests
//	{
//		WixDialogDesignerGenerator dialogDesigner;
//		OpenedFile designerOpenedFile;
//		List<OpenedFile> files;
//		MockTextEditorViewContent primaryView;
//		
//		[SetUp]
//		public void Init()
//		{
//			primaryView = new MockTextEditorViewContent();
//			primaryView.SetFileName(@"d:\projects\test\dialog.wxs");
//			
//			dialogDesigner = new WixDialogDesignerGenerator();
//			MockOpenedFile openedFile = new MockOpenedFile("dialog.designer.wxs", false);
//			dialogDesigner.Attach(new FormsDesignerViewContent(primaryView, openedFile));
//			
//			files = new List<OpenedFile>();
//			IEnumerable<OpenedFile> sourceFiles = dialogDesigner.GetSourceFiles(out designerOpenedFile);
//			if (sourceFiles != null) {
//				files.AddRange(sourceFiles);
//			}
//		}
//		
//		[Test]
//		public void DesignerOpenFileIsFormsDesignerViewPrimaryFile()
//		{
//			Assert.AreSame(primaryView.PrimaryFile, designerOpenedFile);
//		}
//		
//		[Test]
//		public void OneSourceFileReturnedFromGetSourceFiles()
//		{
//			Assert.AreEqual(1, files.Count);
//		}
//		
//		[Test]
//		public void GetSourceFilesContainsFormsDesignerPrimaryFile()
//		{
//			Assert.AreSame(primaryView.PrimaryFile, files[0]);
//		}
//	}
//}
