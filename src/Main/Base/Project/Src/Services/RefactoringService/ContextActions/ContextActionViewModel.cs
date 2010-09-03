// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ContextActionViewModel.
	/// </summary>
	public class ContextActionViewModel
	{
		public ContextActionViewModel()
		{
			this.IsVisible = true;
		}
		
		public ContextActionViewModel(IContextAction action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			this.Action = action;
		}
		
		public string Name { get { return this.action.Title; } }
		
		public string Comment { get; set; }
		
		public ImageSource Image { get; set; }
		
		/// <summary>
		/// Is this action enabled to be offered automatically?
		/// </summary>
		public bool IsVisible { get; set; }
		
		public ObservableCollection<ContextActionViewModel> ChildActions { get; set; }
		
		IContextAction action;
		/// <summary>
		/// Action that gets executed when this view model is activated by user.
		/// </summary>
		public IContextAction Action {
			get { return action; }
			set { 
				action = value; 
				ActionCommand = new ContextActionCommand(action);
			}
		}
		
		public ICommand ActionCommand { get; private set; }
	}
	
	/// <summary>
	/// Just wraps <see cref="IContextAction"></see> inside a WPF Command to be used in XAML.
	/// </summary>
	public class ContextActionCommand : ICommand
	{
		IContextAction action;
		
		public ContextActionCommand(IContextAction action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			this.action = action;
		}
		
		public event EventHandler CanExecuteChanged
		{
			// not supported - Context actions can always be executed
			add { }
			remove { }
		}
		
		public void Execute(object parameter)
		{
			this.action.Execute();
		}
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
	}
}
