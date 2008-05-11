// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class GenerateCodeAction : AbstractMenuCommand
	{
		public override void Run()
		{
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			
			if (viewContent == null || !(viewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textEditorControl = ((ITextEditorControlProvider)viewContent).TextEditorControl;
			
			ParseInformation parseInformation;
			
			if (viewContent.PrimaryFile.IsUntitled) {
				parseInformation = ParserService.ParseFile(textEditorControl.FileName, textEditorControl.Document.TextContent);
			} else {
				parseInformation = ParserService.GetParseInformation(textEditorControl.FileName);
			}
			
			if (parseInformation == null) {
				return;
			}
			
			ICompilationUnit cu = parseInformation.MostRecentCompilationUnit as ICompilationUnit;
			if (cu == null) {
				return;
			}
			IClass currentClass = GetCurrentClass(textEditorControl, cu, textEditorControl.FileName);
			
			if (currentClass != null) {
				var generators = AddInTree.BuildItems<CodeGeneratorBase>("/AddIns/DefaultTextEditor/CodeGenerator", this, true);
				using (CodeGenerationForm form = new CodeGenerationForm(textEditorControl, generators.ToArray(), currentClass)) {
					form.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				}
			}
		}
		
		/// <summary>
		/// Returns the class in which the carret currently is, returns null
		/// if the carret is outside the class boundaries.
		/// </summary>
		IClass GetCurrentClass(TextEditorControl textEditorControl, ICompilationUnit cu, string fileName)
		{
			IDocument document = textEditorControl.Document;
			if (cu != null) {
				int caretLineNumber = document.GetLineNumberForOffset(textEditorControl.ActiveTextAreaControl.Caret.Offset) + 1;
				int caretColumn     = textEditorControl.ActiveTextAreaControl.Caret.Offset - document.GetLineSegment(caretLineNumber - 1).Offset + 1;
				return FindClass(cu.Classes, caretLineNumber, caretColumn);
			}
			return null;
		}
		IClass FindClass(ICollection<IClass> classes, int lineNr, int column)
		{
			foreach (IClass c in classes) {
				if (c.Region.IsInside(lineNr, column)) {
					IClass inner = FindClass(c.InnerClasses, lineNr, column);
					return inner == null ? c : inner;
				}
			}
			return null;
		}
	}
}
