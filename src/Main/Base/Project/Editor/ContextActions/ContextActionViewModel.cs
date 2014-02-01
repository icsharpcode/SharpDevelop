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
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.ContextActions
{
	/// <summary>
	/// ViewModel for a <see cref="ContextAction"/>.
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
			get { return this.action != null ? this.action.GetDisplayName(context) : string.Empty; }
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
	/// Just wraps <see cref="IContextAction"/> inside a WPF Command to be used in XAML.
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
		
		public IContextAction ContextAction
		{
			get { return action; }
		}
		
		public void Execute(object parameter)
		{
			if (action.Provider != null)
				SD.AnalyticsMonitor.TrackFeature(action.Provider.ID);
			this.action.Execute(context);
		}
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
	}
}
