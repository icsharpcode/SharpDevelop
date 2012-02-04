// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class ToolBarCommand : ToolStripButton, IStatusUpdate
	{
		object caller;
		Codon codon;
		ICommand menuCommand = null;
		IEnumerable<ICondition> conditions;
		
		public ToolBarCommand(Codon codon, object caller, bool createCommand, IEnumerable<ICondition> conditions)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller        = caller;
			this.codon         = codon;
			this.conditions  = conditions;
			
			if (createCommand) {
				menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			}
			
			if (codon.Properties.Contains("label")){
				Text = StringParser.Parse(codon.Properties["label"]);
			}
			if (Image == null && codon.Properties.Contains("icon")) {
				Image = WinFormsResourceService.GetBitmap(StringParser.Parse(codon.Properties["icon"]));
			}
			
			UpdateStatus();
			UpdateText();
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			if (menuCommand == null) {
				menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			}
			if (menuCommand != null) {
				menuCommand.Owner = caller;
				AnalyticsMonitorService.TrackFeature(menuCommand.GetType().FullName, "Toolbar");
				menuCommand.Run();
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = Condition.GetFailedAction(conditions, caller);
				this.Visible = failedAction != ConditionFailedAction.Exclude;
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				if (isEnabled && menuCommand != null && menuCommand is IMenuCommand) {
					isEnabled = ((IMenuCommand)menuCommand).IsEnabled;
				}
				this.Enabled = isEnabled;
				
				if (this.Visible && codon.Properties.Contains("icon")) {
					Image = WinFormsResourceService.GetBitmap(StringParser.Parse(codon.Properties["icon"]));
				}
			}
		}
		
		public virtual void UpdateText()
		{
			if (codon != null) {
				if (codon.Properties.Contains("tooltip")) {
					ToolTipText = StringParser.Parse(codon.Properties["tooltip"]);
				}
				
				if (codon.Properties.Contains("label")){
					Text = StringParser.Parse(codon.Properties["label"]);
				}
			}
		}
	}
}
