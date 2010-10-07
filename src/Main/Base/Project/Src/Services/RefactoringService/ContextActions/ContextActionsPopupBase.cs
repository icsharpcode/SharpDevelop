// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ContextActionsPopupBase.
	/// </summary>
	public abstract class ContextActionsPopupBase : ExtendedPopup
	{
		protected ContextActionsPopupBase()
		{
			this.KeyDown += OnKeyDown;
			this.UseLayoutRounding = true;
			TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
		}
		
		void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				Close();
		}
		
		public void Open()
		{
			this.IsOpen = true;
		}
		
		public void Close()
		{
			this.IsOpen = false;
		}
		
		protected void OpenAtPosition(ITextEditor editor, int line, int column, bool openAtWordStart)
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
