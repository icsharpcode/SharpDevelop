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
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	public interface IPythonDesignerGenerator : IDesignerGenerator
	{
		/// <summary>
		/// Updates the python form or user control's InitializeComponent method with any 
		/// changes to the designed form or user control.
		/// </summary>
		void MergeRootComponentChanges(IComponent component);
	}
	
	/// <summary>
	/// Form's designer generator for the Python language.
	/// </summary>
	public class PythonDesignerGenerator : IPythonDesignerGenerator
	{
		FormsDesignerViewContent viewContent;
		ITextEditorProperties textEditorProperties;
		
		public PythonDesignerGenerator(ITextEditorProperties textEditorProperties)
		{
			this.textEditorProperties = textEditorProperties;
		}
		
		/// <summary>
		/// Gets the Python code dom provider.
		/// </summary>
		public CodeDomProvider CodeDomProvider {
			get { return null; }
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
			designerCodeFile = this.ViewContent.PrimaryFile;
			return new [] {designerCodeFile};
		}
		
		public void MergeFormChanges(CodeCompileUnit unit)
		{
		}
		
		/// <summary>
		/// Updates the InitializeComponent method's body with the generated code.
		/// </summary>
		public void MergeRootComponentChanges(IComponent component)
		{
			ParseInformation parseInfo = ParseFile(this.ViewContent.DesignerCodeFile.FileName, this.ViewContent.DesignerCodeFileContent);			
			Merge(component, ViewContent.DesignerCodeFileDocument, parseInfo.BestCompilationUnit, textEditorProperties);
		}
		
		/// <summary>
		/// Merges the generated code into the specified document.
		/// </summary>
		/// <param name="component">The root component in the designer host.</param>
		/// <param name="document">The document that the generated code will be merged into.</param>
		/// <param name="parseInfo">The current compilation unit for the <paramref name="document"/>.</param>
		public static void Merge(IComponent component, IDocument document, ICompilationUnit compilationUnit, ITextEditorProperties textEditorProperties)
		{
			// Get the document's initialize components method.
			IMethod method = GetInitializeComponents(compilationUnit);
			
			// Generate the python source code.
			PythonControl pythonForm = new PythonControl(NRefactoryToPythonConverter.GetIndentString(textEditorProperties));
			int indent = method.Region.BeginColumn;
			string methodBody = pythonForm.GenerateInitializeComponentMethodBody(component as Control, indent);
			
			// Merge the code.
			DomRegion methodRegion = GetBodyRegionInDocument(method);
			int startOffset = GetStartOffset(document, methodRegion);
			int endOffset = GetEndOffset(document, methodRegion);

			document.Replace(startOffset, endOffset - startOffset, methodBody);
		}
		
		/// <summary>
		/// Inserts an event handler.
		/// </summary>
		public bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{		
			// Ensure the text editor has the latest version
			// of the source code before we insert any new code.
			viewContent.MergeFormChanges();
			
			// Insert the event handler at the end of the class with an extra 
			// new line before it.
			IDocument doc = viewContent.DesignerCodeFileDocument;
			string eventHandler = CreateEventHandler(eventMethodName, body, "\t");
			int line = doc.LineSegmentCollection.Count;
			LineSegment lastLineSegment = doc.GetLineSegment(line - 1);
			int offset = lastLineSegment.Offset + lastLineSegment.Length;

			string newContent = "\r\n" + eventHandler;
			if (lastLineSegment.Length > 0) {
				// Add an extra new line between the last line and the event handler.
				newContent = "\r\n" + newContent;
			}
			doc.Insert(offset, newContent);
			
			// Set position so it points to the line
			// where the event handler was inserted.
			position = line + 1;
	
			// Set the filename so it refers to the form being designed.
			file = viewContent.DesignerCodeFile.FileName;
			
			return true;
		}
		
		/// <summary>
		/// Returns a list of method names that could be used as an
		/// event handler with the specified event.
		/// </summary>
		public ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			// Get the form or user control class.
			ParseInformation parseInfo = ParseFile(this.ViewContent.DesignerCodeFile.FileName, this.ViewContent.DesignerCodeFileContent);
			
			// Look at the form's methods and see which are compatible.
			ArrayList methods = new ArrayList();
			IClass c = GetClass(parseInfo.BestCompilationUnit);
			foreach (IMethod method in c.Methods) {
				if (method.Parameters.Count == 2) {
					methods.Add(method.Name);
				}
			}
			
			return methods;
		}
		
		/// <summary>
		/// Gets the non-generated InitializeComponents from parse info.
		/// </summary>
		public static IMethod GetInitializeComponents(ParseInformation parseInfo)
		{
			return GetInitializeComponents(parseInfo.BestCompilationUnit);
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
		/// Converts from the DOM region to a document region.
		/// </summary>
		public static DomRegion GetBodyRegionInDocument(IMethod method)
		{
			DomRegion bodyRegion = method.BodyRegion;
			return new DomRegion(bodyRegion.BeginLine + 1, 1, bodyRegion.EndLine + 1, 1);			
		}
		
		/// <summary>
		/// Gets the view content attached to this generator.
		/// </summary>
		public FormsDesignerViewContent ViewContent {
			get { return viewContent; }
		}
		
		/// <summary>
		/// The default implementation calls the ParserService.ParseFile. This
		/// method is overridable so the class can be easily tested without
		/// the ParserService being required.
		/// </summary>
		protected virtual ParseInformation ParseFile(string fileName, string textContent)
		{
			return ParserService.ParseFile(fileName, textContent);
		}
		
		/// <summary>
		/// Returns the generated event handler.
		/// </summary>
		protected string CreateEventHandler(string eventMethodName, string body, string indentation)
		{			
			if (String.IsNullOrEmpty(body)) {
				body = "pass";
			}

			StringBuilder eventHandler = new StringBuilder();
			
			eventHandler.Append(indentation);
			eventHandler.Append("def ");
			eventHandler.Append(eventMethodName);
			eventHandler.Append("(self, sender, e):");
			eventHandler.AppendLine();
			eventHandler.Append(indentation);
			eventHandler.Append("\t");
			eventHandler.Append(body);
			
			return eventHandler.ToString();
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
			return document.PositionToOffset(new TextLocation(region.BeginColumn - 1, region.BeginLine - 1));			
		}
		
		/// <summary>
		/// Gets the end offset of the region.
		/// </summary>
		static int GetEndOffset(IDocument document, DomRegion region)
		{
			TextLocation endLocation = new TextLocation(region.EndColumn - 1, region.EndLine - 1);
			if (endLocation.Line >= document.TotalNumberOfLines) {
				// At end of document.
				return document.TextLength;
			} 
			return document.PositionToOffset(endLocation);
		}
	}
}
