// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A tool bar button based on the AddIn-tree.
	/// </summary>
	sealed class ToolBarButton : Button, IStatusUpdate
	{
		readonly Codon codon;
		readonly object caller;
		readonly string inputGestureText;
		readonly IEnumerable<ICondition> conditions;
		
		public ToolBarButton(UIElement inputBindingOwner, Codon codon, object caller, bool createCommand, IEnumerable<ICondition> conditions)
		{
			ToolTipService.SetShowOnDisabled(this, true);
			
			this.codon = codon;
			this.caller = caller;
			this.Command = CommandWrapper.GetCommand(codon, caller, createCommand, conditions);
			this.Content = ToolBarService.CreateToolBarItemContent(codon);
			this.conditions = conditions;

			if (codon.Properties.Contains("name")) {
				this.Name = codon.Properties["name"];
			}

			if (!string.IsNullOrEmpty(codon.Properties["shortcut"])) {
				KeyGesture kg = MenuService.ParseShortcut(codon.Properties["shortcut"]);
				MenuCommand.AddGestureToInputBindingOwner(inputBindingOwner, kg, this.Command, GetFeatureName());
				this.inputGestureText = kg.GetDisplayStringForCulture(Thread.CurrentThread.CurrentUICulture);
			}
			UpdateText();
			
			SetResourceReference(FrameworkElement.StyleProperty, ToolBar.ButtonStyleKey);
		}
		
		string GetFeatureName()
		{
			string commandName = codon.Properties["command"];
			if (string.IsNullOrEmpty(commandName)) {
				return codon.Properties["class"];
			} else {
				return commandName;
			}
		}
		
		protected override void OnClick()
		{
			string feature = GetFeatureName();
			if (!string.IsNullOrEmpty(feature)) {
				AnalyticsMonitorService.TrackFeature(feature, "Toolbar");
			}
			base.OnClick();
		}
		
		public void UpdateText()
		{
			if (codon.Properties.Contains("tooltip")) {
				string toolTip = StringParser.Parse(codon.Properties["tooltip"]);
				if (!string.IsNullOrEmpty(inputGestureText))
					toolTip = toolTip + " (" + inputGestureText + ")";
				this.ToolTip = toolTip;
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
