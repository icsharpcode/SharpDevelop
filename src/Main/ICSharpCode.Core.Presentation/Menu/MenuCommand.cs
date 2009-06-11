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
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
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
			
			if(codon.Properties.Contains("shortcut")) {
				var shortcut = codon.Properties["shortcut"];
				var inputBindingInfo = new InputBindingInfo();
				inputBindingInfo.AddIn = codon.AddIn;
				inputBindingInfo.CategoryName = "Menu items";
				inputBindingInfo.ContextName = CommandsRegistry.DefaultContextName;
				inputBindingInfo.RoutedCommandName = routedCommandName;
				inputBindingInfo.Gestures = (InputGestureCollection)new InputGestureCollectionConverter().ConvertFromInvariantString(codon.Properties["gestures"]);
			
				CommandsRegistry.RegisterInputBinding(inputBindingInfo);
				CommandsRegistry.RegisterInputBindingUpdateHandler(CommandsRegistry.DefaultContextName, null, delegate {
				                                                   		var updatedGestures = CommandsRegistry.FindInputGestures(null, null, routedCommandName);
				                                                   		this.InputGestureText = (string)new InputGestureCollectionConverter().ConvertToInvariantString(updatedGestures);
				                                                   });
				CommandsRegistry.InvokeInputBindingUpdateHandlers(CommandsRegistry.DefaultContextName, null);
			}
		}
	}
}
