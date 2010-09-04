// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Scripting
{
	public abstract class ScriptingDesignerGenerator : IScriptingDesignerGenerator
	{
		FormsDesignerViewContent viewContent;
		ITextEditorOptions textEditorOptions;
		
		public ITextEditorOptions TextEditorOptions {
			get { return textEditorOptions; }
		}
		
		public ScriptingDesignerGenerator(ITextEditorOptions textEditorOptions)
		{
			this.textEditorOptions = textEditorOptions;
		}
		
		public CodeDomProvider CodeDomProvider {
			get { return null; }
		}
		
		public FormsDesignerViewContent ViewContent {
			get { return viewContent; }
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
			designerCodeFile = viewContent.PrimaryFile;
			return new [] {designerCodeFile};
		}
		
		public void MergeFormChanges(CodeCompileUnit unit)
		{
		}
		
		/// <summary>
		/// Returns a list of method names that could be used as an
		/// event handler with the specified event.
		/// </summary>
		public ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			// Get the form or user control class.
			ParseInformation parseInfo = ParseFile();
			
			// Look at the form's methods and see which are compatible.
			ArrayList methods = new ArrayList();
			IClass c = GetClass(parseInfo.CompilationUnit);
			foreach (IMethod method in c.Methods) {
				if (method.Parameters.Count == 2) {
					methods.Add(method.Name);
				}
			}
			
			return methods;
		}
		
		public void NotifyFormRenamed(string newName)
		{
		}
		
		/// <summary>
		/// Updates the InitializeComponent method's body with the generated code.
		/// </summary>
		public void MergeRootComponentChanges(IDesignerHost host, IDesignerSerializationManager serializationManager)
		{
			ParseInformation parseInfo = ParseFile();
			Merge(host, ViewContent.DesignerCodeFileDocument, parseInfo.CompilationUnit, serializationManager);
		}
		
		/// <summary>
		/// The default implementation calls the ParserService.ParseFile. This
		/// method is overridable so the class can be easily tested without
		/// the ParserService being required.
		/// </summary>
		protected virtual ParseInformation ParseFile(string fileName, string textContent)
		{
			return ParserService.ParseFile(fileName, new StringTextBuffer(textContent));
		}
		
		/// <summary>
		/// Merges the generated code into the specified document.
		/// </summary>
		/// <param name="component">The designer host.</param>
		/// <param name="document">The document that the generated code will be merged into.</param>
		/// <param name="parseInfo">The current compilation unit for the <paramref name="document"/>.</param>
		public void Merge(IDesignerHost host, IDocument document, ICompilationUnit compilationUnit, IDesignerSerializationManager serializationManager)
		{
			IMethod method = GetInitializeComponents(compilationUnit);
			
			// Generate the python source code.
			IScriptingCodeDomSerializer serializer = CreateCodeDomSerializer(TextEditorOptions);
			int indent = GetIndent(method);
			string rootNamespace = GetProjectRootNamespace(compilationUnit);
			string methodBody = serializer.GenerateInitializeComponentMethodBody(host, serializationManager, rootNamespace, indent);
			
			// Merge the code.
			DomRegion methodRegion = GetBodyRegionInDocument(method);
			int startOffset = GetStartOffset(document, methodRegion);
			int endOffset = GetEndOffset(document, methodRegion);

			document.Replace(startOffset, endOffset - startOffset, methodBody);
		}
		
		public virtual IScriptingCodeDomSerializer CreateCodeDomSerializer(ITextEditorOptions options)
		{
			return null;
		}
		
		public int GetIndent(IMethod method)
		{
			int indent = method.Region.BeginColumn;
			if (textEditorOptions.ConvertTabsToSpaces) {
				indent = (indent / textEditorOptions.IndentationSize);
				if (textEditorOptions.IndentationSize > 1) {
					indent += 1;
				}
			}
			return indent;
		}
		
		public virtual DomRegion GetBodyRegionInDocument(IMethod method)
		{
			return DomRegion.Empty;
		}
		
		/// <summary>
		/// Gets the non-generated InitializeComponents from parse info.
		/// </summary>
		public static IMethod GetInitializeComponents(ParseInformation parseInfo)
		{
			return GetInitializeComponents(parseInfo.CompilationUnit);
		}
		
		/// <summary>
		/// Gets the non-generated InitializeComponents from the compilation unit.
		/// </summary>
		public static IMethod GetInitializeComponents(ICompilationUnit unit)
		{
			foreach (IClass c in unit.Classes) {
				if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(c)) {
					IMethod method = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(c);
					if (method != null) {
						return method;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the form or user control class from the compilation unit.
		/// </summary>
		IClass GetClass(ICompilationUnit unit)
		{
			return unit.Classes[0];
		}

		/// <summary>
		/// Gets the start offset of the region.
		/// </summary>
		static int GetStartOffset(IDocument document, DomRegion region)
		{
			return document.PositionToOffset(region.BeginLine, region.BeginColumn);			
		}
		
		/// <summary>
		/// Gets the end offset of the region.
		/// </summary>
		static int GetEndOffset(IDocument document, DomRegion region)
		{
			if (region.EndLine > document.TotalNumberOfLines) {
				// At end of document.
				return document.TextLength;
			} 
			return document.PositionToOffset(region.EndLine, region.EndColumn);
		}
		
		/// <summary>
		/// Inserts an event handler.
		/// </summary>
		public bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{		
			position = GetExistingEventHandler(eventMethodName);
			if (position == -1) {
				// Ensure the text editor has the latest version
				// of the source code before we insert any new code.
				viewContent.MergeFormChanges();
				
				// Insert the event handler at the end of the class with an extra 
				// new line before it.
				IDocument doc = ViewContent.DesignerCodeFileDocument;
				string eventHandler = CreateEventHandler(eventMethodName, body, TextEditorOptions.IndentationString);
				position = InsertEventHandler(doc, eventHandler);
			}
			
			// Set the filename so it refers to the form being designed.
			file = ViewContent.DesignerCodeFile.FileName;
					
			return true;
		}
		
		/// <summary>
		/// Checks if the event handler already exists.
		/// </summary>
		/// <returns>The line position of the first line of the existing event handler.</returns>
		int GetExistingEventHandler(string methodName)
		{
			ParseInformation parseInfo = ParseFile();
			IClass c = GetClass(parseInfo.CompilationUnit);
			foreach (IMethod method in c.Methods) {
				if ((method.Name == methodName) && (method.Parameters.Count == 2)) {
					return method.Region.BeginLine;
				}
			}
			return -1;
		}
		
		public virtual string CreateEventHandler(string eventMethodName, string body, string indentation)
		{
			return String.Empty;
		}
		
		public virtual int InsertEventHandler(IDocument document, string eventHandler)
		{
			return 0;
		}
		
		/// <summary>
		/// Parses the form or user control being designed.
		/// </summary>
		ParseInformation ParseFile()
		{
			return ParseFile(this.ViewContent.DesignerCodeFile.FileName, this.ViewContent.DesignerCodeFileContent);
		}
		
		string GetProjectRootNamespace(ICompilationUnit compilationUnit)
		{
			IProject project = compilationUnit.ProjectContent.Project as IProject;
			if (project != null) {
				return project.RootNamespace;
			}
			return String.Empty;
		}
	}
}
