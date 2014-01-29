// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	class MenuCommand : CoreMenuItem
	{
		readonly string ActivationMethod;
		
		public MenuCommand(UIElement inputBindingOwner, Codon codon, object caller, bool createCommand, string activationMethod, IReadOnlyCollection<ICondition> conditions) : base(codon, caller, conditions)
		{
			this.ActivationMethod = activationMethod;
			if (createCommand)
				this.Command = CommandWrapper.CreateCommand(codon, conditions);
			else
				this.Command = CommandWrapper.CreateLazyCommand(codon, conditions);
			this.CommandParameter = caller;
			
			if (!string.IsNullOrEmpty(codon.Properties["shortcut"])) {
				KeyGesture kg = MenuService.ParseShortcut(codon.Properties["shortcut"]);
				AddGestureToInputBindingOwner(inputBindingOwner, kg, this.Command, GetFeatureName());
				this.InputGestureText = MenuService.GetDisplayStringForShortcut(kg);
			}
		}
		
		internal static void AddGestureToInputBindingOwner(UIElement inputBindingOwner, KeyGesture kg, System.Windows.Input.ICommand shortcutCommand, string featureName)
		{
			if (inputBindingOwner != null && kg != null && shortcutCommand != null) {
				// wrap the shortcut command so that it can report to UDC with
				// different activation method
				if (!string.IsNullOrEmpty(featureName))
					shortcutCommand = new ShortcutCommandWrapper(shortcutCommand, featureName);
				// try to find an existing input binding which conflicts with this shortcut
				var existingInputBinding =
					(from InputBinding b in inputBindingOwner.InputBindings
					 let gesture = b.Gesture as KeyGesture
					 where gesture != null
					 && gesture.Key == kg.Key
					 && gesture.Modifiers == kg.Modifiers
					 select b
					).FirstOrDefault();
				if (existingInputBinding != null) {
					// modify the existing input binding to allow calling both commands
					existingInputBinding.Command = new AmbiguousCommandWrapper(existingInputBinding.Command, shortcutCommand);
				} else {
					inputBindingOwner.InputBindings.Add(new InputBinding(shortcutCommand, kg));
				}
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
				ServiceSingleton.GetRequiredService<IAnalyticsMonitor>().TrackFeature(feature, ActivationMethod);
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
				ServiceSingleton.GetRequiredService<IAnalyticsMonitor>().TrackFeature(featureName, "Shortcut");
				baseCommand.Execute(parameter);
			}
			
			public bool CanExecute(object parameter)
			{
				return baseCommand.CanExecute(parameter);
			}
		}
		
		sealed class AmbiguousCommandWrapper : System.Windows.Input.ICommand
		{
			System.Windows.Input.ICommand first;
			System.Windows.Input.ICommand second;
			
			public AmbiguousCommandWrapper(System.Windows.Input.ICommand first, System.Windows.Input.ICommand second)
			{
				this.first = first;
				this.second = second;
			}
			
			public event EventHandler CanExecuteChanged {
				add {
					first.CanExecuteChanged += value;
					second.CanExecuteChanged += value;
				}
				remove {
					first.CanExecuteChanged -= value;
					second.CanExecuteChanged -= value;
				}
			}
			
			public void Execute(object parameter)
			{
				if (first.CanExecute(parameter))
					first.Execute(parameter);
				else
					second.Execute(parameter);
			}
			
			public bool CanExecute(object parameter)
			{
				return first.CanExecute(parameter) || second.CanExecute(parameter);
			}
		}
	}
}
