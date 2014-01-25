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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// Interaction logic for ContextActionsBulbControl.xaml
	/// </summary>
	public partial class ContextActionsBulbControl : UserControl
	{
		public ContextActionsBulbControl()
		{
			InitializeComponent();
			this.IsOpen = false;
		}
		
		public event EventHandler ActionExecuted
		{
			add { this.ActionsTreeView.ActionExecuted += value; }
			remove { this.ActionsTreeView.ActionExecuted -= value; }
		}
		
		bool isOpen;
		public bool IsOpen {
			get { return isOpen; }
			set {
				isOpen = value;
				this.Header.Opacity = isOpen ? 1.0 : 0.7;
				this.Header.BorderThickness = isOpen ? new Thickness(1, 1, 1, 0) : new Thickness(1);
				// Show / hide
				this.ContentBorder.Visibility =
					isOpen ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		
		public bool IsHiddenActionsExpanded
		{
			get { return this.HiddenActionsExpander.IsExpanded; }
			set { this.HiddenActionsExpander.IsExpanded = value; }
		}
		
		public new void Focus()
		{
			if (this.ActionsTreeView != null)
				this.ActionsTreeView.Focus();
		}
		
		void Header_MouseUp(object sender, MouseButtonEventArgs e)
		{
			this.IsOpen = !this.IsOpen;
		}
		
		void Expander_Expanded(object sender, RoutedEventArgs e)
		{
			((ContextActionsBulbViewModel)this.DataContext).LoadHiddenActionsAsync(CancellationToken.None).FireAndForget();
		}
		
		void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
		}
	}
}
