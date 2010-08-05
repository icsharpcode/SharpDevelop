// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Provides context actions available for current line of the editor.
	/// </summary>
	public sealed class ContextActionsService
	{
		private static ContextActionsService instance = new ContextActionsService();
		
		public static ContextActionsService Instance {
			get {
				return instance;
			}
		}
		
		List<IContextActionsProvider> providers;
		
		private ContextActionsService()
		{
			this.providers = AddInTree.BuildItems<IContextActionsProvider>("/SharpDevelop/ViewContent/AvalonEdit/ContextActions", null, false);
		}
		
		/// <summary>
		/// Gets actions available for current caret position in the editor.
		/// </summary>
		public IEnumerable<IContextAction> GetAvailableActions(ITextEditor editor)
		{
			if (ParserService.LoadSolutionProjectsThreadRunning)
				yield break;
			var parseTask = ParserService.BeginParseCurrentViewContent();
			parseTask.Wait();
			
			var sw = new Stopwatch(); sw.Start();
			var editorContext = new EditorContext(editor);
			long elapsedEditorContextMs = sw.ElapsedMilliseconds;
			
			// could run providers in parallel
			foreach (var provider in this.providers) {
				foreach (var action in provider.GetAvailableActions(editorContext)) {
					yield return action;
				}
			}
			ICSharpCode.Core.LoggingService.Debug(string.Format("Context actions elapsed {0}ms ({1}ms in EditorContext)",
			                                                   sw.ElapsedMilliseconds, elapsedEditorContextMs));
		}
	}
}
