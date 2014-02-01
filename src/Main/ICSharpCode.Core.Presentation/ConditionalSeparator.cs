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
