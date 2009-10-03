// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the PythonDesignerGenerator's MergeFormChanges
	/// finds the InitializeComponents method and the class.
	/// </summary>
	[TestFixture]
	public class GeneratorMergeFindsInitializeComponentsTestFixture
	{
		DerivedPythonDesignerGenerator generator;
		FormsDesignerViewContent viewContent;
		FormsDesignerViewContent viewContentAttached;
		MockTextEditorViewContent mockViewContent;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MockTextEditorProperties textEditorProperties = new MockTextEditorProperties();
			generator = new DerivedPythonDesignerGenerator();
			mockViewContent = new MockTextEditorViewContent();
			viewContent = new FormsDesignerViewContent(mockViewContent, new MockOpenedFile("Test.py"));
			viewContent.DesignerCodeFileContent = GetTextEditorCode();
			generator.Attach(viewContent);
			viewContentAttached = generator.GetViewContent();

			ParseInformation parseInfo = new ParseInformation();
			PythonParser parser = new PythonParser();
			ICompilationUnit parserCompilationUnit = parser.Parse(new DefaultProjectContent(), "Test.py", GetTextEditorCode());
			parseInfo.SetCompilationUnit(parserCompilationUnit);
			generator.ParseInfoToReturnFromParseFileMethod = parseInfo;
			
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				Form form = (Form)host.RootComponent;			
				form.ClientSize = new Size(499, 309);
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					generator.MergeRootComponentChanges(host, serializationManager);
					generator.Detach();
				}
			}
		}
					
		[Test]
		public void GetDomRegion()
		{
			MockMethod method = new MockMethod();
			DomRegion bodyRegion = new DomRegion(0, 4, 1, 4);
			method.BodyRegion = bodyRegion;
			DomRegion expectedRegion = new DomRegion(bodyRegion.BeginLine + 1, 1, bodyRegion.EndLine + 1, 1);
			
			Assert.AreEqual(expectedRegion, PythonDesignerGenerator.GetBodyRegionInDocument(method));
		}
		
		[Test]
		public void ViewContentSetToNullAfterDetach()
		{
			Assert.IsNull(generator.GetViewContent());
		}
		
		[Test]
		public void ViewContentAttached()
		{
			Assert.AreSame(viewContent, viewContentAttached);
		}
		
		[Test]
		public void DocumentUpdated()
		{
			string expectedText = "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponents()\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponents(self):\r\n" +
					"\t\tself.SuspendLayout()\r\n" +
					"\t\t# \r\n" +
					"\t\t# MainForm\r\n" +
					"\t\t# \r\n" +
					"\t\tself.ClientSize = System.Drawing.Size(499, 309)\r\n" +
					"\t\tself.Name = \"MainForm\"\r\n" +
					"\t\tself.ResumeLayout(False)\r\n";
			
			Assert.AreEqual(expectedText, viewContent.DesignerCodeFileContent);
		}
		
		string GetTextEditorCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponents()\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponents(self):\r\n" +
					"\t\tpass\r\n";	
		}
	}
}
