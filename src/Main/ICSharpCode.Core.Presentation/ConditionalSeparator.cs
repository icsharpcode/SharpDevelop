// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A Separator that is invisible when it is excluded by a condition.
	/// </summary>
	sealed class ConditionalSeparator : Separator, IStatusUpdate
	{
		readonly Codon codon;
		readonly object caller;
		readonly IEnumerable<ICondition> conditions;
		
		public ConditionalSeparator(Codon codon, object caller, bool inToolbar, IEnumerable<ICondition> conditions)
		{
			this.codon = codon;
			this.caller = caller;
			this.conditions = conditions;
			
			if (inToolbar) {
				SetResourceReference(FrameworkElement.StyleProperty, ToolBar.SeparatorStyleKey);
			}
		}
		
		public void UpdateText()
		{
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
