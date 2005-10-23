// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
			
			UpdateText();
			CreateDropDownItems(); // must be created to support shortcuts
			if (DropDownItems.Count == 0 && subItems.Count > 0) {
				DropDownItems.Add(new ToolStripMenuItem());
			}
		}
		
		public Menu(string text, params ToolStripItem[] subItems)
		{
			this.Text = StringParser.Parse(text);
			this.DropDownItems.AddRange(subItems);
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
		}
		
		public virtual void UpdateText()
		{
			if (codon != null) {
				Text = StringParser.Parse(codon.Properties["label"]);
			}
		}
	}
}
