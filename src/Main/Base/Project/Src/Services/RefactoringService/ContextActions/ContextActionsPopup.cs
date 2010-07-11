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
			this.ActionsControl = new ContextActionsControl();
			// Close when any action excecuted
			this.ActionsControl.ActionExecuted += delegate { this.Close(); };
			this.KeyDown += new KeyEventHandler(ContextActionsPopup_KeyDown);
		}

		void ContextActionsPopup_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				Close();
		}
		
		private ContextActionsControl ActionsControl 
		{
			get { return (ContextActionsControl)this.Child; }
			set { this.Child = value; }
		}
		
		public ContextActionsViewModel Actions 
		{
			get { return (ContextActionsViewModel)ActionsControl.DataContext; }
			set { ActionsControl.DataContext = value; }
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
		
		public void Open(ITextEditor editor)
		{
			var editorUIService = editor.GetService(typeof(IEditorUIService)) as IEditorUIService;
			if (editorUIService != null) {
				var document = editor.Document;
				int line = editor.Caret.Line;
				int column = editor.Caret.Column;
				int offset = document.PositionToOffset(line, column);
				int wordStart = document.FindPrevWordStart(offset);
				if (wordStart != -1) {
					var wordStartLocation = document.OffsetToPosition(wordStart);
					line = wordStartLocation.Line;
					column = wordStartLocation.Column;
				}
				var caretScreenPos = editorUIService.GetScreenPosition(line, column);
				this.Placement = PlacementMode.Absolute;
				this.HorizontalOffset = caretScreenPos.X;
				this.VerticalOffset = caretScreenPos.Y;
			} else {
				this.HorizontalOffset = 200;
				this.VerticalOffset = 200;
			}
			this.Open();
			this.Focus();
		}
	}
}
