// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Parser;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace Grunwald.BooBinding.Designer
{
	public class BooDesignerLoader : AbstractCodeDomDesignerLoader
	{
		protected override bool IsReloadNeeded()
		{
			return base.IsReloadNeeded() || this.Generator.ViewContent.DesignerCodeFileContent != lastTextContent;
		}
		
		public BooDesignerLoader(IDesignerGenerator generator)
			: base(generator)
		{
		}
		
		string lastTextContent;
		Module parsedModule;
		
		protected override CodeCompileUnit Parse()
		{
			LoggingService.Debug("BooDesignerLoader.Parse()");
			try {
				CodeCompileUnit ccu = ParseForm();
				LoggingService.Debug("BooDesignerLoader.Parse() finished");
				// Clear the cached module after loading has finished
				this.parsedModule = null;
				return ccu;
			} catch (Boo.Lang.Compiler.CompilerError ex) {
				throw new FormsDesignerLoadException(ex.ToString(true));
			}
		}
		
		CodeCompileUnit ParseForm()
		{
			ParseInformation parseInfo = ParserService.ParseFile(this.Generator.ViewContent.DesignerCodeFile.FileName, new StringTextBuffer(this.Generator.ViewContent.DesignerCodeFileContent));
			Module module = ParseFormAsModule();
			
			#if DEBUG
			Console.WriteLine(module.ToCodeString());
			#endif
			
			CodeDomVisitor visitor = new CodeDomVisitor(parseInfo.CompilationUnit.ProjectContent);
			module.Accept(visitor);
			
			#if DEBUG
			// output generated CodeDOM to the console :
			ICSharpCode.NRefactory.Visitors.CodeDomVerboseOutputGenerator outputGenerator = new ICSharpCode.NRefactory.Visitors.CodeDomVerboseOutputGenerator();
			outputGenerator.GenerateCodeFromMember(visitor.OutputCompileUnit.Namespaces[0].Types[0], Console.Out, null);
			CodeDomProvider cSharpProvider = new Microsoft.CSharp.CSharpCodeProvider();
			cSharpProvider.GenerateCodeFromCompileUnit(visitor.OutputCompileUnit, Console.Out, null);
			#endif
			
			return visitor.OutputCompileUnit;
		}
		
		Module ParseFormAsModule()
		{
			// The module is cached while loading so that
			// determining the localization model and generating the CodeDOM
			// does not require the code to be parsed twice.
			if (this.parsedModule != null && lastTextContent == this.Generator.ViewContent.DesignerCodeFileContent) {
				return this.parsedModule;
			}
			
			lastTextContent = this.Generator.ViewContent.DesignerCodeFileContent;
			
			ParseInformation parseInfo = ParserService.ParseFile(this.Generator.ViewContent.DesignerCodeFile.FileName, new StringTextBuffer(this.Generator.ViewContent.DesignerCodeFileContent));
			// ensure that there are no syntax errors in the file:
			Module mainModule = Parse(this.Generator.ViewContent.DesignerCodeFile.FileName, lastTextContent);
			
			IClass formClass;
			bool isFirstClassInFile;
			IList<IClass> parts = NRefactoryDesignerLoader.FindFormClassParts(parseInfo, out formClass, out isFirstClassInFile);
			
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
			
			System.Diagnostics.Debug.Assert(FileUtility.IsEqualFileName(initMethod.DeclaringType.CompilationUnit.FileName, this.Generator.ViewContent.DesignerCodeFile.FileName));
			
			foreach (IField f in formClass.Fields) {
				Field field = new Field();
				field.Name = f.Name;
				if (f.ReturnType.IsDefaultReturnType) {
					field.Type = new SimpleTypeReference(f.ReturnType.FullyQualifiedName);
				}
				field.Modifiers = CodeCompletion.ConvertVisitor.ConvertVisibilityBack(f.Modifiers);
				cld.Members.Add(field);
			}
			
			// Now find InitializeComponent in parsed module and put it into our new module
			foreach (TypeMember m in mainModule.Members) {
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
						this.parsedModule = module;
						return module;
					}
				}
			}
			throw new FormsDesignerLoadException("Could not find InitializeComponent in parsed module.");
		}
		
		static Module Parse(string fileName, string fileContent)
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
				this.Generator.MergeFormChanges(unit);
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		protected override CodeDomLocalizationModel GetCurrentLocalizationModelFromDesignedFile()
		{
			Module m = ParseFormAsModule();
			FindLocalizationModelVisitor visitor = new FindLocalizationModelVisitor();
			m.Accept(visitor);
			return visitor.Model;
		}
		
		sealed class FindLocalizationModelVisitor : DepthFirstVisitor
		{
			CodeDomLocalizationModel model = CodeDomLocalizationModel.None;
			
			public CodeDomLocalizationModel Model {
				get { return this.model; }
			}
			
			public override bool EnterMethod(Method node)
			{
				return node.Name == "InitializeComponent" || node.Name == "InitializeComponents";
			}
			
			public override bool EnterBinaryExpression(BinaryExpression node)
			{
				return this.model != CodeDomLocalizationModel.PropertyReflection && node.Operator == BinaryOperatorType.Assign;
			}
			
			public override bool EnterMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (this.model == CodeDomLocalizationModel.PropertyReflection) return false;
				
				MemberReferenceExpression member = node.Target as MemberReferenceExpression;
				if (member != null) {
					ReferenceExpression refex = member.Target as ReferenceExpression;
					if (refex != null && refex.Name.Equals("resources", StringComparison.InvariantCulture)) {
						if (member.Name.Equals("ApplyResources", StringComparison.InvariantCulture)) {
							this.model = CodeDomLocalizationModel.PropertyReflection;
						} else if (member.Name.Equals("GetString", StringComparison.InvariantCulture)) {
							this.model = CodeDomLocalizationModel.PropertyAssignment;
						}
					}
				}
				return false;
			}
		}
	}
}
