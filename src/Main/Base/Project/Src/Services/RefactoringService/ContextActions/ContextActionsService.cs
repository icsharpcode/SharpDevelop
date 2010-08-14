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
			foreach (var provider in providers) {
				// load from configuration
				provider.IsVisible = true;
			}
		}
		
		public EditorActionsProvider GetAvailableActions(ITextEditor editor)
		{
			return new EditorActionsProvider(editor, this.providers);
		}
	}
	
	
	public class EditorActionsProvider
	{
		ITextEditor editor { get; set; }
		IList<IContextActionsProvider> providers { get; set; }
		EditorContext context { get; set; }
		
		public EditorActionsProvider(ITextEditor editor, IList<IContextActionsProvider> providers)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			if (providers == null)
				throw new ArgumentNullException("providers");
			this.editor = editor;
			this.providers = providers;
			ParserService.ParseCurrentViewContent();
			this.context = new EditorContext(editor);
		}
		
		public IEnumerable<IContextAction> GetVisibleActions()
		{
			return GetActions(this.providers.Where(p => p.IsVisible));
		}
		
		public IEnumerable<IContextAction> GetHiddenActions()
		{
			return GetActions(this.providers.Where(p => !p.IsVisible));
		}
		
		public void SetVisible(IContextAction action, bool isVisible)
		{
			IContextActionsProvider provider;
			if (providerForAction.TryGetValue(action, out provider)) {
				provider.IsVisible = isVisible;
			}
		}
		
		Dictionary<IContextAction, IContextActionsProvider> providerForAction = new Dictionary<IContextAction, IContextActionsProvider>();

		/// <summary>
		/// Gets actions available for current caret position in the editor.
		/// </summary>
		IEnumerable<IContextAction> GetActions(IEnumerable<IContextActionsProvider> providers)
		{
			if (ParserService.LoadSolutionProjectsThreadRunning)
				yield break;
			// DO NOT USE Wait on the main thread!
			// causes deadlocks!
			//parseTask.Wait();
			
			var sw = new Stopwatch(); sw.Start();
			var editorContext = new EditorContext(this.editor);
			long elapsedEditorContextMs = sw.ElapsedMilliseconds;
			
			// could run providers in parallel
			foreach (var provider in providers) {
				foreach (var action in provider.GetAvailableActions(editorContext)) {
					providerForAction[action] = provider;
					yield return action;
				}
			}
//			ICSharpCode.Core.LoggingService.Debug(string.Format("Context actions elapsed {0}ms ({1}ms in EditorContext)",
//			                                                    sw.ElapsedMilliseconds, elapsedEditorContextMs));
		}
	}
}
