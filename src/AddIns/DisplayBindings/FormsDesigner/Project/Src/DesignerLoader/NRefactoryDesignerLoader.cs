// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ReturnTypeOptions = ICSharpCode.SharpDevelop.Dom.NRefactoryResolver.TypeVisitor.ReturnTypeOptions;

namespace ICSharpCode.FormsDesigner
{
	public class NRefactoryDesignerLoader : AbstractCodeDomDesignerLoader
	{
		SupportedLanguage    language;
		
		protected override bool IsReloadNeeded()
		{
			return base.IsReloadNeeded() || this.Generator.ViewContent.DesignerCodeFileContent != lastTextContent;
		}
		
		public NRefactoryDesignerLoader(SupportedLanguage language, IDesignerGenerator generator)
			: base(generator)
		{
			this.language = language;
		}
		
		string lastTextContent;
		
		public static IList<IClass> FindFormClassParts(ParseInformation parseInfo, out IClass formClass, out bool isFirstClassInFile)
		{
			#if DEBUG
			if ((Control.ModifierKeys & (Keys.Alt | Keys.Control)) == (Keys.Alt | Keys.Control)) {
				System.Diagnostics.Debugger.Break();
			}
			#endif
			
			formClass = null;
			isFirstClassInFile = true;
			foreach (IClass c in parseInfo.CompilationUnit.Classes) {
				if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(c)) {
					formClass = c;
					break;
				}
				isFirstClassInFile = false;
			}
			if (formClass == null)
				throw new FormsDesignerLoadException("No class derived from Form or UserControl was found.");
			
			// Initialize designer for formClass
			formClass = formClass.GetCompoundClass();
			if (formClass is CompoundClass) {
				return (formClass as CompoundClass).Parts;
			} else {
				return new IClass[] { formClass };
			}
		}
		
		// Steps to load the designer:
		// - Parse main file
		// - Find other files containing parts of the form
		// - Parse all files and look for fields (for controls) and InitializeComponents method
		// - Create CodeDom objects for fields and InitializeComponents statements
		// - If debug build and Ctrl pressed, output CodeDom to console
		// - Return CodeDom objects to the .NET designer
		protected override CodeCompileUnit Parse()
		{
			LoggingService.Debug("NRefactoryDesignerLoader.Parse()");
			
			lastTextContent = this.Generator.ViewContent.DesignerCodeFileContent;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(this.Generator.ViewContent.DesignerCodeFile.FileName);
			
			IClass formClass;
			bool isFirstClassInFile;
			IList<IClass> parts = FindFormClassParts(parseInfo, out formClass, out isFirstClassInFile);
			
			const string missingReferenceMessage = "Your project is missing a reference to '${Name}' - please add it using 'Project > Add Reference'.";
			
			if (formClass.ProjectContent.GetClass("System.Drawing.Point", 0) == null) {
				throw new FormsDesignerLoadException(StringParser.Parse(missingReferenceMessage, new StringTagPair("Name", "System.Drawing")));
			}
			if (formClass.ProjectContent.GetClass("System.Windows.Forms.Form", 0) == null) {
				throw new FormsDesignerLoadException(StringParser.Parse(missingReferenceMessage, new StringTagPair("Name" , "System.Windows.Forms")));
			}
			
			List<KeyValuePair<string, CompilationUnit>> compilationUnits = new List<KeyValuePair<string, CompilationUnit>>();
			bool foundInitMethod = false;
			foreach (IClass part in parts) {
				string fileName = part.CompilationUnit.FileName;
				if (fileName == null) continue;
				bool found = false;
				foreach (KeyValuePair<string, CompilationUnit> entry in compilationUnits) {
					if (FileUtility.IsEqualFileName(fileName, entry.Key)) {
						found = true;
						break;
					}
				}
				if (found) continue;
				
				ITextBuffer fileContent;
				if (FileUtility.IsEqualFileName(fileName, this.Generator.ViewContent.PrimaryFileName)) {
					fileContent = this.Generator.ViewContent.PrimaryFileContent;
				} else if (FileUtility.IsEqualFileName(fileName, this.Generator.ViewContent.DesignerCodeFile.FileName)) {
					fileContent = new StringTextBuffer(this.Generator.ViewContent.DesignerCodeFileContent);
				} else {
					fileContent = ParserService.GetParseableFileContent(fileName);
				}
				
				ICSharpCode.NRefactory.IParser p = ICSharpCode.NRefactory.ParserFactory.CreateParser(language, fileContent.CreateReader());
				p.Parse();
				if (p.Errors.Count > 0) {
					throw new FormsDesignerLoadException("Syntax errors in " + fileName + ":\r\n" + p.Errors.ErrorOutput);
				}
				
				// Try to fix the type names to fully qualified ones
				FixTypeNames(p.CompilationUnit, part.CompilationUnit, ref foundInitMethod);
				compilationUnits.Add(new KeyValuePair<string, CompilationUnit>(fileName, p.CompilationUnit));
			}
			
			if (!foundInitMethod)
				throw new FormsDesignerLoadException("The InitializeComponent method was not found. Designer cannot be loaded.");
			
			CompilationUnit combinedCu = new CompilationUnit();
			NamespaceDeclaration nsDecl = new NamespaceDeclaration(formClass.Namespace);
			combinedCu.AddChild(nsDecl);
			TypeDeclaration formDecl = new TypeDeclaration(Modifiers.Public, null);
			nsDecl.AddChild(formDecl);
			formDecl.Name = formClass.Name;
			foreach (KeyValuePair<string, CompilationUnit> entry in compilationUnits) {
				foreach (object o in entry.Value.Children) {
					TypeDeclaration td = o as TypeDeclaration;
					if (td != null && td.Name == formDecl.Name) {
						foreach (INode node in td.Children)
							formDecl.AddChild(node);
						formDecl.BaseTypes.AddRange(td.BaseTypes);
					}
					if (o is NamespaceDeclaration) {
						foreach (object o2 in ((NamespaceDeclaration)o).Children) {
							td = o2 as TypeDeclaration;
							if (td != null && td.Name == formDecl.Name) {
								foreach (INode node in td.Children)
									formDecl.AddChild(node);
								formDecl.BaseTypes.AddRange(td.BaseTypes);
							}
						}
					}
				}
			}
			
			CodeDomVisitor visitor = new CodeDomVisitor();
			visitor.EnvironmentInformationProvider = new ICSharpCode.SharpDevelop.Dom.NRefactoryResolver.NRefactoryInformationProvider(formClass.ProjectContent);
			visitor.VisitCompilationUnit(combinedCu, null);
			
			// output generated CodeDOM to the console :
			#if DEBUG
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
				CodeDomVerboseOutputGenerator outputGenerator = new CodeDomVerboseOutputGenerator();
				outputGenerator.GenerateCodeFromMember(visitor.codeCompileUnit.Namespaces[0].Types[0], Console.Out, null);
				this.CodeDomProvider.GenerateCodeFromCompileUnit(visitor.codeCompileUnit, Console.Out, null);
			}
			#endif
			
			LoggingService.Debug("NRefactoryDesignerLoader.Parse() finished");
			
			if (!isFirstClassInFile) {
				MessageService.ShowWarning("The form must be the first class in the file in order for form resources be compiled correctly.\n" +
				                           "Please move other classes below the form class definition or move them to other files.");
			}
			
			return visitor.codeCompileUnit;
		}
		
		
		/// <summary>
		/// Fix type names and remove unused methods.
		/// </summary>
		void FixTypeNames(object o, ICSharpCode.SharpDevelop.Dom.ICompilationUnit domCu, ref bool foundInitMethod)
		{
			if (domCu == null)
				return;
			CompilationUnit cu = o as CompilationUnit;
			if (cu != null) {
				foreach (object c in cu.Children) {
					FixTypeNames(c, domCu, ref foundInitMethod);
				}
				return;
			}
			NamespaceDeclaration namespaceDecl = o as NamespaceDeclaration;
			if (namespaceDecl != null) {
				foreach (object c in namespaceDecl.Children) {
					FixTypeNames(c, domCu, ref foundInitMethod);
				}
				return;
			}
			TypeDeclaration typeDecl = o as TypeDeclaration;
			if (typeDecl != null) {
				for (int i = 0; i < typeDecl.BaseTypes.Count; i++) {
					typeDecl.BaseTypes[i] = FixTypeReference(typeDecl.BaseTypes[i], typeDecl.StartLocation, domCu, ReturnTypeOptions.BaseTypeReference);
				}
				for (int i = 0; i < typeDecl.Children.Count; i++) {
					object child = typeDecl.Children[i];
					MethodDeclaration method = child as MethodDeclaration;
					if (method != null) {
						// remove all methods except InitializeComponents
						if ((method.Name == "InitializeComponents" || method.Name == "InitializeComponent") && method.Parameters.Count == 0) {
							method.Name = "InitializeComponent";
							if (foundInitMethod) {
								throw new FormsDesignerLoadException("There are multiple InitializeComponent methods in the class. Designer cannot be loaded.");
							}
							foundInitMethod = true;
						} else {
							typeDecl.Children.RemoveAt(i--);
						}
					} else if (child is TypeDeclaration || child is FieldDeclaration) {
						FixTypeNames(child, domCu, ref foundInitMethod);
					} else {
						// child is property, event etc.
						typeDecl.Children.RemoveAt(i--);
					}
				}
				
				return;
			}
			FieldDeclaration fieldDecl = o as FieldDeclaration;
			if (fieldDecl != null) {
				fieldDecl.TypeReference = FixTypeReference(fieldDecl.TypeReference, fieldDecl.StartLocation, domCu, ReturnTypeOptions.None);
				foreach (VariableDeclaration var in fieldDecl.Fields) {
					if (var != null) {
						var.TypeReference = FixTypeReference(var.TypeReference, fieldDecl.StartLocation, domCu, ReturnTypeOptions.None);
					}
				}
			}
		}
		
		TypeReference FixTypeReference(TypeReference type, Location location, ICompilationUnit domCu, ReturnTypeOptions options)
		{
			if (type == null || type.IsKeyword)
				return type;
			ICSharpCode.SharpDevelop.Dom.IClass curType = domCu.GetInnermostClass(location.Y, location.X);
			IReturnType rt = SharpDevelop.Dom.NRefactoryResolver.TypeVisitor.CreateReturnType(
				type, curType, null, location.Y, location.X, domCu.ProjectContent, options);
			if (rt != null && rt.GetUnderlyingClass() != null) {
				return SharpDevelop.Dom.Refactoring.CodeGenerator.ConvertType(rt, null);
			} else {
				return type;
			}
		}
		
		protected override void Write(CodeCompileUnit unit)
		{
			LoggingService.Info("DesignerLoader.Write called");
			// output generated CodeDOM to the console :
			#if DEBUG
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
				this.CodeDomProvider.GenerateCodeFromCompileUnit(unit, Console.Out, null);
			}
			#endif
			try {
				this.Generator.MergeFormChanges(unit);
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		protected override CodeDomLocalizationModel GetCurrentLocalizationModelFromDesignedFile()
		{
			ParseInformation parseInfo = ParserService.ParseFile(this.Generator.ViewContent.DesignerCodeFile.FileName, new StringTextBuffer(this.Generator.ViewContent.DesignerCodeFileContent));
			
			IClass formClass;
			bool isFirstClassInFile;
			IList<IClass> parts = FindFormClassParts(parseInfo, out formClass, out isFirstClassInFile);
			
			foreach (string fileName in
			         parts.Select(p => p.CompilationUnit.FileName)
			         .Where(n => n != null)
			         .Distinct(StringComparer.OrdinalIgnoreCase)) {
				
				ICSharpCode.NRefactory.IParser p = ICSharpCode.NRefactory.ParserFactory.CreateParser(language, ParserService.GetParseableFileContent(fileName).CreateReader());
				p.Parse();
				if (p.Errors.Count > 0) {
					throw new FormsDesignerLoadException("Syntax errors in " + fileName +  ":\r\n" + p.Errors.ErrorOutput);
				}
				
				FindLocalizationModelVisitor visitor = new FindLocalizationModelVisitor();
				p.CompilationUnit.AcceptVisitor(visitor, null);
				if (visitor.Model != CodeDomLocalizationModel.None) {
					return visitor.Model;
				}
				
			}
			
			return CodeDomLocalizationModel.None;
		}
		
		sealed class FindLocalizationModelVisitor : AbstractAstVisitor
		{
			bool inInitMethod;
			CodeDomLocalizationModel model = CodeDomLocalizationModel.None;
			
			public CodeDomLocalizationModel Model {
				get { return this.model; }
			}
			
			public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
			{
				if (methodDeclaration.Name == "InitializeComponents" || methodDeclaration.Name == "InitializeComponent") {
					this.inInitMethod = true;
					try {
						return base.VisitMethodDeclaration(methodDeclaration, data);
					} finally {
						this.inInitMethod = false;
					}
				}
				return null;
			}
			
			public override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data)
			{
				if (this.model != CodeDomLocalizationModel.PropertyReflection && this.inInitMethod) {
					IdentifierExpression iex = memberReferenceExpression.TargetObject as IdentifierExpression;
					if (iex != null && iex.Identifier == "resources") {
						if (memberReferenceExpression.MemberName == "ApplyResources") {
							this.model = CodeDomLocalizationModel.PropertyReflection;
						} else if (memberReferenceExpression.MemberName == "GetString") {
							this.model = CodeDomLocalizationModel.PropertyAssignment;
						}
					}
				}
				return null;
			}
		}
	}
}
