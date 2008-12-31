using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SharpDevelop.XamlDesigner.Commanding;

namespace SharpDevelop.XamlDesigner
{
	public class ViewModel : INotifyPropertyChanged, IHasCommands
	{
		public ViewModel()
		{
			Commands = new List<CommandBase>();
		}

		public List<CommandBase> Commands { get; private set; }
		public event PropertyChangedEventHandler PropertyChanged;

		public void RaisePropertyChanged(string name)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(name));
			}
		}

		public void AddCommand(string commandName, Action execute)
		{
			AddCommand(commandName, execute, null);
		}

		public void AddCommand(string commandName, Action execute, Func<bool> canExecute)
		{
			Commands.Add(new MethodCommand(commandName, execute, canExecute));
		}
	}
}
