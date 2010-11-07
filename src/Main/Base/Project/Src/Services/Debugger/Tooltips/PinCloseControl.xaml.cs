// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace Services.Debugger.Tooltips
{
	public partial class PinCloseControl : UserControl
	{
		readonly DebuggerTooltipControl control;
		
		public PinCloseControl(DebuggerTooltipControl control)
		{
			Margin = new Thickness(5, 0, 0, 0);
			InitializeComponent();

			this.control = control;			
			this.control.CommentChanged += delegate { Mark.Comment = control.Comment; };
		}
		
		private PinBookmark Mark {
			get { return this.control.containingPopup.Mark; }
		}
		
		void Unpin()
		{
			BookmarkManager.RemoveMark(Mark);
		}
		
		void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Unpin();
			
			this.control.containingPopup.CloseSelfAndChildren();
		}
		
		void CommentButton_Checked(object sender, RoutedEventArgs e)
		{
			this.control.ShowComment(true);
		}
		
		void CommentButton_Unchecked(object sender, RoutedEventArgs e)
		{
			this.control.ShowComment(false);
		}
		
		void UnpinButton_Checked(object sender, RoutedEventArgs e)
		{
			Unpin();
		}
		
		void UnpinButton_Unchecked(object sender, RoutedEventArgs e)
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
			if (provider != null) {
				ITextEditor editor = provider.TextEditor;
				if (!string.IsNullOrEmpty(editor.FileName)) {
					
					BookmarkManager.ToggleBookmark(
							editor, 
							Mark.Location.Line, 
							b => b.CanToggle && b is PinBookmark,
							location => Mark);
				}
			}
		}
	}
}