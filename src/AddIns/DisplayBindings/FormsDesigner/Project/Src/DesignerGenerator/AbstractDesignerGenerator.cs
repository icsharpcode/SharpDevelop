// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ReflectionLayer = ICSharpCode.SharpDevelop.Dom.ReflectionLayer;

namespace ICSharpCode.FormsDesigner
{
	public abstract class AbstractDesignerGenerator : IDesignerGenerator
	{
		/// <summary>The currently open part of the class being designed.</summary>
		IClass  c;
		/// <summary>The complete class being designed.</summary>
		IClass  completeClass;
		/// <summary>The class part containing the designer code.</summary>
		IClass  formClass;
		IMethod initializeComponents;
		
		FormsDesignerViewContent viewContent;
		bool failedDesignerInitialize = false;
		CodeDomProvider provider;
		
		public CodeDomProvider CodeDomProvider {
			get {
				if (this.provider == null) {
					this.provider = this.CreateCodeProvider();
				}
				return this.provider;
			}
		}
		
		public FormsDesignerViewContent ViewContent {
			get {
				return viewContent;
			}
		}
		
		public void Attach(FormsDesignerViewContent viewContent)
		{
			this.viewContent = viewContent;
		}
		
		public void Detach()
		{
			this.viewContent = null;
		}
		
		/// <summary>
		/// Removes the field declaration with the specified name from the source file.
		/// </summary>
		void RemoveField(string fieldName)
		{
			try {
				LoggingService.Info("Remove field declaration: "+fieldName);
				Reparse();
				IField field = GetField(formClass, fieldName);
				if (field != null) {
					int startOffset = document.PositionToOffset(new TextLocation(0, field.Region.BeginLine - 1));
					int endOffset   = document.PositionToOffset(new TextLocation(0, field.Region.EndLine));
					document.Remove(startOffset, endOffset - startOffset);
				} else if ((field = GetField(completeClass, fieldName)) != null) {
					// TODO: Remove the field in the part where it is declared
					LoggingService.Warn("Removing field declaration in non-designer part currently not supported");
				}
				SaveDocument();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		protected virtual string GenerateFieldDeclaration(CodeDOMGenerator domGenerator, CodeMemberField field)
		{
			StringWriter writer = new StringWriter();
			domGenerator.ConvertContentDefinition(field, writer);
			return writer.ToString().Trim();
		}
		
		/// <summary>
		/// Contains the tabs in front of the InitializeComponents declaration.
		/// Used to indent the fields and generated statements.
		/// </summary>
		protected string tabs;
		
		/// <summary>
		/// Adds the declaration for the specified field to the source file
		/// or replaces the already present declaration for a field with the same name.
		/// </summary>
		/// <param name="domGenerator">The CodeDOMGenerator used to generate the field declaration.</param>
		/// <param name="newField">The CodeDom field to be added or replaced.</param>
		void AddOrReplaceField(CodeDOMGenerator domGenerator, CodeMemberField newField)
		{
			try {
				Reparse();
				IField oldField = GetField(formClass, newField.Name);
				if (oldField != null) {
					int startOffset = document.PositionToOffset(new TextLocation(0, oldField.Region.BeginLine - 1));
					int endOffset   = document.PositionToOffset(new TextLocation(0, oldField.Region.EndLine));
					document.Replace(startOffset, endOffset - startOffset, tabs + GenerateFieldDeclaration(domGenerator, newField) + Environment.NewLine);
				} else {
					if ((oldField = GetField(completeClass, newField.Name)) != null) {
						// TODO: Replace the field in the part where it is declared
						LoggingService.Warn("Field declaration replacement in non-designer part currently not supported");
					} else {
						int endOffset = document.PositionToOffset(new TextLocation(0, initializeComponents.BodyRegion.EndLine));
						document.Insert(endOffset, tabs + GenerateFieldDeclaration(domGenerator, newField) + Environment.NewLine);
					}
				}
				SaveDocument();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		protected abstract System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider();
		
		protected abstract DomRegion GetReplaceRegion(ICSharpCode.TextEditor.Document.IDocument document, IMethod method);
		
		protected virtual void FixGeneratedCode(IClass formClass, CodeMemberMethod code)
		{
		}
		
		public virtual void MergeFormChanges(CodeCompileUnit unit)
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
			if (this.formClass == null) {
				MessageService.ShowMessage("Cannot save form: InitializeComponent method does not exist anymore. You should not modify the Designer.cs file while editing a form.");
				return;
			}
			
			if (formClass.Name != this.formClass.Name) {
				LoggingService.Info("Renaming form to " + formClass.Name);
				ICSharpCode.SharpDevelop.Refactoring.FindReferencesAndRenameHelper.RenameClass(this.formClass, formClass.Name);
				Reparse();
			}
			
			FixGeneratedCode(this.formClass, initializeComponent);
			
			// generate file and get initialize components string
			StringWriter writer = new StringWriter();
			CodeDOMGenerator domGenerator = new CodeDOMGenerator(this.CodeDomProvider, tabs + '\t');
			domGenerator.ConvertContentDefinition(initializeComponent, writer);
			
			string statements = writer.ToString();
			
			// initializeComponents.BodyRegion.BeginLine + 1
			DomRegion bodyRegion = GetReplaceRegion(document, initializeComponents);
			if (bodyRegion.BeginColumn <= 0 || bodyRegion.EndColumn <= 0)
				throw new InvalidOperationException("Column must be > 0");
			int startOffset = document.PositionToOffset(new TextLocation(bodyRegion.BeginColumn - 1, bodyRegion.BeginLine - 1));
			int endOffset   = document.PositionToOffset(new TextLocation(bodyRegion.EndColumn - 1, bodyRegion.EndLine - 1));
			
			document.Replace(startOffset, endOffset - startOffset, statements);
			SaveDocument();
			
			// apply changes the designer made to field declarations
			// first loop looks for added and changed fields
			foreach (CodeTypeMember m in formClass.Members) {
				if (m is CodeMemberField) {
					CodeMemberField newField = (CodeMemberField)m;
					IField oldField = GetField(completeClass, newField.Name);
					if (oldField == null || FieldChanged(oldField, newField)) {
						AddOrReplaceField(domGenerator, newField);
					}
				}
			}
			
			// second loop looks for removed fields
			List<string> removedFields = new List<string>();
			foreach (IField field in completeClass.Fields) {
				bool found = false;
				foreach (CodeTypeMember m in formClass.Members) {
					if (m is CodeMemberField && m.Name == field.Name) {
						found = true;
						break;
					}
				}
				if (!found) {
					removedFields.Add(field.Name);
				}
			}
			// removing fields is done in two steps because
			// we must not modify the c.Fields collection while it is enumerated
			removedFields.ForEach(RemoveField);
			
			ParserService.EnqueueForParsing(designerFile, document.TextContent);
		}
		
		/// <summary>
		/// Compares the SharpDevelop.Dom field declaration oldField to
		/// the CodeDom field declaration newField.
		/// </summary>
		/// <returns>true, if the fields are different in type or modifiers, otherwise false.</returns>
		bool FieldChanged(IField oldField, CodeMemberField newField)
		{
			// compare types
			if (oldField.ReturnType != null) { // ignore type changes to untyped VB fields
				if (oldField.ReturnType.FullyQualifiedName != newField.Type.BaseType) {
					LoggingService.Debug("FieldChanged: "+oldField.Name+", "+oldField.ReturnType.FullyQualifiedName+" -> "+newField.Type.BaseType);
					return true;
				}
			}
			
			// compare modifiers
			ModifierEnum oldModifiers = oldField.Modifiers & ModifierEnum.VisibilityMask;
			MemberAttributes newModifiers = newField.Attributes & MemberAttributes.AccessMask;
			
			// SharpDevelop.Dom always adds Private modifier, even if not specified
			// CodeDom omits Private modifier if not present (although it is the default)
			if (oldModifiers == ModifierEnum.Private) {
				if (newModifiers != 0 && newModifiers != MemberAttributes.Private) {
					return true;
				}
			}
			
			ModifierEnum[] sdModifiers = new ModifierEnum[] {ModifierEnum.Protected, ModifierEnum.ProtectedAndInternal, ModifierEnum.Internal, ModifierEnum.Public};
			MemberAttributes[] cdModifiers = new MemberAttributes[] {MemberAttributes.Family, MemberAttributes.FamilyOrAssembly, MemberAttributes.Assembly, MemberAttributes.Public};
			for (int i = 0; i < sdModifiers.Length; i++) {
				if ((oldModifiers  == sdModifiers[i]) ^ (newModifiers  == cdModifiers[i])) {
					return true;
				}
			}
			
			return false;
		}
		
		IDocument document;
		string saveDocumentToFile;
		string designerFile;
		
		/// <summary>
		/// The document containing the content of the <see cref="DesignerFile"/>. Can be a
		/// text editor document or independent of a text editor if partial classes are used.
		/// </summary>
		protected IDocument Document {
			get { return document; }
			set { document = value; }
		}
		/// <summary>
		/// only set when InitializeComponent was loaded from code-behind file that was not open,
		/// is normally either null or the same value as <see cref="DesignerFile"/>.
		/// </summary>
		protected string SaveDocumentToFile {
			get { return saveDocumentToFile; }
			set { saveDocumentToFile = value; }
		}
		/// <summary>
		/// file that contains InitializeComponents
		/// </summary>
		protected string DesignerFile {
			get { return designerFile; }
			set { designerFile = value; }
		}
		
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
			ParseInformation info = ParserService.ParseFile(viewContent.TextEditorControl.FileName, content, false);
			ICompilationUnit cu = (ICompilationUnit)info.BestCompilationUnit;
			foreach (IClass c in cu.Classes) {
				if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(c)) {
					initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(c);
					if (initializeComponents != null) {
						designerFile = initializeComponents.DeclaringType.CompilationUnit.FileName;
						string designerContent;
						if (FileUtility.IsEqualFileName(viewContent.TextEditorControl.FileName, designerFile)) {
							designerContent = content;
							document = viewContent.Document;
						} else {
							IViewContent designerFileViewContent = FileService.GetOpenFile(designerFile);
							if (designerFileViewContent == null) {
								document = new DocumentFactory().CreateDocument();
								designerContent = ParserService.GetParseableFileContent(designerFile);
								document.TextContent = designerContent;
								saveDocumentToFile = designerFile;
							} else {
								ITextEditorControlProvider tecp = designerFileViewContent as ITextEditorControlProvider;
								if (tecp == null)
									throw new ApplicationException("designer file viewcontent must implement ITextEditorControlProvider");
								document = tecp.TextEditorControl.Document;
								designerContent = document.TextContent;
							}
							ParserService.ParseFile(designerFile, designerContent, false);
							initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(c);
						}
						using (StringReader r = new StringReader(designerContent)) {
							int count = initializeComponents.Region.BeginLine;
							for (int i = 1; i < count; i++)
								r.ReadLine();
							string line = r.ReadLine();
							tabs = line.Substring(0, line.Length - line.TrimStart().Length);
						}
						this.c = c;
						this.completeClass = c.GetCompoundClass();
						this.formClass = initializeComponents.DeclaringType;
						break;
					}
				}
			}
		}
		
		protected abstract string CreateEventHandler(EventDescriptor edesc, string eventMethodName, string body, string indentation);
		
		protected virtual int GetCursorLine(IDocument document, IMethod method)
		{
			return method.BodyRegion.BeginLine + 1;
		}
		
		protected virtual int GetCursorLineAfterEventHandlerCreation()
		{
			return 2;
		}
		
		/// <summary>
		/// If found return true and int as position
		/// </summary>
		/// <param name="component"></param>
		/// <param name="edesc"></param>
		/// <returns></returns>
		public virtual bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{
			if (this.failedDesignerInitialize) {
				position = 0;
				file = c.CompilationUnit.FileName;
				return false;
			}

			Reparse();
			
			foreach (IMethod method in completeClass.Methods) {
				if (method.Name == eventMethodName) {
					position = GetCursorLine(document, method);
					file = method.DeclaringType.CompilationUnit.FileName;
					return true;
				}
			}
			viewContent.MergeFormChanges();
			Reparse();
			
			file = c.CompilationUnit.FileName;
			int line = GetEventHandlerInsertionLine(c);
			
			int offset = viewContent.Document.GetLineSegment(line - 1).Offset;
			
			viewContent.Document.Insert(offset, CreateEventHandler(edesc, eventMethodName, body, tabs));
			position = line + GetCursorLineAfterEventHandlerCreation();
			
			return true;
		}
		
		/// <summary>
		/// Gets a method implementing the signature specified by the event descriptor
		/// </summary>
		protected IMethod ConvertDescriptorToDom(EventDescriptor edesc, string methodName)
		{
			MethodInfo mInfo = edesc.EventType.GetMethod("Invoke");
			DefaultMethod m = new DefaultMethod(completeClass, methodName);
			m.ReturnType = ReflectionLayer.ReflectionReturnType.Create(m, mInfo.ReturnType, false);
			foreach (ParameterInfo pInfo in mInfo.GetParameters()) {
				m.Parameters.Add(new ReflectionLayer.ReflectionParameter(pInfo, m));
			}
			return m;
		}
		
		/// <summary>
		/// Gets a method implementing the signature specified by the event descriptor
		/// </summary>
		protected ICSharpCode.NRefactory.Ast.MethodDeclaration
			ConvertDescriptorToNRefactory(EventDescriptor edesc, string methodName)
		{
			return ICSharpCode.SharpDevelop.Dom.Refactoring.CodeGenerator.ConvertMember(
				ConvertDescriptorToDom(edesc, methodName),
				new ClassFinder(c, c.BodyRegion.BeginLine + 1, 1)
			) as ICSharpCode.NRefactory.Ast.MethodDeclaration;
		}
		
		protected virtual int GetEventHandlerInsertionLine(IClass c)
		{
			return c.Region.EndLine;
		}
		
		public virtual ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			Reparse();
			ArrayList compatibleMethods = new ArrayList();
			MethodInfo methodInfo = edesc.EventType.GetMethod("Invoke");
			foreach (IMethod method in completeClass.Methods) {
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
		
		public virtual ICollection GetCompatibleMethods(EventInfo edesc)
		{
			Reparse();
			ArrayList compatibleMethods = new ArrayList();
			MethodInfo methodInfo = edesc.GetAddMethod();
			ParameterInfo pInfo = methodInfo.GetParameters()[0];
			string eventName = pInfo.ParameterType.ToString().Replace("EventHandler", "EventArgs");
			
			foreach (IMethod method in completeClass.Methods) {
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
		
		protected IField GetField(IClass c, string name)
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
