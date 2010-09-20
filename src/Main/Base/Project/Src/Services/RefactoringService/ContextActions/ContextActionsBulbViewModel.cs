// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ContextActionsHiddenViewModel.
	/// </summary>
	public class ContextActionsBulbViewModel : ContextActionsViewModel, INotifyPropertyChanged
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
			this.Actions = new ObservableCollection<ContextActionViewModel>(
				model.GetVisibleActions().Select(a => new ContextActionViewModel(a) { IsVisible = true } ));
			this.HiddenActions = new ObservableCollection<ContextActionViewModel>();
			this.ActionVisibleChangedCommand = new ActionVisibleChangedCommand(model);
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
