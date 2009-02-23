// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A tool bar button based on the AddIn-tree.
	/// </summary>
	sealed class ToolBarSplitButton : SplitButton, IStatusUpdate
	{
		ICommand menuCommand;
		object caller;
		Codon codon;
		
		public ToolBarSplitButton(Codon codon, object caller, IList submenu)
		{
			ToolTipService.SetShowOnDisabled(this, true);
			
			this.codon = codon;
			this.caller = caller;
			
			if (codon.Properties.Contains("icon")) {
				var image = PresentationResourceService.GetImage(StringParser.Parse(codon.Properties["icon"]));
				image.Height = 16;
				image.SetResourceReference(StyleProperty, ToolBarService.ImageStyleKey);
				this.Content = new PixelSnapper(image);
			}
			
			menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			menuCommand.Owner = this;
			
			this.Command = new CommandWrapper(codon, caller, menuCommand);
			this.DropDownMenu = MenuService.CreateContextMenu(submenu);
			
			UpdateText();
		}
		
		public void UpdateText()
		{
			if (codon.Properties.Contains("label")){
				this.Content = StringParser.Parse(codon.Properties["label"]);
			}
			if (codon.Properties.Contains("tooltip")) {
				this.ToolTip = StringParser.Parse(codon.Properties["tooltip"]);
			}
		}
		
		public void UpdateStatus()
		{
			if (codon.GetFailedAction(caller) == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
		}
	}
}
