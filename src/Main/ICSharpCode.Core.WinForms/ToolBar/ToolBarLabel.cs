// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="John Simons" email="johnsimons007@yahoo.com.au"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class ToolBarLabel : ToolStripLabel, IStatusUpdate
	{
		object caller;
		Codon  codon;
		ICommand menuCommand = null;
		
		public object Caller {
			get {
				return caller;
			}
		}
		
		public ToolBarLabel(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;			
			this.caller  = caller;
			this.codon   = codon;

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
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
								
				return isEnabled;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null)
			{
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
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
