// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ContextActionsHiddenViewModel.
	/// </summary>
	public class ContextActionsHiddenViewModel : ContextActionsViewModel
	{
		public EditorActionsProvider Model { get; private set; }
		
		public ObservableCollection<ContextActionViewModel> HiddenActions { get; set; }
		
		public ContextActionsHiddenViewModel(EditorActionsProvider model)
		{
			this.Model = model;
			this.Actions = new ObservableCollection<ContextActionViewModel>(
				model.GetVisibleActions().Select(a => new ContextActionViewModel(a) { IsVisible = true } ));
			this.HiddenActions = new ObservableCollection<ContextActionViewModel>();
		}
		
		bool hiddenActionsLoaded = false;
		
		public void LoadHiddenActions()
		{
			if (hiddenActionsLoaded)
				return;
			
			this.HiddenActions.AddRange(
				Model.GetHiddenActions().Select(a => new ContextActionViewModel(a) { IsVisible = false } ));
			hiddenActionsLoaded = true;
		}
	}
}
