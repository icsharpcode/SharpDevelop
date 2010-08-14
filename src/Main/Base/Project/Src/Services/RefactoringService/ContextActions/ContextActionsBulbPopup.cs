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
			this.ChildControl = new ContextActionsBulbControl();
			// Close when any action excecuted
			this.ChildControl.ActionExecuted += delegate { this.Close(); };
		}
		
		private new ContextActionsBulbControl ChildControl
		{
			get { return (ContextActionsBulbControl)this.Child; }
			set { this.Child = value; }
		}
		
		public ContextActionsHiddenViewModel ViewModel
		{
			get { return (ContextActionsHiddenViewModel)this.DataContext; }
			set { this.DataContext = value; }
		}
		
		public bool IsDropdownOpen { get { return ChildControl.IsOpen; } set {ChildControl.IsOpen = value; } }
		
		public bool IsHiddenActionsExpanded { get { return ChildControl.IsHiddenActionsExpanded; } set {ChildControl.IsHiddenActionsExpanded = value; } }
		
		public new void Focus()
		{
			if (this.ViewModel.Actions.Count > 0) {
				this.ChildControl.ActionsTreeView.Focus();
			} else {
				this.ChildControl.HiddenActionsTreeView.Focus();
			}
		}
		
		public void OpenAtLineStart(ITextEditor editor)
		{
			OpenAtPosition(editor, editor.Caret.Line, 1, false);
			this.VerticalOffset -= 16;
		}
	}
}
