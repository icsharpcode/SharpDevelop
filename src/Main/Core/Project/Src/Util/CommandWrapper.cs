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
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace ICSharpCode.Core
{
	public sealed class CommandWrapper : ICommand
	{
		/// <summary>
		/// A delegate that is set by the host application, and gets executed to create link commands.
		/// </summary>
		public static Func<string, ICommand> LinkCommandCreator { get; set; }
		
		/// <summary>
		/// A delegate that is set by the host application, and gets executed to create well-known commands.
		/// </summary>
		public static Func<string, ICommand> WellKnownCommandCreator { get; set; }
		
		public static Action<EventHandler> RegisterConditionRequerySuggestedHandler { get; set; }
		public static Action<EventHandler> UnregisterConditionRequerySuggestedHandler { get; set; }
		
		/// <summary>
		/// Creates a lazy command.
		/// </summary>
		public static ICommand CreateLazyCommand(Codon codon, IReadOnlyCollection<ICondition> conditions)
		{
			if (codon.Properties["loadclasslazy"] == "false") {
				// if lazy loading was explicitly disabled, create the actual command now
				return CreateCommand(codon, conditions);
			}
			if (codon.Properties.Contains("command") && !codon.Properties.Contains("loadclasslazy")) {
				// If we're using the 'command=' syntax, this is most likely a built-in command
				// where lazy loading isn't useful (and hurts if CanExecute is used).
				// Don't use lazy loading unless loadclasslazy is set explicitly.
				return CreateCommand(codon, conditions);
			}
			// Create the wrapper that lazily loads the actual command.
			return new CommandWrapper(codon, conditions);
		}
		
		/// <summary>
		/// Creates a non-lazy command.
		/// </summary>
		public static ICommand CreateCommand(Codon codon, IReadOnlyCollection<ICondition> conditions)
		{
			ICommand command = CreateCommand(codon);
			if (command != null && conditions.Count == 0)
				return command;
			else
				return new CommandWrapper(command, conditions);
		}
		
		public static ICommand Unwrap(ICommand command)
		{
			CommandWrapper w = command as CommandWrapper;
			if (w != null) {
				w.EnsureCommandCreated();
				return w.addInCommand;
			} else {
				return command;
			}
		}
		
		static ICommand CreateCommand(Codon codon)
		{
			ICommand command = null;
			if (codon.Properties.Contains("command")) {
				string commandName = codon.Properties["command"];
				if (WellKnownCommandCreator != null) {
					command = WellKnownCommandCreator(commandName);
				}
				if (command == null) {
					command = GetCommandFromStaticProperty(codon.AddIn, commandName);
				}
				if (command == null) {
					MessageService.ShowError("Could not find command '" + commandName + "'.");
				}
			} else if (codon.Properties.Contains("link")) {
				if (LinkCommandCreator == null)
					throw new NotSupportedException("MenuCommand.LinkCommandCreator is not set, cannot create LinkCommands.");
				command = LinkCommandCreator(codon.Properties["link"]);
			} else {
				command = (ICommand)codon.AddIn.CreateObject(codon.Properties["class"]);
			}
			return command;
		}
		
		static ICommand GetCommandFromStaticProperty(AddIn addIn, string commandName)
		{
			int pos = commandName.LastIndexOf('.');
			if (pos > 0) {
				string className = commandName.Substring(0, pos);
				string propertyName = commandName.Substring(pos + 1);
				Type classType = addIn.FindType(className);
				if (classType != null) {
					PropertyInfo p = classType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
					if (p != null)
						return (ICommand)p.GetValue(null, null);
					FieldInfo f = classType.GetField(propertyName, BindingFlags.Public | BindingFlags.Static);
					if (f != null)
						return (ICommand)f.GetValue(null);
				}
			}
			return null;
		}
		
		bool commandCreated;
		ICommand addInCommand;
		readonly IReadOnlyCollection<ICondition> conditions;
		readonly Codon codon;
		
		private CommandWrapper(Codon codon, IReadOnlyCollection<ICondition> conditions)
		{
			if (conditions == null)
				throw new ArgumentNullException("conditions");
			this.codon = codon;
			this.conditions = conditions;
			this.canExecuteChangedHandlersToRegisterOnCommand = new WeakCollection<EventHandler>();
		}
		
		private CommandWrapper(ICommand command, IReadOnlyCollection<ICondition> conditions)
		{
			if (conditions == null)
				throw new ArgumentNullException("conditions");
			this.addInCommand = command;
			this.conditions = conditions;
			this.commandCreated = true;
		}
		
		void EnsureCommandCreated()
		{
			if (!commandCreated) {
				commandCreated = true;
				addInCommand = CreateCommand(codon);
				if (canExecuteChangedHandlersToRegisterOnCommand != null) {
					var handlers = canExecuteChangedHandlersToRegisterOnCommand.ToArray();
					canExecuteChangedHandlersToRegisterOnCommand = null;
					
					foreach (var handler in handlers) {
						if (addInCommand != null)
							addInCommand.CanExecuteChanged += handler;
						// Creating the command potentially changes the CanExecute state, so we should raise the event handlers once:
						handler(this, EventArgs.Empty);
					}
				}
			}
		}
		
		// maintain weak reference semantics for CanExecuteChanged
		WeakCollection<EventHandler> canExecuteChangedHandlersToRegisterOnCommand;
		
		public event EventHandler CanExecuteChanged {
			add {
				if (value == null)
					return;
				if (conditions.Count > 0 && RegisterConditionRequerySuggestedHandler != null)
					RegisterConditionRequerySuggestedHandler(value);
				
				if (addInCommand != null)
					addInCommand.CanExecuteChanged += value;
				else if (canExecuteChangedHandlersToRegisterOnCommand != null)
					canExecuteChangedHandlersToRegisterOnCommand.Add(value);
			}
			remove {
				if (value == null)
					return;
				if (conditions.Count > 0 && UnregisterConditionRequerySuggestedHandler != null)
					UnregisterConditionRequerySuggestedHandler(value);
				
				if (addInCommand != null)
					addInCommand.CanExecuteChanged -= value;
				else if (canExecuteChangedHandlersToRegisterOnCommand != null)
					canExecuteChangedHandlersToRegisterOnCommand.Remove(value);
			}
		}
		
		public void Execute(object parameter)
		{
			EnsureCommandCreated();
			if (CanExecute(parameter)) {
				addInCommand.Execute(parameter);
			}
		}
		
		public bool CanExecute(object parameter)
		{
			if (Condition.GetFailedAction(conditions, parameter) != ConditionFailedAction.Nothing)
				return false;
			if (!commandCreated)
				return true;
			return addInCommand != null && addInCommand.CanExecute(parameter);
		}
	}
}
