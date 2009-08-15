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

using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the indent information in the ITextEditorProperties is passed to the generator.
	/// </summary>
	[TestFixture]
	public class TextEditorIndentPassedToGeneratorTestFixture
	{
		IDocument document;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (FormsDesignerViewContent viewContent = new FormsDesignerViewContent(null, new MockOpenedFile("Test.py"))) {
				viewContent.DesignerCodeFileContent = "class MainForm(Form):\r\n" +
														" def __init__(self):\r\n" +
														"  self.InitializeComponent()\r\n" +
														"\r\n" +
														" def InitializeComponent(self):\r\n" +
														"  pass\r\n";

				document = viewContent.DesignerCodeFileDocument;

				ParseInformation parseInfo = new ParseInformation();
				PythonParser parser = new PythonParser();
				ICompilationUnit compilationUnit = parser.Parse(new DefaultProjectContent(), @"test.py", document.TextContent);
				parseInfo.SetCompilationUnit(compilationUnit);

				using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
					IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
					Form form = (Form)host.RootComponent;			
					form.ClientSize = new Size(284, 264);
					
					PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
					PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
					namePropertyDescriptor.SetValue(form, "MainForm");
					
					MockTextEditorProperties textEditorProperties = new MockTextEditorProperties();
					textEditorProperties.ConvertTabsToSpaces = true;
					textEditorProperties.IndentationSize = 1;
 	
					DerivedPythonDesignerGenerator generator = new DerivedPythonDesignerGenerator(textEditorProperties);
					generator.ParseInfoToReturnFromParseFileMethod = parseInfo;
					generator.Attach(viewContent);
					DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
					using (serializationManager.CreateSession()) {
						generator.MergeRootComponentChanges(host, serializationManager);
					}
				}
			}
		}
		
		[Test]
		public void GeneratedCode()
		{
			string expectedCode = "class MainForm(Form):\r\n" + 
								" def __init__(self):\r\n" +
								"  self.InitializeComponent()\r\n" +
								"\r\n" +
								" def InitializeComponent(self):\r\n" +
								"  self.SuspendLayout()\r\n" +
								"  # \r\n" +
								"  # MainForm\r\n" +
								"  # \r\n" +
								"  self.ClientSize = System.Drawing.Size(284, 264)\r\n" +
								"  self.Name = \"MainForm\"\r\n" +
								"  self.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedCode, document.TextContent);
		}
	}
}
