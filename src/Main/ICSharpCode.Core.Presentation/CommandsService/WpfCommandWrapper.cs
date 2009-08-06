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
		/// Creates instance of <see cref="WpfCommandWrapper" />
		/// </summary>
		/// <param name="command">SharpDevelop native command</param>
		public WpfCommandWrapper(ICSharpCode.Core.ICommand command)
		{
			this.command = command;
		}
		
		/// <summary>
		/// Occurs when <see cref="CanExecute" /> returned value changes
		/// 
		/// Not used because SharpDevelop's native command implementation
		/// doesn't support it
		/// </summary>
		public event EventHandler CanExecuteChanged {
			add { }
			remove { }
		}
		
		/// <inheritdoc />
		public void Execute(object parameter)
		{
			command.Run();
		}
		
		/// <inheritdoc />
		public bool CanExecute(object parameter)
		{
			return true;
		}
	}
}
