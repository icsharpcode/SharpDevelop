// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop.Widgets
{
	/// <summary>
	/// A command that invokes a delegate.
	/// The command parameter must be of type T.
	/// </summary>
	public class RelayCommand<T> : ICommand
	{
		readonly Predicate<T> canExecute;
		readonly Action<T> execute;

		public RelayCommand(Action<T> execute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");
			this.execute = execute;
		}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged {
			add {
				if (canExecute != null)
					CommandManager.RequerySuggested += value;
			}
			remove {
				if (canExecute != null)
					CommandManager.RequerySuggested -= value;
			}
		}

		public bool CanExecute(object parameter)
		{
			if (parameter != null && !(parameter is T))
				return false;
			return canExecute == null ? true : canExecute((T)parameter);
		}

		public void Execute(object parameter)
		{
			execute((T)parameter);
		}
	}

	/// <summary>
	/// A command that invokes a delegate.
	/// This class does not provide the command parameter to the delegate -
	/// if you need that, use the generic version of this class instead.
	/// </summary>
	public class RelayCommand : ICommand
	{
		readonly Func<bool> canExecute;
		readonly Action execute;

		public RelayCommand(Action execute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");
			this.execute = execute;
		}

		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");
			this.execute = execute;
			this.canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged {
			add {
				if (canExecute != null)
					CommandManager.RequerySuggested += value;
			}
			remove {
				if (canExecute != null)
					CommandManager.RequerySuggested -= value;
			}
		}

		public bool CanExecute(object parameter)
		{
			return canExecute == null ? true : canExecute();
		}

		public void Execute(object parameter)
		{
			execute();
		}
	}
}
