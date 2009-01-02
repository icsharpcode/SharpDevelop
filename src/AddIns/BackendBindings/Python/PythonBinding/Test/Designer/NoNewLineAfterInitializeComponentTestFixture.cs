// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the code can be generated if there is no new line after the InitializeComponent method.
	/// </summary>
	[TestFixture]
	public class NoNewLineAfterInitializeComponentMethodTestFixture
	{
		IDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (TextEditorControl textEditor = new TextEditorControl()) {
				document = textEditor.Document;
				textEditor.Text = GetTextEditorCode();

				PythonParser parser = new PythonParser();
				ICompilationUnit compilationUnit = parser.Parse(new DefaultProjectContent(), @"test.py", document.TextContent);

				using (Form form = new Form()) {
					form.Name = "MainForm";
					form.ClientSize = new Size(499, 309);
					
					PythonDesignerGenerator.Merge(form, document, compilationUnit, new MockTextEditorProperties());
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "from System.Windows.Forms import Form\r\n" +
									"\r\n" +
									"class MainForm(Form):\r\n" +
									"\tdef __init__(self):\r\n" +
									"\t\tself.InitializeComponent()\r\n" +
									"\t\r\n" +
									"\tdef InitializeComponent(self):\r\n" +
									"\t\tself.SuspendLayout()\r\n" +
									"\t\t# \r\n" +
									"\t\t# MainForm\r\n" +
									"\t\t# \r\n" + 
									"\t\tself.ClientSize = System.Drawing.Size(499, 309)\r\n" +
									"\t\tself.Name = \"MainForm\"\r\n" +
									"\t\tself.ResumeLayout(False)\r\n" +
									"\t\tself.PerformLayout()\r\n";
			
			Assert.AreEqual(expectedCode, document.TextContent);
		}
		
		/// <summary>
		/// No new line after the pass statement for InitializeComponent method.
		/// </summary>
		string GetTextEditorCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\t\r\n" +
					 "\tdef InitializeComponent(self):\r\n" +
					"\t\tpass"; 						
		}
	}
}
