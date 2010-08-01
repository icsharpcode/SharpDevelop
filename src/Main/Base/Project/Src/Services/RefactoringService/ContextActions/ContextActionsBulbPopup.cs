// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Description of ContextActionsBulbPopup.
	/// </summary>
	public class ContextActionsBulbPopup : ContextActionsPopupBase
	{
		public ContextActionsBulbPopup()
		{
			this.StaysOpen = true;
			this.AllowsTransparency = true;
			this.ActionsControl = new ContextActionsBulbControl();
			// Close when any action excecuted
			this.ActionsControl.ActionExecuted += delegate { this.Close(); };
		}
		
		public ContextActionsBulbControl ActionsControl
		{
			get { return (ContextActionsBulbControl)this.Child; }
			set { this.Child = value; }
		}
		
		public ContextActionsViewModel Actions
		{
			get { return (ContextActionsViewModel)ActionsControl.DataContext; }
			set { 
				ActionsControl.DataContext = value; 
			}
		}
		
		public bool IsDropdownOpen { get { return ActionsControl.IsOpen; } set {ActionsControl.IsOpen = value; } }
		
		public new void Focus()
		{
			this.ActionsControl.Focus();
		}
		
		public void OpenAtLineStart(ITextEditor editor)
		{
			OpenAtPosition(editor, editor.Caret.Line, 1, false);
			this.VerticalOffset -= 16;
		}
	}
}
