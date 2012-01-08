// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class MenuCheckBox : ToolStripMenuItem , IStatusUpdate
	{
		object caller;
		Codon  codon;
		string description   = String.Empty;
		ICheckableMenuCommand menuCommand = null;
		IEnumerable<ICondition> conditions;
		
		void CreateMenuCommand()
		{
			if (menuCommand == null) {
				try {
					menuCommand = (ICheckableMenuCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
				} catch (Exception e) {
					MessageService.ShowException(e, "Can't create menu command : " + codon.Id);
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
			this.conditions = Enumerable.Empty<ICondition>();
		}
		public MenuCheckBox(Codon codon, object caller, IEnumerable<ICondition> conditions)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller = caller;
			this.codon  = codon;
			this.conditions = conditions;
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
				ConditionFailedAction failedAction = Condition.GetFailedAction(conditions, caller);
				return failedAction != ConditionFailedAction.Disable;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = Condition.GetFailedAction(conditions, caller);
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
