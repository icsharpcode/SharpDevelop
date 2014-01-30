// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.FormsDesigner
{
	/*
	public abstract class AbstractDesignerGenerator : IDesignerGenerator
	{
		/// <summary>The currently open part of the class being designed.</summary>
		IUnresolvedTypeDefinition currentClassPart;
		/// <summary>The complete class being designed.</summary>
		//ITypeDefinition completeClass;
		/// <summary>The class part containing the designer code.</summary>
		IUnresolvedTypeDefinition formClass;
		IUnresolvedMethod initializeComponents;
		
		FormsDesignerViewContent viewContent;
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
		
		/// <summary>
		/// Gets the part of the designed class that is open in the source code editor which the designer view is attached to.
		/// </summary>
		protected IUnresolvedTypeDefinition CurrentClassPart {
			get { return this.currentClassPart; }
			set { this.currentClassPart = value; }
		}
		
		public void Attach(FormsDesignerViewContent viewContent)
		{
			this.viewContent = viewContent;
		}
		
		public void Detach()
		{
			this.viewContent = null;
		}
		
		public IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile)
		{
			// get new initialize components
			var fileName = this.viewContent.PrimaryFileName;
			var parsedFile = SD.ParserService.ParseFile(fileName, this.viewContent.PrimaryFileContent);
			var compilation = SD.ParserService.GetCompilationForFile(fileName);
			foreach (var utd in parsedFile.TopLevelTypeDefinitions) {
				var td = utd.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
				if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(td)) {
					this.currentClassPart = utd;
					var initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(td);
					if (initializeComponents != null) {
						this.initializeComponents = (IUnresolvedMethod)initializeComponents.UnresolvedMember;
						string designerFileName = this.initializeComponents.UnresolvedFile.FileName;
						if (designerFileName != null) {
							designerCodeFile = SD.FileService.GetOrCreateOpenedFile(designerFileName);
							return td.Parts
								.Select(p => SD.FileService.GetOrCreateOpenedFile(p.UnresolvedFile.FileName))
								.Distinct();
						}
					}
				}
			}
			
			throw new FormsDesignerLoadException("Could not find InitializeComponent method in any part of the open class.");
		}
		
		/// <summary>
		/// Removes the field declaration with the specified name from the source file.
		/// </summary>
		void RemoveField(string fieldName)
		{
			try {
				LoggingService.Info("Remove field declaration: "+fieldName);
				var completeClass = Reparse();
				
				IField field = GetField(completeClass, fieldName);
				if (field != null) {
					string fileName = field.Region.FileName;
					LoggingService.Debug("-> Field is declared in file '" + fileName + "', Region: " + field.Region.ToString());
					OpenedFile file = SD.FileService.GetOpenedFile(fileName);
					if (file == null) throw new InvalidOperationException("The file where the field is declared is not open, although it belongs to the class.");
					IDocument doc = this.ViewContent.GetDocumentForFile(file);
					if (doc == null) throw new InvalidOperationException("Could not get document for file '" + file.FileName + "'.");
					
					this.RemoveFieldDeclaration(doc, field);
					file.MakeDirty();
				} else {
					LoggingService.Warn("-> Field '" + fieldName + "' not found in class");
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
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
				var completeClass = Reparse();
				IField oldField = GetField(completeClass, newField.Name);
				if (oldField != null) {
					string fileName = oldField.Region.FileName;
					LoggingService.Debug("-> Old field is declared in file '" + fileName + "', Region: " + oldField.Region.ToString());
					OpenedFile file = SD.FileService.GetOpenedFile(fileName);
					if (file == null) throw new InvalidOperationException("The file where the field is declared is not open, although it belongs to the class.");
					IDocument doc = this.ViewContent.GetDocumentForFile(file);
					if (doc == null) throw new InvalidOperationException("Could not get document for file '" + file.FileName + "'.");
					this.ReplaceFieldDeclaration(doc, oldField, GenerateFieldDeclaration(domGenerator, newField));
					file.MakeDirty();
				} else {
					int endOffset = this.ViewContent.DesignerCodeFileDocument.PositionToOffset(initializeComponents.BodyRegion.EndLine + 1, 1);
					this.ViewContent.DesignerCodeFileDocument.Insert(endOffset, tabs + GenerateFieldDeclaration(domGenerator, newField) + Environment.NewLine);
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		protected abstract System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider();
		
		protected abstract DomRegion GetReplaceRegion(IDocument document, IUnresolvedMethod method);
		
		/// <summary>
		/// Removes a field declaration from the source code document.
		/// </summary>
		/// <remarks>
		/// The default implementation assumes that the field region starts at the very beginning
		/// of the line of the field declaration and ends at the end of that line.
		/// Override this method if that is not the case in a specific language.
		/// </remarks>
		protected virtual void RemoveFieldDeclaration(IDocument document, IField field)
		{
			int startOffset = document.PositionToOffset(field.Region.BeginLine, 1);
			int endOffset   = document.PositionToOffset(field.Region.EndLine + 1, 1);
			document.Remove(startOffset, endOffset - startOffset);
		}
		
		/// <summary>
		/// Replaces a field declaration in the source code document.
		/// </summary>
		/// <remarks>
		/// The default implementation assumes that the field region starts at the very beginning
		/// of the line of the field declaration and ends at the end of that line.
		/// Override this method if that is not the case in a specific language.
		/// </remarks>
		protected virtual void ReplaceFieldDeclaration(IDocument document, IField oldField, string newFieldDeclaration)
		{
			int startOffset = document.PositionToOffset(oldField.Region.BeginLine, 1);
			int endOffset   = document.PositionToOffset(oldField.Region.EndLine + 1, 1);
			document.Replace(startOffset, endOffset - startOffset, tabs + newFieldDeclaration + Environment.NewLine);
		}
		
		protected virtual void FixGeneratedCode(IUnresolvedTypeDefinition formClass, CodeMemberMethod code)
		{
		}
		
		public virtual void NotifyComponentRenamed(object component, string newName, string oldName)
		{
			Reparse();
			LoggingService.Info(string.Format("Renaming form '{0}' to '{1}'.", oldName, newName));
			if (this.formClass == null) {
				LoggingService.Warn("Cannot rename, formClass not found");
			} else {
				if (viewContent.Host == null || viewContent.Host.Container == null ||
				    viewContent.Host.Container.Components == null || viewContent.Host.Container.Components.Count == 0)
					return;
				
				// verify if we should rename the class
				if (viewContent.Host.Container.Components[0] == component) {
					ICSharpCode.SharpDevelop.Refactoring.FindReferencesAndRenameHelper.RenameClass(this.formClass, newName);
				} else {
					// rename a member - if exists
					IField field = GetField(this.formClass, oldName);
					if (field != null) {
						ICSharpCode.SharpDevelop.Refactoring.FindReferencesAndRenameHelper.RenameMember(field, newName);
					}
				}
				Reparse();
			}
		}
		
		public virtual void MergeFormChanges(CodeCompileUnit unit)
		{
			var oldCompleteClass = Reparse();
			
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
			if (oldCompleteClass == null || this.formClass == null) {
				MessageService.ShowMessage("Cannot save form: InitializeComponent method does not exist anymore. You should not modify the Designer.cs file while editing a form.");
				return;
			}
			
			RemoveUnsupportedCode(formClass, initializeComponent);
			
			FixGeneratedCode(this.formClass, initializeComponent);
			
			// generate file and get initialize components string
			StringWriter writer = new StringWriter();
			string indentation = tabs + SD.EditorControlService.GlobalOptions.IndentationString;
			CodeDOMGenerator domGenerator = new CodeDOMGenerator(this.CodeDomProvider, indentation);
			domGenerator.ConvertContentDefinition(initializeComponent, writer);
			
			string statements = writer.ToString();
			
			// initializeComponents.BodyRegion.BeginLine + 1
			DomRegion bodyRegion = GetReplaceRegion(this.ViewContent.DesignerCodeFileDocument, initializeComponents);
			if (bodyRegion.BeginColumn <= 0 || bodyRegion.EndColumn <= 0)
				throw new InvalidOperationException("Column must be > 0");
			int startOffset = this.ViewContent.DesignerCodeFileDocument.PositionToOffset(bodyRegion.BeginLine, bodyRegion.BeginColumn);
			int endOffset   = this.ViewContent.DesignerCodeFileDocument.PositionToOffset(bodyRegion.EndLine, bodyRegion.EndColumn);
			
			this.ViewContent.DesignerCodeFileDocument.Replace(startOffset, endOffset - startOffset, statements);
			
			// apply changes the designer made to field declarations
			// first loop looks for added and changed fields
			foreach (CodeTypeMember m in formClass.Members) {
				if (m is CodeMemberField) {
					CodeMemberField newField = (CodeMemberField)m;
					IField oldField = GetField(oldCompleteClass, newField.Name);
					if (oldField == null || FieldChanged(oldField, newField)) {
						AddOrReplaceField(domGenerator, newField);
					}
				}
			}
			
			// second loop looks for removed fields
			foreach (IField field in oldCompleteClass.Fields) {
				bool found = false;
				foreach (CodeTypeMember m in formClass.Members) {
					if (m is CodeMemberField && m.Name == field.Name) {
						found = true;
						break;
					}
				}
				if (!found) {
					RemoveField(field.Name);
				}
			}
			
			SD.ParserService.ParseFileAsync(this.ViewContent.DesignerCodeFile.FileName, this.ViewContent.DesignerCodeFileDocument).FireAndForget();
		}
		
		/// <summary>
		/// This method solves all problems that are caused if the designer generates
		/// code that is not supported in a previous version of .NET. (3.5 and below)
		/// Currently it fixes:
		///  - remove calls to ISupportInitialize.BeginInit/EndInit, if the interface is not implemented by the type in the target framework.
		/// </summary>
		/// <remarks>When adding new workarounds make sure that the code does not remove too much code!</remarks>
		void RemoveUnsupportedCode(CodeTypeDeclaration formClass, CodeMemberMethod initializeComponent)
		{
			ICompilation compilation = null;
			if (compilation.GetProject() is MSBuildBasedProject) {
				MSBuildBasedProject p = (MSBuildBasedProject)compilation.GetProject();
				string v = (p.GetEvaluatedProperty("TargetFrameworkVersion") ?? "").Trim('v');
				Version version;
				if (!Version.TryParse(v, out version) || version.Major >= 4)
					return;
			}
			
			List<CodeStatement> stmtsToRemove = new List<CodeStatement>();
			var iSupportInitializeInterface = compilation.FindType(typeof(ISupportInitialize)).GetDefinition();
			
			if (iSupportInitializeInterface == null)
				return;
			
			foreach (var stmt in initializeComponent.Statements.OfType<CodeExpressionStatement>().Where(ces => ces.Expression is CodeMethodInvokeExpression)) {
				CodeMethodInvokeExpression invocation = (CodeMethodInvokeExpression)stmt.Expression;
				CodeCastExpression expr = invocation.Method.TargetObject as CodeCastExpression;
				if (expr != null) {
					if (expr.TargetType.BaseType != "System.ComponentModel.ISupportInitialize")
						continue;
					var fieldType = GetTypeOfControl(compilation, expr.Expression, initializeComponent, formClass).GetDefinition();
					if (fieldType == null)
						continue;
					if (!fieldType.IsDerivedFrom(iSupportInitializeInterface))
						stmtsToRemove.Add(stmt);
				}
			}
			
			foreach (var stmt in stmtsToRemove) {
				initializeComponent.Statements.Remove(stmt);
			}
		}
		
		/// <summary>
		/// Tries to find the type of the expression.
		/// </summary>
		IType GetTypeOfControl(ICompilation compilation, CodeExpression expression, CodeMemberMethod initializeComponentMethod, CodeTypeDeclaration formTypeDeclaration)
		{
			StringComparer comparer = compilation.NameComparer;
			if (expression is CodeVariableReferenceExpression) {
				string name = ((CodeVariableReferenceExpression)expression).VariableName;
				var decl = initializeComponentMethod.Statements.OfType<CodeVariableDeclarationStatement>().Single(v => comparer.Equals(v.Name, name));
				return ReflectionHelper.ParseReflectionName(decl.Type.BaseType).Resolve(compilation);
			}
			if (expression is CodeFieldReferenceExpression && ((CodeFieldReferenceExpression)expression).TargetObject is CodeThisReferenceExpression) {
				string name = ((CodeFieldReferenceExpression)expression).FieldName;
				var decl = formTypeDeclaration.Members.OfType<CodeMemberField>().FirstOrDefault(f => comparer.Equals(name, f.Name));
				if (decl != null)
					return ReflectionHelper.ParseReflectionName(decl.Type.BaseType).Resolve(compilation);
				var formType = formClass.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
				var field = formType.GetFields(f => comparer.Equals(f.Name, name)).LastOrDefault();
				if (field == null)
					return null;
				return field.ReturnType;
			}
			return null;
		}
		
		/// <summary>
		/// Compares the SharpDevelop.Dom field declaration oldField to
		/// the CodeDom field declaration newField.
		/// </summary>
		/// <returns>true, if the fields are different in type or modifiers, otherwise false.</returns>
		static bool FieldChanged(IField oldField, CodeMemberField newField)
		{
			// compare types
			if (oldField.ReturnType != null && newField.Type != null) {
				if (AreTypesDifferent(oldField.ReturnType, newField.Type)) {
					LoggingService.Debug("FieldChanged (type): "+oldField.Name+", "+oldField.ReturnType.FullName+" -> "+newField.Type.BaseType);
					return true;
				}
			}
			
			// compare accessibility modifiers
			Accessibility oldModifiers = oldField.Accessibility;
			MemberAttributes newModifiers = newField.Attributes & MemberAttributes.AccessMask;
			
			// SharpDevelop.Dom always adds Private modifier, even if not specified
			// CodeDom omits Private modifier if not present (although it is the default)
			if (oldModifiers == Accessibility.Private) {
				if (newModifiers != 0 && newModifiers != MemberAttributes.Private) {
					return true;
				}
			}
			
			Accessibility[] sdModifiers = {Accessibility.Protected, Accessibility.ProtectedAndInternal, Accessibility.Internal, Accessibility.Public};
			MemberAttributes[] cdModifiers = {MemberAttributes.Family, MemberAttributes.FamilyOrAssembly, MemberAttributes.Assembly, MemberAttributes.Public};
			for (int i = 0; i < sdModifiers.Length; i++) {
				if ((oldModifiers  == sdModifiers[i]) ^ (newModifiers  == cdModifiers[i])) {
					return true;
				}
			}
			
			return false;
		}
		
		static bool AreTypesDifferent(IType oldType, CodeTypeReference newType)
		{
			IType oldClass = oldType.GetDefinition();
			if (oldClass == null) {
				// ignore type changes to fields with unresolved type
				return false;
			}
			if (newType.BaseType == "System.Void") {
				// field types get replaced with System.Void if the type cannot be resolved
				// (e.g. generic fields in the Boo designer which aren't converted to CodeDom)
				// we'll ignore such type changes (fields should never have the type void)
				return false;
			}
			
			return oldType.ReflectionName == newType.BaseType;
		}
		
		protected ITypeDefinition Reparse()
		{
			Dictionary<OpenedFile, ParseInformation> parsings = new Dictionary<OpenedFile, ParseInformation>();
			ParseInformation info;
			
			// Reparse all source files for the designed form
			foreach (KeyValuePair<OpenedFile, IDocument> entry in this.ViewContent.SourceFiles) {
				parsings.Add(entry.Key, SD.ParserService.Parse(entry.Key.FileName, entry.Value, project));
			}
			ICompilation compilation = SD.ParserService.GetCompilation(project);
			
			// Update currentClassPart from PrimaryFile
			this.currentClassPart = null;
			if (this.ViewContent.PrimaryFile != null && parsings.TryGetValue(this.ViewContent.PrimaryFile, out info)) {
				foreach (var utd in info.UnresolvedFile.TopLevelTypeDefinitions) {
					var td = utd.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
					if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(td)) {
						if (FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(td) != null) {
							this.currentClassPart = utd;
							break;
						}
					}
				}
				if (this.currentClassPart == null) {
					LoggingService.Warn("AbstractDesignerGenerator.Reparse: Could not find designed class in primary file '" + this.ViewContent.PrimaryFile.FileName + "'");
				}
			} else {
				LoggingService.Debug("AbstractDesignerGenerator.Reparse: Primary file is unavailable");
			}
			
			// Update initializeComponents, completeClass and formClass
			// from designer code file
			ITypeDefinition completeClass = null;
			this.formClass = null;
			this.initializeComponents = null;
			if (this.ViewContent.DesignerCodeFile == null ||
			    !parsings.TryGetValue(this.ViewContent.DesignerCodeFile, out info)) {
				LoggingService.Warn("AbstractDesignerGenerator.Reparse: Designer source code file is unavailable");
				return null;
			}
			
			var cu = info.UnresolvedFile;
			foreach (var utd in cu.TopLevelTypeDefinitions) {
				var td = utd.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)).GetDefinition();
				if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(td)) {
					var initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(td);
					if (initializeComponents != null) {
						this.initializeComponents = (IUnresolvedMethod)initializeComponents.UnresolvedMember;
						using (StringReader r = new StringReader(this.ViewContent.DesignerCodeFileContent)) {
							int count = this.initializeComponents.Region.BeginLine;
							for (int i = 1; i < count; i++)
								r.ReadLine();
							string line = r.ReadLine();
							tabs = GetIndentation(line);
						}
						completeClass = td;
						this.formClass = this.initializeComponents.DeclaringTypeDefinition;
						break;
					}
				}
			}
			
			if (completeClass == null || this.formClass == null) {
				LoggingService.Warn("AbstractDesignerGenerator.Reparse: Could not find InitializeComponents in designer source code file '" + this.ViewContent.DesignerCodeFile.FileName + "'");
			}
			return completeClass;
		}
		
		protected static string GetIndentation(string line)
		{
			return line.Substring(0, line.Length - line.TrimStart().Length);
		}
		
		protected abstract string CreateEventHandler(Type eventType, string eventMethodName, string body, string indentation);
		
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
			if (edesc == null)
				throw new ArgumentNullException("edesc");
			var completeClass = Reparse();
			
			LoggingService.Debug("Forms designer: AbstractDesignerGenerator.InsertComponentEvent: eventMethodName=" + eventMethodName);
			
			foreach (IMethod method in completeClass.Methods) {
				if (CompareMethodNames(method.Name, eventMethodName)) {
					file = method.Region.FileName;
					OpenedFile openedFile = SD.FileService.GetOpenedFile(file);
					IDocument doc;
					if (openedFile != null && (doc = this.ViewContent.GetDocumentForFile(openedFile)) != null) {
						position = GetCursorLine(doc, method);
					} else {
						try {
							position = GetCursorLine(FindReferencesAndRenameHelper.GetDocumentInformation(file).Document, method);
						} catch (FileNotFoundException) {
							position = 0;
							return false;
						}
					}
					return true;
				}
			}
			
			viewContent.MergeFormChanges();
			completeClass = Reparse();
			
			file = currentClassPart.ParsedFile.FileName;
			int line = GetEventHandlerInsertionLine(currentClassPart);
			LoggingService.Debug("-> Inserting new event handler at line " + line.ToString(System.Globalization.CultureInfo.InvariantCulture));
			
			if (line - 1 == this.viewContent.PrimaryFileDocument.LineCount) {
				// insert a newline at the end of file if necessary (can happen in Boo if there is no newline at the end of the document)
				this.viewContent.PrimaryFileDocument.Insert(this.viewContent.PrimaryFileDocument.TextLength, Environment.NewLine);
			}
			
			int offset = this.viewContent.PrimaryFileDocument.GetLine(line).Offset;
			
			this.viewContent.PrimaryFileDocument.Insert(offset, CreateEventHandler(edesc.EventType, eventMethodName, body, tabs));
			position = line + GetCursorLineAfterEventHandlerCreation();
			this.viewContent.PrimaryFile.MakeDirty();
			
			return true;
		}
		
		protected virtual bool CompareMethodNames(string strA, string strB)
		{
			return strA == strB;
		}
		
		/// <summary>
		/// Gets a method implementing the signature specified by the event descriptor
		/// </summary>
		protected static IUnresolvedMethod ConvertEventInvokeMethodToDom(Type eventType, string methodName)
		{
			MethodInfo mInfo = eventType.GetMethod("Invoke");
			DefaultMethod m = new DefaultMethod(declaringType, methodName);
			m.ReturnType = ReflectionLayer.ReflectionReturnType.Create(m, mInfo.ReturnType);
			foreach (ParameterInfo pInfo in mInfo.GetParameters()) {
				m.Parameters.Add(new ReflectionLayer.ReflectionParameter(pInfo, m));
			}
			return m;
		}
		
		/// <summary>
		/// Gets a method implementing the signature specified by the event descriptor
		/// </summary>
		protected static ICSharpCode.NRefactory.Ast.MethodDeclaration
			ConvertEventInvokeMethodToNRefactory(IClass context, Type eventType, string methodName)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			
			return ICSharpCode.SharpDevelop.Dom.Refactoring.CodeGenerator.ConvertMember(
				ConvertEventInvokeMethodToDom(context, eventType, methodName),
				new ClassFinder(context, context.BodyRegion.BeginLine + 1, 1)
			) as ICSharpCode.NRefactory.Ast.MethodDeclaration;
		}
		
		protected virtual int GetEventHandlerInsertionLine(IUnresolvedTypeDefinition c)
		{
			return c.Region.EndLine;
		}
		
		public virtual ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			var completeClass = Reparse();
			ArrayList compatibleMethods = new ArrayList();
			MethodInfo methodInfo = edesc.EventType.GetMethod("Invoke");
			var methodInfoParameters = methodInfo.GetParameters();
			foreach (IMethod method in completeClass.Methods) {
				if (method.Parameters.Count == methodInfoParameters.Length) {
					bool found = true;
					for (int i = 0; i < methodInfoParameters.Length; ++i) {
						ParameterInfo pInfo = methodInfoParameters[i];
						IParameter p = method.Parameters[i];
						if (p.Type.ReflectionName != pInfo.ParameterType.ToString()) {
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
		
		protected IField GetField(ITypeDefinition c, string name)
		{
			if (c == null)
				return null;
			foreach (IField field in c.Fields) {
				if (field.Name == name) {
					return field;
				}
			}
			return null;
		}
	}*/
}
