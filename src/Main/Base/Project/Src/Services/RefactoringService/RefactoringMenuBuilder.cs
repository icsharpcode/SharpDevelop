// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class RefactoringMenuContext
	{
		public TextArea TextArea;
		public ExpressionResult ExpressionResult;
		public ResolveResult ResolveResult;
		public bool IsDefinition;
	}
	
	/// <summary>
	/// Build a menu with refactoring commands for the item that has been clicked on in the text editor.
	/// </summary>
	public class RefactoringMenuBuilder : ISubmenuBuilder
	{
		public ToolStripItem[] BuildSubmenu(Codon codon, object owner)
		{
			ToolStripMenuItem item;
			
			TextEditorControl textEditorControl = (TextEditorControl)owner;
			if (textEditorControl.FileName == null)
				return new ToolStripItem[0];
			List<ToolStripItem> resultItems = new List<ToolStripItem>();
			TextArea textArea = textEditorControl.ActiveTextAreaControl.TextArea;
			IDocument doc = textArea.Document;
			int caretLine = textArea.Caret.Line;
			
			// list of dotnet names that have definition bookmarks in this line
			List<string> definitions = new List<string>();
			
			// Include definitions (use the bookmarks which should already be present)
			
			// we need to use .ToArray() because the bookmarks might change during enumeration:
			// building member/class submenus can cause reparsing the current file, which might change
			// the available bookmarks
			foreach (Bookmark mark in doc.BookmarkManager.Marks.ToArray()) {
				if (mark != null && mark.LineNumber == caretLine) {
					ClassMemberBookmark cmb = mark as ClassMemberBookmark;
					ClassBookmark cb = mark as ClassBookmark;
					IClass type = null;
					if (cmb != null) {
						definitions.Add(cmb.Member.DotNetName);
						item = new ToolStripMenuItem(MemberNode.GetText(cmb.Member),
						                             ClassBrowserIconService.ImageList.Images[cmb.IconIndex]);
						MenuService.AddItemsToMenu(item.DropDown.Items, mark, ClassMemberBookmark.ContextMenuPath);
						resultItems.Add(item);
						type = cmb.Member.DeclaringType;
					} else if (cb != null) {
						type = cb.Class;
					}
					if (type != null) {
						definitions.Add(type.DotNetName);
						item = new ToolStripMenuItem(ClassNode.GetText(type), ClassBrowserIconService.ImageList.Images[ClassBrowserIconService.GetIcon(type)]);
						MenuService.AddItemsToMenu(item.DropDown.Items,
						                           cb ?? new ClassBookmark(textArea.Document, type),
						                           ClassBookmark.ContextMenuPath);
						resultItems.Add(item);
					}
				}
			}
			
			// Include menu for member that has been clicked on
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textEditorControl.FileName);
			ExpressionResult expressionResult;
			ResolveResult rr;
			int insertIndex = resultItems.Count;	// Insert items at this position to get the outermost expression first, followed by the inner expressions (if any).
			expressionResult = FindFullExpressionAtCaret(textArea, expressionFinder);
		repeatResolve:
			rr = ResolveExpressionAtCaret(textArea, expressionResult);
			RefactoringMenuContext context = new RefactoringMenuContext {
				TextArea = textArea,
				ResolveResult = rr,
				ExpressionResult = expressionResult
			};
			item = null;
			if (rr is MethodGroupResolveResult) {
				item = MakeItem(definitions, ((MethodGroupResolveResult)rr).GetMethodIfSingleOverload());
			} else if (rr is MemberResolveResult) {
				MemberResolveResult mrr = (MemberResolveResult)rr;
				item = MakeItem(definitions, mrr.ResolvedMember);
				if (RefactoringService.FixIndexerExpression(expressionFinder, ref expressionResult, mrr)) {
					if (item != null) {
						resultItems.Insert(insertIndex, item);
					}
					// Include menu for the underlying expression of the
					// indexer expression as well.
					goto repeatResolve;
				}
			} else if (rr is TypeResolveResult) {
				item = MakeItem(definitions, ((TypeResolveResult)rr).ResolvedClass);
			} else if (rr is LocalResolveResult) {
				context.IsDefinition = caretLine + 1 == ((LocalResolveResult)rr).VariableDefinitionRegion.BeginLine;
				item = MakeItem((LocalResolveResult)rr, context);
				insertIndex = 0;	// Insert local variable menu item at the topmost position.
			} else if (rr is UnknownIdentifierResolveResult) {
				item = MakeItemForResolveError((UnknownIdentifierResolveResult)rr, expressionResult.Context, textArea);
				insertIndex = 0;	// Insert menu item at the topmost position.
			} else if (rr is UnknownConstructorCallResolveResult) {
				item = MakeItemForResolveError((UnknownConstructorCallResolveResult)rr, expressionResult.Context, textArea);
				insertIndex = 0;	// Insert menu item at the topmost position.
			}
			if (item != null) {
				resultItems.Insert(insertIndex, item);
			}
			
			// Include menu for current class and method
			ICompilationUnit cu = null;
			IMember callingMember = null;
			if (rr != null && rr.CallingMember != null) {
				callingMember = rr.CallingMember;
			} else {
				ParseInformation parseInfo = ParserService.GetParseInformation(textEditorControl.FileName);
				if (parseInfo != null) {
					cu = parseInfo.MostRecentCompilationUnit;
					if (cu != null) {
						IClass callingClass = cu.GetInnermostClass(caretLine + 1, textArea.Caret.Column + 1);
						callingMember = GetCallingMember(callingClass, caretLine + 1, textArea.Caret.Column + 1);
					}
				}
			}
			if (callingMember != null) {
				item = MakeItem(definitions, callingMember);
				if (item != null) {
					item.Text = StringParser.Parse("${res:SharpDevelop.Refactoring.CurrentMethod}: ") + callingMember.Name;
					resultItems.Add(item);
				}
			}
			
			if (resultItems.Count == 0) {
				return new ToolStripItem[0];
			} else {
				resultItems.Add(new MenuSeparator());
				return resultItems.ToArray();
			}
		}
		
		ToolStripMenuItem MakeItemForResolveError(UnknownIdentifierResolveResult rr, ExpressionContext context, TextArea textArea)
		{
			return MakeItemForUnknownClass(rr.CallingClass, rr.Identifier, textArea);
		}
		
		ToolStripMenuItem MakeItemForResolveError(UnknownConstructorCallResolveResult rr, ExpressionContext context, TextArea textArea)
		{
			return MakeItemForUnknownClass(rr.CallingClass, rr.TypeName, textArea);
		}
		
		ToolStripMenuItem MakeItemForUnknownClass(IClass callingClass, string unknownClassName, TextArea textArea)
		{
			if (callingClass == null)
				return null;
			IProjectContent pc = callingClass.ProjectContent;
			if (!pc.Language.RefactoringProvider.IsEnabledForFile(callingClass.CompilationUnit.FileName))
				return null;
			ToolStripMenuItem item = MakeItemInternal(unknownClassName, ClassBrowserIconService.GotoArrowIndex, callingClass.CompilationUnit, DomRegion.Empty);
			List<IClass> searchResults = new List<IClass>();
			SearchAllClassesWithName(searchResults, pc, unknownClassName, pc.Language);
			foreach (IProjectContent rpc in pc.ReferencedContents) {
				SearchAllClassesWithName(searchResults, rpc, unknownClassName, pc.Language);
			}
			if (searchResults.Count == 0)
				return null;
			foreach (IClass c in searchResults) {
				string newNamespace = c.Namespace;
				ToolStripMenuItem subItem = new ToolStripMenuItem("using " + newNamespace, ClassBrowserIconService.ImageList.Images[ClassBrowserIconService.NamespaceIndex]);
				item.DropDownItems.Add(subItem);
				subItem.Click += delegate {
					NamespaceRefactoringService.AddUsingDeclaration(callingClass.CompilationUnit, textArea.Document, newNamespace, true);
					ParserService.StartAsyncParse(callingClass.CompilationUnit.FileName, textArea.Document.TextContent);
					textArea.MotherTextEditorControl.Refresh();
				};
			}
			return item;
		}
		
		void SearchAllClassesWithName(List<IClass> searchResults, IProjectContent pc, string name, LanguageProperties language)
		{
			foreach (string ns in pc.NamespaceNames) {
				IClass c = pc.GetClass(ns + "." + name, 0, language, GetClassOptions.None);
				if (c != null)
					searchResults.Add(c);
			}
		}
		
		IMember GetCallingMember(IClass callingClass, int caretLine, int caretColumn)
		{
			if (callingClass == null) {
				return null;
			}
			foreach (IMethod method in callingClass.Methods) {
				if (method.BodyRegion.IsInside(caretLine, caretColumn)) {
					return method;
				}
			}
			foreach (IProperty property in callingClass.Properties) {
				if (property.BodyRegion.IsInside(caretLine, caretColumn)) {
					return property;
				}
			}
			return null;
		}
		
		ToolStripMenuItem MakeItem(LocalResolveResult local, RefactoringMenuContext context)
		{
			Debug.Assert(local == context.ResolveResult);
			ToolStripMenuItem item = MakeItemInternal(local.VariableName,
			                                          local.IsParameter ? ClassBrowserIconService.ParameterIndex : ClassBrowserIconService.LocalVariableIndex,
			                                          local.CallingClass.CompilationUnit,
			                                          context.IsDefinition ? DomRegion.Empty : local.VariableDefinitionRegion);
			string treePath = "/SharpDevelop/ViewContent/DefaultTextEditor/Refactoring/";
			treePath += local.IsParameter ? "Parameter" : "LocalVariable";
			if (context.IsDefinition) treePath += "Definition";
			MenuService.AddItemsToMenu(item.DropDown.Items, context, treePath);
			return item;
		}
		
		ToolStripMenuItem MakeItem(List<string> definitions, IMember member)
		{
			if (member == null) return null;
			if (definitions.Contains(member.DotNetName)) return null;
			definitions.Add(member.DotNetName);
			ToolStripMenuItem item = MakeItem(MemberNode.Create(member), member.DeclaringType.CompilationUnit, member.Region);
			ToolStripMenuItem declaringType = MakeItem(null, member.DeclaringType);
			if (declaringType != null) {
				item.DropDown.Items.Add(new ToolStripSeparator());
				declaringType.Text = StringParser.Parse("${res:SharpDevelop.Refactoring.DeclaringType}: ") + declaringType.Text;
				item.DropDown.Items.Add(declaringType);
			}
			return item;
		}
		
		ToolStripMenuItem MakeItem(List<string> definitions, IClass c)
		{
			if (c == null) return null;
			if (definitions != null) {
				if (definitions.Contains(c.DotNetName)) return null;
				definitions.Add(c.DotNetName);
			}
			return MakeItem(new ClassNode((IProject)c.ProjectContent.Project, c), c.CompilationUnit, c.Region);
		}
		
		ToolStripMenuItem MakeItemInternal(string title, int imageIndex, ICompilationUnit cu, DomRegion region)
		{
			ToolStripMenuItem item = new ToolStripMenuItem(title, ClassBrowserIconService.ImageList.Images[imageIndex]);
			
			//ToolStripMenuItem titleItem = new ToolStripMenuItem(title);
			//titleItem.Enabled = false;
			//item.DropDown.Items.Add(titleItem);
			//item.DropDown.Items.Add(new ToolStripSeparator());
			
			if (cu != null && cu.FileName != null && !region.IsEmpty) {
				ToolStripMenuItem gotoDefinitionItem = new ToolStripMenuItem(StringParser.Parse("${res:ICSharpCode.NAntAddIn.GotoDefinitionMenuLabel}"),
				                                                             ClassBrowserIconService.ImageList.Images[ClassBrowserIconService.GotoArrowIndex]);
				gotoDefinitionItem.ShortcutKeys = Keys.Control | Keys.Enter;
				gotoDefinitionItem.Click += delegate {
					FileService.JumpToFilePosition(cu.FileName, region.BeginLine - 1, region.BeginColumn - 1);
				};
				item.DropDown.Items.Add(gotoDefinitionItem);
				item.DropDown.Items.Add(new ToolStripSeparator());
			}
			return item;
		}
		
		ToolStripMenuItem MakeItem(ExtTreeNode classBrowserTreeNode, ICompilationUnit cu, DomRegion region)
		{
			ToolStripMenuItem item = MakeItemInternal(classBrowserTreeNode.Text, classBrowserTreeNode.ImageIndex, cu, region);
			MenuService.AddItemsToMenu(item.DropDown.Items, classBrowserTreeNode, classBrowserTreeNode.ContextmenuAddinTreePath);
			return item;
		}
		
		static ExpressionResult FindFullExpressionAtCaret(TextArea textArea, IExpressionFinder expressionFinder)
		{
			if (expressionFinder != null) {
				return expressionFinder.FindFullExpression(textArea.Document.TextContent, textArea.Caret.Offset);
			} else {
				return new ExpressionResult(null);
			}
		}
		
		static ResolveResult ResolveExpressionAtCaret(TextArea textArea, ExpressionResult expressionResult)
		{
			if (expressionResult.Expression != null) {
				return ParserService.Resolve(expressionResult, textArea.Caret.Line + 1, textArea.Caret.Column + 1, textArea.MotherTextEditorControl.FileName, textArea.Document.TextContent);
			}
			return null;
		}
	}
}





