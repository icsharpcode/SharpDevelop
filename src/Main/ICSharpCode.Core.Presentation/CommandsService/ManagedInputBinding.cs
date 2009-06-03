using System;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// ManagedKeyBinding is used to distinguish input bindings managed
	/// by CommandsRegistry from other input bindings not managed by CommandsRegistry. 
	/// 
	/// If input binding is not managed then CommansRegistry ignores it when 
	/// performing any action
	/// </summary>
	public class ManagedInputBinding : System.Windows.Input.InputBinding
	{
		/// <see cref="System.Windows.Input.InputBinding()"></see>
		public ManagedInputBinding() : base()
 		{ }
		
		/// <see cref="System.Windows.Input.InputBinding(System.Windows.Input.ICommand, System.Windows.Input.InputGesture)"></see>
		public ManagedInputBinding(System.Windows.Input.ICommand command, System.Windows.Input.InputGesture gesture) 
			: base(command, gesture)
 		{ }
	}
}
