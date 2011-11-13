// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class ToolBarLabel : ToolStripLabel, IStatusUpdate
	{
		object caller;
		Codon  codon;
		ICommand menuCommand = null;
		IEnumerable<ICondition> conditions;
		
		public object Caller {
			get {
				return caller;
			}
		}
		
		public ToolBarLabel(Codon codon, object caller, IEnumerable<ICondition> conditions)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller  = caller;
			this.codon   = codon;
			this.conditions = conditions;

			if (codon.Properties.Contains("class"))
			{
				menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
				menuCommand.Owner = this;
			}

			UpdateText();
			UpdateStatus();
		}
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = Condition.GetFailedAction(conditions, caller);
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
				return isEnabled;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null)
			{
				ConditionFailedAction failedAction = Condition.GetFailedAction(conditions, caller);
				this.Enabled = failedAction != ConditionFailedAction.Disable;
				this.Visible = failedAction != ConditionFailedAction.Exclude;
			}
		}
		
		public virtual void UpdateText()
		{
			if (codon.Properties.Contains("label")) {
				Text = StringParser.Parse(codon.Properties["label"]);
			}
			if (codon.Properties.Contains("tooltip")) {
				ToolTipText = StringParser.Parse(codon.Properties["tooltip"]);
			}
		}
	}
}
