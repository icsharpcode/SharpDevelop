// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Converter
{
	/// <summary>
	/// Tests the ConvertCSharpToPythonMenuCommand.
	/// </summary>
	[TestFixture]
	public class ConvertCSharpToPythonMenuCommandTestFixture : ConvertToPythonMenuCommand
	{
		string newFileText;
		string defaultFileName;
		string language;
		MockEditableViewContent mockViewContent;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			mockViewContent = new MockEditableViewContent();
			mockViewContent.Text = "class Foo { }";
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
		public void GeneratedPythonCode()
		{
			string code = "class Foo(object):\r\n" +
						  "    pass";
			Assert.AreEqual(code, newFileText);
		}
		
		[Test]
		public void NewFileName()
		{
			Assert.AreEqual("Generated.py", defaultFileName);
		}

		[Test]
		public void NewFileLanguage()
		{
			Assert.AreEqual("Python", language);
		}
		
		[Test]
		public void TabIndent()
		{
			MockTextEditorProperties properties = new MockTextEditorProperties();
			properties.ConvertTabsToSpaces = false;
			Assert.AreEqual("\t", NRefactoryToPythonConverter.GetIndentString(properties));
		}
		
		[Test]
		public void TwoChaSpaceIndent()
		{
			MockTextEditorProperties properties = new MockTextEditorProperties();
			properties.ConvertTabsToSpaces = true;
			properties.IndentationSize = 2;
			Assert.AreEqual("  ", NRefactoryToPythonConverter.GetIndentString(properties));
		}		
		
		protected override void NewFile(string defaultName, string language, string content)
		{
			defaultFileName = defaultName;
			this.language = language;
			newFileText = content;
		}
	}
}
