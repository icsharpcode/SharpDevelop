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
		ICheckableMenuCommand cmd;
		// We need to keep the reference to the event handler around
		// because the IsCheckedChanged event may be a weak event
		EventHandler isCheckedChangedHandler;
		
		public MenuCheckBox(UIElement inputBindingOwner, Codon codon, object caller, IReadOnlyCollection<ICondition> conditions)
			: base(codon, caller, conditions)
		{
			this.Command = CommandWrapper.CreateCommand(codon, conditions);
			this.CommandParameter = caller;
			
			cmd = CommandWrapper.Unwrap(this.Command) as ICheckableMenuCommand;
			if (cmd != null) {
				isCheckedChangedHandler = cmd_IsCheckedChanged;
				cmd.IsCheckedChanged += isCheckedChangedHandler;
				this.IsChecked = cmd.IsChecked(caller);
			}
			
			if (!string.IsNullOrEmpty(codon.Properties["shortcut"])) {
				KeyGesture kg = MenuService.ParseShortcut(codon.Properties["shortcut"]);
				MenuCommand.AddGestureToInputBindingOwner(inputBindingOwner, kg, this.Command, null);
				this.InputGestureText = MenuService.GetDisplayStringForShortcut(kg);
			}
		}

		void cmd_IsCheckedChanged(object sender, EventArgs e)
		{
			this.IsChecked = cmd.IsChecked(caller);
		}
	}
}
