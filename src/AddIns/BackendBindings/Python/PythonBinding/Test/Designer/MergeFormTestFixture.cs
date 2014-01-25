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
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using AvalonEdit = ICSharpCode.AvalonEdit;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the GeneratedInitializeComponentMethod class
	/// can merge the changes into the text editor.
	/// </summary>
	[TestFixture]
	public class MergeFormTestFixture
	{
		TextDocument document;
		MockResourceService resourceService;
		MockResourceWriter resourceWriter;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resourceWriter = new MockResourceWriter();
			resourceService = new MockResourceService();
			resourceService.SetResourceWriter(resourceWriter);
			
			AvalonEdit.TextEditor textEditor = new AvalonEdit.TextEditor();
			document = textEditor.Document;
			textEditor.Text = GetTextEditorCode();
			
			PythonParser parser = new PythonParser();
			ICompilationUnit compilationUnit = parser.Parse(new DefaultProjectContent(), @"test.py", document.Text);

			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(499, 309);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					AvalonEditDocumentAdapter adapter = new AvalonEditDocumentAdapter(document, null);
					MockTextEditorOptions options = new MockTextEditorOptions();
					PythonDesignerGenerator generator = new PythonDesignerGenerator(options);
					generator.Merge(host, adapter, compilationUnit, serializationManager);
				}
			}
		}
		
		[Test]
		public void MergedDocumentText()
		{
			string expectedText = GetTextEditorCode().Replace(GetTextEditorInitializeComponentMethod(), GetGeneratedInitializeComponentMethod());
			Assert.AreEqual(expectedText, document.Text);
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
					"\t\t# \r\n" +
					"\t\t# MainForm\r\n" +
					"\t\t# \r\n" + 
					"\t\tself.ClientSize = System.Drawing.Size(499, 309)\r\n" +
					"\t\tself.Name = \"MainForm\"\r\n" +
					"\t\tself.ResumeLayout(False)\r\n";
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
