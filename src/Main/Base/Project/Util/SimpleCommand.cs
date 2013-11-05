// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Base class for ICommand implementations.
	/// </summary>
	public abstract class SimpleCommand : ICommand
	{
		public virtual event EventHandler CanExecuteChanged { add {} remove {} }
		
		public virtual bool CanExecute(object parameter)
		{
			return true;
		}
		
		public abstract void Execute(object parameter);
	}
}
