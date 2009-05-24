// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// A set of input bindings and event handlers for the text area.
	/// </summary>
	public interface ITextAreaInputHandler
	{
		/// <summary>
		/// Gets the text area that the input handler belongs to.
		/// </summary>
		TextArea TextArea {
			get;
		}
		
		/// <summary>
		/// Attaches an input handler to the text area.
		/// </summary>
		void Attach();
		
		/// <summary>
		/// Detaches the input handler from the text area.
		/// </summary>
		void Detach();
	}
	
	/// <summary>
	/// Default-implementation of <see cref="ITextAreaInputHandler"/>.
	/// </summary>
	public class TextAreaInputHandler : ITextAreaInputHandler
	{
		readonly ObserveAddRemoveCollection<CommandBinding> commandBindings;
		readonly ObserveAddRemoveCollection<InputBinding> inputBindings;
		readonly ObserveAddRemoveCollection<ITextAreaInputHandler> nestedInputHandlers;
		readonly TextArea textArea;
		bool isAttached;
		
		/// <summary>
		/// Creates a new TextAreaInputHandler.
		/// </summary>
		public TextAreaInputHandler(TextArea textArea)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.textArea = textArea;
			commandBindings = new ObserveAddRemoveCollection<CommandBinding>(CommandBinding_Added, CommandBinding_Removed);
			inputBindings = new ObserveAddRemoveCollection<InputBinding>(InputBinding_Added, InputBinding_Removed);
			nestedInputHandlers = new ObserveAddRemoveCollection<ITextAreaInputHandler>(NestedInputHandler_Added, NestedInputHandler_Removed);
		}
		
		/// <inheritdoc/>
		public TextArea TextArea {
			get { return textArea; }
		}
		
		/// <summary>
		/// Gets whether the input handler is currently attached to the text area.
		/// </summary>
		public bool IsAttached {
			get { return isAttached; }
		}
		
		#region CommandBindings / InputBindings
		/// <summary>
		/// Gets the command bindings of this input handler.
		/// </summary>
		public ICollection<CommandBinding> CommandBindings {
			get { return commandBindings; }
		}
		
		void CommandBinding_Added(CommandBinding commandBinding)
		{
			if (isAttached)
				textArea.CommandBindings.Add(commandBinding);
		}
		
		void CommandBinding_Removed(CommandBinding commandBinding)
		{
			if (isAttached)
				textArea.CommandBindings.Remove(commandBinding);
		}
		
		/// <summary>
		/// Gets the input bindings of this input handler.
		/// </summary>
		public ICollection<InputBinding> InputBindings {
			get { return inputBindings; }
		}
		
		void InputBinding_Added(InputBinding inputBinding)
		{
			if (isAttached)
				textArea.InputBindings.Add(inputBinding);
		}
		
		void InputBinding_Removed(InputBinding inputBinding)
		{
			if (isAttached)
				textArea.InputBindings.Remove(inputBinding);
		}
		
		/// <summary>
		/// Adds a command and input binding.
		/// </summary>
		/// <param name="command">The command ID.</param>
		/// <param name="modifiers">The modifiers of the keyboard shortcut.</param>
		/// <param name="key">The key of the keyboard shortcut.</param>
		/// <param name="handler">The event handler to run when the command is executed.</param>
		public void AddBinding(ICommand command, ModifierKeys modifiers, Key key, ExecutedRoutedEventHandler handler)
		{
			this.CommandBindings.Add(new CommandBinding(command, handler));
			this.InputBindings.Add(new KeyBinding(command, key, modifiers));
		}
		#endregion
		
		#region NestedInputHandlers
		/// <summary>
		/// Gets the collection of nested input handlers. NestedInputHandlers are activated and deactivated
		/// together with this input handler.
		/// </summary>
		public ICollection<ITextAreaInputHandler> NestedInputHandlers {
			get { return nestedInputHandlers; }
		}
		
		void NestedInputHandler_Added(ITextAreaInputHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler");
			if (handler.TextArea != textArea)
				throw new ArgumentException("The nested handler must be working for the same text area!");
			if (isAttached)
				handler.Attach();
		}
		
		void NestedInputHandler_Removed(ITextAreaInputHandler handler)
		{
			if (isAttached)
				handler.Detach();
		}
		#endregion
		
		#region Attach/Detach
		/// <inheritdoc/>
		public virtual void Attach()
		{
			if (isAttached)
				throw new InvalidOperationException("Input handler is already attached");
			isAttached = true;
			
			textArea.CommandBindings.AddRange(commandBindings);
			textArea.InputBindings.AddRange(inputBindings);
			foreach (ITextAreaInputHandler handler in nestedInputHandlers)
				handler.Attach();
		}
		
		/// <inheritdoc/>
		public virtual void Detach()
		{
			if (!isAttached)
				throw new InvalidOperationException("Input handler is not attached");
			isAttached = false;
			
			foreach (CommandBinding b in commandBindings)
				textArea.CommandBindings.Remove(b);
			foreach (InputBinding b in inputBindings)
				textArea.InputBindings.Remove(b);
			foreach (ITextAreaInputHandler handler in nestedInputHandlers)
				handler.Detach();
		}
		#endregion
	}
}
