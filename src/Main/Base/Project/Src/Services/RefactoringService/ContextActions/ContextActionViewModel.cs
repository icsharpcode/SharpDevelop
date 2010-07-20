// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
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
}
