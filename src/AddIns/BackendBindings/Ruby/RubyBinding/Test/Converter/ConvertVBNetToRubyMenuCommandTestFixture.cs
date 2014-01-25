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
	public class ConvertVBNetToRubyMenuCommandTestFixture : ConvertToRubyMenuCommand
	{
		string newFileText;
		string defaultFileName;
		string language;
		string fileNamePassedToGetParseInformation;
		
		[SetUp]
		public void Init()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent("test.vb");
			workbench.ActiveMockEditableViewContent.Text = 
				"class Foo\r\n" +
				"    Public Sub New\r\n" +
				"    End Sub\r\n" +
				"end class";
			
			MockTextEditorOptions options = workbench.ActiveMockEditableViewContent.MockTextEditorOptions;	
			options.ConvertTabsToSpaces = false;
			options.IndentationSize = 2;
			
			Run(workbench);
		}
		
		[Test]
		public void GeneratedRubyCode()
		{
			string expectedRubyCode = 
				"class Foo\r\n" +
				"\tdef initialize()\r\n" +
				"\tend\r\n" +
				"end";
			
			Assert.AreEqual(expectedRubyCode, newFileText);
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
			Assert.AreEqual("test.vb", fileNamePassedToGetParseInformation);
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
