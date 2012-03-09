// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Provides context actions available for current line of the editor.
	/// </summary>
	public sealed class ContextActionsService
	{
		static readonly ContextActionsService instance = new ContextActionsService();
		
		/// <summary>
		/// Key for storing the names of disabled providers in PropertyService.
		/// </summary>
		const string PropertyServiceKey = "DisabledContextActionProviders";
		
		public static ContextActionsService Instance {
			get { return instance; }
		}
		
		private ContextActionsService()
		{
		}
		
		public EditorActionsProvider CreateActionsProvider(ITextEditor editor)
		{
			var editorContext = new EditorContext(editor);
			var providers = AddInTree.BuildItems<IContextActionsProvider>("/SharpDevelop/ViewContent/AvalonEdit/ContextActions", editorContext, false);
			var disabledActions = new HashSet<string>(LoadProviderVisibilities());
			foreach (var provider in providers) {
				provider.IsVisible = !disabledActions.Contains(provider.ID);
			}
			return new EditorActionsProvider(editorContext, providers);
		}
		
		static List<string> LoadProviderVisibilities()
		{
			return PropertyService.Get(PropertyServiceKey, new List<string>());
		}
		
		public void SaveProviderVisibilities(IEnumerable<IContextActionsProvider> providers)
		{
			List<string> disabledProviders = providers.Where(p => !p.IsVisible).Select(p => p.ID).ToList();
			PropertyService.Set(PropertyServiceKey, disabledProviders);
		}
	}
	
	/// <summary>
	/// Returned by <see cref="ContextActionsService.GetAvailableActions()"></see>.
	/// </summary>
	public class EditorActionsProvider
	{
		readonly IList<IContextActionsProvider> providers;
		readonly EditorContext editorContext;
		
		public EditorContext EditorContext {
			get { return editorContext; }
		}
		
		public EditorActionsProvider(EditorContext editorContext, IList<IContextActionsProvider> providers)
		{
			if (editorContext == null)
				throw new ArgumentNullException("editorContext");
			if (providers == null)
				throw new ArgumentNullException("providers");
			this.providers = providers;
			this.editorContext = editorContext;
		}
		
		public Task<IEnumerable<IContextAction>> GetVisibleActionsAsync(CancellationToken cancellationToken)
		{
			return GetActionsAsync(this.providers.Where(p => p.IsVisible), cancellationToken);
		}
		
		public Task<IEnumerable<IContextAction>> GetHiddenActionsAsync(CancellationToken cancellationToken)
		{
			return GetActionsAsync(this.providers.Where(p => !p.IsVisible), cancellationToken);
		}
		
		public void SetVisible(IContextAction action, bool isVisible)
		{
			IContextActionsProvider provider;
			if (providerForAction.TryGetValue(action, out provider)) {
				provider.IsVisible = isVisible;
			}
			ContextActionsService.Instance.SaveProviderVisibilities(providers);
		}
		
		/// <summary>
		/// For every returned action remembers its provider for so that SetVisible can work.
		/// </summary>
		Dictionary<IContextAction, IContextActionsProvider> providerForAction = new Dictionary<IContextAction, IContextActionsProvider>();

		/// <summary>
		/// Gets actions available for current caret position in the editor.
		/// </summary>
		async Task<IEnumerable<IContextAction>> GetActionsAsync(IEnumerable<IContextActionsProvider> providers, CancellationToken cancellationToken)
		{
			if (ParserService.LoadSolutionProjectsThreadRunning)
				return EmptyList<IContextAction>.Instance;
			var providerList = providers.ToList();
			var actions = await Task.WhenAll(providerList.Select(p => p.GetAvailableActionsAsync(this.EditorContext, cancellationToken)));
			for (int i = 0; i < actions.Length; i++) {
				if (actions[i] != null) {
					foreach (var action in actions[i]) {
						providerForAction[action] = providerList[i];
					}
				}
			}
			return actions.Where(a => a != null).SelectMany(a => a);
		}
	}
}
