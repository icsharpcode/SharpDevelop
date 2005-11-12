// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.CodeDom;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.FormDesigner
{
	public abstract class AbstractDesignerGenerator : IDesignerGenerator
	{
		IClass  c;
		IMethod initializeComponents;
		
		FormDesignerViewContent viewContent;
		bool failedDesignerInitialize = false;
		
		CodeDomProvider provider;
		
		public const string NonVisualComponentContainerName = "components";
		
		public CodeDomProvider CodeDomProvider {
			get {
				if (this.provider == null) {
					this.provider = this.CreateCodeProvider();
				}
				return this.provider;
			}
		}
		
		public void Attach(FormDesignerViewContent viewContent)
		{
			this.viewContent = viewContent;
			IComponentChangeService componentChangeService = (IComponentChangeService)viewContent.DesignSurface.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentAdded    += new ComponentEventHandler(ComponentAdded);
			componentChangeService.ComponentRename   += new ComponentRenameEventHandler(ComponentRenamed);
			componentChangeService.ComponentRemoving += new ComponentEventHandler(ComponentRemoved);
		}
		
		public void Detach()
		{
			IComponentChangeService componentChangeService = (IComponentChangeService)viewContent.DesignSurface.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentAdded    -= new ComponentEventHandler(ComponentAdded);
			componentChangeService.ComponentRename   -= new ComponentRenameEventHandler(ComponentRenamed);
			componentChangeService.ComponentRemoving -= new ComponentEventHandler(ComponentRemoved);
			this.viewContent = null;
		}
		
		void ComponentRemoved(object sender, ComponentEventArgs e)
		{
			try {
				Reparse();
				foreach (IField field in c.Fields) {
					if (field.Name == e.Component.Site.Name) {
						int startOffset = document.PositionToOffset(new Point(0, field.Region.BeginLine - 1));
						int endOffset   = document.PositionToOffset(new Point(0, field.Region.EndLine));
						document.Remove(startOffset, endOffset - startOffset);
					}
				}
				SaveDocument();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		protected abstract string GenerateFieldDeclaration(Type fieldType, string name);
		
		/// <summary>
		/// Contains the tabs in front of the InitializeComponents declaration.
		/// Used to indent the fields and generated statements.
		/// </summary>
		protected string tabs;
		
		void ComponentAdded(object sender, ComponentEventArgs e)
		{
			try {
				Reparse();
				int endOffset = document.PositionToOffset(new Point(0, initializeComponents.BodyRegion.EndLine));
				document.Insert(endOffset, tabs + GenerateFieldDeclaration(e.Component.GetType(), e.Component.Site.Name) + Environment.NewLine);
				if (CodeDOMGenerator.IsNonVisualComponent(viewContent.Host, e.Component)) {
					if (!IsNonVisualComponentContainerDefined) {
						document.Insert(endOffset, tabs + GenerateFieldDeclaration(typeof(Container), NonVisualComponentContainerName) + Environment.NewLine);
					}
				}
				SaveDocument();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		void ComponentRenamed(object sender, ComponentRenameEventArgs e)
		{
			try {
				Reparse();
				foreach (IField field in c.Fields) {
					if (field.Name == e.OldName) {
						int startOffset = document.PositionToOffset(new Point(0, field.Region.BeginLine - 1));
						int endOffset   = document.PositionToOffset(new Point(0, field.Region.EndLine));
						document.Replace(startOffset, endOffset - startOffset, tabs + GenerateFieldDeclaration(e.Component.GetType(), e.NewName) + Environment.NewLine);
					}
				}
				SaveDocument();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		protected abstract System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider();
		
		protected abstract DomRegion GetReplaceRegion(ICSharpCode.TextEditor.Document.IDocument document, IMethod method);
		
		public void MergeFormChanges(CodeCompileUnit unit)
		{
			Reparse();
			
			// find InitializeComponent method and the class it is declared in
			CodeTypeDeclaration formClass = null;
			CodeMemberMethod initializeComponent = null;
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
			
			if (formClass == null || initializeComponent == null) {
				throw new InvalidOperationException("InitializeComponent method not found in framework-generated CodeDom.");
			}
			
			// generate file and get initialize components string
			StringWriter writer = new StringWriter();
			CodeDOMGenerator domGenerator = new CodeDOMGenerator(this.CodeDomProvider, tabs + '\t');
			domGenerator.ConvertContentDefinition(initializeComponent, writer);
			
			string statements = writer.ToString();
			
			// initializeComponents.BodyRegion.BeginLine + 1
			DomRegion bodyRegion = GetReplaceRegion(document, initializeComponents);
			if (bodyRegion.BeginColumn <= 0 || bodyRegion.EndColumn <= 0)
				throw new InvalidOperationException("Column must be > 0");
			int startOffset = document.PositionToOffset(new Point(bodyRegion.BeginColumn - 1, bodyRegion.BeginLine - 1));
			int endOffset   = document.PositionToOffset(new Point(bodyRegion.EndColumn - 1, bodyRegion.EndLine - 1));
			
			document.Replace(startOffset, endOffset - startOffset, statements);
			SaveDocument();
		}
		
		IDocument document;
		string saveDocumentToFile; // only set when InitializeComponent was loaded from code-behind file that was not opened
		
		void SaveDocument()
		{
			if (saveDocumentToFile != null) {
				NamedFileOperationDelegate method = delegate(string fileName) {
					using (StreamWriter writer = new StreamWriter(fileName, false, System.Text.Encoding.UTF8)) {
						writer.Write(document.TextContent);
					}
				};
				FileUtility.ObservedSave(method, saveDocumentToFile, FileErrorPolicy.Inform);
			}
		}
		
		protected void Reparse()
		{
			saveDocumentToFile = null;
			
			// get new initialize components
			string content = viewContent.Document.TextContent;
			ParseInformation info = ParserService.ParseFile(viewContent.TextEditorControl.FileName, content, false, true);
			ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
			foreach (IClass c in cu.Classes) {
				if (FormDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(c)) {
					initializeComponents = FormDesignerSecondaryDisplayBinding.GetInitializeComponents(c);
					if (initializeComponents != null) {
						string designerFile = initializeComponents.DeclaringType.CompilationUnit.FileName;
						string designerContent;
						if (FileUtility.IsEqualFileName(viewContent.TextEditorControl.FileName, designerFile)) {
							designerContent = content;
							document = viewContent.Document;
						} else {
							IWorkbenchWindow window = FileService.GetOpenFile(designerFile);
							if (window == null) {
								document = new DocumentFactory().CreateDocument();
								designerContent = ParserService.GetParseableFileContent(designerFile);
								document.TextContent = designerContent;
								saveDocumentToFile = designerFile;
							} else {
								ITextEditorControlProvider tecp = window.ViewContent as ITextEditorControlProvider;
								if (tecp == null)
									throw new ApplicationException("designer file viewcontent must implement ITextEditorControlProvider");
								document = tecp.TextEditorControl.Document;
								designerContent = document.TextContent;
							}
							ParserService.ParseFile(designerFile, designerContent, false, true);
						}
						using (StringReader r = new StringReader(designerContent)) {
							int count = initializeComponents.Region.BeginLine;
							for (int i = 1; i < count; i++)
								r.ReadLine();
							string line = r.ReadLine();
							tabs = line.Substring(0, line.Length - line.TrimStart().Length);
						}
						this.c = c;
						break;
					}
				}
			}
		}
		
		protected abstract string CreateEventHandler(EventDescriptor edesc, string eventMethodName, string body, string indentation);
		
		/// <summary>
		/// If found return true and int as position
		/// </summary>
		/// <param name="component"></param>
		/// <param name="edesc"></param>
		/// <returns></returns>
		public bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out int position)
		{
			if (this.failedDesignerInitialize) {
				position = 0;
				return false;
			}

			Reparse();
			
			foreach (IMethod method in c.Methods) {
				if (method.Name == eventMethodName) {
					position = method.Region.BeginLine + 1;
					return true;
				}
			}
			viewContent.MergeFormChanges();
			Reparse();
			
			position = c.Region.EndLine + 1;
			
			int offset = viewContent.Document.GetLineSegment(GetEventHandlerInsertionLine(c) - 1).Offset;
			
			viewContent.Document.Insert(offset, CreateEventHandler(edesc, eventMethodName, body, tabs));
			
			return false;
		}
		
		protected virtual int GetEventHandlerInsertionLine(IClass c)
		{
			return c.Region.EndLine;
		}
		
		public ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			Reparse();
			ArrayList compatibleMethods = new ArrayList();
			MethodInfo methodInfo = edesc.EventType.GetMethod("Invoke");
			c = c.DefaultReturnType.GetUnderlyingClass();
			foreach (IMethod method in c.Methods) {
				if (method.Parameters.Count == methodInfo.GetParameters().Length) {
					bool found = true;
					for (int i = 0; i < methodInfo.GetParameters().Length; ++i) {
						ParameterInfo pInfo = methodInfo.GetParameters()[i];
						IParameter p = method.Parameters[i];
						if (p.ReturnType.FullyQualifiedName != pInfo.ParameterType.ToString()) {
							found = false;
							break;
						}
					}
					if (found) {
						compatibleMethods.Add(method.Name);
					}
				}
			}
			
			return compatibleMethods;
		}
		
		public ICollection GetCompatibleMethods(EventInfo edesc)
		{
			Reparse();
			ArrayList compatibleMethods = new ArrayList();
			MethodInfo methodInfo = edesc.GetAddMethod();
			ParameterInfo pInfo = methodInfo.GetParameters()[0];
			string eventName = pInfo.ParameterType.ToString().Replace("EventHandler", "EventArgs");
			
			c = c.DefaultReturnType.GetUnderlyingClass();
			foreach (IMethod method in c.Methods) {
				if (method.Parameters.Count == 2) {
					bool found = true;
					
					IParameter p = method.Parameters[1];
					if (p.ReturnType.FullyQualifiedName != eventName) {
						found = false;
					}
					if (found) {
						compatibleMethods.Add(method.Name);
					}
				}
			}
			return compatibleMethods;
		}
		
		bool IsNonVisualComponentContainerDefined
		{
			get {
				return GetField(c, NonVisualComponentContainerName) != null;
			}
		}
		
		IField GetField(IClass c, string name)
		{
			foreach (IField field in c.Fields) {
				if (field.Name == name) {
					return field;
				}
			}
			return null;
		}
	}
}
