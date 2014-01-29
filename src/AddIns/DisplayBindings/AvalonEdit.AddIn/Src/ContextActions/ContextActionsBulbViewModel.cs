// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ICSharpCode.SharpDevelop.Editor.ContextActions;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// Description of ContextActionsHiddenViewModel.
	/// </summary>
	public class ContextActionsBulbViewModel : ContextActionsPopupViewModel, INotifyPropertyChanged
	{
		public EditorActionsProvider Model { get; private set; }
		
		public ObservableCollection<ContextActionViewModel> HiddenActions { get; set; }
		
		bool isHiddenActionsExpanded;
		public bool IsHiddenActionsExpanded {
			get { return isHiddenActionsExpanded; }
			set {
				isHiddenActionsExpanded = value;
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("IsHiddenActionsExpanded"));
			}
		}
		
		public ContextActionsBulbViewModel(EditorActionsProvider model)
		{
			this.Model = model;
			this.Actions = new ObservableCollection<ContextActionViewModel>();
			this.HiddenActions = new ObservableCollection<ContextActionViewModel>();
			this.ActionVisibleChangedCommand = new ActionVisibleChangedCommand(model);
		}
		
		bool actionsLoaded;
		
		public async Task LoadActionsAsync(CancellationToken cancellationToken)
		{
			if (actionsLoaded)
				return;
			actionsLoaded = true;
			this.Actions.Clear();
			foreach (var action in await Model.GetVisibleActionsAsync(cancellationToken)) {
				this.Actions.Add(new ContextActionViewModel(action, this.Model.EditorContext) { IsVisible = true });
			}
		}
		
		bool hiddenActionsLoaded;
		
		public async Task LoadHiddenActionsAsync(CancellationToken cancellationToken)
		{
			if (hiddenActionsLoaded)
				return;
			hiddenActionsLoaded = true;
			this.HiddenActions.Clear();
			foreach (var action in await Model.GetHiddenActionsAsync(cancellationToken)) {
				this.HiddenActions.Add(new ContextActionViewModel(action, this.Model.EditorContext) { IsVisible = false });
			}
		}
		
		public ActionVisibleChangedCommand ActionVisibleChangedCommand { get; private set; }
		
		public event PropertyChangedEventHandler PropertyChanged;
	}
	
	public class ActionVisibleChangedCommand : ICommand
	{
		public EditorActionsProvider Model { get; private set; }
		
		public ActionVisibleChangedCommand(EditorActionsProvider model)
		{
			if (model == null)
				throw new ArgumentNullException("model");
			this.Model = model;
		}
		
		public event EventHandler CanExecuteChanged
		{
			// not supported - Checkbox can be always clicked
			add { }
			remove { }
		}
		
		public void Execute(object parameter)
		{
			var clickedAction = (ContextActionViewModel)parameter;
			this.Model.SetVisible(clickedAction.Action, clickedAction.IsVisible);
		}
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
	}
}
