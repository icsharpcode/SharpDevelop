// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class NavigateBack : ICommand
	{
		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		
		public bool CanExecute(object parameter)
		{
			return NavigationService.CanNavigateBack;
		}
		
		public void Execute(object parameter)
		{
			NavigationService.Go(-1);
		}
	}
	
	public class NavigateForward : ICommand
	{
		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		
		public bool CanExecute(object parameter)
		{
			return NavigationService.CanNavigateForwards;
		}
		
		public void Execute(object parameter)
		{
			NavigationService.Go(+1);
		}
	}
}
