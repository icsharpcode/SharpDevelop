// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ContextActionsPopup.
	/// </summary>
	public class ContextActionsPopup : Popup
	{
		public ContextActionsPopup()
		{
			this.StaysOpen = false;
			// Close on lost focus
			this.AllowsTransparency = true;
			this.ActionsControl = new ContextActionsHeaderedControl();
			// Close when any action excecuted
			this.ActionsControl.ActionExecuted += delegate { this.Close(); };
			this.KeyDown += new KeyEventHandler(ContextActionsPopup_KeyDown);
		}

		void ContextActionsPopup_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				Close();
		}
		
		public bool IsDropdownOpen { get { return ActionsControl.IsOpen; } set {ActionsControl.IsOpen = value; } }
		public bool IsDropdownAlwaysOpen { get { return ActionsControl.IsAlwaysOpen; } set {ActionsControl.IsAlwaysOpen = value; } }
		
		ContextActionsHeaderedControl ActionsControl
		{
			get { return (ContextActionsHeaderedControl)this.Child; }
			set { this.Child = value; }
		}
		
		public ContextActionsViewModel Actions
		{
			get { return (ContextActionsViewModel)ActionsControl.DataContext; }
			set { 
				ActionsControl.DataContext = value; 
			}
		}
		
		public new void Focus()
		{
			this.ActionsControl.Focus();
		}
		
		public void Open()
		{
			this.IsOpen = true;
		}
		
		public void Close()
		{
			this.IsOpen = false;
		}
		
		public void OpenAtCaretAndFocus()
		{
			OpenAtMousePosition();
			//OpenAtPosition(editor, editor.Caret.Line, editor.Caret.Column, true);
			this.Focus();
		}
		
		public void OpenAtLineStart(ITextEditor editor)
		{
			OpenAtPosition(editor, editor.Caret.Line, 1, false);
			this.VerticalOffset -= 16;
		}
		
		void OpenAtMousePosition()
		{
			this.Placement = PlacementMode.MousePoint;
			this.Open();
		}
		
		void OpenAtPosition(ITextEditor editor, int line, int column, bool openAtWordStart)
		{
			var editorUIService = editor == null ? null : editor.GetService(typeof(IEditorUIService)) as IEditorUIService;
			if (editorUIService != null) {
				var document = editor.Document;
				int offset = document.PositionToOffset(line, column);
				if (openAtWordStart) {
					int wordStart = document.FindPrevWordStart(offset);
					if (wordStart != -1) {
						var wordStartLocation = document.OffsetToPosition(wordStart);
						line = wordStartLocation.Line;
						column = wordStartLocation.Column;
					}
				}
				this.Placement = PlacementMode.Absolute;
				try
				{
					var caretScreenPos = editorUIService.GetScreenPosition(line, column);
					this.HorizontalOffset = caretScreenPos.X;
					this.VerticalOffset = caretScreenPos.Y;
				}
				catch
				{
					this.Placement = PlacementMode.MousePoint;
				}
				
			} else {
				// if no editor information, open at mouse positions
				this.Placement = PlacementMode.MousePoint;
			}
			this.Open();
		}
	}
}
