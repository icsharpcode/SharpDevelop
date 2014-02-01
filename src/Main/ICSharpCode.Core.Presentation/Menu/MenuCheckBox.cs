// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
