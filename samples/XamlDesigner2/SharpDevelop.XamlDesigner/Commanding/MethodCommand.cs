using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpDevelop.XamlDesigner.Commanding
{
	public class MethodCommand : CommandBase
	{
		public MethodCommand(string commandName, Action execute)
			: this(commandName, execute, null)
		{
		}

		public MethodCommand(string commandName, Action execute, Func<bool> canExecute)
		{
			CommandName = commandName;
			ExecuteDelegate = execute;
			CanExecuteDelegate = canExecute;
		}

		public Action ExecuteDelegate { get; set; }
		public Func<bool> CanExecuteDelegate { get; set; }

		public override bool CanExecute(object parameter)
		{
			if (CanExecuteDelegate != null) {
				return CanExecuteDelegate();
			}
			return true;
		}

		public override void Execute(object parameter)
		{
			ExecuteDelegate();
		}
	}
}
