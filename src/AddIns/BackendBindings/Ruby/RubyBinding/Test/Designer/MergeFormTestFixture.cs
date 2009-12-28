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
	/// Tests that the GeneratedInitializeComponentMethod class
	/// can merge the changes into the text editor.
	/// </summary>
	[TestFixture]
	public class MergeFormTestFixture
	{
		IDocument document;
		MockResourceService resourceService;
		MockResourceWriter resourceWriter;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			resourceWriter = new MockResourceWriter();
			resourceService = new MockResourceService();
			resourceService.SetResourceWriter(resourceWriter);
			
			using (TextEditorControl textEditor = new TextEditorControl()) {
				document = textEditor.Document;
				textEditor.Text = GetTextEditorCode();

				RubyParser parser = new RubyParser();
				ICompilationUnit compilationUnit = parser.Parse(new DefaultProjectContent(), @"test.rb", document.TextContent);

				using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
					IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
					Form form = (Form)host.RootComponent;
					form.ClientSize = new Size(499, 309);

					PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
					PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
					namePropertyDescriptor.SetValue(form, "MainForm");
					
					DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
					using (serializationManager.CreateSession()) {
						RubyDesignerGenerator.Merge(host, document, compilationUnit, new MockTextEditorProperties(), serializationManager);
					}
				}
			}
		}
		
		[Test]
		public void MergedDocumentText()
		{
			string expectedText = GetTextEditorCode().Replace(GetTextEditorInitializeComponentMethod(), GetGeneratedInitializeComponentMethod());
			Assert.AreEqual(expectedText, document.TextContent);
		}

		string GetGeneratedCode()
		{
			return  "require \"System.Windows.Forms\"\r\n" +
					"\r\n" +
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"\tdef initialize()\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\tend\r\n" +
					"\t\r\n" +
					GetGeneratedInitializeComponentMethod() +
					"end\r\n";
		}
		
		string GetGeneratedInitializeComponentMethod()
		{
			return	"\tdef InitializeComponent()\r\n" +
					"\t\tself.SuspendLayout()\r\n" +
					"\t\t# \r\n" +
					"\t\t# MainForm\r\n" +
					"\t\t# \r\n" + 
					"\t\tself.ClientSize = System::Drawing::Size.new(499, 309)\r\n" +
					"\t\tself.Name = \"MainForm\"\r\n" +
					"\t\tself.ResumeLayout(false)\r\n" +
					"\tend\r\n";
		}
		
		string GetTextEditorCode()
		{
			return "require \"System.Windows.Forms\"\r\n" +
					"\r\n" +
					"class MainForm < Form\r\n" +
					"\tdef initialize()\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\tend\r\n" +
					"\t\r\n" +
				GetTextEditorInitializeComponentMethod() +
				"end\r\n";
		}
		
		string GetTextEditorInitializeComponentMethod()
		{
			return
				"\tdef InitializeComponent()\r\n" +
				"\t\t\r\n" +
				"\tend\r\n";
		}
	}
}
