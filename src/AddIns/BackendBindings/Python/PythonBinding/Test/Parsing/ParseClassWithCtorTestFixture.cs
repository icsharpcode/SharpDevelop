// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace PythonBinding.Tests.Parsing
{
	/// <summary>
	/// Having a newline at the end of the class's last method was 
	/// causing the "pass" statement to be truncated when the constructor
	/// was folded. This test fixture tests for that bug.
	/// </summary>
	[TestFixture]
	public class ParseClassWithCtorTestFixture
	{
		ICompilationUnit compilationUnit;
		IClass c;
		IMethod method;
		FoldingSection methodFold;
		FoldingSection classFold;
		TextDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = "class Test:\r\n" +
							"\tdef __init__(self):\r\n" +
							"\t\tpass\r\n";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);			
			if (compilationUnit.Classes.Count > 0) {
				c = compilationUnit.Classes[0];
				if (c.Methods.Count > 0) {
					method = c.Methods[0];
				}
				
				// Get folds.
				TextArea textArea = new TextArea();
				document = new TextDocument();
				textArea.Document = document;
				textArea.Document.Text = python;
				
				ParserFoldingStrategy foldingStrategy = new ParserFoldingStrategy(textArea);
				
				ParseInformation parseInfo = new ParseInformation(compilationUnit);
				foldingStrategy.UpdateFoldings(parseInfo);
				List<FoldingSection> folds = new List<FoldingSection>(foldingStrategy.FoldingManager.AllFoldings);
				
				if (folds.Count > 1) {
					classFold = folds[0];
					methodFold = folds[1];
				}
			}
		}
		
		[Test]
		public void MethodBodyRegion()
		{
			int startLine = 2;
			int startColumn = 21;
			int endLine = 3;
			int endColumn = 7;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), method.BodyRegion.ToString());
		}
		
		[Test]
		public void MethodFoldTextInsideFoldIsMethodBody()
		{
			string textInsideFold = document.GetText(methodFold.StartOffset, methodFold.Length);
			Assert.AreEqual("\r\n\t\tpass", textInsideFold);
		}
		
		[Test]
		public void MethodIsConstructor()
		{
			Assert.IsTrue(method.IsConstructor);
		}
	}
}
