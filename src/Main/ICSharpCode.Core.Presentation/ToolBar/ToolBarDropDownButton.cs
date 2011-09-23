// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A tool bar button that opens a drop down menu.
	/// </summary>
	sealed class ToolBarDropDownButton : DropDownButton, IStatusUpdate
	{
		readonly Codon codon;
		readonly object caller;
		Label innerLabel;
		
		public ToolBarDropDownButton(Codon codon, object caller, IList subMenu)
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
			this.DropDownMenu = MenuService.CreateContextMenu(subMenu);
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
