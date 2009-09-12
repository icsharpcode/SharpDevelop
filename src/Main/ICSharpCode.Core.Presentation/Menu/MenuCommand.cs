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

namespace ICSharpCode.Core.Presentation
{
	class CommandWrapper : System.Windows.Input.ICommand
	{
		public static System.Windows.Input.ICommand GetCommand(Codon codon, object caller, bool createCommand)
		{
			string commandName = codon.Properties["command"];
			if (!string.IsNullOrEmpty(commandName)) {
				var wpfCommand = MenuService.GetRegisteredCommand(codon.AddIn, commandName);
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
		
		public ICommand GetAddInCommand()
		{
			if (!commandCreated) {
				CreateCommand();
			}
			return addInCommand;
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
				MessageService.ShowException(e, "Can't create menu command : " + codon.Id);
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
			this.Command = CommandWrapper.GetCommand(codon, caller, createCommand);
			if (!string.IsNullOrEmpty(codon.Properties["shortcut"])) {
				KeyGesture kg = MenuService.ParseShortcut(codon.Properties["shortcut"]);
				if (inputBindingOwner != null) {
					var shortcutCommand = this.Command;
					string featureName = GetFeatureName();
					if (shortcutCommand != null && !string.IsNullOrEmpty(featureName))
						shortcutCommand = new ShortcutCommandWrapper(shortcutCommand, featureName);
					inputBindingOwner.InputBindings.Add(new InputBinding(shortcutCommand, kg));
				}
				this.InputGestureText = kg.GetDisplayStringForCulture(Thread.CurrentThread.CurrentUICulture);
			}
		}
		
		string GetFeatureName()
		{
			string commandName = codon.Properties["command"];
			if (string.IsNullOrEmpty(commandName)) {
				return codon.Properties["class"];
			} else {
				return commandName;
			}
		}
		
		protected override void OnClick()
		{
			base.OnClick();
			string feature = GetFeatureName();
			if (!string.IsNullOrEmpty(feature)) {
				AnalyticsMonitorService.TrackFeature(feature, "Menu");
			}
		}
		
		sealed class ShortcutCommandWrapper : System.Windows.Input.ICommand
		{
			readonly System.Windows.Input.ICommand baseCommand;
			readonly string featureName;
			
			public ShortcutCommandWrapper(System.Windows.Input.ICommand baseCommand, string featureName)
			{
				Debug.Assert(baseCommand != null);
				Debug.Assert(featureName != null);
				this.baseCommand = baseCommand;
				this.featureName = featureName;
			}
			
			public event EventHandler CanExecuteChanged {
				add { baseCommand.CanExecuteChanged += value; }
				remove { baseCommand.CanExecuteChanged -= value; }
			}
			
			public void Execute(object parameter)
			{
				AnalyticsMonitorService.TrackFeature(featureName, "Shortcut");
				baseCommand.Execute(parameter);
			}
			
			public bool CanExecute(object parameter)
			{
				return baseCommand.CanExecute(parameter);
			}
		}
	}
}
