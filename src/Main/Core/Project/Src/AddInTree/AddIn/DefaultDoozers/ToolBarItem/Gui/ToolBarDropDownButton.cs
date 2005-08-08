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
	public class ToolBarDropDownButton : ToolStripDropDownButton , IStatusUpdate
	{
		object caller;
		Codon codon;
		ICommand menuCommand = null;
		
		public ToolBarDropDownButton(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller        = caller;
			this.codon         = codon;
			
			if (Image == null && codon.Properties.Contains("icon")) {
				Image = ResourceService.GetBitmap(codon.Properties["icon"]);
			}
			menuCommand = codon.AddIn.CreateObject(codon.Properties["class"]) as ICommand;
			menuCommand.Owner = this;
			UpdateStatus();
			UpdateText();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			menuCommand.Run();
		}
		
//		protected override void OnSelect(System.EventArgs e)
//		{
//			base.OnSelect(e);
//			StatusBarService.SetMessage(description);
//		}
		
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
				if (menuCommand != null && menuCommand is IMenuCommand) {
					isEnabled &= ((IMenuCommand)menuCommand).IsEnabled;
				}
				
				return isEnabled;
			}
		}
		
		public virtual void UpdateText()
		{
			if (codon != null) {
				ToolTipText = StringParser.Parse(codon.Properties["tooltip"]);
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
	}
}
