// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A command that never is enabled.
	/// </summary>
	public class NotAvailableCommand : ICommand
	{
		public static readonly ICommand Instance = new NotAvailableCommand();
		
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
}
