// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.RubyBinding;
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
