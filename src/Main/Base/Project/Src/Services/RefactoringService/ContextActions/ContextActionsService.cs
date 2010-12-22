// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		/// <summary>
		/// Key for storing the names of disabled providers in PropertyService.
		/// </summary>
		const string PropertyServiceKey = "DisabledContextActionProviders";
		
		public static ContextActionsService Instance {
			get {
				return instance;
			}
		}
		
		List<IContextActionsProvider> providers;
		
		private ContextActionsService()
		{
			this.providers = AddInTree.BuildItems<IContextActionsProvider>("/SharpDevelop/ViewContent/AvalonEdit/ContextActions", null, false);
			var disabledActions = LoadProviderVisibilities().ToLookup(s => s);
			foreach (var provider in providers) {
				provider.IsVisible = !disabledActions.Contains(provider.GetType().FullName);
			}
		}
		
		public EditorActionsProvider GetAvailableActions(ITextEditor editor)
		{
			return new EditorActionsProvider(editor, this.providers);
		}
		
		static List<string> LoadProviderVisibilities()
		{
			return PropertyService.Get(PropertyServiceKey, new List<string>());
		}
		
		public void SaveProviderVisibilities()
		{
			List<string> disabledProviders = this.providers.Where(p => !p.IsVisible).Select(p => p.GetType().FullName).ToList();
			PropertyService.Set(PropertyServiceKey, disabledProviders);
		}
	}
	
	
	public class EditorActionsProvider
	{
		ITextEditor editor { get; set; }
		IList<IContextActionsProvider> providers { get; set; }
		
		public EditorActionsProvider(ITextEditor editor, IList<IContextActionsProvider> providers)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			if (providers == null)
				throw new ArgumentNullException("providers");
			this.editor = editor;
			this.providers = providers;
			ParserService.ParseCurrentViewContent();
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
			ContextActionsService.Instance.SaveProviderVisibilities();
		}
		
		/// <summary>
		/// For every returned action remembers its provider for so that SetVisible can work.
		/// </summary>
		Dictionary<IContextAction, IContextActionsProvider> providerForAction = new Dictionary<IContextAction, IContextActionsProvider>();

		/// <summary>
		/// Gets actions available for current caret position in the editor.
		/// </summary>
		IEnumerable<IContextAction> GetActions(IEnumerable<IContextActionsProvider> providers)
		{
			if (ParserService.LoadSolutionProjectsThreadRunning)
				yield break;
			// Not necessary to reparse here because the ContextActionsRenderer timeout 
			// is large enough for the standard parser to finish first - not completely reliable, but saves one full reparse.
			// In case the standard parser does not finish in time, the DOM is a little outdated but nothing bad happens
			// (context action can just display an old class/method).
			
			// DO NOT USE Wait on the main thread!
			// causes deadlocks!
			// parseTask.Wait();
			
			// var sw = new Stopwatch(); sw.Start();
			var editorContext = new EditorContext(this.editor);
			// long elapsedEditorContextMs = sw.ElapsedMilliseconds;
			
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
