// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Converter
{
	[TestFixture]
	public class ConvertVBNetToPythonMenuCommandTestFixture : ConvertToPythonMenuCommand
	{
		string newFileText;
		string defaultFileName;
		string language;
		
		[SetUp]
		public void Init()
		{
			MockWorkbench workbench = MockWorkbench.CreateWorkbenchWithOneViewContent("test.vb");
			workbench.ActiveMockEditableViewContent.Text = 
				"class Foo\r\n" +
				"end class";
			
			MockTextEditorOptions options = workbench.ActiveMockEditableViewContent.MockTextEditorOptions;
			options.ConvertTabsToSpaces = false;
			options.IndentationSize = 2;
			
			Run(workbench);
		}
		
		[Test]
		public void Run_VBNetFileOpen_PythonCodeIsSavedInFileCreated()
		{
			string expectedNewFileText = 
				"class Foo(object):\r\n" +
				"\tpass";
			
			Assert.AreEqual(expectedNewFileText, newFileText);
		}
		
		[Test]
		public void Run_VBNetFileOpen_NewFileNameIsGeneratedPy()
		{
			Assert.AreEqual("Generated.py", defaultFileName);
		}

		[Test]
		public void Run_VBNetFileOpen_NewFileLanguageIsPython()
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
