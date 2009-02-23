// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
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
		
		public ConditionalSeparator(Codon codon, object caller, bool inToolbar)
		{
			this.codon = codon;
			this.caller = caller;
			
			if (inToolbar) {
				SetResourceReference(FrameworkElement.StyleProperty, ToolBar.SeparatorStyleKey);
			}
		}
		
		public void UpdateText()
		{
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
