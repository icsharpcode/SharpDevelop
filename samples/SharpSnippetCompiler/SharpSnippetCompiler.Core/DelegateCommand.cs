// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;

namespace ICSharpCode.SharpSnippetCompiler.Core
{
	public class DelegateCommand : ICommand
	{
		Action<object> execute;
		Predicate<object> canExecute;
		
		public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
		{
			this.execute = execute;
			this.canExecute = canExecute;
		}
		
		public DelegateCommand(Action<object> execute)
			: this(execute, null)
		{
		}
		
		public event EventHandler CanExecuteChanged {
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}
		
		public void Execute(object parameter)
		{
			execute(parameter);
		}
		
		public bool CanExecute(object parameter)
		{
			if (canExecute != null) {
				return canExecute(parameter);
			}
			return true;
		}
	}
}
