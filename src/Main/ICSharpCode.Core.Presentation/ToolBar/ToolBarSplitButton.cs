// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A tool bar button based on the AddIn-tree.
	/// </summary>
	sealed class ToolBarSplitButton : SplitButton, IStatusUpdate
	{
		ICommand menuCommand;
		object caller;
		Codon codon;
		Label innerLabel;
		
		public ToolBarSplitButton(Codon codon, object caller, IList submenu)
		{
			ToolTipService.SetShowOnDisabled(this, true);
			
			this.codon = codon;
			this.caller = caller;

			this.Content = ToolBarService.CreateToolBarItemContent(codon, out innerLabel);
			if (codon.Properties.Contains("name")) {
				this.Name = codon.Properties["name"];
			}
			else
			{
				this.Name = NameUtil.ControlName("ToolBar", codon.Id, codon.Name);
			}
			
			menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			menuCommand.Owner = this;
			
			this.Command = new CommandWrapper(codon, caller, menuCommand);
			this.DropDownMenu = MenuService.CreateContextMenu(submenu);
			
			UpdateText();
		}
		
		public void UpdateText()
		{
			if (codon.Properties.Contains("tooltip")) {
				this.ToolTip = StringParser.Parse(codon.Properties["tooltip"]);
			}
			
			if (codon.Properties.Contains("label")) {
				innerLabel.Content = StringParser.Parse(codon.Properties["label"]);
			}
		}
		
		public void UpdateStatus()
		{
			if (codon.GetFailedAction(caller) == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
		}
	}
}
