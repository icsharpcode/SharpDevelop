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
	/// Tests the ConvertVBNetToRubyMenuCommand.
	/// </summary>
	[TestFixture]
	public class ConvertVBNetToRubyMenuCommandTestFixture : ConvertToRubyMenuCommand
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
				"    Public Sub New\r\n" +
				"    End Sub\r\n" +
				"end class";
			mockViewContent.PrimaryFileName = "test.vb";
			
			MockWorkbench workbench = new MockWorkbench();
			MockWorkbenchWindow window = new MockWorkbenchWindow();
			window.ActiveViewContent = mockViewContent;
			workbench.ActiveWorkbenchWindow = window;
			
			MockTextEditorProperties textEditorProperties = new MockTextEditorProperties();
			textEditorProperties.ConvertTabsToSpaces = false;
			textEditorProperties.IndentationSize = 2;
			
			Run(workbench, textEditorProperties);
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
			return new ParseInformation();
		}
	}
}
