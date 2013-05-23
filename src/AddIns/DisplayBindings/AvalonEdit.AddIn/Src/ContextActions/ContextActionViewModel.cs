// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// Description of ContextActionViewModel.
	/// </summary>
	public class ContextActionViewModel
	{
		readonly EditorRefactoringContext context;
		
		public ContextActionViewModel()
		{
			this.IsVisible = true;
		}
		
		public ContextActionViewModel(IContextAction action, EditorRefactoringContext context)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			this.IsVisible = true;
			this.context = context;
			this.Action = action;
		}
		
		public string Name {
			get { return this.action != null ? this.action.DisplayName : string.Empty; }
		}
		
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
				ActionCommand = new ContextActionCommand(action, context);
			}
		}
		
		public ICommand ActionCommand { get; private set; }
	}
	
	/// <summary>
	/// Just wraps <see cref="IContextAction"></see> inside a WPF Command to be used in XAML.
	/// </summary>
	public class ContextActionCommand : ICommand
	{
		readonly IContextAction action;
		readonly EditorRefactoringContext context;
		
		public ContextActionCommand(IContextAction action, EditorRefactoringContext context)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			this.action = action;
			this.context = context;
		}
		
		public event EventHandler CanExecuteChanged
		{
			// not supported - Context actions can always be executed
			add { }
			remove { }
		}
		
		public void Execute(object parameter)
		{
			this.action.Execute(context);
		}
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
	}
}
