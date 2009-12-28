// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests the the project's root namespace is passed to the RubyDesigner
	/// </summary>
	[TestFixture]
	public class ProjectRootNamespacePassedToMergeTestFixture
	{
		IDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (TextEditorControl textEditor = new TextEditorControl()) {
				document = textEditor.Document;
				textEditor.Text = GetTextEditorCode();

				RubyParser parser = new RubyParser();
				MockProjectContent projectContent = new MockProjectContent();
				MockProject project = new MockProject();
				project.RootNamespace = "RootNamespace";
				projectContent.Project = project;
				ICompilationUnit compilationUnit = parser.Parse(projectContent, @"test.py", document.TextContent);

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
					
					MockTextEditorProperties properties = new MockTextEditorProperties();
					properties.ConvertTabsToSpaces = true;
					properties.IndentationSize = 4;
					
					DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
					using (serializationManager.CreateSession()) {
						RubyDesignerGenerator.Merge(host, document, compilationUnit, properties, serializationManager);
					}
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "require \"System.Windows.Forms\"\r\n" +
									"\r\n" +
									"class MainForm < Form\r\n" +
									"    def initialize()\r\n" +
									"        self.InitializeComponent()\r\n" +
									"    end\r\n" +
									"    \r\n" +
									"    def InitializeComponent()\r\n" +
									"        resources = System::Resources::ResourceManager.new(\"RootNamespace.MainForm\", System::Reflection::Assembly.GetEntryAssembly())\r\n" +
									"        @pictureBox1 = System::Windows::Forms::PictureBox.new()\r\n" +
									"        self.SuspendLayout()\r\n" +
									"        # \r\n" +
									"        # pictureBox1\r\n" +
									"        # \r\n" +
									"        @pictureBox1.Image = resources.GetObject(\"pictureBox1.Image\")\r\n" +
									"        @pictureBox1.Location = System::Drawing::Point.new(0, 0)\r\n" +
									"        @pictureBox1.Name = \"pictureBox1\"\r\n" +
									"        @pictureBox1.Size = System::Drawing::Size.new(100, 120)\r\n" +
									"        @pictureBox1.TabIndex = 0\r\n" +
									"        @pictureBox1.TabStop = false\r\n" +
									"        # \r\n" +
									"        # MainForm\r\n" +
									"        # \r\n" +
									"        self.ClientSize = System::Drawing::Size.new(200, 300)\r\n" +
									"        self.Controls.Add(@pictureBox1)\r\n" +
									"        self.Name = \"MainForm\"\r\n" +
									"        self.ResumeLayout(false)\r\n" +
									"    end\r\n" +
									"end";
			
			Assert.AreEqual(expectedCode, document.TextContent, document.TextContent);
		}
		
		string GetTextEditorCode()
		{
			return "require \"System.Windows.Forms\"\r\n" +
					"\r\n" +
					"class MainForm < Form\r\n" +
					"    def initialize()\r\n" +
					"        self.InitializeComponent()\r\n" +
					"    end\r\n" +
					"    \r\n" +
					"    def InitializeComponent()\r\n" +
					"    end\r\n" +
					"end";
		}
	}
}
