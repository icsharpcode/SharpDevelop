// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.ContextActions;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// Pop-up menu for context actions; used for Find derived classes (F6) and commands that open popups.
	/// </summary>
	public class ContextActionsPopup : Popup
	{
		public ContextActionsPopup()
		{
			this.UseLayoutRounding = true;
			TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
			
			// Close on lost focus
			this.StaysOpen = false;
			this.AllowsTransparency = true;
			this.ActionsControl = new ContextActionsHeaderedControl();
			// Close when any action excecuted
			this.ActionsControl.ActionExecuted += delegate { this.IsOpen = false; };
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Key == Key.Escape)
				this.IsOpen = false;
		}
		
		public ContextActionsHeaderedControl ActionsControl
		{
			get { return (ContextActionsHeaderedControl)this.Child; }
			set { this.Child = value; }
		}
		
		public ContextActionsPopupViewModel Actions
		{
			get { return (ContextActionsPopupViewModel)ActionsControl.DataContext; }
			set {
				ActionsControl.DataContext = value;
			}
		}
		
		public new void Focus()
		{
			this.ActionsControl.Focus();
		}
		
		public void OpenAtCaretAndFocus()
		{
			ITextEditor currentEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (currentEditor == null) {
				this.Placement = PlacementMode.MousePoint;
			} else {
				SetPosition(this, currentEditor, currentEditor.Caret.Line, currentEditor.Caret.Column, true);
			}
			this.IsOpen = true;
			this.Focus();
		}
		
		public void OpenAtCursorAndFocus()
		{
			this.Placement = PlacementMode.MousePoint;
			this.IsOpen = true;
			this.Focus();
		}
		
		public static void SetPosition(Popup popup, ITextEditor editor, int line, int column, bool openAtWordStart = false)
		{
			var editorUIService = editor == null ? null : editor.GetService(typeof(IEditorUIService)) as IEditorUIService;
			if (editorUIService != null) {
				var document = editor.Document;
				int offset = document.GetOffset(line, column);
				if (openAtWordStart) {
					int wordStart = document.FindPrevWordStart(offset);
					if (wordStart != -1) {
						var wordStartLocation = document.GetLocation(wordStart);
						line = wordStartLocation.Line;
						column = wordStartLocation.Column;
					}
				}
				var caretScreenPos = editorUIService.GetScreenPosition(line, column);
				popup.HorizontalOffset = caretScreenPos.X;
				popup.VerticalOffset = caretScreenPos.Y;
				popup.Placement = PlacementMode.Absolute;
			} else {
				// if no editor information, open at mouse positions
				popup.Placement = PlacementMode.MousePoint;
			}
		}
	}
}
