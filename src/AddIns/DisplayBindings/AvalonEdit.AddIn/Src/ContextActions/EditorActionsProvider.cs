// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// Provides context actions available for current line of the editor.
	/// </summary>
	public class EditorActionsProvider
	{
		/// <summary>
		/// Key for storing the names of disabled providers in PropertyService.
		/// </summary>
		const string PropertyServiceKey = "DisabledContextActionProviders";
		
		internal static void LoadProviderVisibilities(IEnumerable<IContextActionProvider> providers)
		{
			var disabledActions = new HashSet<string>(PropertyService.GetList<string>(PropertyServiceKey));
			foreach (var provider in providers) {
				provider.IsVisible = !(provider.AllowHiding && disabledActions.Contains(provider.ID));
			}
		}
		
		internal static void SaveProviderVisibilities(IEnumerable<IContextActionProvider> providers)
		{
			PropertyService.SetList(PropertyServiceKey, providers.Where(p => !p.IsVisible).Select(p => p.ID));
		}
		
		readonly IList<IContextActionProvider> providers;
		readonly EditorRefactoringContext editorContext;
		
		public EditorRefactoringContext EditorContext {
			get { return editorContext; }
		}
		
		public EditorActionsProvider(EditorRefactoringContext editorContext, IList<IContextActionProvider> providers)
		{
			if (editorContext == null)
				throw new ArgumentNullException("editorContext");
			if (providers == null)
				throw new ArgumentNullException("providers");
			LoadProviderVisibilities(providers);
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
			IContextActionProvider provider;
			if (providerForAction.TryGetValue(action, out provider)) {
				provider.IsVisible = isVisible;
			}
			SaveProviderVisibilities(providers);
		}
		
		/// <summary>
		/// For every returned action remembers its provider for so that SetVisible can work.
		/// </summary>
		Dictionary<IContextAction, IContextActionProvider> providerForAction = new Dictionary<IContextAction, IContextActionProvider>();

		/// <summary>
		/// Gets actions available for current caret position in the editor.
		/// </summary>
		async Task<IEnumerable<IContextAction>> GetActionsAsync(IEnumerable<IContextActionProvider> providers, CancellationToken cancellationToken)
		{
			if (SD.ParserService.LoadSolutionProjectsThread.IsRunning)
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
