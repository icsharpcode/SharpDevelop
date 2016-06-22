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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// The popup for context actions.
	/// </summary>
	public class ContextActionsBulbPopup : ExtendedPopup
	{
		public ContextActionsBulbPopup(UIElement parent) : base(parent)
		{
			this.UseLayoutRounding = true;
			TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
			
			this.StaysOpen = true;
			this.AllowsTransparency = true;
			this.ChildControl = new ContextActionsBulbControl();
			// Close when any action excecuted
			this.ChildControl.ActionExecuted += delegate { Close(); };
		}
		
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.Key == Key.Escape)
				this.IsOpenIfFocused = false;
		}
		
		public void Close()
		{
			this.IsOpenIfFocused = false;
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
			ContextActionsPopup.SetPosition(this, editor, editor.Caret.Line, 1);
			this.VerticalOffset -= 16;
			this.IsOpenIfFocused = true;
		}
	}
}
