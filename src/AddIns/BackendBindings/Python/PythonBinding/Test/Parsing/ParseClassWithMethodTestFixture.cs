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
	[TestFixture]
	public class ParseClassWithMethodTestFixture
	{
		ICompilationUnit compilationUnit;
		IClass c;
		IMethod method;
		FoldingSection methodFold = null;
		FoldingSection classFold = null;
		TextDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = "class Test:\r\n" +
							"\tdef foo(self):\r\n" +
							"\t\tpass";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);			
			if (compilationUnit.Classes.Count > 0) {
				c = compilationUnit.Classes[0];
				if (c.Methods.Count > 0) {
					method = c.Methods[0];
				}
				
				TextArea textArea = new TextArea();
				document = new TextDocument();
				textArea.Document = document;
				textArea.Document.Text = python;
				
				ParserFoldingStrategy foldingStrategy = new ParserFoldingStrategy(textArea);
				
				ParseInformation parseInfo = new ParseInformation(compilationUnit);
				foldingStrategy.UpdateFoldings(parseInfo);
				List<FoldingSection> folds = new List<FoldingSection>(foldingStrategy.FoldingManager.AllFoldings);
				
				if (folds.Count > 0) {
					classFold = folds[0];
				}
				if (folds.Count > 1) {
					methodFold = folds[1];
				}
			}
		}
		
		[Test]
		public void OneClass()
		{
			Assert.AreEqual(1, compilationUnit.Classes.Count);
		}
		
		[Test]
		public void ClassName()
		{
			Assert.AreEqual("Test", c.Name);
		}
		
		[Test]
		public void ClassBodyRegion()
		{
			int startLine = 1;
			int startColumn = 12;
			int endLine = 3;
			int endColumn = 7;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), c.BodyRegion.ToString());
		}
		
		/// <summary>
		/// The class declaration region needs to extend up to and
		/// including the colon.
		/// </summary>
		[Test]
		public void ClassRegion()
		{
			int startLine = 1;
			int startColumn = 1;
			int endLine = 3;
			int endColumn = 7;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), c.Region.ToString());
		}		
		
		[Test]
		public void MethodName()
		{
			Assert.AreEqual("foo", method.Name);
		}
		
		[Test]
		public void MethodBodyRegion()
		{
			int startLine = 2;
			int startColumn = 16;
			int endLine = 3;
			int endColumn = 7;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), method.BodyRegion.ToString());
		}
		
		/// <summary>
		/// The method region needs to extend up just after the colon. It does not include the body.
		/// </summary>
		[Test]
		public void MethodRegion()
		{
			int startLine = 2;
			int startColumn = 2;
			int endLine = 2;
			int endColumn = 16;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), method.Region.ToString());
		}
		
		[Test]
		public void MethodFoldTextInsideFoldIsMethodBody()
		{
			string textInsideFold = document.GetText(methodFold.StartOffset, methodFold.Length);
			Assert.AreEqual("\r\n\t\tpass", textInsideFold);
		}
		
		[Test]
		public void MethodIsNotConstructor()
		{
			Assert.IsFalse(method.IsConstructor);
		}
		
		[Test]
		public void MethodIsPublic()
		{
			ModifierEnum modifiers = ModifierEnum.Public;
			Assert.AreEqual(modifiers, method.Modifiers);
		}
		
		[Test]
		public void ClassFoldTextInsideFoldIsClassBody()
		{
			string textInsideFold = document.GetText(classFold.StartOffset, classFold.Length);
			Assert.AreEqual("\r\n\tdef foo(self):\r\n\t\tpass", textInsideFold);
		}
	}
}
