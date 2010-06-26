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
	/// Description of ContextActionCommand.
	/// </summary>
	public class ContextActionCommand : ICommand
	{
		ContextAction action;
		
		public ContextActionCommand(ContextAction action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			this.action = action;
		}
		
		public event EventHandler CanExecuteChanged
		{
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
