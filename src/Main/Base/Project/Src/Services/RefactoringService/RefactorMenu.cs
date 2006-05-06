// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

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
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
				return false;
			}
			ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorControlProvider;
			if (provider == null) return false;
			LanguageProperties language = ParserService.CurrentProjectContent.Language;
			if (language == null) return false;
			
			string supports = condition.Properties["supports"];
			if (supports == "*") return true;
			RefactoringProvider rp = language.RefactoringProvider;
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
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
				return;
			}
			ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorControlProvider;
			if (provider == null) return;
			LanguageProperties language = ParserService.CurrentProjectContent.Language;
			if (language == null) return;
			
			RefactoringProvider rp = language.RefactoringProvider;
			Run(provider.TextEditorControl, rp);
			provider.TextEditorControl.Refresh();
		}
		
		protected abstract void Run(TextEditorControl textEditor, RefactoringProvider provider);
	}
	
	public class RemoveUnusedUsingsCommand : AbstractRefactoringCommand
	{
		protected override void Run(TextEditorControl textEditor, RefactoringProvider provider)
		{
			NamespaceRefactoringService.ManageUsings(textEditor.FileName, textEditor.Document, true, true);
		}
	}
}
