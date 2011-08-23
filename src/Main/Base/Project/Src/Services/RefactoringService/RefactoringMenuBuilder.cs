// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.ClassBrowser;
using ICSharpCode.SharpDevelop.Project;
using Ast = ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	public class RefactoringMenuContext
	{
		public readonly ITextEditor Editor;
		public readonly ExpressionResult ExpressionResult;
		public readonly ResolveResult ResolveResult;
		public readonly bool IsDefinition;
		/// <remarks>Can be null.</remarks>
		public readonly IProjectContent ProjectContent;
		public readonly ICompilationUnit CompilationUnit;
		
		public RefactoringMenuContext(ITextEditor editor, ExpressionResult expressionResult,
		                              ResolveResult resolveResult, bool isDefinition,
		                              IProjectContent projectContent, ICompilationUnit compilationUnit)
		{
			this.Editor = editor;
			this.ExpressionResult = expressionResult;
			this.ResolveResult = resolveResult;
			this.IsDefinition = isDefinition;
			this.ProjectContent = projectContent;
			this.CompilationUnit = compilationUnit;
		}
	}
	
	public interface IRefactoringMenuItemFactory
	{
		MenuItem Create(RefactoringMenuContext context);
	}
	
	/// <summary>
	/// Build refactoring commands for the item that has been clicked on in the text editor.
	/// The commands are inserted to the top level of the context menu.
	/// Path:
	/// /SharpDevelop/ViewContent/TextEditor/ContextMenu, id=Refactoring
	/// </summary>
	public class RefactoringMenuBuilder : IMenuItemBuilder
	{
		public ICollection BuildItems(Codon codon, object owner)
		{
			ITextEditor textEditor = (ITextEditor)owner;
			if (string.IsNullOrEmpty(textEditor.FileName))
				return new object[0];
			List<object> resultItems = new List<object>();
			// list of dotnet names that have definitions in this line
			List<string> definitions = new List<string>();
			
			// Include menu for member that has been clicked on
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(textEditor.FileName);
			ExpressionResult expressionResult = FindFullExpressionAtCaret(textEditor, expressionFinder);
			
			AddTopLevelItems(resultItems, textEditor, expressionResult, definitions, false);
			
			AddItemForCurrentClassAndMethod(resultItems, textEditor, expressionResult, definitions);
			
			if (resultItems.Count > 0) {
				resultItems.Add(new Separator());
			}
			return resultItems;
		}
		
		void AddTopLevelItems(List<object> resultItems, ITextEditor textEditor, ExpressionResult expressionResult, List<string> definitions, bool addAsSubmenu)
		{
			// Insert items at this position to get the outermost expression first, followed by the inner expressions (if any).
			int insertIndex = resultItems.Count;
			ResolveResult rr = ResolveExpressionAtCaret(textEditor, expressionResult);
			MenuItem item = null;
			
			if (rr is MethodGroupResolveResult) {
				item = MakeItem(definitions, ((MethodGroupResolveResult)rr).GetMethodIfSingleOverload());
			} else if (rr is MemberResolveResult) {
				MemberResolveResult mrr = (MemberResolveResult)rr;
				item = MakeItem(definitions, mrr.ResolvedMember);
				// Seems not to be needed, as AddItemForCurrentClassAndMethod works for indexer as well (martinkonicek)
				/*if (RefactoringService.FixIndexerExpression(expressionFinder, ref expressionResult, mrr)) {
					if (item != null) {
						// Insert this member
						resultItems.Insert(insertIndex, item);
					}
					// Include menu for the underlying expression of the
					// indexer expression as well.
					AddTopLevelItems(textEditor, expressionResult, true);
				}*/
			} else if (rr is TypeResolveResult) {
				item = MakeItem(definitions, ((TypeResolveResult)rr).ResolvedClass);
			} else if (rr is LocalResolveResult) {
				bool isDefinition = textEditor.Caret.Line == ((LocalResolveResult)rr).VariableDefinitionRegion.BeginLine;
				ParseInformation pi = ParserService.GetParseInformation(textEditor.FileName);
				IProjectContent pc = null; 
				if (pi != null)
					pc = pi.CompilationUnit.ProjectContent;
				RefactoringMenuContext context = new RefactoringMenuContext(textEditor, expressionResult, rr, isDefinition, pc, pi.CompilationUnit);
				item = MakeItem((LocalResolveResult)rr, context);
				insertIndex = 0;	// Insert local variable menu item at the topmost position.
			}
			if (item != null) {
				if (addAsSubmenu) {
					resultItems.Insert(insertIndex, item);
				} else {
					foreach (object subItem in item.Items) {
						resultItems.Add(subItem);
					}
					item.Items.Clear(); // detach resultItems from old parent
				}
			}
		}
		
		void AddItemForCurrentClassAndMethod(List<object> resultItems, ITextEditor textEditor, ExpressionResult expressionResult, List<string> definitions)
		{
			ResolveResult rr = ResolveExpressionAtCaret(textEditor, expressionResult);
			MenuItem item = null;
			int caretLine = textEditor.Caret.Line;
			// Include menu for current class and method
			ICompilationUnit cu = null;
			IMember callingMember = null;
			if (rr != null && rr.CallingMember != null) {
				callingMember = rr.CallingMember;
			} else {
				ParseInformation parseInfo = ParserService.GetParseInformation(textEditor.FileName);
				if (parseInfo != null) {
					cu = parseInfo.CompilationUnit;
					if (cu != null) {
						IClass callingClass = cu.GetInnermostClass(caretLine, textEditor.Caret.Column);
						callingMember = GetCallingMember(callingClass, caretLine, textEditor.Caret.Column);
					}
				}
			}
			if (callingMember != null) {
				item = MakeItem(definitions, callingMember);
				if (item != null) {
					item.Header = StringParser.Parse("${res:SharpDevelop.Refactoring.CurrentMethod}: ") + callingMember.Name;
					resultItems.Add(item);
				}
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
		
		MenuItem MakeItem(LocalResolveResult local, RefactoringMenuContext context)
		{
			Debug.Assert(local == context.ResolveResult);
			MenuItem item = MakeItemWithGoToDefinition(local.VariableName,
			                                           local.IsParameter ? ClassBrowserIconService.Parameter : ClassBrowserIconService.LocalVariable,
			                                           local.CallingClass.CompilationUnit,
			                                           context.IsDefinition ? DomRegion.Empty : local.VariableDefinitionRegion);
			string treePath = "/SharpDevelop/ViewContent/DefaultTextEditor/Refactoring/";
			treePath += local.IsParameter ? "Parameter" : "LocalVariable";
			if (context.IsDefinition) treePath += "Definition";
			foreach (object obj in MenuService.CreateMenuItems(null, context, treePath))
				item.Items.Add(obj);
			return item;
		}
		
		MenuItem MakeItem(List<string> definitions, IMember member)
		{
			if (member == null) return null;
			if (definitions.Contains(member.DotNetName)) return null;
			definitions.Add(member.DotNetName);
			MenuItem item = MakeItem(MemberNode.Create(member), member.DeclaringType.CompilationUnit, member.Region);
			MenuItem declaringType = MakeItem(null, member.DeclaringType);
			if (declaringType != null) {
				declaringType.Header = StringParser.Parse("${res:SharpDevelop.Refactoring.DeclaringType}: ") + declaringType.Header;
				item.Items.Add(declaringType);
			}
			return item;
		}
		
		MenuItem MakeItem(List<string> definitions, IClass c)
		{
			if (c == null) return null;
			if (definitions != null) {
				if (definitions.Contains(c.DotNetName)) return null;
				definitions.Add(c.DotNetName);
			}
			return MakeItem(new ClassNode((IProject)c.ProjectContent.Project, c), c.CompilationUnit, c.Region);
		}
		
		MenuItem MakeItemWithGoToDefinition(string title, IImage image, ICompilationUnit cu, DomRegion region)
		{
			MenuItem item = new MenuItem();
			item.Header = title;
			item.Icon = image.CreateImage();
			if (cu != null && cu.FileName != null && !region.IsEmpty) {
				MenuItem gotoDefinitionItem = new MenuItem();
				gotoDefinitionItem.Header = MenuService.ConvertLabel(StringParser.Parse("${res:ICSharpCode.NAntAddIn.GotoDefinitionMenuLabel}"));
				gotoDefinitionItem.Icon = ClassBrowserIconService.GotoArrow.CreateImage();
				gotoDefinitionItem.InputGestureText = new KeyGesture(Key.Enter, ModifierKeys.Control).GetDisplayStringForCulture(Thread.CurrentThread.CurrentUICulture);
				gotoDefinitionItem.Click += delegate {
					FileService.JumpToFilePosition(cu.FileName, region.BeginLine, region.BeginColumn);
				};
				item.Items.Add(gotoDefinitionItem);
			}
			return item;
		}
		
		MenuItem MakeItem(ExtTreeNode classBrowserTreeNode, ICompilationUnit cu, DomRegion region)
		{
			MenuItem item = MakeItemWithGoToDefinition(classBrowserTreeNode.Text, ClassBrowserIconService.GetImageByIndex(classBrowserTreeNode.ImageIndex), cu, region);
			foreach (object obj in MenuService.CreateMenuItems(null, classBrowserTreeNode, classBrowserTreeNode.ContextmenuAddinTreePath))
				item.Items.Add(obj);
			return item;
		}
		
		static ExpressionResult FindFullExpressionAtCaret(ITextEditor textArea, IExpressionFinder expressionFinder)
		{
			if (expressionFinder != null) {
				return expressionFinder.FindFullExpression(textArea.Document.Text, textArea.Caret.Offset);
			} else {
				return new ExpressionResult(null);
			}
		}
		
		static ResolveResult ResolveExpressionAtCaret(ITextEditor textArea, ExpressionResult expressionResult)
		{
			if (expressionResult.Expression != null) {
				return ParserService.Resolve(expressionResult, textArea.Caret.Line, textArea.Caret.Column, textArea.FileName, textArea.Document.Text);
			}
			return null;
		}
	}
}
