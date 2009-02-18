// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
#endregion

namespace WorkflowDesigner.Loaders
{
	
	/// <summary>
	/// Description of CodeDesignerLoader.
	/// </summary>
	public class CodeDesignerLoader : BasicWorkflowDesignerLoader
	{
		public CodeDesignerLoader(IViewContent viewContent) : base(viewContent)
		{
		}
		
		protected override void Initialize()
		{
			base.Initialize();

			LoaderHost.AddService(typeof(IMemberCreationService), new MemberCreationService(LoaderHost));
			LoaderHost.AddService(typeof(CodeDomProvider), Project.LanguageProperties.CodeDomProvider);
			LoaderHost.AddService(typeof(IEventBindingService), new CSharpWorkflowDesignerEventBindingService(LoaderHost, ViewContent.PrimaryFileName));
		}

		protected override void DoPerformLoad(IDesignerSerializationManager serializationManager)
		{

			// Step 1, Get the CodeDom
			CodeCompileUnit ccu = Parse();
			
			// Step 2, Find the first class
			CodeTypeDeclaration codeTypeDeclaration = ccu.Namespaces[0].Types[0];
			
			TypeResolutionService typeResolutionService = GetService(typeof(ITypeResolutionService)) as TypeResolutionService;
			Type baseType = typeResolutionService.GetType(codeTypeDeclaration.BaseTypes[0].BaseType);
			if (baseType == null)
				throw new WorkflowDesignerLoadException("Unable to resolve type " + codeTypeDeclaration.BaseTypes[0].BaseType);
			
			// Step 3, Deserialize it!
			TypeCodeDomSerializer serializer = serializationManager.GetSerializer(baseType, typeof(TypeCodeDomSerializer)) as TypeCodeDomSerializer;

			#if DEBUG
			Project.LanguageProperties.CodeDomProvider.GenerateCodeFromType(codeTypeDeclaration, Console.Out, null);
			#endif


			// Step 4, load up the designer.
			Activity rootActivity = serializer.Deserialize(serializationManager, codeTypeDeclaration) as Activity;
			
			// FIXME: This is a workaraound as the deserializer does not appear to add the 
			// components to the container with the activity.name so
			// the designer complains the name is already used when the 
			// name of an activity is the same as a field name in the workflow!
			// When I work out how the IMemberCreationService fits into
			// the scheme of things this will probably go away!
			// (Worth noting CodeDomDesignerLoader has the same problem!)
			//int i = 0;
			foreach(IComponent c in LoaderHost.Container.Components) {
				if (c is Activity) {
					LoaderHost.Container.Remove(c);
					LoaderHost.Container.Add(c, ((Activity)c).Name);
				}
			}
			
			SetBaseComponentClassName(ccu.Namespaces[0].Name + "." + codeTypeDeclaration.Name);
		}

		protected override void DoPerformFlush(IDesignerSerializationManager serializationManager)
		{
			// TODO: Update the InitializeComponent() method here.
		}
		
		
		#region Taken from FormDesigner.NRefactoryDesignerLoad to get a single CodeCompileUnit for the activity.
		protected CodeCompileUnit Parse()
		{
			ParseInformation parseInfo = ParserService.GetParseInformation(FileName);
			
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
			
			if (!isFirstClassInFile) {
				MessageService.ShowWarning("The form must be the first class in the file in order for form resources be compiled correctly.\n" +
				                           "Please move other classes below the form class definition or move them to other files.");
			}
			
			return visitor.codeCompileUnit;
		}
		
		public static IList<IClass> FindFormClassParts(ParseInformation parseInfo, out IClass formClass, out bool isFirstClassInFile)
		{
			
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
				return (formClass as CompoundClass).Parts;
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
			if (type.IsKeyword)
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
