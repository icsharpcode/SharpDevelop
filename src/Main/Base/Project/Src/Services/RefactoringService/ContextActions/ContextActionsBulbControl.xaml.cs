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
		
		public new ContextActionsHiddenViewModel DataContext
		{
			get { return (ContextActionsHiddenViewModel)base.DataContext; }
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
		
		void ActionsTreeView_ActionVisibleChanged(object sender, ContextActionViewModelEventArgs e)
		{
			var clickedAction = e.Action;
			this.DataContext.Model.SetVisible(clickedAction.Action, false);
//			this.DataContext.Actions.Remove(clickedAction);
//			this.DataContext.HiddenActions.Add(clickedAction);
			
		}
		
		void HiddenActionsTreeView_ActionVisibleChanged(object sender, ContextActionViewModelEventArgs e)
		{
			var clickedAction = e.Action;
			this.DataContext.Model.SetVisible(clickedAction.Action, true);
//			this.DataContext.HiddenActions.Remove(clickedAction);
//			this.DataContext.Actions.Add(clickedAction);
		}
		
		void Expander_Expanded(object sender, RoutedEventArgs e)
		{
			this.DataContext.LoadHiddenActions();
		}
	}
}