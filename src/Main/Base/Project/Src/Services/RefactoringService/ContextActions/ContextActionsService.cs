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
		
		static ContextActionsService() {}
		
		/// <summary>
		/// Key for storing the names of disabled providers in PropertyService.
		/// </summary>
		const string PropertyServiceKey = "DisabledContextActionProviders";
		
		public static ContextActionsService Instance {
			get { return instance; }
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
		
		public EditorActionsProvider CreateActionsProvider(ITextEditor editor)
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
	
	/// <summary>
	/// Returned by <see cref="ContextActionsService.GetAvailableActions()"></see>.
	/// </summary>
	public class EditorActionsProvider
	{
		readonly IList<IContextActionsProvider> providers;
		public EditorContext EditorContext { get; set; }
		
		public EditorActionsProvider(ITextEditor editor, IList<IContextActionsProvider> providers)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			if (providers == null)
				throw new ArgumentNullException("providers");
			this.providers = providers;
			
			this.EditorContext = new EditorContext(editor);
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
			ContextActionsService.Instance.SaveProviderVisibilities();
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
				foreach (var action in actions[i]) {
					providerForAction[action] = providerList[i];
				}
			}
			return actions.SelectMany(_ => _);
		}
	}
}
