using System;
using System.Windows.Input;
using ICSharpCode.Core;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Wraps SharpDevelop's native command inside WPF command
	/// </summary>
	public class WpfCommandWrapper : System.Windows.Input.ICommand
	{
		ICommand command;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="command">SharpDevelop native command</param>
		public WpfCommandWrapper(ICommand command)
		{
			this.command = command;
		}
		
		/// <see cref="System.Windows.Input.ICommand.CanExecuteChanged">
		/// Not used because SharpDevelop's native command implementation
		/// doesn't support it
		/// </see>
		public event EventHandler CanExecuteChanged;
		
		/// <see cref="System.Windows.Input.ICommand.Execute(object)" />
		public void Execute(object parameter)
		{
			command.Run();
		}
		
		/// <see cref="System.Windows.Input.ICommand.CanExecute(object)" />
		public bool CanExecute(object parameter)
		{
			return true;
		}
	}
}
