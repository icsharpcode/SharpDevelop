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
using System.Windows.Forms;

namespace ICSharpCode.Core.WinForms
{
	public class ToolBarCheckBox : ToolStripButton , IStatusUpdate
	{
		object caller;
		Codon  codon;
		string description   = String.Empty;
		ICheckableMenuCommand menuCommand = null;
		IEnumerable<ICondition> conditions;
		
		public ICheckableMenuCommand MenuCommand {
			get {
				return menuCommand;
			}
		}
		
		public object Caller {
			get {
				return caller;
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
		
		public ToolBarCheckBox(Codon codon, object caller, IEnumerable<ICondition> conditions)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller = caller;
			this.codon  = codon;
			this.conditions = conditions;
			try {
				menuCommand = (ICheckableMenuCommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			} catch (Exception) {
			}
			if (menuCommand == null) {
				MessageService.ShowError("Can't create toolbar checkbox : " + codon.Id);
			}
			
			if (codon.Properties.Contains("label")){
				Text = StringParser.Parse(codon.Properties["label"]);
			}
			if (Image == null && codon.Properties.Contains("icon")) {
				Image = WinFormsResourceService.GetBitmap(StringParser.Parse(codon.Properties["icon"]));
			}
			UpdateText();
			UpdateStatus();
		}
		
		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			if (menuCommand != null) {
				menuCommand.Execute(caller);
				Checked = menuCommand.IsChecked(caller);
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
				bool isVisible = failedAction != ConditionFailedAction.Exclude;
				if (isVisible != Visible)
					Visible = isVisible;
				if (menuCommand != null) {
					bool isChecked = menuCommand.IsChecked(caller);
					if (isChecked != Checked)
						Checked = isChecked;
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
