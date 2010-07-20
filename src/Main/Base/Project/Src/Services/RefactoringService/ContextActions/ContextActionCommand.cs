// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Just wraps <see cref="IContextAction"></see> inside a WPF Command to be used in XAML.
	/// </summary>
	public class ContextActionCommand : ICommand
	{
		IContextAction action;
		
		public ContextActionCommand(IContextAction action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			this.action = action;
		}
		
		public event EventHandler CanExecuteChanged
		{
			// not supported - Context actions can always be executed
			add { }
			remove { }
		}
		
		public void Execute(object parameter)
		{
			this.action.Execute();
		}
		
		public bool CanExecute(object parameter)
		{
			return true;
		}
	}
}
