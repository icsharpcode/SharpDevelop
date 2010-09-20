// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Refactoring;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using AvalonEdit = ICSharpCode.AvalonEdit;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the the project's root namespace is passed to the PythonDesigner
	/// </summary>
	[TestFixture]
	public class ProjectRootNamespacePassedToMergeTestFixture
	{
		TextDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			AvalonEdit.TextEditor textEditor = new AvalonEdit.TextEditor();
			document = textEditor.Document;
			textEditor.Text = GetTextEditorCode();
			
			PythonParser parser = new PythonParser();
			MockProjectContent projectContent = new MockProjectContent();
			MockProject project = new MockProject();
			project.RootNamespace = "RootNamespace";
			projectContent.Project = project;
			ICompilationUnit compilationUnit = parser.Parse(projectContent, @"test.py", document.Text);

			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				Form form = (Form)host.RootComponent;
				form.ClientSize = new Size(200, 300);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
			
				// Add picture box
				PictureBox pictureBox = (PictureBox)host.CreateComponent(typeof(PictureBox), "pictureBox1");
				pictureBox.Location = new Point(0, 0);
				pictureBox.Image = new Bitmap(10, 10);
				pictureBox.Size = new Size(100, 120);
				pictureBox.TabIndex = 0;
				form.Controls.Add(pictureBox);
			
				MockTextEditorOptions options = new MockTextEditorOptions();
				options.ConvertTabsToSpaces = true;
				options.IndentationSize = 4;
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					AvalonEditDocumentAdapter adapter = new AvalonEditDocumentAdapter(document, null);
					PythonDesignerGenerator generator = new PythonDesignerGenerator(options);
					generator.Merge(host, adapter, compilationUnit, serializationManager);
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode =
				"from System.Windows.Forms import Form\r\n" +
				"\r\n" +
				"class MainForm(Form):\r\n" +
				"    def __init__(self):\r\n" +
				"        self.InitializeComponent()\r\n" +
				"    \r\n" +
				"    def InitializeComponent(self):\r\n" +
				"        resources = System.Resources.ResourceManager(\"RootNamespace.MainForm\", System.Reflection.Assembly.GetEntryAssembly())\r\n" +
				"        self._pictureBox1 = System.Windows.Forms.PictureBox()\r\n" +
				"        self.SuspendLayout()\r\n" +
				"        # \r\n" +
				"        # pictureBox1\r\n" +
				"        # \r\n" +
				"        self._pictureBox1.Image = resources.GetObject(\"pictureBox1.Image\")\r\n" +
				"        self._pictureBox1.Location = System.Drawing.Point(0, 0)\r\n" +
				"        self._pictureBox1.Name = \"pictureBox1\"\r\n" +
				"        self._pictureBox1.Size = System.Drawing.Size(100, 120)\r\n" +
				"        self._pictureBox1.TabIndex = 0\r\n" +
				"        self._pictureBox1.TabStop = False\r\n" +
				"        # \r\n" +
				"        # MainForm\r\n" +
				"        # \r\n" +
				"        self.ClientSize = System.Drawing.Size(200, 300)\r\n" +
				"        self.Controls.Add(self._pictureBox1)\r\n" +
				"        self.Name = \"MainForm\"\r\n" +
				"        self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, document.Text, document.Text);
		}
		
		string GetTextEditorCode()
		{
			return
				"from System.Windows.Forms import Form\r\n" +
				"\r\n" +
				"class MainForm(Form):\r\n" +
				"    def __init__(self):\r\n" +
				"        self.InitializeComponent()\r\n" +
				"    \r\n" +
				"    def InitializeComponent(self):\r\n" +
				"        pass\r\n";
		}
	}
}
