// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class MenuSeparator : ToolStripSeparator, IStatusUpdate
	{
		object caller;
		Codon codon;
		
		public MenuSeparator()
		{
			this.RightToLeft = RightToLeft.Inherit;
		}
		
		public MenuSeparator(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller              = caller;
			this.codon = codon;
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				this.Enabled = failedAction != ConditionFailedAction.Disable;
				this.Visible = failedAction != ConditionFailedAction.Exclude;
			}
		}
		
		public virtual void UpdateText()
		{
		}
	}
}
