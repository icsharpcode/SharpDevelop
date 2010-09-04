// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.FormsDesigner;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Designer
{
	[TestFixture]
	[RequiresSTA]
	public class ScriptingDesignerGeneratorTests
	{
		MockTextEditorOptions textEditorOptions;
		TestableScriptingDesignerGenerator generator;
		FormsDesignerViewContent formsDesignerView;
		MockTextEditorViewContent textEditorViewContent;
		MockOpenedFile formsDesignerOpenedFile;
		
		[Test]
		public void GetSourceFiles_FormDesignerViewHasOpenFile_ReturnsOneFile()
		{
			CreateDesignerGenerator();
			OpenedFile designerOpenedFile;
			IEnumerable<OpenedFile> files = generator.GetSourceFiles(out designerOpenedFile);
			int count = HowManyInCollection(files);
			
			Assert.AreEqual(1, count);
		}
		
		void CreateDesignerGenerator()
		{
			textEditorViewContent = new MockTextEditorViewContent();
			formsDesignerOpenedFile = new MockOpenedFile("test.py");
			textEditorViewContent.PrimaryFile = formsDesignerOpenedFile;
			formsDesignerView = new FormsDesignerViewContent(textEditorViewContent, formsDesignerOpenedFile);
			textEditorOptions = new MockTextEditorOptions();
			generator = new TestableScriptingDesignerGenerator(textEditorOptions);
			generator.Attach(formsDesignerView);
		}
		
		int HowManyInCollection(IEnumerable<OpenedFile> files)
		{
			int count = 0;
			foreach (OpenedFile file in files) {
				count++;
			}
			return count;
		}
		
		[Test]
		public void GetSourceFiles_FormsDesignerHasOpenFile_DesignerOpenedFileParameterIsSetToFormsDesignerViewOpenedFile()
		{
			CreateDesignerGenerator();
			OpenedFile designerOpenedFile;
			generator.GetSourceFiles(out designerOpenedFile);
			
			AssertAreEqual(formsDesignerOpenedFile, designerOpenedFile);
		}
		
		void AssertAreEqual(OpenedFile expectedOpenedFile, OpenedFile actualOpenedFile)
		{	
			string fileName = actualOpenedFile.FileName.ToString();
			string expectedFileName = expectedOpenedFile.FileName.ToString();
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetSourceFiles_FormsDesignerHasOpenFile_FormsDesignerFileReturnedInFiles()
		{
			CreateDesignerGenerator();
			OpenedFile designerOpenedFile;
			IEnumerable<OpenedFile> files = generator.GetSourceFiles(out designerOpenedFile);
			IEnumerator<OpenedFile> enumerator = files.GetEnumerator();
			enumerator.MoveNext();
			OpenedFile file = enumerator.Current;
			
			AssertAreEqual(formsDesignerOpenedFile, file);
		}
	}
}
