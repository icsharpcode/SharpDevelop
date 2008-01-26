// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

using ICSharpCode.PythonBinding;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.CodeDom;
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
		CodeTypeDeclaration formClass;
		CodeMemberMethod initializeComponent;
		FormsDesignerViewContent viewContent;
		FormsDesignerViewContent viewContentAttached;
		MockTextEditorViewContent mockViewContent;
		MockMethod mockMethod;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonProvider provider = new PythonProvider();
			CodeCompileUnit unit = provider.Parse(new StringReader(GetPythonCode()));
		
			formClass = null;
			initializeComponent = null;
			foreach (CodeNamespace n in unit.Namespaces) {
				foreach (CodeTypeDeclaration typeDecl in n.Types) {
					foreach (CodeTypeMember m in typeDecl.Members) {
						if (m is CodeMemberMethod && m.Name == "InitializeComponent") {
							formClass = typeDecl;
							initializeComponent = (CodeMemberMethod)m;
							break;
						}
					}
				}
			}
						
			generator = new DerivedPythonDesignerGenerator();
			mockViewContent = new MockTextEditorViewContent();
			mockViewContent.TextEditorControl.Document.TextContent = GetTextEditorCode();
			viewContent = new FormsDesignerViewContent(mockViewContent, null, null);
			generator.Attach(viewContent);
			viewContentAttached = generator.GetViewContent();
			
			mockMethod = new MockMethod();
			mockMethod.BodyRegion = new DomRegion(1, 1, 3, 1);
			generator.MethodToReturnFromInitializeComponents = mockMethod;
			
			ParseInformation parseInfo = new ParseInformation();
			PythonParser parser = new PythonParser();
			ICompilationUnit parserCompilationUnit = parser.Parse(new DefaultProjectContent(), "Test.py", GetTextEditorCode());
			parseInfo.SetCompilationUnit(parserCompilationUnit);
			generator.ParseInfoToReturnFromParseFileMethod = parseInfo;
			
			generator.MergeFormChanges(unit);
			generator.Detach();
		}
		
		[Test]
		public void SetUpFixtureSucceeded()
		{
			// Check that we found the form class and initialize component
			// method.
			Assert.IsNotNull(formClass);
			Assert.IsNotNull(initializeComponent);			
		}
		
		[Test]
		public void InitializeComponentsMethodLocated()
		{
			Assert.AreSame(initializeComponent, generator.GenerateInitializeComponentsMethod);
		}
		
		[Test]
		public void FormClassLocated()
		{
			Assert.AreSame(formClass, generator.GeneratedFormClass);
		}
		
		[Test]
		public void GetDomRegion()
		{
			MockMethod method = new MockMethod();
			DomRegion bodyRegion = new DomRegion(0, 4, 1, 4);
			method.BodyRegion = bodyRegion;
			DomRegion expectedRegion = new DomRegion(bodyRegion.BeginLine + 1, 1, bodyRegion.EndLine + 1, 1);
			
			Assert.AreEqual(expectedRegion, generator.CallGetBodyRegionInDocument(method));
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
					"\t\tself.ClientSize = System.Drawing.Size(499, 309)\r\n" +
					"\t\tself.Name = 'MainForm'\r\n" +
					"\t\tself.ResumeLayout(false)\r\n"; 						
			Assert.AreEqual(expectedText, viewContent.TextEditorControl.Document.TextContent);
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
				
		string GetPythonCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponent(self):\r\n" +
					"\t\tself.SuspendLayout()\r\n" +
					"\t\tself.ClientSize = System.Drawing.Size(499, 309)\r\n" +
					"\t\tself.Name = 'MainForm'\r\n" +
					"\t\tself.ResumeLayout(false)\r\n"; 						
		}
	}
}
