// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ICSharpCode.Core.Presentation
{
	sealed class ToolBarCheckBox : CheckBox, IStatusUpdate
	{
		readonly Codon codon;
		readonly object caller;
		BindingExpressionBase isCheckedBinding;
		
		public ToolBarCheckBox(Codon codon, object caller)
		{
			ToolTipService.SetShowOnDisabled(this, true);
			
			this.codon = codon;
			this.caller = caller;
			this.Command = CommandWrapper.GetCommand(codon, caller, true);
			CommandWrapper wrapper = this.Command as CommandWrapper;
			if (wrapper != null) {
				ICheckableMenuCommand cmd = wrapper.GetAddInCommand() as ICheckableMenuCommand;
				if (cmd != null) {
					isCheckedBinding = SetBinding(IsCheckedProperty, new Binding("IsChecked") { Source = cmd, Mode = BindingMode.OneWay });
				}
			}

			this.Content = ToolBarService.CreateToolBarItemContent(codon);
			if (codon.Properties.Contains("name")) {
				this.Name = codon.Properties["name"];
			}
			UpdateText();
			
			SetResourceReference(FrameworkElement.StyleProperty, ToolBar.CheckBoxStyleKey);
		}
		
		public void UpdateText()
		{
			if (codon.Properties.Contains("tooltip")) {
				this.ToolTip = StringParser.Parse(codon.Properties["tooltip"]);
			}
		}
		
		public void UpdateStatus()
		{
			if (codon.GetFailedAction(caller) == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
		}
		
		protected override void OnClick()
		{
			base.OnClick();
			isCheckedBinding.UpdateTarget();
		}
	}
}
