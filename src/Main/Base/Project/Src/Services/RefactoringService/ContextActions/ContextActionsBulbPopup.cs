// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		private ContextActionsBulbControl ChildControl
		{
			get { return (ContextActionsBulbControl)this.Child; }
			set { this.Child = value; }
		}
		
		public ContextActionsBulbViewModel ViewModel
		{
			get { return (ContextActionsBulbViewModel)this.DataContext; }
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
