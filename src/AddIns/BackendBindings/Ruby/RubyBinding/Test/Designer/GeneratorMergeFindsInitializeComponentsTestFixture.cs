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

using ICSharpCode.RubyBinding;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the RubyDesignerGenerator's MergeFormChanges
	/// finds the InitializeComponents method and the class.
	/// </summary>
	[TestFixture]
	public class GeneratorMergeFindsInitializeComponentsTestFixture
	{
		DerivedRubyDesignerGenerator generator;
		FormsDesignerViewContent viewContent;
		FormsDesignerViewContent viewContentAttached;
		MockTextEditorViewContent mockViewContent;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			MockTextEditorProperties textEditorProperties = new MockTextEditorProperties();
			generator = new DerivedRubyDesignerGenerator();
			mockViewContent = new MockTextEditorViewContent();
			viewContent = new FormsDesignerViewContent(mockViewContent, new MockOpenedFile("Test.rb"));
			viewContent.DesignerCodeFileContent = GetTextEditorCode();
			generator.Attach(viewContent);
			viewContentAttached = generator.GetViewContent();

			ParseInformation parseInfo = new ParseInformation();
			RubyParser parser = new RubyParser();
			ICompilationUnit parserCompilationUnit = parser.Parse(new DefaultProjectContent(), "Test.rb", GetTextEditorCode());
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
			DomRegion expectedRegion = new DomRegion(bodyRegion.BeginLine + 1, 1, bodyRegion.EndLine, 1);
			
			Assert.AreEqual(expectedRegion, RubyDesignerGenerator.GetBodyRegionInDocument(method));
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
			string expectedText = "require \"mscorlib\"\r\n" +
					"require \"System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n" +
					"require \"System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n" +
					"\r\n" +
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"\tdef initialize()\r\n" +
					"\t\tself.InitializeComponents()\r\n" +
					"\tend\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponents()\r\n" +
					"\t\tself.SuspendLayout()\r\n" +
					"\t\t# \r\n" +
					"\t\t# MainForm\r\n" +
					"\t\t# \r\n" +
					"\t\tself.ClientSize = System::Drawing::Size.new(499, 309)\r\n" +
					"\t\tself.Name = \"MainForm\"\r\n" +
					"\t\tself.ResumeLayout(false)\r\n" +
					"\tend\r\n" +
					"end";
			
			Assert.AreEqual(expectedText, viewContent.DesignerCodeFileContent);
		}
		
		string GetTextEditorCode()
		{
			return "require \"mscorlib\"\r\n" +
					"require \"System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\r\n" +
					"require \"System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a\"\r\n" +
					"\r\n" +
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"\tdef initialize()\r\n" +
					"\t\tself.InitializeComponents()\r\n" +
					"\tend\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponents()\r\n" +
					"\tend\r\n" +
					"end";			
		}
	}
}
