// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public class Menu : ToolStripMenuItem, IStatusUpdate
	{
		Codon  codon;
		object caller;
		ArrayList subItems;
		
		public Menu(Codon codon, object caller, ArrayList subItems)
		{
			this.codon    = codon;
			this.caller   = caller;
			this.subItems = subItems;
			this.RightToLeft = RightToLeft.Inherit;
			
			CreateDropDownItems();
			if (DropDownItems.Count == 0 && subItems.Count > 0) {
				DropDownItems.Add(new ToolStripMenuItem());
			}
		}
		
		void CreateDropDownItems()
		{
			DropDownItems.Clear();
			foreach (object item in subItems) {
				if (item is ToolStripItem) {
					DropDownItems.Add((ToolStripItem)item);
					((IStatusUpdate)item).UpdateStatus();
				} else {
					ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
					DropDownItems.AddRange(submenuBuilder.BuildSubmenu(codon, caller));
				}
			}
		}
		protected override void OnDropDownShow(EventArgs e)
		{
			base.OnDropDownShow(e);
			CreateDropDownItems();
			foreach (object item in subItems) {
				if (item is ToolStripItem) {
					if (item is IStatusUpdate) {
						((IStatusUpdate)item).UpdateStatus();
					}
				}
			}
		}

		protected override void OnDropDownOpened(System.EventArgs e)
		{
			base.OnDropDownOpened(e);
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
			}
			
			Text = StringParser.Parse(codon.Properties["label"]);
		}
	}
}
