// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring
{
	/// <summary>
	/// Provides "Add check for null" and "Add range check" commands in
	/// context menu of parameter declarations.
	/// </summary>
	public class ParameterCheckRefactoringMenuBuilder : ISubmenuBuilder, IMenuItemBuilder
	{
		public ICollection BuildItems(Codon codon, object owner)
		{
			return BuildSubmenu(codon, owner).TranslateToWpf();
		}
		
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
			ITextEditor textArea = context.Editor;
			IMember m = context.ResolveResult.CallingMember;
			int methodStart = FindMethodStartOffset(textArea.Document, m.BodyRegion);
			if (methodStart < 0)
				return;
			textArea.Select(methodStart, 0);
			using (textArea.Document.OpenUndoGroup()) {
				int startLine = textArea.Caret.Line;
				foreach (string newCodeLine in newCode.Split('\n')) {
					textArea.Document.Insert(textArea.Caret.Offset,
					                         DocumentUtilitites.GetLineTerminator(textArea.Document, textArea.Caret.Line) + newCodeLine);
				}
				int endLine = textArea.Caret.Line;
				textArea.Language.FormattingStrategy.IndentLines(textArea, startLine, endLine);
			}
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
		
		static int FindMethodStartOffset(IDocument document, DomRegion bodyRegion)
		{
			if (bodyRegion.IsEmpty)
				return -1;
			int offset = document.PositionToOffset(bodyRegion.BeginLine, bodyRegion.BeginColumn);
			while (offset < document.TextLength) {
				if (document.GetCharAt(offset) == '{') {
					return offset + 1;
				}
				offset++;
			}
			return -1;
		}
	}
}
