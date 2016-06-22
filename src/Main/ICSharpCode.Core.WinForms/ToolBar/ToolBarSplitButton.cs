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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ICSharpCode.Core.WinForms
{
	public class ToolBarSplitButton : ToolStripSplitButton , IStatusUpdate
	{
		object caller;
		Codon codon;
		ArrayList subItems;
		ICommand menuCommand = null;
		Image imgButtonEnabled = null;
		Image imgButtonDisabled = null;
		bool buttonEnabled = true;
		bool dropDownEnabled = true;
		IEnumerable<ICondition> conditions;
		
		public ToolBarSplitButton(Codon codon, object caller, ArrayList subItems, IEnumerable<ICondition> conditions)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller      = caller;
			this.codon       = codon;
			this.subItems	 = subItems;
			this.conditions  = conditions;

			if (codon.Properties.Contains("label")){
				Text = StringParser.Parse(codon.Properties["label"]);
			}
			if (imgButtonEnabled == null && codon.Properties.Contains("icon")) {
				imgButtonEnabled = WinFormsResourceService.GetBitmap(StringParser.Parse(codon.Properties["icon"]));
			}
			if (imgButtonDisabled == null && codon.Properties.Contains("disabledIcon")) {
				imgButtonDisabled = WinFormsResourceService.GetBitmap(StringParser.Parse(codon.Properties["disabledIcon"]));
			}
			if (imgButtonDisabled == null) {
				imgButtonDisabled = imgButtonEnabled;
			}
			menuCommand = codon.AddIn.CreateObject(codon.Properties["class"]) as ICommand;
			UpdateStatus();
			UpdateText();
		}

		void CreateDropDownItems()
		{
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
					IMenuItemBuilder submenuBuilder = (IMenuItemBuilder)item;
					DropDownItems.AddRange(submenuBuilder.BuildItems(codon, caller).Cast<ToolStripItem>().ToArray());
				}
			}
		}
		protected override void OnDropDownShow(EventArgs e)
		{
			if (!dropDownEnabled) {
				return;
			}
			if (codon != null && !this.DropDown.Visible)
			{
				CreateDropDownItems();
			}
			base.OnDropDownShow(e);
		}

		protected override void OnButtonClick(EventArgs e)
		{
			if (!buttonEnabled) {
				return;
			}
			base.OnButtonClick(e);
			menuCommand.Execute(caller);
		}
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = Condition.GetFailedAction(conditions, caller);
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
				if (menuCommand != null) {
					
					// menuCommand.IsEnabled is checked first so that it's get method can set dropDownEnabled as needed
					isEnabled &= menuCommand.CanExecute(caller) || dropDownEnabled;
				}
				
				return isEnabled;
			}
		}
		
		public bool ButtonEnabled {
			get {
				return buttonEnabled;
			}
			set {
				buttonEnabled = value;
				UpdateButtonImage();
			}
		}
		
		private void UpdateButtonImage()
		{
			//LoggingService.Info("UpdatingButtonImage: buttonEnabled=="+buttonEnabled.ToString());
			Image = buttonEnabled ? imgButtonEnabled : imgButtonDisabled;
		}
		
		public bool DropDownEnabled {
			get {
				return dropDownEnabled;
			}
			set {
				dropDownEnabled = value;
			}
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = Condition.GetFailedAction(conditions, caller);
				bool isVisible = failedAction != ConditionFailedAction.Exclude;
				if (base.Visible != isVisible) {
					base.Visible = isVisible;
				}
				
				if (this.Visible) {
					if (buttonEnabled && imgButtonEnabled!=null) {
						Image = imgButtonEnabled;
					} else if (imgButtonDisabled!=null) {
						Image = imgButtonDisabled;
					}
				}
				base.Enabled = this.Enabled; // fix for SD2-938 suggested by Matt Ward
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
