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
using ICSharpCode.Core;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Converter
{
	[TestFixture]
	public class ConvertCSharpToRubyMenuCommandTestFixture : ConvertToRubyMenuCommand
	{
		string newFileText;
		string defaultFileName;
		string language;
		string fileNamePassedToGetParseInformation;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent("test.cs");
			workbench.ActiveMockEditableViewContent.Text = 
				"class Foo\r\n" +
				"{\r\n" +
				"    public Foo()\r\n" +
				"    {\r\n" +
				"    }\r\n" +
				"}";
			
			MockTextEditorOptions options = workbench.ActiveMockEditableViewContent.MockTextEditorOptions;
			options.IndentationSize = 4;
			options.ConvertTabsToSpaces = true;
			
			Run(workbench);
		}
		
		[Test]
		public void GeneratedRubyCode()
		{
			string code = 
				"class Foo\r\n" +
				"    def initialize()\r\n" +
				"    end\r\n" +
				"end";
			
			Assert.AreEqual(code, newFileText);
		}
		
		[Test]
		public void NewFileName()
		{
			Assert.AreEqual("Generated.rb", defaultFileName);
		}

		[Test]
		public void NewFileLanguage()
		{
			Assert.AreEqual("Ruby", language);
		}
		
		[Test]
		public void NameOfFileBeingConvertedIsPassedToParserServiceGetInformation()
		{
			Assert.AreEqual("test.cs", fileNamePassedToGetParseInformation);
		}
		
		protected override void NewFile(string defaultName, string language, string content)
		{
			defaultFileName = defaultName;
			this.language = language;
			newFileText = content;
		}
		
		protected override ParseInformation GetParseInformation(string fileName)
		{
			fileNamePassedToGetParseInformation = fileName;
			return new ParseInformation(new DefaultCompilationUnit(new DefaultProjectContent()));
		}
	}
}
