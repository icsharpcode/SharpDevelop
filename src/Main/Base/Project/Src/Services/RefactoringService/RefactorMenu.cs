// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Tests if the refactoring provider for the current document
	/// supports the specified option.
	/// </summary>
	/// <attribute name="supports">
	/// Same of the action that should be supported.
	/// "*" to test if refactoring is supported at all.
	/// </attribute>
	/// <example title="Test if refactoring is supported">
	/// &lt;Condition name="RefactoringProviderSupports" supports="*"&gt;
	/// </example>
	/// <example title="Test if managing imports is supported">
	/// &lt;Condition name="RefactoringProviderSupports" supports="FindUnusedUsingDeclarations"&gt;
	/// </example>
	public class RefactoringProviderSupportsConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null) {
				return false;
			}
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (provider == null)
				return false;
			LanguageProperties language = ParserService.CurrentProjectContent.Language;
			if (language == null)
				return false;
			if (string.IsNullOrEmpty(provider.TextEditor.FileName))
				return false;
			
			RefactoringProvider rp = language.RefactoringProvider;
			if (!rp.IsEnabledForFile(provider.TextEditor.FileName))
				return false;
			
			string supports = condition.Properties["supports"];
			if (supports == "*")
				return true;
			
			Type t = rp.GetType();
			try {
				return (bool)t.InvokeMember("Supports" + supports, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, rp, null);
			} catch (Exception ex) {
				LoggingService.Warn(ex.ToString());
				return false;
			}
		}
	}
	
	public abstract class AbstractRefactoringCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ParserService.LoadSolutionProjectsThreadRunning) {
				return;
			}
			if (WorkbenchSingleton.Workbench == null) {
				return;
			}
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (provider == null) return;
			LanguageProperties language = ParserService.CurrentProjectContent.Language;
			if (language == null) return;
			
			RefactoringProvider rp = language.RefactoringProvider;
			Run(provider.TextEditor, rp);
		}
		
		protected ResolveResult ResolveAtCaret(ITextEditor textEditor)
		{
			string fileName = textEditor.FileName;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
			if (expressionFinder == null) return null;
			string content = textEditor.Document.Text;
			ExpressionResult expr = expressionFinder.FindFullExpression(content, textEditor.Caret.Offset);
			if (expr.Expression == null) return null;
			return ParserService.Resolve(expr, textEditor.Caret.Line, textEditor.Caret.Column, fileName, content);
		}
		
		protected abstract void Run(ITextEditor textEditor, RefactoringProvider provider);
	}
	
	public class RemoveUnusedUsingsCommand : AbstractRefactoringCommand
	{
		protected override void Run(ITextEditor textEditor, RefactoringProvider provider)
		{
			using (var pm = Gui.AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.RemoveUnusedImports}")) {
				NamespaceRefactoringService.ManageUsings(pm, textEditor.FileName, textEditor.Document, true, true);
			}
		}
	}
	
	public class RenameCommand : AbstractRefactoringCommand
	{
		protected override void Run(ITextEditor textEditor, RefactoringProvider provider)
		{
			ResolveResult rr = ResolveAtCaret(textEditor);
			if (rr is MixedResolveResult) rr = (rr as MixedResolveResult).PrimaryResult;
			if (rr is TypeResolveResult) {
				Rename((rr as TypeResolveResult).ResolvedClass);
			} else if (rr is MemberResolveResult) {
				Rename((rr as MemberResolveResult).ResolvedMember);
			} else if (rr is MethodGroupResolveResult) {
				Rename((rr as MethodGroupResolveResult).GetMethodIfSingleOverload());
			} else if (rr is LocalResolveResult) {
				RenameLocalVariableCommand.Run(rr as LocalResolveResult);
			} else {
				ShowUnknownSymbolError();
			}
		}
		
		static void ShowUnknownSymbolError()
		{
			MessageService.ShowMessage("${res:SharpDevelop.Refactoring.CannotRenameElement}");
		}
		static void ShowNoUserCodeError()
		{
			MessageService.ShowMessage("${res:SharpDevelop.Refactoring.CannotRenameBecauseNotUserCode}");
		}
		
		static void Rename(IMember member)
		{
			if (member == null) {
				ShowUnknownSymbolError();
			} else if (member.DeclaringType.CompilationUnit.FileName == null) {
				ShowNoUserCodeError();
			} else {
				IMethod method = member as IMethod;
				if (method != null && method.IsConstructor) {
					Rename(method.DeclaringType);
				} else {
					FindReferencesAndRenameHelper.RenameMember(member);
				}
			}
		}
		
		static void Rename(IClass c)
		{
			if (c == null) {
				ShowUnknownSymbolError();
			} else if (c.CompilationUnit.FileName == null) {
				ShowNoUserCodeError();
			} else {
				FindReferencesAndRenameHelper.RenameClass(c);
			}
		}
	}
}
