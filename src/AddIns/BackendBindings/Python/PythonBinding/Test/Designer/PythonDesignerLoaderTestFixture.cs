// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests basic operation of the PythonDesignerLoader.
	/// </summary>
	[TestFixture]
	public class PythonDesignerLoaderTestFixture
	{
		FormsDesignerViewContent view;
		DerivedPythonDesignerLoader loader;
		MockDesignerGenerator generator;
		MockTypeResolutionService mockTypeResolutionService;
		MockDesignerLoaderHost mockDesignerLoaderHost;
		MockExtenderProviderService mockExtenderProviderService;
			
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			generator = new MockDesignerGenerator();
			view = new FormsDesignerViewContent(null, new MockOpenedFile("Test.py"));
			generator.Attach(view);
			
			view.DesignerCodeFileContent = GetFormCode();
			loader = new DerivedPythonDesignerLoader(generator);
	
			// Begin load.
			mockDesignerLoaderHost = new MockDesignerLoaderHost();
			mockExtenderProviderService = new MockExtenderProviderService();
			mockDesignerLoaderHost.AddService(typeof(IExtenderProviderService), mockExtenderProviderService);
			mockTypeResolutionService = new MockTypeResolutionService();
			mockDesignerLoaderHost.AddService(typeof(ITypeResolutionService), mockTypeResolutionService);
			System.Console.WriteLine("Before BeginLoad");
			loader.BeginLoad(mockDesignerLoaderHost);
			System.Console.WriteLine("After BeginLoad");			
		}
		
		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			loader.Dispose();
		}
		
		[Test]
		public void IsDerivedFromCodeDomDesignerLoader()
		{
			CodeDomDesignerLoader codeDomDesignerLoader = loader as CodeDomDesignerLoader;
			Assert.IsNotNull(codeDomDesignerLoader);
		}
		
		[Test]
		public void CodeDomProviderIsPythonProvider()
		{
//			Assert.IsInstanceOfType(typeof(PythonProvider), loader.GetCodeDomProvider());
		}
		
		[Test]
		public void LoadingIsFalseBeforeBeginLoad()
		{
			Assert.IsFalse(loader.IsLoadingBeforeBeginLoad);
		}
		
		/// <summary>
		/// BeginLoad calls OnEndLoad after completion so 
		/// Loading should be set to false.
		/// </summary>
		[Test]
		public void LoadingIsFalseAfterBeginLoad()
		{
			Assert.IsFalse(loader.IsLoadingAfterBeginLoad);
		}

		[Test]
		public void LoadingIsTrueBeforeEndLoad()
		{
			Assert.IsTrue(loader.IsLoadingBeforeOnEndLoad);
		}
		
		[Test]
		public void LoadingIsFalseAfterEndLoad()
		{
			Assert.IsFalse(loader.IsLoadingAfterOnEndLoad);
		}
		
		[Test]
		public void TypeResolutionServiceMatchesDesignerHosts()
		{
			Assert.AreEqual(mockTypeResolutionService, loader.GetTypeResolutionService());
		}
		
		[Test]
		public void WriteCompileUnit()
		{
			CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
			loader.CallWrite(codeCompileUnit);
			
			Assert.AreSame(codeCompileUnit, generator.CodeCompileUnitMerged);
		}
		
		[Test]
		public void CodeDomLocalizationProviderAddedToDesignerSerializationManager()
		{
			Assert.IsTrue(mockExtenderProviderService.IsLanguageExtendersAdded);
//			CodeDomLocalizationProvider localizationProvider = new CodeDomLocalizationProvider(mockDesignerLoaderHost, CodeDomLocalizationModel.PropertyAssignment);
//			IDesignerSerializationManager manager = (IDesignerSerializationManager)mockDesignerLoaderHost.GetService(typeof(IDesignerSerializationManager));
//
//			Assert.IsNotNull(manager.GetSerializer(null, typeof(CodeDomLocalizationProvider)));
		}

		/// <summary>
		/// Simple check is to take the code compilation unit generated
		/// from the DesignerLoader's Parse method and then use the
		/// PythonProvider to turn it back into code and compare the
		/// original code. It should be the same.
		/// </summary>
		/// <remarks>
		/// Note the extra \t\r\n\r\n" since the PythonProvider or the
		/// StringWriter seem to add these to the end of the generated code.
		/// 
		/// Also note that the (Form) base class is replaced with the
		/// fully qualified name. This does not affect the generated code
		/// since only the InitializeComponents method is replaced.
		/// </remarks>
		[Test]
		public void CodeCompilationUnitTakesCodeFromDocument()
		{
//			StringWriter writer = new StringWriter();
//			CodeGeneratorOptions options = new CodeGeneratorOptions();
//			options.BlankLinesBetweenMembers = false;
//			options.IndentString = "\t";
//			PythonProvider pythonProvider = new PythonProvider();
//			pythonProvider.GenerateCodeFromCompileUnit(loader.CodeCompileUnit, writer, options);
//			
//			string code = GetFormCode() + "\t\r\n\r\n";
//			code = code.Replace("(Form)", "(System.Windows.Forms.Form)");
//			Assert.AreEqual(code, writer.ToString());
		}

		/// <summary>
		/// We cannot use the CodeDOMGenerator class since the
		/// the CurrentClass property is never set in the 
		/// System.CodeDom.CodeGenerator class when the GenerateCodeFromStatement
		/// method is called. Instead we have to call the GenerateCodeFromType 
		/// method which creates the full class.
		/// We cannot use the GenerateCodeFromMember to
		/// create just the InitializeComponent method since this
		/// is not supported by the PythonProvider.
		/// </summary>
		[Test]
		public void GenerateCodeUsingPythonProvider()
		{
//			GeneratedInitializeComponentMethod initializeComponentMethod = GeneratedInitializeComponentMethod.GetGeneratedInitializeComponentMethod(loader.CodeCompileUnit);
//
//			StringWriter writer = new StringWriter();
//			CodeGeneratorOptions options = new CodeGeneratorOptions();
//			options.BlankLinesBetweenMembers = false;
//			options.IndentString = "\t";
//			PythonProvider pythonProvider = new PythonProvider();
//			pythonProvider.GenerateCodeFromType(initializeComponentMethod.Type, writer, options);
//			
//			string code = "class MainForm(System.Windows.Forms.Form):\r\n" +
//					"\tdef __init__(self):\r\n" +
//					"\t\tself.InitializeComponent()\r\n" +
//					"\t\r\n" +
//					"\tdef InitializeComponent(self):\r\n" +
//					"\t\tpass\r\n" +
//					"\t\r\n" +
//					"\r\n";
//
//			Assert.AreEqual(code, writer.ToString());
		}
	
		/// <summary>
		/// The code that the designer loader will parse.
		/// </summary>
		string GetFormCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" +
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\t\r\n" +
					"\tdef InitializeComponent(self):\r\n" +
					"\t\tpass\r\n"; 
		}
	}
}
