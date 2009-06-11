// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A tool bar button based on the AddIn-tree.
	/// </summary>
	sealed class ToolBarButton : Button, IStatusUpdate
	{
		readonly Codon codon;
		readonly object caller;
		
		public ToolBarButton(Codon codon, object caller, bool createCommand)
		{
			ToolTipService.SetShowOnDisabled(this, true);
			
			this.codon = codon;
			this.caller = caller;

			
			string routedCommandName = null;
			string routedCommandText = null;
			
			if(codon.Properties.Contains("command")) {
				routedCommandName = codon.Properties["command"];				
				routedCommandText = codon.Properties["command"];
			} else if(codon.Properties.Contains("link") || codon.Properties.Contains("class")) {
				routedCommandName = string.IsNullOrEmpty(codon.Properties["link"]) ? codon.Properties["class"] : codon.Properties["link"];
				routedCommandText = "Menu item \"" + codon.Properties["label"] + "\"";
			}

			var routedCommand = CommandsRegistry.GetRoutedUICommand(routedCommandName);
			if(routedCommand == null) {
				routedCommand = CommandsRegistry.RegisterRoutedUICommand(routedCommandName, routedCommandText);
			}
			   
			this.Command = routedCommand;
			
			if(!codon.Properties.Contains("command") && (codon.Properties.Contains("link") || codon.Properties.Contains("class"))) {
				var commandBindingInfo = new CommandBindingInfo();
				commandBindingInfo.AddIn = codon.AddIn;
				commandBindingInfo.ContextName = CommandsRegistry.DefaultContextName;
				commandBindingInfo.Class = CommandWrapper.GetCommand(codon, caller, createCommand);
				commandBindingInfo.RoutedCommandName = routedCommandName;
				commandBindingInfo.IsLazy = true;
				
				CommandsRegistry.RegisterCommandBinding(commandBindingInfo);
				CommandsRegistry.InvokeCommandBindingUpdateHandlers(CommandsRegistry.DefaultContextName, null);
			}
			
			if (codon.Properties.Contains("icon")) {
				var image = PresentationResourceService.GetImage(StringParser.Parse(codon.Properties["icon"]));
				image.Height = 16;
				image.SetResourceReference(StyleProperty, ToolBarService.ImageStyleKey);
				this.Content = new PixelSnapper(image);
			} else {
				this.Content = codon.Id;
			}
			UpdateText();
			
			SetResourceReference(FrameworkElement.StyleProperty, ToolBar.ButtonStyleKey);
		}
		
		public void UpdateText()
		{
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
