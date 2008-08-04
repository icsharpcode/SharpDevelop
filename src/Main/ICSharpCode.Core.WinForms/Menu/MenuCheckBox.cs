// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class MenuCheckBox : ToolStripMenuItem , IStatusUpdate
	{
		object caller;
		Codon  codon;
		string description   = String.Empty;
		ICheckableMenuCommand menuCommand = null;
		
		void CreateMenuCommand()
		{
			if (menuCommand == null) {
				try {
					menuCommand = (ICheckableMenuCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
				} catch (Exception e) {
					MessageService.ShowError(e, "Can't create menu command : " + codon.Id);
				}
			}
		}
		
		public ICheckableMenuCommand MenuCommand {
			get {
				CreateMenuCommand();
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
		public MenuCheckBox(string text)
		{
			this.RightToLeft = RightToLeft.Inherit;
			Text = text;
		}
		public MenuCheckBox(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller = caller;
			this.codon  = codon;
			UpdateText();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			if (codon != null) {
				MenuCommand.Run();
				Checked = MenuCommand.IsChecked;
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
				if (menuCommand == null && !string.IsNullOrEmpty(codon.Properties["checked"])) {
					Checked = string.Equals(StringParser.Parse(codon.Properties["checked"]),
					                        bool.TrueString, StringComparison.OrdinalIgnoreCase);
				} else {
					CreateMenuCommand();
					if (menuCommand != null) {
						Checked = menuCommand.IsChecked;
					}
				}
			}
		}
		
		public virtual void UpdateText()
		{
			if (codon != null) {
				Text = StringParser.Parse(codon.Properties["label"]);
			}
		}
	}
}
