// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.IO;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using IronPython.CodeDom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the GeneratedInitializeComponentMethod class
	/// can merge the changes into the text editor.
	/// </summary>
	[TestFixture]
	public class MergeFormTestFixture
	{
		IDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (TextEditorControl textEditor = new TextEditorControl()) {
				document = textEditor.Document;
				textEditor.Text = GetTextEditorCode();

				PythonParser parser = new PythonParser();
				ICompilationUnit textEditorCompileUnit = parser.Parse(new DefaultProjectContent(), @"test.py", document.TextContent);
				
				PythonProvider provider = new PythonProvider();
				CodeCompileUnit unit = provider.Parse(new StringReader(GetGeneratedCode()));
				GeneratedInitializeComponentMethod initComponentMethod = GeneratedInitializeComponentMethod.GetGeneratedInitializeComponentMethod(unit);
				initComponentMethod.Merge(document, textEditorCompileUnit);
			}
		}
		
		/// <summary>
		/// The comments are stripped out by the PythonProvider
		/// when the code is generated.
		/// </summary>
		[Test]
		public void MergedDocumentText()
		{
			string expectedText = GetTextEditorCode().Replace(GetTextEditorInitializeComponentMethod(), GetGeneratedInitializeComponentMethod());
			expectedText = expectedText.Replace("\t\t#\r\n" +
					"\t\t# MainForm\r\n" +
					"\t\t#\r\n",
					String.Empty);

			Assert.AreEqual(expectedText, document.TextContent);
		}

		string GetGeneratedCode()
		{
			return  "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(System.Windows.Forms.Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\t\r\n" +
					GetGeneratedInitializeComponentMethod();
		}
		
		string GetGeneratedInitializeComponentMethod()
		{
			return	"\tdef InitializeComponent(self):\r\n" +
					"\t\tself.SuspendLayout()\r\n" +
					"\t\t#\r\n" +
					"\t\t# MainForm\r\n" +
					"\t\t#\r\n" + 
					"\t\tself.AutoScaleDimensions = System.Drawing.SizeF(6, 13)\r\n" +
					"\t\tself.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font\r\n" +
					"\t\tself.ClientSize = System.Drawing.Size(499, 309)\r\n" +
					"\t\tself.Name = 'MainForm'\r\n" +
					"\t\tself.ResumeLayout(false)\r\n"; 						
		}
		
		string GetTextEditorCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\t\r\n" +
				GetTextEditorInitializeComponentMethod();
		}
		
		string GetTextEditorInitializeComponentMethod()
		{
			return "\tdef InitializeComponent(self):\r\n" +
					"\t\tpass\r\n"; 						
		}
	}
}
