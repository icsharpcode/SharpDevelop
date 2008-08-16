// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	class CommandWrapper : System.Windows.Input.ICommand
	{
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
		
		void CommandManager_RequerySuggested(object sender, EventArgs e)
		{
			//LoggingService.Debug("Received CommandManager_RequerySuggested ");
			if (canExecuteChanged != null)
				canExecuteChanged(this, e);
		}
		
		// keep a reference to the event handler to prevent it from being gargabe collected
		// (CommandManager.RequerySuggested only keeps weak references to the event handlers)
		EventHandler requerySuggestedEventHandler;
		
		// only attach to CommandManager.RequerySuggested if someone listens to the CanExecuteChanged event
		EventHandler canExecuteChanged;
		
		public event EventHandler CanExecuteChanged {
			add {
				if (canExecuteChanged == null && value != null) {
					//LoggingService.Debug("Attach CommandManager_RequerySuggested " + codon.Id);
					if (requerySuggestedEventHandler == null)
						requerySuggestedEventHandler = CommandManager_RequerySuggested;
					CommandManager.RequerySuggested += requerySuggestedEventHandler;
				}
				canExecuteChanged += value;
			}
			remove {
				canExecuteChanged -= value;
				if (canExecuteChanged == null) {
					//LoggingService.Debug("Detach CommandManager_RequerySuggested " + codon.Id);
					CommandManager.RequerySuggested -= requerySuggestedEventHandler;
				}
			}
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
		public MenuCommand(Codon codon, object caller, bool createCommand) : base(codon, caller)
		{
			this.Command = new CommandWrapper(codon, caller, createCommand);
		}
	}
}
