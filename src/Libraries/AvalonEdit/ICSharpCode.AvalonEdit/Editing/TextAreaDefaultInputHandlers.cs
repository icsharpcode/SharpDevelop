// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;

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
		
		/// <summary>
		/// Creates a new TextAreaDefaultInputHandler instance.
		/// </summary>
		public TextAreaDefaultInputHandler(TextArea textArea) : base(textArea)
		{
			this.NestedInputHandlers.Add(CaretNavigation = CaretNavigationCommandHandler.Create(textArea));
			this.NestedInputHandlers.Add(Editing = EditingCommandHandler.Create(textArea));
			this.NestedInputHandlers.Add(MouseSelection = new SelectionMouseHandler(textArea));
			
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, ExecuteUndo, CanExecuteUndo));
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, ExecuteRedo, CanExecuteRedo));
		}
		
		#region Undo / Redo
		UndoStack GetUndoStack()
		{
			TextDocument document = this.TextArea.Document;
			if (document != null)
				return document.UndoStack;
			else
				return null;
		}
		
		void ExecuteUndo(object sender, ExecutedRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			if (undoStack != null) {
				if (undoStack.CanUndo)
					undoStack.Undo();
				e.Handled = true;
			}
		}
		
		void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			if (undoStack != null) {
				e.Handled = true;
				e.CanExecute = undoStack.CanUndo;
			}
		}
		
		void ExecuteRedo(object sender, ExecutedRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			if (undoStack != null) {
				if (undoStack.CanRedo)
					undoStack.Redo();
				e.Handled = true;
			}
		}
		
		void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			if (undoStack != null) {
				e.Handled = true;
				e.CanExecute = undoStack.CanRedo;
			}
		}
		#endregion
	}
}
