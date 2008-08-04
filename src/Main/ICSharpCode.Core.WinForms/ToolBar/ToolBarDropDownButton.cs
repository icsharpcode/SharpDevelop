// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class ToolBarDropDownButton : ToolStripDropDownButton , IStatusUpdate
	{
		object caller;
		Codon codon;
		ICommand menuBuilder = null;
		ArrayList subItems;
		
		public ToolBarDropDownButton(Codon codon, object caller, ArrayList subItems)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller        = caller;
			this.codon         = codon;
			this.subItems	   = subItems;

			if (codon.Properties.Contains("label")){
				Text = StringParser.Parse(codon.Properties["label"]);
			}
			if (Image == null && codon.Properties.Contains("icon")) {
				Image = WinFormsResourceService.GetBitmap(StringParser.Parse(codon.Properties["icon"]));
			}
			if (menuBuilder == null && codon.Properties.Contains("class")) {
				menuBuilder = codon.AddIn.CreateObject(StringParser.Parse(codon.Properties["class"])) as ICommand;
				menuBuilder.Owner = this;
			}

			UpdateStatus();
			UpdateText();
		}

		void CreateDropDownItems()
		{
			// let's assume that if a menuBuilder exists,
			// as in the Search Results panel or the Class
			// Browser toolbar, it will handle this step.
			if (menuBuilder != null) {
				return;
			}

			// also, let's prevent a null exception
			// in the event that there are no subitems
			if (subItems == null || subItems.Count==0) {
				return;
			}
			
			DropDownItems.Clear();
			foreach (object item in subItems)
			{
				if (item is ToolStripItem)
				{
					DropDownItems.Add((ToolStripItem)item);
					if (item is IStatusUpdate)
					{
						((IStatusUpdate)item).UpdateStatus();
						((IStatusUpdate)item).UpdateText();
					}
				}
				else
				{
					ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
					DropDownItems.AddRange(submenuBuilder.BuildSubmenu(codon, caller));
				}
			}
		}
		protected override void OnDropDownShow(EventArgs e)
		{
			if (codon != null && !this.DropDown.Visible)
			{
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
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
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
