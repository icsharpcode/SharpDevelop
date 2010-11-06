// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace Services.Debugger.Tooltips
{
	/// <summary>
	/// Interaction logic for PinCloseControl.xaml
	/// </summary>
	public partial class PinCloseControl : UserControl
	{
		readonly DebuggerTooltipControl toolTipControl;
				
		public PinCloseControl(DebuggerTooltipControl parent)
		{
			Margin = new Thickness(5, 0, 0, 0);
			InitializeComponent();
			
			this.toolTipControl = parent;
		}
		
		void Unpin()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
			if (provider != null) {
				ITextEditor editor = provider.TextEditor;
				if (!string.IsNullOrEmpty(editor.FileName)) {
					
					var pin = new PinBookmark(editor.FileName, toolTipControl.LogicalPosition);
					BookmarkManager.RemoveMark(pin);
				}
			}
		}
		
		void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Unpin();
			toolTipControl.containingPopup.CloseSelfAndChildren();
		}
		
		void CommentButton_Checked(object sender, RoutedEventArgs e)
		{
			toolTipControl.ShowComment(true);
		}
		
		void CommentButton_Unchecked(object sender, RoutedEventArgs e)
		{
			toolTipControl.ShowComment(false);
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
					
					var pin = new PinBookmark(editor.FileName, toolTipControl.LogicalPosition);
					
					BookmarkManager.ToggleBookmark(
							editor, 
							toolTipControl.LogicalPosition.Line, 
							b => b.CanToggle && b is PinBookmark,
							location => pin);
				}
			}
		}
	}
}