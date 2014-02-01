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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace ICSharpCode.Core.Presentation
{
	sealed class ToolBarCheckBox : CheckBox, IStatusUpdate
	{
		readonly Codon codon;
		readonly object caller;
		BindingExpressionBase isCheckedBinding;
		readonly IEnumerable<ICondition> conditions;
		
		public ToolBarCheckBox(Codon codon, object caller, IReadOnlyCollection<ICondition> conditions)
		{
			ToolTipService.SetShowOnDisabled(this, true);
			
			this.codon = codon;
			this.caller = caller;
			this.conditions = conditions;
			this.Command = CommandWrapper.CreateCommand(codon, conditions);
			this.CommandParameter = caller;
			ICheckableMenuCommand cmd = CommandWrapper.Unwrap(this.Command) as ICheckableMenuCommand;
			if (cmd != null) {
				isCheckedBinding = SetBinding(IsCheckedProperty, new Binding("IsChecked") { Source = cmd, Mode = BindingMode.OneWay });
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
			if (codon.Properties.Contains("label")) {
				this.Content = ToolBarService.CreateToolBarItemContent(codon);
			}
		}
		
		public void UpdateStatus()
		{
			if (Condition.GetFailedAction(conditions, caller) == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
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
