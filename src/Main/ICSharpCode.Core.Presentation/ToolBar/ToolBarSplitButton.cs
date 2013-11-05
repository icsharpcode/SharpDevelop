// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A tool bar button based on the AddIn-tree.
	/// </summary>
	sealed class ToolBarSplitButton : SplitButton, IStatusUpdate
	{
		object caller;
		Codon codon;
		IReadOnlyCollection<ICondition> conditions;
		
		public ToolBarSplitButton(Codon codon, object caller, IList submenu, IReadOnlyCollection<ICondition> conditions)
		{
			ToolTipService.SetShowOnDisabled(this, true);
			
			this.codon = codon;
			this.caller = caller;
			this.conditions = conditions;

			this.Content = ToolBarService.CreateToolBarItemContent(codon);
			if (codon.Properties.Contains("name")) {
				this.Name = codon.Properties["name"];
			}
			
			this.Command = CommandWrapper.CreateLazyCommand(codon, conditions);
			this.CommandParameter = caller;
			this.DropDownMenu = MenuService.CreateContextMenu(submenu);
			
			UpdateText();
		}
		
		public void UpdateText()
		{
			if (codon.Properties.Contains("tooltip")) {
				this.ToolTip = StringParser.Parse(codon.Properties["tooltip"]);
			}
		}
		
		public void UpdateStatus()
		{
			if (Condition.GetFailedAction(conditions, caller) == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
		}
	}
}
