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
