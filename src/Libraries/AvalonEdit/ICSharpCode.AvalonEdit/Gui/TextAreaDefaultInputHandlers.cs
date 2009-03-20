// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Gui
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
			if (undoStack != null && undoStack.CanUndo)
				undoStack.Undo();
		}
		
		void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			e.CanExecute = undoStack != null && undoStack.CanUndo;
		}
		
		void ExecuteRedo(object sender, ExecutedRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			if (undoStack != null && undoStack.CanRedo)
				undoStack.Redo();
		}
		
		void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
		{
			var undoStack = GetUndoStack();
			e.CanExecute = undoStack != null && undoStack.CanRedo;
		}
		#endregion
	}
}
