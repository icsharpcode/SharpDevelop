// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Design;
using System.Windows.Forms;
#endregion

namespace WorkflowDesigner
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
	
	/// <summary>
	/// Description of CodeDomLoader.
	/// </summary>
	public class NRefactoryDesignerLoader : CodeDomDesignerLoader
	{
		TextEditorControl textEditorControl;
		//IDesignerGenerator    generator = new CSharpDesignerGenerator();
		IViewContent viewContent;
		
		#region Constructors
		public NRefactoryDesignerLoader(TextEditorControl textEditorControl, IViewContent viewContent) : base()
		{
			this.textEditorControl = textEditorControl;
			this.viewContent = viewContent;
		}
		#endregion
		
		CodeDomProvider codeDomProvider = Microsoft.CSharp.CSharpCodeProvider.CreateProvider("CSharp");
		protected override CodeDomProvider CodeDomProvider {
			get {
				return codeDomProvider;
			}
		}
		
		protected override ITypeResolutionService TypeResolutionService {
			get {
				return GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
			}
		}
		
		
		protected override void Write(CodeCompileUnit unit)
		{
			LoggingService.Debug("NRefactoryDesignerLoader.Write()");
			throw new NotImplementedException();
		}
		
		protected override void Initialize()
		{
			ITypeProvider typeProvider = TypeProviderService.GetTypeProvider(ProjectService.OpenSolution.FindProjectContainingFile(viewContent.PrimaryFileName));
			try {
				typeProvider.GetType("System.ComponentModel.Design.Serialization.CodeDomSerializer", true);			
			} catch (Exception ex) {
				MessageService.ShowErrorFormatted("Unable to find class CodeDomSerializer, make sure the project has a reference to assembly System.Design");
				throw ex;
			}

			LoaderHost.AddService(typeof(ITypeProvider), typeProvider);
			LoaderHost.AddService(typeof(IIdentifierCreationService), new IdentifierCreationService());
			LoaderHost.AddService(typeof(IMemberCreationService), new MemberCreationService(LoaderHost));
			LoaderHost.AddService(typeof(IEventBindingService), new CSharpWorkflowDesignerEventBindingService(LoaderHost, viewContent.PrimaryFileName));
			LoaderHost.AddService(typeof(IToolboxService), new WorkflowToolboxService(LoaderHost));
			LoaderHost.AddService(typeof(MemberRelationshipService), new DefaultMemberRelationshipService());
			LoaderHost.AddService(typeof(IMenuCommandService), new WorkflowMenuCommandService(LoaderHost));
			LoaderHost.AddService(typeof(ITypeResolutionService), new TypeResolutionService(LoaderHost));
			LoaderHost.AddService(typeof(IPropertyValueUIService), new PropertyValueUIService());
			
			base.Initialize();
		}
		
		
		#region Taken from FormDesigner.NRefactoryDesignerLoad to get a CodeCompileUnit for the activity.
		string lastTextContent;
		protected override CodeCompileUnit Parse()
		{
			LoggingService.Debug("NRefactoryDesignerLoader.Parse()");
			
			lastTextContent = textEditorControl.Document.TextContent;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(textEditorControl.FileName);
			
			IClass formClass;
			bool isFirstClassInFile;
			IList<IClass> parts = FindFormClassParts(parseInfo, out formClass, out isFirstClassInFile);
			
			const string missingReferenceMessage = "Your project is missing a reference to '${Name}' - please add it using 'Project > Add Reference'.";
			
			if (formClass.ProjectContent.GetClass("System.Workflow.Activities.CodeActivity", 0) == null) {
				throw new WorkflowDesignerLoadException(
					StringParser.Parse(
						missingReferenceMessage, new string[,] {{ "Name" , "System.Workflow.Activities"}}
					));
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
				
				string fileContent = ParserService.GetParseableFileContent(fileName);
				
				ICSharpCode.NRefactory.IParser p = ICSharpCode.NRefactory.ParserFactory.CreateParser(SupportedLanguage.CSharp, new StringReader(fileContent));
				p.Parse();
				if (p.Errors.Count > 0) {
					throw new WorkflowDesignerLoadException("Syntax errors in " + fileName + ":\r\n" + p.Errors.ErrorOutput);
				}
				
				// Try to fix the type names to fully qualified ones
				FixTypeNames(p.CompilationUnit, part.CompilationUnit, ref foundInitMethod);
				compilationUnits.Add(new KeyValuePair<string, CompilationUnit>(fileName, p.CompilationUnit));
			}
			
			if (!foundInitMethod)
				throw new WorkflowDesignerLoadException("The InitializeComponent method was not found. Designer cannot be loaded.");
			
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
//			if ((Control.ModifierKeys & Keys.Control) == Keys.Control) {
//				CodeDomVerboseOutputGenerator outputGenerator = new CodeDomVerboseOutputGenerator();
//				outputGenerator.GenerateCodeFromMember(visitor.codeCompileUnit.Namespaces[0].Types[0], Console.Out, null);
//				this.CodeDomProvider.GenerateCodeFromCompileUnit(visitor.codeCompileUnit, Console.Out, null);
//			}
			#endif
			
			LoggingService.Debug("NRefactoryDesignerLoader.Parse() finished");
			
			if (!isFirstClassInFile) {
				MessageService.ShowWarning("The form must be the first class in the file in order for form resources be compiled correctly.\n" +
				                           "Please move other classes below the form class definition or move them to other files.");
			}
			
			return visitor.codeCompileUnit;
		}
		
		public static IList<IClass> FindFormClassParts(ParseInformation parseInfo, out IClass formClass, out bool isFirstClassInFile)
		{
			#if DEBUG
			if ((Control.ModifierKeys & (Keys.Alt | Keys.Control)) == (Keys.Alt | Keys.Control)) {
				System.Diagnostics.Debugger.Break();
			}
			#endif
			
			formClass = null;
			isFirstClassInFile = true;
			foreach (IClass c in parseInfo.BestCompilationUnit.Classes) {
				if (WorkflowDesignerSecondaryDisplayBinding.BaseClassIsWorkflow(c)) {
					formClass = c;
					break;
				}
				isFirstClassInFile = false;
			}
			if (formClass == null)
				throw new WorkflowDesignerLoadException("No class derived from Form or UserControl was found.");
			
			// Initialize designer for formClass
			formClass = formClass.GetCompoundClass();
			if (formClass is CompoundClass) {
				return (formClass as CompoundClass).GetParts();
			} else {
				return new IClass[] { formClass };
			}
		}
		
		/// <summary>
		/// Fix type names and remove unused methods.
		/// </summary>
		public static void FixTypeNames(object o, ICSharpCode.SharpDevelop.Dom.ICompilationUnit domCu, ref bool foundInitMethod)
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
							if (foundInitMethod) {
								throw new WorkflowDesignerLoadException("There are multiple InitializeComponent methods in the class. Designer cannot be loaded.");
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
				FixTypeReference(fieldDecl.TypeReference, fieldDecl.StartLocation, domCu);
				foreach (VariableDeclaration var in fieldDecl.Fields) {
					if (var != null) {
						FixTypeReference(var.TypeReference, fieldDecl.StartLocation, domCu);
					}
				}
			}
		}
		
		public static void FixTypeReference(TypeReference type, Location location, ICSharpCode.SharpDevelop.Dom.ICompilationUnit domCu)
		{
			if (type == null)
				return;
			if (type.SystemType != type.Type)
				return;
			foreach (TypeReference tref in type.GenericTypes) {
				FixTypeReference(tref, location, domCu);
			}
			ICSharpCode.SharpDevelop.Dom.IClass curType = domCu.GetInnermostClass(location.Y, location.X);
			ICSharpCode.SharpDevelop.Dom.IReturnType rt = domCu.ProjectContent.SearchType(new SearchTypeRequest(type.Type, type.GenericTypes.Count, curType, domCu, location.Y, location.X)).Result;
			if (rt != null) {
				type.Type = rt.FullyQualifiedName;
			}
		}
		
		#endregion
	}
}
