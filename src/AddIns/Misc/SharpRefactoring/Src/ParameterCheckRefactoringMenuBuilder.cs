// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;

namespace SharpRefactoring
{
	/// <summary>
	/// Provides "Add check for null" and "Add range check" commands in
	/// context menu of parameter declarations.
	/// </summary>
	public class ParameterCheckRefactoringMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			List<ToolStripItem> resultItems = new List<ToolStripItem>();
			RefactoringMenuContext context = (RefactoringMenuContext)owner;
			LocalResolveResult lrr = context.ResolveResult as LocalResolveResult;
			if (lrr == null || lrr.CallingClass == null || lrr.ResolvedType == null)
				return resultItems.ToArray();
			LanguageProperties language = lrr.CallingClass.ProjectContent.Language;
			if (language != LanguageProperties.CSharp)
				return resultItems.ToArray();
			
			IClass parameterTypeClass = lrr.ResolvedType.GetUnderlyingClass();
			if (parameterTypeClass == null || parameterTypeClass.ClassType != ClassType.Enum && parameterTypeClass.ClassType != ClassType.Struct) {
				// the parameter is a reference type
				resultItems.Add(MakeItem("Add check for null", delegate { AddCheckForNull(context); }));
			}
			if (parameterTypeClass != null) {
				if (parameterTypeClass.FullyQualifiedName == "System.Int32") {
					resultItems.Add(MakeItem("Add range check", delegate { AddRangeCheck(context); }));
				}
			}
			return resultItems.ToArray();
		}
		
		ToolStripMenuItem MakeItem(string title, EventHandler onClick)
		{
			ToolStripMenuItem menuItem = new ToolStripMenuItem(StringParser.Parse(title));
			menuItem.Click += onClick;
			return menuItem;
		}
		
		void AddCheck(RefactoringMenuContext context, string newCode)
		{
			var codeGen = context.ResolveResult.CallingClass.ProjectContent.Language.CodeGenerator;
			TextArea textArea = context.TextArea;
			IMember m = context.ResolveResult.CallingMember;
			TextLocation methodStart = FindMethodStart(textArea.Document, m.BodyRegion);
			if (methodStart.IsEmpty)
				return;
			textArea.Caret.Position = methodStart;
			textArea.SelectionManager.ClearSelection();
			textArea.Document.UndoStack.StartUndoGroup();
			try {
				foreach (string newCodeLine in newCode.Split('\n')) {
					new Return().Execute(textArea);
					textArea.InsertString(newCodeLine);
				}
			} finally {
				textArea.Document.UndoStack.EndUndoGroup();
			}
			textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			textArea.Document.CommitUpdate();
		}
		
		void AddCheckForNull(RefactoringMenuContext context)
		{
			string name = ((LocalResolveResult)context.ResolveResult).VariableName;
			AddCheck(context,
			         "if (" + name + " == null)\n" +
			         "throw new ArgumentNullException(\"" + name + "\");");
		}
		
		void AddRangeCheck(RefactoringMenuContext context)
		{
			string name = ((LocalResolveResult)context.ResolveResult).VariableName;
			AddCheck(context,
			         "if (" + name + " < 0 || " + name + " > upper_bound)\n" +
			         "throw new ArgumentOutOfRangeException(\"" + name + "\", " + name + ", \"Value must be between 0 and \" + upper_bound);");
		}
		
		static TextLocation FindMethodStart(ICSharpCode.TextEditor.Document.IDocument document, DomRegion bodyRegion)
		{
			if (bodyRegion.IsEmpty)
				return TextLocation.Empty;
			int offset = document.PositionToOffset(new TextLocation(bodyRegion.BeginColumn - 1, bodyRegion.BeginLine - 1));
			while (offset < document.TextLength) {
				if (document.GetCharAt(offset) == '{') {
					return document.OffsetToPosition(offset + 1);
				}
				offset++;
			}
			return TextLocation.Empty;
		}
	}
}
