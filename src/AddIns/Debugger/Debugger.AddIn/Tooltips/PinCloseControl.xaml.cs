// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

namespace Debugger.AddIn.Tooltips
{
	public class ShowingCommentEventArgs : EventArgs
	{
		public bool ShowComment { get; private set; }
		
		public ShowingCommentEventArgs(bool showComment)
		{
			ShowComment = showComment;
		}
	}
	
	public partial class PinCloseControl : UserControl
	{
		public event EventHandler Closed;
		
		public event EventHandler PinningChanged;
		
		public event EventHandler<ShowingCommentEventArgs> ShowingComment;
		
		public PinCloseControl()
		{
			InitializeComponent();
		}
		
		public bool IsChecked {
			get {
				return UnpinButton.IsChecked.GetValueOrDefault(false);
			}
		}
		
		void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			var handler = Closed;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		
		void CommentButton_Checked(object sender, RoutedEventArgs e)
		{
			var handler = ShowingComment;
			if (handler != null)
				handler(this, new ShowingCommentEventArgs(true));
		}
		
		void CommentButton_Unchecked(object sender, RoutedEventArgs e)
		{
			var handler = ShowingComment;
			if (handler != null)
				handler(this, new ShowingCommentEventArgs(false));
		}
		
		void UnpinButton_Checked(object sender, RoutedEventArgs e)
		{
			var handler = PinningChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		
		void UnpinButton_Unchecked(object sender, RoutedEventArgs e)
		{
			var handler = PinningChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}