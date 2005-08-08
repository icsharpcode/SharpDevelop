// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public class ToolBarCommand : ToolStripMenuItem, IStatusUpdate
	{
		object caller;
		Codon codon;
		ICommand menuCommand = null;
		
		public ToolBarCommand(Codon codon, object caller, bool createCommand)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller        = caller;
			this.codon         = codon;
			
			if (createCommand) {
				menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			}
			
			if (Image == null && codon.Properties.Contains("icon")) {
				Image = ResourceService.GetBitmap(codon.Properties["icon"]);
			}
			
			UpdateStatus();
			UpdateText();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (menuCommand == null) {
				menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			}
			if (menuCommand != null) {
				menuCommand.Owner = caller;
				menuCommand.Run();
			}
		}
		
//		protected override void OnSelect(System.EventArgs e)
//		{
//			base.OnSelect(e);
//			StatusBarService.SetMessage(description);
//		}
		
		public bool LastEnabledStatus = false;
		public bool CurrentEnableStatus {
			get {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
				if (menuCommand != null && menuCommand is IMenuCommand) {
					isEnabled &= ((IMenuCommand)menuCommand).IsEnabled;
				}
				return isEnabled;
			}
		}
		
		public override bool Enabled {
			get {
				bool isEnabled = CurrentEnableStatus;
				LastEnabledStatus  = isEnabled;
				return isEnabled;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				bool isVisible = failedAction != ConditionFailedAction.Exclude;
				if (base.Visible != isVisible) {
					base.Visible = isVisible;
				}
			}
		}
		
		public virtual void UpdateText()
		{
			if (codon != null) {
				ToolTipText = StringParser.Parse(codon.Properties["tooltip"]);
			}
		}
	}
}
