// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text;

namespace ICSharpCode.Core.Presentation
{
	class CommandWrapper : System.Windows.Input.ICommand
	{
		public static System.Windows.Input.ICommand GetCommand(Codon codon, object caller, bool createCommand)
		{
			string commandName = codon.Properties["command"];
			if (!string.IsNullOrEmpty(commandName)) {
				var wpfCommand = MenuService.GetRegisteredCommand(commandName);
				if (wpfCommand != null) {
					return wpfCommand;
				} else {
					MessageService.ShowError("Could not find WPF command '" + commandName + "'.");
					// return dummy command
					return new CommandWrapper(codon, caller, null);
				}
			}
			return new CommandWrapper(codon, caller, createCommand);
		}
		
		bool commandCreated;
		ICommand addInCommand;
		readonly Codon codon;
		readonly object caller;
		
		public CommandWrapper(Codon codon, object caller, bool createCommand)
		{
			this.codon = codon;
			this.caller = caller;
			if (createCommand) {
				commandCreated = true;
				CreateCommand();
			}
		}
		
		public CommandWrapper(Codon codon, object caller, ICommand command)
		{
			this.codon = codon;
			this.caller = caller;
			this.addInCommand = command;
			commandCreated = true;
		}
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification="We're displaying the message to the user.")]
		void CreateCommand()
		{
			commandCreated = true;
			try {
				string link = codon.Properties["link"];
				ICommand menuCommand;
				if (link != null && link.Length > 0) {
					if (MenuService.LinkCommandCreator == null)
						throw new NotSupportedException("MenuCommand.LinkCommandCreator is not set, cannot create LinkCommands.");
					menuCommand = MenuService.LinkCommandCreator(codon.Properties["link"]);
				} else {
					menuCommand = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
				}
				if (menuCommand != null) {
					menuCommand.Owner = caller;
				}
				addInCommand = menuCommand;
			} catch (Exception e) {
				MessageService.ShowError(e, "Can't create menu command : " + codon.Id);
			}
		}
		
		public event EventHandler CanExecuteChanged {
			add { System.Windows.Input.CommandManager.RequerySuggested += value; }
			remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
		}
		
		public void Execute(object parameter)
		{
			if (!commandCreated) {
				CreateCommand();
			}
			LoggingService.Debug("Execute " + codon.Id);
			if (CanExecute(parameter)) {
				addInCommand.Run();
			}
		}
		
		public bool CanExecute(object parameter)
		{
			//LoggingService.Debug("CanExecute " + codon.Id);
			if (codon.GetFailedAction(caller) != ConditionFailedAction.Nothing)
				return false;
			if (!commandCreated)
				return true;
			if (addInCommand == null)
				return false;
			IMenuCommand menuCommand = addInCommand as IMenuCommand;
			if (menuCommand != null) {
				return menuCommand.IsEnabled;
			} else {
				return true;
			}
		}
	}
	
	class MenuCommand : CoreMenuItem
	{
		public MenuCommand(UIElement inputBindingOwner, Codon codon, object caller, bool createCommand) : base(codon, caller)
		{
			string routedCommandName = null;
			string routedCommandText = null;
			
			if(codon.Properties.Contains("command")) {
				routedCommandName = codon.Properties["command"];				
				routedCommandText = codon.Properties["command"];
			} else if(codon.Properties.Contains("link") || codon.Properties.Contains("class")) {
				routedCommandName = string.IsNullOrEmpty(codon.Properties["link"]) ? codon.Properties["class"] : codon.Properties["link"];
				routedCommandText = "Menu item \"" + codon.Properties["label"] + "\"";
			}

			var routedCommand = CommandManager.GetRoutedUICommand(routedCommandName);
			if(routedCommand == null) {
				routedCommand = CommandManager.RegisterRoutedUICommand(routedCommandName, routedCommandText);
			}
			   
			this.Command = routedCommand;
			
			if(!codon.Properties.Contains("command") && (codon.Properties.Contains("link") || codon.Properties.Contains("class"))) {
				var commandBindingInfoName = "MenuCommandBinding_" + routedCommandName + "_" + codon.AddIn.Name + "_" + CommandManager.DefaultContextName;
				var commandBindingInfo = CommandManager.GetCommandBindingInfo(commandBindingInfoName);
				
				if(commandBindingInfo == null) {
					commandBindingInfo = new CommandBindingInfo();
					commandBindingInfo.AddIn = codon.AddIn;
					commandBindingInfo.OwnerTypeName = CommandManager.DefaultContextName;
					commandBindingInfo.CommandInstance = CommandWrapper.GetCommand(codon, caller, createCommand);
					commandBindingInfo.RoutedCommandName = routedCommandName;
					commandBindingInfo.IsLazy = true;
					
					commandBindingInfo.Name = commandBindingInfoName;
					CommandManager.RegisterCommandBinding(commandBindingInfo);
				}
			}
			
			if(codon.Properties.Contains("shortcut")) {
				var inputBindingInfoName = "MenuInputBinding_" + routedCommandName + "_" + codon.AddIn.Name + "_" + CommandManager.DefaultContextName;
				var inputBindingInfo = CommandManager.GetInputBindingInfo(inputBindingInfoName);
				
				if(inputBindingInfo == null) {
					var shortcut = codon.Properties["shortcut"];
					inputBindingInfo = new InputBindingInfo();
					inputBindingInfo.AddIn = codon.AddIn;
					inputBindingInfo.Categories.AddRange(CommandManager.RegisterInputBindingCategories("Menu Items"));
					inputBindingInfo.OwnerTypeName = CommandManager.DefaultContextName;
					inputBindingInfo.RoutedCommandName = routedCommandName;
					inputBindingInfo.Gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromInvariantString(codon.Properties["gestures"]);
					
					inputBindingInfo.Name = inputBindingInfoName;
					CommandManager.RegisterInputBinding(inputBindingInfo);
				}				
				
				BindingsUpdatedHandler gesturesUpdateHandler = delegate {
					var updatedGestures = CommandManager.FindInputGestures(null, null, null, null, routedCommandName);
					this.InputGestureText = (string)new InputGestureCollectionConverter().ConvertToInvariantString(updatedGestures);
				};
				
				gesturesUpdateHandler.Invoke();
				CommandManager.RegisterClassInputBindingsUpdateHandler(CommandManager.DefaultContextName, gesturesUpdateHandler);
			}
		}
	}
}
