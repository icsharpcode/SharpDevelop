// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Converter
{
	/// <summary>
	/// Tests the ConvertCSharpToRubyMenuCommand.
	/// </summary>
	[TestFixture]
	public class ConvertCSharpToRubyMenuCommandTestFixture : ConvertToRubyMenuCommand
	{
		string newFileText;
		string defaultFileName;
		string language;
		string fileNamePassedToGetParseInformation;
		MockEditableViewContent mockViewContent;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			mockViewContent = new MockEditableViewContent();
			mockViewContent.Text = 
				"class Foo\r\n" +
				"{\r\n" +
				"    public Foo()\r\n" +
				"    {\r\n" +
				"    }\r\n" +
				"}";
			
			mockViewContent.PrimaryFileName = "test.cs";
			
			MockWorkbench workbench = new MockWorkbench();
			MockWorkbenchWindow window = new MockWorkbenchWindow();
			window.ActiveViewContent = mockViewContent;
			workbench.ActiveWorkbenchWindow = window;
			
			MockTextEditorProperties textEditorProperties = new MockTextEditorProperties();
			textEditorProperties.IndentationSize = 4;
			textEditorProperties.ConvertTabsToSpaces = true;
			
			Run(workbench, textEditorProperties);
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
			return new ParseInformation();
		}
	}
}
