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
	/// Interaction logic for ContextActionsHeaderedControl.xaml
	/// </summary>
	public partial class ContextActionsHeaderedControl : UserControl
	{
		public ContextActionsHeaderedControl()
		{
			InitializeComponent();
			this.IsAlwaysOpen = false;
			this.IsOpen = false;
		}
		
		bool isAlwaysOpen;
		public bool IsAlwaysOpen {
			get { return isAlwaysOpen; }
			set { 
				isAlwaysOpen = value; 
				if (value)
					IsOpen = true;
			}
		}
		
		bool isOpen;
		public bool IsOpen {
			get { return isOpen; }
			set { 
				if (IsAlwaysOpen && !value)
					throw new InvalidOperationException("Cannot set IsOpen to false when IsAlwaysOpen is true");
				isOpen = value;
				this.Header.Opacity = isOpen ? 1.0 : 0.5;
				this.Header.BorderThickness = isOpen ? new Thickness(1, 1, 1, 0) : new Thickness(1);
				this.ActionsTreeView.Visibility = isOpen ? Visibility.Visible : Visibility.Collapsed;
			}
		}
		
		public event EventHandler ActionExecuted
		{
			add { this.ActionsTreeView.ActionExecuted += value; }
			remove { this.ActionsTreeView.ActionExecuted -= value; }
		}
		
		public new void Focus()
		{
			if (this.ActionsTreeView != null)
				this.ActionsTreeView.Focus();
		}
		
		void Header_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!this.IsAlwaysOpen)
			{
				this.IsOpen = !this.IsOpen;
			}
		}
	}
}