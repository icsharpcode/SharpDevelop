// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="John Simons" email="johnsimons007@yahoo.com.au"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

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
		
		public ToolBarSplitButton(Codon codon, object caller, ArrayList subItems)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller        = caller;
			this.codon         = codon;
			this.subItems	   = subItems;

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
			menuCommand.Owner = this;
			UpdateStatus();
			UpdateText();
		}

		void CreateDropDownItems()
		{
			ToolStripItem[] itemsToAdd = null;
			
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
					itemsToAdd = submenuBuilder.BuildSubmenu(codon, caller);
					if (itemsToAdd!=null) {
						DropDownItems.AddRange(itemsToAdd);
					}
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
			menuCommand.Run();
		}
		
		public override bool Enabled {
			get {
				if (codon == null) {
					return base.Enabled;
				}
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				
				bool isEnabled = failedAction != ConditionFailedAction.Disable;
				
				if (menuCommand != null && menuCommand is IMenuCommand) {
					
					// menuCommand.IsEnabled is checked first so that it's get method can set dropDownEnabled as needed
					isEnabled &= (((IMenuCommand)menuCommand).IsEnabled || dropDownEnabled);
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
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
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
