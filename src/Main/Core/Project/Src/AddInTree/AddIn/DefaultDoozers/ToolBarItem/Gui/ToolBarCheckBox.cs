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
	public class ToolBarCheckBox : ToolStripButton , IStatusUpdate
	{
		object caller;
		Codon  codon;
		string description   = String.Empty;
		ICheckableMenuCommand menuCommand = null;
		
		public ICheckableMenuCommand MenuCommand {
			get {
				return menuCommand;
			}
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public ToolBarCheckBox(string text)
		{
			this.RightToLeft = RightToLeft.Inherit;
			Text = text;
		}
		
		public ToolBarCheckBox(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller = caller;
			this.codon  = codon;
			try {
				menuCommand = (ICheckableMenuCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			} catch (Exception) {
			}
			if (menuCommand == null) {
				MessageService.ShowError("Can't create toolbar checkbox : " + codon.Id);
			}
			
			if (Image == null && codon.Properties.Contains("icon")) {
				Image = ResourceService.GetBitmap(codon.Properties["icon"]);
			}
			UpdateStatus();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (menuCommand != null) {
				menuCommand.Run();
				Checked = menuCommand.IsChecked;
			}
		}
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				return failedAction != ConditionFailedAction.Disable;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				this.Visible = failedAction != ConditionFailedAction.Exclude;
				if (codon.Properties.Contains("tooltip")) {
					ToolTipText = StringParser.Parse(codon.Properties["tooltip"]);
				}
				Checked     = menuCommand.IsChecked;
				Text        = StringParser.Parse(codon.Properties["label"]);
			}
		}
	}
}
