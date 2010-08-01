// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
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
		
		bool isOpen;
		public bool IsOpen {
			get { return isOpen; }
			set {
				isOpen = value;
				this.Header.Opacity = isOpen ? 1.0 : 1.0;
				this.Header.BorderThickness = isOpen ? new Thickness(1, 1, 1, 0) : new Thickness(1);
				this.ActionsTreeView.Visibility = isOpen ? Visibility.Visible : Visibility.Collapsed;
			}
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
	}
}