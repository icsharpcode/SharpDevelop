// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Refactoring
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
		
		public new ContextActionsBulbViewModel DataContext
		{
			get { return (ContextActionsBulbViewModel)base.DataContext; }
			set { base.DataContext = value; }
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
			this.DataContext.LoadHiddenActions();
		}
		
		void CheckBox_Click(object sender, RoutedEventArgs e)
		{
			e.Handled = true;
		}
	}
}
