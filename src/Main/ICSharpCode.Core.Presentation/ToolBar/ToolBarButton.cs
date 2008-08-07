// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// 
	/// </summary>
	sealed class ToolBarButton : Button, IStatusUpdate
	{
		readonly Codon codon;
		readonly object caller;
		ICommand menuCommand;
		
		public ToolBarButton(Codon codon, object caller, bool createCommand)
		{
			ToolTipService.SetShowOnDisabled(this, true);
			
			this.codon = codon;
			this.caller = caller;
			
			if (createCommand) {
				CreateCommand();
			}
			
			if (codon.Properties.Contains("icon")) {
				Image image = PresentationResourceService.GetImage(StringParser.Parse(codon.Properties["icon"]));
				image.Height = 16;
				image.SetResourceReference(StyleProperty, ToolBarService.ImageStyleKey);
				this.Content = image;
			}
			UpdateText();
			
			SetResourceReference(FrameworkElement.StyleProperty, ToolBar.ButtonStyleKey);
		}
		
		protected override void OnClick()
		{
			base.OnClick();
			if (menuCommand == null) {
				CreateCommand();
			}
			if (menuCommand != null) {
				menuCommand.Run();
			}
		}
		
		void CreateCommand()
		{
			menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			if (menuCommand != null) {
				menuCommand.Owner = caller;
			}
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
			this.IsEnabled = this.IsEnabledCore;
			if (this.IsEnabled) {
				this.Visibility = Visibility.Visible;
			} else {
				if (codon.GetFailedAction(caller) == ConditionFailedAction.Exclude)
					this.Visibility = Visibility.Collapsed;
				else
					this.Visibility = Visibility.Visible;
			}
		}
		
		protected override bool IsEnabledCore {
			get {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				bool isEnabled = failedAction == ConditionFailedAction.Nothing;
				if (isEnabled && menuCommand != null && menuCommand is IMenuCommand) {
					isEnabled = ((IMenuCommand)menuCommand).IsEnabled;
				}
				return isEnabled;
			}
		}
	}
}
