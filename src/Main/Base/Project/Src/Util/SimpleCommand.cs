// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Simple ICommand implementation that calls a delegate.
	/// </summary>
	public sealed class SimpleCommand : ICommand
	{
		public static readonly ICommand NotAvailable = new NotAvailableCommand();
		
		class NotAvailableCommand : ICommand
		{
			public event EventHandler CanExecuteChanged { add {} remove {} }
			
			public bool CanExecute(object parameter)
			{
				return false;
			}
			
			public void Execute(object parameter)
			{
				throw new NotSupportedException();
			}
		}
		
		readonly Action<object> execute;
		
		public SimpleCommand(Action<object> execute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");
			this.execute = execute;
		}
		
		public event EventHandler CanExecuteChanged { add {} remove {} }
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
		
		public void Execute(object parameter)
		{
			this.execute(parameter);
		}
	}
}
