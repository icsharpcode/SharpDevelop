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
				MenuCommand.Execute(caller);
				Checked = MenuCommand.IsChecked(caller);
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
						Checked = menuCommand.IsChecked(caller);
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
