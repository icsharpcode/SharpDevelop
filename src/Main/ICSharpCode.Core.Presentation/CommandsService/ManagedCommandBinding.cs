using System;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// ManagedCommandBinding is used to distinguish command bindings managed
	/// by CommandsRegistry from other command bindings not managed by CommandsRegistry 
	/// 
	/// If command binding is not managed then CommandsRegistry ignores it when 
	/// performing any action
	/// </summary>
	public class ManagedCommandBinding : System.Windows.Input.CommandBinding
	{
		/// <see cref="System.Windows.Input.CommandBinding()" />
		public ManagedCommandBinding() 
			: base()
		{ }
		
		/// <see cref="System.Windows.Input.CommandBinding(System.Windows.Input.ICommand)" />
		public ManagedCommandBinding(System.Windows.Input.ICommand command) 
			: base(command)
		{ }
		
		/// <see cref="System.Windows.Input.CommandBinding(System.Windows.Input.ICommand, System.Windows.Input.ExecutedRoutedEventHandler)" />
		public ManagedCommandBinding(System.Windows.Input.ICommand command, System.Windows.Input.ExecutedRoutedEventHandler executed) 
			: base(command, executed)
		{ }
		
		/// <see cref="System.Windows.Input.CommandBinding(System.Windows.Input.ICommand, System.Windows.Input.ExecutedRoutedEventHandler, System.Windows.Input.CanExecuteRoutedEventHandler)" />
		public ManagedCommandBinding(System.Windows.Input.ICommand command, System.Windows.Input.ExecutedRoutedEventHandler executed, System.Windows.Input.CanExecuteRoutedEventHandler canExecute) 
			: base(command, executed, canExecute)
		{ }
	}
}
