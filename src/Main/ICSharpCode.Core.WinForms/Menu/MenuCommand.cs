// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ICSharpCode.Core.WinForms
{
	public class MenuCommand : ToolStripMenuItem, IStatusUpdate
	{
		object caller;
		Codon codon;
		ICommand command;
		string description = "";
		IEnumerable<ICondition> conditions;
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public MenuCommand(Codon codon, object caller, IReadOnlyCollection<ICondition> conditions)
			: this(codon, caller, false, conditions)
		{
			
		}
		
		public MenuCommand(Codon codon, object caller, bool createCommand, IReadOnlyCollection<ICondition> conditions)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller        = caller;
			this.codon         = codon;
			this.conditions  = conditions;
			
			if (createCommand) {
				this.command = CommandWrapper.CreateCommand(codon, conditions);
			} else {
				this.command = CommandWrapper.CreateLazyCommand(codon, conditions);
			}
			
			UpdateText();
		}
		
		public MenuCommand(string label, EventHandler handler) : this(label)
		{
			this.Click  += handler;
		}
		
		public MenuCommand(string label)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.codon  = null;
			this.caller = null;
			Text = StringParser.Parse(label);
			this.conditions = Enumerable.Empty<ICondition>();
		}
		
		protected override void OnClick(System.EventArgs e)
		{
			base.OnClick(e);
			command = CommandWrapper.Unwrap(command);
			if (command != null) {
				MenuService.ExecuteCommand(command, caller);
			}
		}
		
//		protected override void OnSelect(System.EventArgs e)
//		{
//			base.OnSelect(e);
//			StatusBarService.SetMessage(description);
//		}
		
		bool GetVisible()
		{
			if (codon == null)
				return true;
			else
				return Condition.GetFailedAction(conditions, caller) != ConditionFailedAction.Exclude;
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				if (Image == null && codon.Properties.Contains("icon")) {
					try {
						Image = WinFormsResourceService.GetBitmap(codon.Properties["icon"]);
					} catch (ResourceNotFoundException) {}
				}
				Visible = GetVisible();
				Enabled = command != null && command.CanExecute(caller);
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
