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
	/// <summary>
	/// Support folding when no classes are defined.
	/// </summary>
	[TestFixture]
	public class ParseMethodsWithNoClassTestFixture
	{
		ICompilationUnit compilationUnit;
		TextDocument document;
		FoldingSection fooMethodFold = null;
		FoldingSection barMethodFold = null;
		IClass globalClass;
		IMethod fooMethod;
		IMethod barMethod;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = "def foo():\r\n" +
							"\tpass\r\n" +
							"\r\n" +
							"def bar(i):\r\n" +
							"\tpass";
			
			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);			
			
			if (compilationUnit.Classes.Count > 0) {
				globalClass = compilationUnit.Classes[0];
				if (globalClass.Methods.Count > 1) {
					fooMethod = globalClass.Methods[0];
					barMethod = globalClass.Methods[1];
				}
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
				fooMethodFold = folds[0];
				barMethodFold = folds[1];
			}
		}
		
		[Test]
		public void OneClass()
		{
			Assert.AreEqual(1, compilationUnit.Classes.Count);
		}
		
		[Test]
		public void GlobalClassName()
		{
			Assert.AreEqual("test", globalClass.Name);
		}
		
		[Test]
		public void GlobalClassHasTwoMethods()
		{
			Assert.AreEqual(2, globalClass.Methods.Count);
		}
		
		[Test]
		public void FooMethodName()
		{
			Assert.AreEqual("foo", fooMethod.Name);
		}

		[Test]
		public void BarMethodName()
		{
			Assert.AreEqual("bar", barMethod.Name);
		}
		
		[Test]
		public void FooMethodDefaultReturnType()
		{
			Assert.AreEqual(globalClass, fooMethod.ReturnType.GetUnderlyingClass());
		}
		
		[Test]
		public void BarMethodDefaultReturnType()
		{
			Assert.AreEqual(globalClass, barMethod.ReturnType.GetUnderlyingClass());
		}		
		
		[Test]
		public void FooMethodDeclaringType()
		{
			Assert.AreEqual(globalClass, fooMethod.DeclaringType);
		}
		
		[Test]
		public void FooMethodBodyRegion()
		{
			int startLine = 1;
			int startColumn = 11;
			int endLine = 2;
			int endColumn = 6;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), fooMethod.BodyRegion.ToString());
		}
		
		/// <summary>
		/// The method region needs to extend up just after the colon. It does not include the body.
		/// </summary>
		[Test]
		public void FooMethodRegion()
		{
			int startLine = 1;
			int startColumn = 1;
			int endLine = 1;
			int endColumn = 11;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), fooMethod.Region.ToString());
		}

		[Test]
		public void BarMethodBodyRegion()
		{
			int startLine = 4;
			int startColumn = 12;
			int endLine = 5;
			int endColumn = 6;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), barMethod.BodyRegion.ToString());
		}
		
		/// <summary>
		/// The method region needs to extend up just after the colon. It does not include the body.
		/// </summary>
		[Test]
		public void BarMethodRegion()
		{
			int startLine = 4;
			int startColumn = 1;
			int endLine = 4;
			int endColumn = 12;
			DomRegion region = new DomRegion(startLine, startColumn, endLine, endColumn);
			Assert.AreEqual(region.ToString(), barMethod.Region.ToString());
		}
		
		[Test]
		public void BarMethodHasOneParameter()
		{
			Assert.AreEqual(1, barMethod.Parameters.Count);
		}
		
		[Test]
		public void FooMethodTextInsideFoldIsFooMethodBody()
		{
			string textInsideFold = document.GetText(fooMethodFold.StartOffset, fooMethodFold.Length);
			Assert.AreEqual("\r\n\tpass", textInsideFold);
		}
		
		[Test]
		public void BarMethodTextInsideFoldIsBarMethodBody()
		{
			string textInsideFold = document.GetText(barMethodFold.StartOffset, barMethodFold.Length);
			Assert.AreEqual("\r\n\tpass", textInsideFold);
		}
		
		[Test]
		public void FooMethodCollapsedFoldTitleIsNull()
		{
			Assert.IsNull(fooMethodFold.Title);
		}
		
		[Test]
		public void BarMethodCollapsedFoldTitleIsNull()
		{
			Assert.IsNull(barMethodFold.Title);
		}
	}
}
