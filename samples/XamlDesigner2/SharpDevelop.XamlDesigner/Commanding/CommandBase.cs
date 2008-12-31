using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SharpDevelop.XamlDesigner.Commanding
{
	public abstract class CommandBase
	{
		public string CommandName { get; internal set; }

		public virtual bool CanExecute(object parameter)
		{
			return true;
		}

		public virtual void Execute(object parameter)
		{
		}
	}
}
