// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class Menu : ToolStripMenuItem, IStatusUpdate
	{
		Codon  codon;
		object caller;
		IList subItems;
		bool isInitialized;
		IEnumerable<ICondition> conditions;
		
		public Menu(Codon codon, object caller, IList subItems, IEnumerable<ICondition> conditions)
		{
			if (subItems == null) subItems = new ArrayList(); // don't crash when item has no children
			this.codon    = codon;
			this.caller   = caller;
			this.subItems = subItems;
			this.RightToLeft = RightToLeft.Inherit;
			this.conditions = conditions;
			
			UpdateText();
		}
		
		public Menu(string text, IEnumerable<ICondition> conditions, params ToolStripItem[] subItems)
		{
			this.Text = StringParser.Parse(text);
			this.DropDownItems.AddRange(subItems);
			this.conditions = conditions;
		}
		
		void CreateDropDownItems()
		{
			DropDownItems.Clear();
			foreach (object item in subItems) {
				if (item is ToolStripItem) {
					DropDownItems.Add((ToolStripItem)item);
					if (item is IStatusUpdate) {
						((IStatusUpdate)item).UpdateStatus();
						((IStatusUpdate)item).UpdateText();
					}
				} else {
					ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
					DropDownItems.AddRange(submenuBuilder.BuildSubmenu(codon, caller));
				}
			}
		}
		protected override void OnDropDownShow(EventArgs e)
		{
			if (codon != null && !this.DropDown.Visible) {
				CreateDropDownItems();
			}
			base.OnDropDownShow(e);
		}
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				return Condition.GetFailedAction(conditions, caller) != ConditionFailedAction.Disable;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = Condition.GetFailedAction(conditions, caller);
				this.Visible = failedAction != ConditionFailedAction.Exclude;
				if (!isInitialized && failedAction != ConditionFailedAction.Exclude) {
					isInitialized = true;
					CreateDropDownItems(); // must be created to support shortcuts
					if (DropDownItems.Count == 0 && subItems.Count > 0) {
						DropDownItems.Add(new ToolStripMenuItem());
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
