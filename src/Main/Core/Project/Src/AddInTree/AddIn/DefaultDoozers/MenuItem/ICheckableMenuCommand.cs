// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;

namespace ICSharpCode.Core
{
	public interface ICheckableMenuCommand : ICommand
	{
		bool IsChecked(object parameter);
		
		/// <summary>
		/// Event that occurs when the checked state of the command changes.
		/// Warning: this is a weak event like <c>ICommand.CanExecuteChanged</c>!
		/// Subscribers need to ensure they keep a reference to the delegate alive.
		/// Implementers need to ensure they use a weak reference to the delegate.
		/// </summary>
		event EventHandler IsCheckedChanged;
	}
}
