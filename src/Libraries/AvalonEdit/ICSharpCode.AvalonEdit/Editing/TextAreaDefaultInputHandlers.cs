// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core.Presentation;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Contains the predefined input handlers.
	/// </summary>
	public class TextAreaDefaultInputHandler : TextAreaInputHandler
	{
		/// <summary>
		/// Gets the caret navigation input handler.
		/// </summary>
		public TextAreaInputHandler CaretNavigation { get; private set; }
		
		/// <summary>
		/// Gets the editing input handler.
		/// </summary>
		public TextAreaInputHandler Editing { get; private set; }
		
		/// <summary>
		/// Gets the mouse selection input handler.
		/// </summary>
		public ITextAreaInputHandler MouseSelection { get; private set; }
		
		public static BindingGroup ClassWideBindingGroup
		{
			get; private set;
		}
		
		private static bool bindingsRegistered;
		
		public static void RegisterBindings()
		{
			if(bindingsRegistered) return;
			bindingsRegistered = true;
			
			ClassWideBindingGroup = new BindingGroup("TextAreaDefaultInputHandler");	
			
			AddCommandBinding("ApplicationCommands.Undo", CanExecuteUndo, ExecuteUndo);
			AddCommandBinding("ApplicationCommands.Redo", CanExecuteRedo, ExecuteRedo);
			
			CaretNavigationCommandHandler.RegisterBindings();
			EditingCommandHandler.RegisterBindings();
		}
		
		static void AddCommandBinding(string routedCommandName, CanExecuteRoutedEventHandler canExecuteHandler, ExecutedRoutedEventHandler executedHandler)
		{
			var commandBinding = new CommandBindingInfo();
			commandBinding.OwnerTypeName = SDCommandManager.GetShortAssemblyQualifiedName(typeof(TextArea));
			commandBinding.ExecutedEventHandler = executedHandler;
			commandBinding.CanExecuteEventHandler = canExecuteHandler;
			commandBinding.IsLazy = false;
			commandBinding.Groups.Add(ClassWideBindingGroup);
			commandBinding.RoutedCommandName = routedCommandName;
			SDCommandManager.RegisterCommandBindingInfo(commandBinding);
		}
		
		/// <summary>
		/// Creates a new TextAreaDefaultInputHandler instance.
		/// </summary>
		public TextAreaDefaultInputHandler(TextArea textArea) : base(textArea)
		{
			BindingGroup = ClassWideBindingGroup;
			
			this.NestedInputHandlers.Add(CaretNavigation = CaretNavigationCommandHandler.Create(textArea));
			this.NestedInputHandlers.Add(Editing = EditingCommandHandler.Create(textArea));
			this.NestedInputHandlers.Add(MouseSelection = new SelectionMouseHandler(textArea));
		}
		
		#region Undo / Redo
		static UndoStack GetUndoStack(TextArea textArea)
		{
			if (textArea != null && textArea.Document != null)
				return textArea.Document.UndoStack;
			else
				return null;
		}
		
		static void ExecuteUndo(object sender, ExecutedRoutedEventArgs e)
		{
			var undoStack = GetUndoStack(sender as TextArea);
			if (undoStack != null) {
				if (undoStack.CanUndo)
					undoStack.Undo();
				e.Handled = true;
			}
		}
		
		static void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
		{
			var undoStack = GetUndoStack(sender as TextArea);
			if (undoStack != null) {
				e.Handled = true;
				e.CanExecute = undoStack.CanUndo;
			}
		}
		
		static void ExecuteRedo(object sender, ExecutedRoutedEventArgs e)
		{
			var undoStack = GetUndoStack(sender as TextArea);
			if (undoStack != null) {
				if (undoStack.CanRedo)
					undoStack.Redo();
				e.Handled = true;
			}
		}
		
		static void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
		{
			var undoStack = GetUndoStack(sender as TextArea);
			if (undoStack != null) {
				e.Handled = true;
				e.CanExecute = undoStack.CanRedo;
			}
		}
		#endregion
	}
}
