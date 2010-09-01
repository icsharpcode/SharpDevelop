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
	[TestFixture]
	public class ConvertCSharpToPythonMenuCommandTestFixture : ConvertToPythonMenuCommand
	{
		string newFileText;
		string defaultFileName;
		string language;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent("test.cs");
			workbench.ActiveMockEditableViewContent.Text = "class Foo { }";
			
			MockTextEditorOptions options = workbench.ActiveMockEditableViewContent.MockTextEditorOptions;
			options.IndentationSize = 4;
			options.ConvertTabsToSpaces = true;
			
			Run(workbench);
		}
		
		[Test]
		public void Run_CSharpFileOpen_PythonCodeGeneratedInNewFile()
		{
			string code = 
				"class Foo(object):\r\n" +
				"    pass";
			Assert.AreEqual(code, newFileText);
		}
		
		[Test]
		public void Run_CSharpFileOpen_NewFileNameIsGeneratedPy()
		{
			Assert.AreEqual("Generated.py", defaultFileName);
		}

		[Test]
		public void Run_CSharpFileOpen_NewFileLanguageIsPython()
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
