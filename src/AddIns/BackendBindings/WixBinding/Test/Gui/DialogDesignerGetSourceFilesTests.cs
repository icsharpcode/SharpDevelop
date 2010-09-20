// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Gui
{
	[TestFixture]
	public class DialogDesignerGetSourceFilesTests
	{
		WixDialogDesignerGenerator dialogDesigner;
		OpenedFile designerOpenedFile;
		List<OpenedFile> files;
		MockTextEditorViewContent primaryView;
		
		[SetUp]
		public void Init()
		{
			primaryView = new MockTextEditorViewContent();
			primaryView.SetFileName(@"d:\projects\test\dialog.wxs");
			
			dialogDesigner = new WixDialogDesignerGenerator();
			MockOpenedFile openedFile = new MockOpenedFile("dialog.designer.wxs", false);
			dialogDesigner.Attach(new FormsDesignerViewContent(primaryView, openedFile));
			
			files = new List<OpenedFile>();
			IEnumerable<OpenedFile> sourceFiles = dialogDesigner.GetSourceFiles(out designerOpenedFile);
			if (sourceFiles != null) {
				files.AddRange(sourceFiles);
			}
		}
		
		[Test]
		public void DesignerOpenFileIsFormsDesignerViewPrimaryFile()
		{
			Assert.AreSame(primaryView.PrimaryFile, designerOpenedFile);
		}
		
		[Test]
		public void OneSourceFileReturnedFromGetSourceFiles()
		{
			Assert.AreEqual(1, files.Count);
		}
		
		[Test]
		public void GetSourceFilesContainsFormsDesignerPrimaryFile()
		{
			Assert.AreSame(primaryView.PrimaryFile, files[0]);
		}
	}
}
