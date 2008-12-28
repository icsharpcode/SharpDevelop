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
	/// Tests the ConvertVBNetToPythonMenuCommand.
	/// </summary>
	[TestFixture]
	public class ConvertVBNetToPythonMenuCommandTestFixture : ConvertToPythonMenuCommand
	{
		string newFileText;
		string defaultFileName;
		string language;
		MockEditableViewContent mockViewContent;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			mockViewContent = new MockEditableViewContent();
			mockViewContent.Text = "class Foo\r\n" +
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
		public void GeneratedPythonCode()
		{
			Assert.AreEqual("class Foo(object):\r\n\tpass", newFileText);
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
		
		protected override void NewFile(string defaultName, string language, string content)
		{
			defaultFileName = defaultName;
			this.language = language;
			newFileText = content;
		}
	}
}
