// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
					IMenuItemBuilder submenuBuilder = (IMenuItemBuilder)item;
					DropDownItems.AddRange(submenuBuilder.BuildItems(codon, caller).Cast<ToolStripItem>().ToArray());
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
