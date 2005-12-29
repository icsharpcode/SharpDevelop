// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class GenerateCodeAction : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textEditorControl = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			
			ParseInformation parseInformation;
			
			if (window.ViewContent.IsUntitled) {
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
				ArrayList categories = new ArrayList();
				ArrayList generators = AddInTree.BuildItems("/AddIns/DefaultTextEditor/CodeGenerator", this, true);
				using (CodeGenerationForm form = new CodeGenerationForm(textEditorControl, (CodeGeneratorBase[])generators.ToArray(typeof(CodeGeneratorBase)), currentClass)) {
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
		IClass FindClass(ICollection classes, int lineNr, int column)
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
	
	public class SurroundCodeAction : AbstractEditAction
	{
		
		public override void Execute(TextArea editActionHandler)
		{
//			SelectionWindow selectionWindow = new SelectionWindow("Surround");
//			selectionWindow.Show();
		}
	}
}
