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
using System.IO;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Parsing
{
	/// <summary>
	/// Tests the PythonParser.
	/// </summary>
	[TestFixture]
	public class ParserTestFixture
	{
		PythonParser parser;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonMSBuildEngineHelper.InitMSBuildEngine();
			parser = new PythonParser();
		}
		
		[Test]
		public void CanParsePythonFileName()
		{
			Assert.IsTrue(parser.CanParse("test.py"));
		}

		[Test]
		public void CanParseTextFileName()
		{
			Assert.IsFalse(parser.CanParse("test.txt"));
		}
		
		[Test]
		public void CanParseNullFileName()
		{
			string fileName = null;
			Assert.IsFalse(parser.CanParse(fileName));
		}
		
		[Test]
		public void CanParseUpperCasePythonFileName()
		{
			Assert.IsTrue(parser.CanParse("TEST.PY"));
		}
		
		[Test]
		public void NoLexerTags()
		{
			Assert.AreEqual(0, parser.LexerTags.Length);
		}
		
		[Test]
		public void SetLexerTags()
		{
			PythonParser parser = new PythonParser();
			string[] tags = new string[] {"Test"};
			parser.LexerTags = tags;
			
			Assert.AreEqual(tags, parser.LexerTags);
		}
		
		[Test]
		public void LanguageProperties()
		{
			Assert.IsNotNull(parser.Language);
		}
		
		[Test]
		public void CannotParseNullProject()
		{
			IProject project = null;
			Assert.IsFalse(parser.CanParse(project));
		}
		
		[Test]
		public void CanParsePythonProject()
		{
			ProjectCreateInformation createInfo = new ProjectCreateInformation();
			createInfo.Solution = new Solution(new MockProjectChangeWatcher());
			createInfo.OutputProjectFileName = @"C:\projects\test.pyproj";
			PythonProject project = new PythonProject(createInfo);
			Assert.IsTrue(parser.CanParse(project));
		}
	
		[Test]
		public void CannotParseNonPythonLanguageProject()
		{
			MockProject project = new MockProject();
			project.Language = "Test";
			Assert.IsFalse(parser.CanParse(project));
		}
		
		[Test]
		public void ParseWithNullFileContent()
		{
			DefaultProjectContent projectContent = new DefaultProjectContent();
			ICompilationUnit unit = parser.Parse(projectContent, null, (ICSharpCode.SharpDevelop.ITextBuffer)null);
			Assert.IsInstanceOf(typeof(DefaultCompilationUnit), unit);
		}
		
		[Test]
		public void PythonExpressionFinderCreated()
		{
			IExpressionFinder expressionFinder = parser.CreateExpressionFinder(@"c:\Projects\test.py");
			Assert.IsInstanceOf(typeof(PythonExpressionFinder), expressionFinder);
		}
		
		[Test]
		public void Resolver()
		{
			Assert.IsInstanceOf(typeof(PythonResolver), parser.CreateResolver());
		}
	}
}
