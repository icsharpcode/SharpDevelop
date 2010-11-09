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
		private readonly DebuggerTooltipControl toolTipControl;
		
		public PinBookmark Mark { get; set; }
				
		public PinCloseControl(DebuggerTooltipControl parent)
		{
			Margin = new Thickness(5, 0, 0, 0);
			InitializeComponent();
						
			this.toolTipControl = parent;
			
			DebuggerService.DebugStopped += new EventHandler(DebuggerService_DebugStopped);
			DebuggerService.DebugStarted += new EventHandler(DebuggerService_DebugStarted);
			this.toolTipControl.CommentChanged += delegate { Mark.Comment = this.toolTipControl.Comment; };
		}

		void DebuggerService_DebugStarted(object sender, EventArgs e)
		{
			//this.toolTipControl.containingPopup.Open();
		}

		void DebuggerService_DebugStopped(object sender, EventArgs e)
		{
			this.toolTipControl.containingPopup.CloseSelfAndChildren();
		}
		
		void Unpin()
		{
			BookmarkManager.RemoveMark(Mark);
		}
		
		void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Unpin();
			this.toolTipControl.containingPopup.CloseSelfAndChildren();
		}
		
		void CommentButton_Checked(object sender, RoutedEventArgs e)
		{
			this.toolTipControl.ShowComment(true);
		}
		
		void CommentButton_Unchecked(object sender, RoutedEventArgs e)
		{
			this.toolTipControl.ShowComment(false);
		}
		
		void UnpinButton_Checked(object sender, RoutedEventArgs e)
		{
			Unpin();
		}
		
		void UnpinButton_Unchecked(object sender, RoutedEventArgs e)
		{
			if(BookmarkManager.Bookmarks.Contains(Mark))
				BookmarkManager.RemoveMark(Mark);
			
			BookmarkManager.AddMark(Mark);
		}
	}
}