// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.ComponentModel.Design;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ICSharpCode.FormsDesigner;
using ICSharpCode.FormsDesigner.Services;
using Boo.Lang.Parser;
using Boo.Lang.Compiler.Ast;

namespace Grunwald.BooBinding.Designer
{
	public class BooDesignerLoader : CodeDomDesignerLoader
	{
		bool                  loading               = true;
		IDesignerLoaderHost   designerLoaderHost    = null;
		IDesignerGenerator    generator;
		ITypeResolutionService typeResolutionService = null;
		CodeDomProvider       provider = new Microsoft.CSharp.CSharpCodeProvider();
		
		TextEditorControl textEditorControl;
		
		public string TextContent {
			get {
				return textEditorControl.Document.TextContent;
			}
		}
		
		public override bool Loading {
			get {
				return loading;
			}
		}
		
		public IDesignerLoaderHost DesignerLoaderHost {
			get {
				return designerLoaderHost;
			}
		}
		
		protected override CodeDomProvider CodeDomProvider {
			get {
				return provider;
			}
		}
		
		protected override ITypeResolutionService TypeResolutionService {
			get {
				return typeResolutionService;
			}
		}
		
		protected override bool IsReloadNeeded()
		{
			return base.IsReloadNeeded() || TextContent != lastTextContent;
		}
		
		public BooDesignerLoader(TextEditorControl textEditorControl, IDesignerGenerator generator)
		{
			this.textEditorControl = textEditorControl;
			this.generator = generator;
		}
		
		public override void BeginLoad(IDesignerLoaderHost host)
		{
			this.loading = true;
			typeResolutionService = (ITypeResolutionService)host.GetService(typeof(ITypeResolutionService));
			base.BeginLoad(host);
		}
		
		protected override void OnEndLoad(bool successful, System.Collections.ICollection errors)
		{
			this.loading = false;
			base.OnEndLoad(successful, errors);
		}
		
		string lastTextContent;
		
		protected override CodeCompileUnit Parse()
		{
			LoggingService.Debug("BooDesignerLoader.Parse()");
			try {
				CodeCompileUnit ccu = ParseForm();
				LoggingService.Debug("BooDesignerLoader.Parse() finished");
				return ccu;
			} catch (Boo.Lang.Compiler.CompilerError ex) {
				throw new FormsDesignerLoadException(ex.ToString(true));
			}
		}
		
		CodeCompileUnit ParseForm()
		{
			lastTextContent = TextContent;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(textEditorControl.FileName);
			// ensure that there are no syntax errors in the file:
			Module mainModule = Parse(textEditorControl.FileName, lastTextContent);
			
			IClass formClass;
			List<IClass> parts = NRefactoryDesignerLoader.FindFormClassParts(parseInfo, out formClass);
			
			IMethod initMethod = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(formClass);
			
			if (initMethod == null)
				throw new FormsDesignerLoadException("The InitializeComponent method was not found. Designer cannot be loaded.");
			
			Module module = new Module();
			module.Namespace = new NamespaceDeclaration(formClass.Namespace);
			ClassDefinition cld = new ClassDefinition();
			cld.Name = formClass.Name;
			module.Members.Add(cld);
			if (formClass.BaseClass == null)
				throw new FormsDesignerLoadException("formClass.BaseClass returned null.");
			cld.BaseTypes.Add(new SimpleTypeReference(formClass.BaseClass.FullyQualifiedName));
			
			foreach (IField f in formClass.Fields) {
				if (f.ReturnType.IsDefaultReturnType) {
					Field field = new Field();
					field.Name = f.Name;
					field.Type = new SimpleTypeReference(f.ReturnType.FullyQualifiedName);
					cld.Members.Add(field);
				}
			}
			
			string fileName = initMethod.DeclaringType.CompilationUnit.FileName;
			Module parsedModule;
			if (FileUtility.IsEqualFileName(fileName, textEditorControl.FileName))
				parsedModule = mainModule;
			else
				parsedModule = Parse(fileName, ParserService.GetParseableFileContent(fileName));
			
			// Now find InitializeComponent in parsed module and put it into our new module
			foreach (TypeMember m in parsedModule.Members) {
				TypeDefinition td = m as TypeDefinition;
				if (td == null)
					continue;
				foreach (TypeMember m2 in td.Members) {
					Method method = m2 as Method;
					if (method != null
					    && FormsDesignerSecondaryDisplayBinding.IsInitializeComponentsMethodName(method.Name)
					    && method.Parameters.Count == 0)
					{
						cld.Members.Add(method);
						
						Console.WriteLine(module.ToCodeString());
						
						CodeDomVisitor visitor = new CodeDomVisitor(parseInfo.MostRecentCompilationUnit.ProjectContent);
						module.Accept(visitor);
						
						// output generated CodeDOM to the console :
						ICSharpCode.NRefactory.Parser.CodeDomVerboseOutputGenerator outputGenerator = new ICSharpCode.NRefactory.Parser.CodeDomVerboseOutputGenerator();
						outputGenerator.GenerateCodeFromMember(visitor.OutputCompileUnit.Namespaces[0].Types[0], Console.Out, null);
						provider.GenerateCodeFromCompileUnit(visitor.OutputCompileUnit, Console.Out, null);
						
						return visitor.OutputCompileUnit;
					}
				}
			}
			throw new FormsDesignerLoadException("Could not find InitializeComponent in parsed module.");
		}
		
		Module Parse(string fileName, string fileContent)
		{
			BooParsingStep step = new BooParsingStep();
			
			StringBuilder errors = new StringBuilder();
			Module module = BooParser.ParseModule(4, new CompileUnit(), fileName,
			                                      new StringReader(fileContent),
			                                      delegate(antlr.RecognitionException e) {
			                                      	errors.AppendLine(e.ToString());
			                                      });
			
			if (errors.Length > 0) {
				throw new FormsDesignerLoadException("Syntax errors in " + fileName + ":\r\n" + errors.ToString());
			}
			return module;
		}
		
		protected override void Write(CodeCompileUnit unit)
		{
			LoggingService.Info("BooDesignerLoader.Write called");
			try {
				generator.MergeFormChanges(unit);
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
	}
}
