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
	/// Interaction logic for ContextActionsHeaderedControl.xaml
	/// </summary>
	public partial class ContextActionsHeaderedControl : UserControl
	{
		public ContextActionsHeaderedControl()
		{
			InitializeComponent();
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
	}
}
