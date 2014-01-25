// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
