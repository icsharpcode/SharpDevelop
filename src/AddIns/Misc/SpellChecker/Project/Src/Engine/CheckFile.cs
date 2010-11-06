// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SpellChecker.Engine
{
	/// <summary>
	/// Description of CheckFile.
	/// </summary>
	internal static class CheckFile
	{
		private static ICodeEditorProvider provider;
		
		private static char[] separators = {' ', ',', '.', '>', '<', '(', ')', '{', '}', '[', ']',
												';', '*', '/', ':', '"','\'', '|', '!', '@', '#', '$',
												'%', '^', '&', '*', '\\', '?', '=', '+', '-'};
		
		public static void Check()
		{
			provider = WorkbenchSingleton.Workbench.ActiveViewContent as ICodeEditorProvider;
			
			if (provider != null) {
				provider.CodeEditor.ContextMenuOpening += new ContextMenuEventHandler(provider_CodeEditor_ContextMenuOpening);
				
				GetIncorrectWords();
			}			
		}
		
		private static void GetIncorrectWords() {		
			TaskService.Clear();
			var checker = Constants.CurrentHunspell;
			
			int lines = provider.TextEditor.Document.TotalNumberOfLines;
			
			for (int i = 1 ; i <= lines ; ++i) {
				IDocumentLine line = provider.TextEditor.Document.GetLine(i);				
					
				string lineText = line.Text.Trim();
				
				if (lineText.StartsWith("///") ||
					lineText.StartsWith("/*") ||
					lineText.StartsWith("//") ||
					lineText.StartsWith("*")|| 
					lineText.StartsWith("#region"))
				{
//					WorkbenchSingleton.SafeThreadAsyncCall(delegate {
						foreach(string w in line.Text.Split(separators)) {
							string word = w.ToLowerInvariant().Trim();
							
							if (!string.IsNullOrEmpty(word) && !checker.Spell(word)) {
								var list = checker.Suggest(word);
							
								if (list != null && list.Count > 0) {
									Task t = new Task(
										provider.TextEditor.FileName,
										w + ": " + list.GetString(),
										line.Text.IndexOf(w) + 1,
										i, TaskType.Warning);
									t.Tag = list;
									
									TaskService.Add(t);
								}
							}
						}
//					});
				}
			}
		}
		
		private static void provider_CodeEditor_ContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			// get task at caret
			var caret = provider.TextEditor.Caret;
			var offset = provider.TextEditor.Document.PositionToOffset(caret.Line, caret.Column);
			var word = provider.TextEditor.Document.GetWordAt(offset);
				
			var tasks = TaskService.Tasks
				.Where(t => t.TaskType == TaskType.Warning &&
				       caret.Line == t.Line && 
				       (caret.Column >= t.Column && caret.Column <= t.Column + word.Length))
				.Select(t => t).ToList();
			
			if (tasks == null || tasks.Count == 0)
				return;
			
			var task = tasks[0];
			var suggestions = task.Tag as List<string>;	
			if(suggestions == null || suggestions.Count == 0) return;

			// set source
			provider.CodeEditor.ContextMenu.ItemsSource = suggestions;
			provider.CodeEditor.ContextMenu.PreviewMouseLeftButtonDown+= new MouseButtonEventHandler(provider_CodeEditor_ContextMenu_PreviewMouseLeftButtonDown);
			provider.CodeEditor.ContextMenu.Tag = task;
		}

		private static void provider_CodeEditor_ContextMenu_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			// check conditions
			if (e.Handled) return;
			if (provider == null) return;			
			if (!(e.OriginalSource is TextBlock)) return;
			if (!((sender as ContextMenu).Tag is Task)) return;
			
			// get important data
			TextBlock tb = (TextBlock)e.OriginalSource;
			Task task = (Task)((ContextMenu)sender).Tag;

			// get word
			var caret = provider.TextEditor.Caret;			
			var offset = provider.TextEditor.Document.PositionToOffset(caret.Line, caret.Column);
			var word = provider.TextEditor.Document.GetWordAt(offset);
			var offsetStart = provider.TextEditor.Document.PositionToOffset(task.Line, task.Column);
			string newText = tb.Text;
			// replace with new one
			provider.TextEditor.Document.Remove(offsetStart, word.Length);
			provider.TextEditor.Document.Insert(offsetStart, newText);
			
			e.Handled = true;
			
			TaskService.Remove(task);
		}
		
		private static string GetString(this IList<string> list)
		{
			if (list == null || list.Count == 0) return string.Empty;
			
			StringBuilder sb = new StringBuilder();
			
			for (int i = 0 ; i < list.Count; ++i) {
				if(i == list.Count - 1)
					sb.Append(list[i]);
				else
					sb.Append(list[i] + ", ");
			}
			
			return sb.ToString();
		}
	}
}