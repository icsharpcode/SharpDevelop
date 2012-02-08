// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace ICSharpCode.Core.Presentation
{
	sealed class MenuCheckBox : CoreMenuItem
	{
		BindingExpressionBase isCheckedBinding;
		
		public MenuCheckBox(UIElement inputBindingOwner, Codon codon, object caller, IEnumerable<ICondition> conditions)
			: base(codon, caller, conditions)
		{
			this.Command = CommandWrapper.GetCommand(codon, caller, true, conditions);
			CommandWrapper wrapper = this.Command as CommandWrapper;
			if (wrapper != null) {
				ICheckableMenuCommand cmd = wrapper.GetAddInCommand() as ICheckableMenuCommand;
				if (cmd != null) {
					isCheckedBinding = SetBinding(IsCheckedProperty, new Binding("IsChecked") { Source = cmd, Mode = BindingMode.OneWay });
				}
			}
			
			if (!string.IsNullOrEmpty(codon.Properties["shortcut"])) {
				KeyGesture kg = MenuService.ParseShortcut(codon.Properties["shortcut"]);
				MenuCommand.AddGestureToInputBindingOwner(inputBindingOwner, kg, this.Command, null);
				this.InputGestureText = kg.GetDisplayStringForCulture(Thread.CurrentThread.CurrentUICulture);
			}
		}
		
		public override void UpdateStatus()
		{
			base.UpdateStatus();
			if (isCheckedBinding != null)
				isCheckedBinding.UpdateTarget();
		}
		
		protected override void OnClick()
		{
			base.OnClick();
			Dispatcher.BeginInvoke(
				DispatcherPriority.DataBind,
				new Action(
					delegate {
						if (isCheckedBinding != null)
							isCheckedBinding.UpdateTarget();
					}));
		}
	}
}
