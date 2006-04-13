// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.ComponentModel.Design;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.FormsDesigner
{
	public class DefaultMemberRelationshipService : MemberRelationshipService
	{
		public override bool SupportsRelationship(MemberRelationship source, MemberRelationship relationship)
		{
			return true;
		}
		protected override MemberRelationship GetRelationship(MemberRelationship source)
		{
			return base.GetRelationship(source);
		}
	}
	
	public class NRefactoryDesignerLoader : CodeDomDesignerLoader
	{
		bool                  loading               = true;
		IDesignerLoaderHost   designerLoaderHost    = null;
		ITypeResolutionService typeResolutionService = null;
		IDesignerGenerator    generator;
		SupportedLanguage    language;
		
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
				return generator.CodeDomProvider;
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
		
		public NRefactoryDesignerLoader(SupportedLanguage language, TextEditorControl textEditorControl, IDesignerGenerator generator)
		{
			this.language = language;
			this.textEditorControl = textEditorControl;
			this.generator = generator;
		}
		
		public override void BeginLoad(IDesignerLoaderHost host)
		{
			this.loading = true;
			typeResolutionService = (ITypeResolutionService)host.GetService(typeof(ITypeResolutionService));
			this.designerLoaderHost = host;
			base.BeginLoad(host);
		}
		
		protected override void Initialize()
		{
			CodeDomLocalizationProvider localizationProvider = new CodeDomLocalizationProvider(designerLoaderHost, CodeDomLocalizationModel.PropertyAssignment);
			IDesignerSerializationManager manager = (IDesignerSerializationManager)designerLoaderHost.GetService(typeof(IDesignerSerializationManager));
			manager.AddSerializationProvider(localizationProvider);
			base.Initialize();
		}
		
		protected override void OnEndLoad(bool successful, ICollection errors)
		{
			this.loading = false;
			//when control's Dispose() has a exception and on loading also raised exception
			//then this is only place where this error can be logged, because after errors is
			//catched internally in .net
			try {
				base.OnEndLoad(successful, errors);
			} catch(ExceptionCollection e) {
				LoggingService.Error("DesignerLoader.OnEndLoad error" + e.Message);
				foreach(Exception ine in e.Exceptions) {
					LoggingService.Error("DesignerLoader.OnEndLoad error" + ine.Message);
				}
				throw;
			} catch(Exception e) {
				LoggingService.Error("DesignerLoader.OnEndLoad error" + e.Message);
				throw;
			}
		}
		
		string lastTextContent;
		
		public static List<IClass> FindFormClassParts(ParseInformation parseInfo, out IClass formClass)
		{
			#if DEBUG
			if ((Control.ModifierKeys & (Keys.Alt | Keys.Control)) == (Keys.Alt | Keys.Control)) {
				System.Diagnostics.Debugger.Break();
			}
			#endif
			
			formClass = null;
			foreach (IClass c in parseInfo.BestCompilationUnit.Classes) {
				if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(c)) {
					formClass = c;
					break;
				}
			}
			if (formClass == null)
				throw new FormsDesignerLoadException("No class derived from Form or UserControl was found.");
			
			// Initialize designer for formClass
			formClass = formClass.GetCompoundClass();
			List<IClass> parts;
			if (formClass is CompoundClass) {
				parts = (formClass as CompoundClass).Parts;
			} else {
				parts = new List<IClass>();
				parts.Add(formClass);
			}
			return parts;
		}
		
		protected override CodeCompileUnit Parse()
		{
			LoggingService.Debug("NRefactoryDesignerLoader.Parse()");
			
			lastTextContent = TextContent;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(textEditorControl.FileName);
			
			IClass formClass;
			List<IClass> parts = FindFormClassParts(parseInfo, out formClass);
			
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
				
				string fileContent = ParserService.GetParseableFileContent(fileName);
				
				ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(language, new StringReader(fileContent));
				p.Parse();
				if (p.Errors.count > 0) {
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
			TypeDeclaration formDecl = new TypeDeclaration(Modifier.Public, null);
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
			
			CodeDOMVisitor visitor = new CodeDOMVisitor();
			visitor.EnvironmentInformationProvider = new ICSharpCode.SharpDevelop.Dom.NRefactoryResolver.NRefactoryInformationProvider(formClass.ProjectContent, formClass);
			visitor.Visit(combinedCu, null);
			
			// output generated CodeDOM to the console :
			#if DEBUG
			if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
				CodeDOMVerboseOutputGenerator outputGenerator = new CodeDOMVerboseOutputGenerator();
				outputGenerator.GenerateCodeFromMember(visitor.codeCompileUnit.Namespaces[0].Types[0], Console.Out, null);
				this.CodeDomProvider.GenerateCodeFromCompileUnit(visitor.codeCompileUnit, Console.Out, null);
			}
			#endif
			
			LoggingService.Debug("NRefactoryDesignerLoader.Parse() finished");
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
				foreach (TypeReference tref in typeDecl.BaseTypes) {
					FixTypeReference(tref, typeDecl.StartLocation, domCu);
				}
				for (int i = 0; i < typeDecl.Children.Count; i++) {
					object child = typeDecl.Children[i];
					MethodDeclaration method = child as MethodDeclaration;
					if (method != null) {
						// remove all methods except InitializeComponents
						if ((method.Name == "InitializeComponents" || method.Name == "InitializeComponent") && method.Parameters.Count == 0) {
							method.Name = "InitializeComponent";
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
				FixTypeReference(fieldDecl.TypeReference, fieldDecl.StartLocation, domCu);
				foreach (VariableDeclaration var in fieldDecl.Fields) {
					if (var != null) {
						FixTypeReference(var.TypeReference, fieldDecl.StartLocation, domCu);
					}
				}
			}
		}
		
		void FixTypeReference(TypeReference type, Point location, ICSharpCode.SharpDevelop.Dom.ICompilationUnit domCu)
		{
			if (type == null)
				return;
			if (type.SystemType != type.Type)
				return;
			foreach (TypeReference tref in type.GenericTypes) {
				FixTypeReference(tref, location, domCu);
			}
			ICSharpCode.SharpDevelop.Dom.IClass curType = domCu.GetInnermostClass(location.Y, location.X);
			ICSharpCode.SharpDevelop.Dom.IReturnType rt = domCu.ProjectContent.SearchType(type.Type, type.GenericTypes.Count, curType, domCu, location.Y, location.X);
			if (rt != null) {
				type.Type = rt.FullyQualifiedName;
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
			generator.MergeFormChanges(unit);
		}
		
//		public void Reload()
//		{
//			base.Reload(BasicDesignerLoader.ReloadFlags.Default);
//		}
//		public override void Flush()
//		{
//			base.Flush();
//		}
		
//		void InitializeExtendersForProject(IDesignerHost host)
//		{
//			IExtenderProviderService elsi = (IExtenderProviderService)host.GetService(typeof(IExtenderProviderService));
//			elsi.AddExtenderProvider(new ICSharpCode.FormDesigner.Util.NameExtender());
//		}
		
		public override void Dispose()
		{
			base.Dispose();
		}
	}
}
