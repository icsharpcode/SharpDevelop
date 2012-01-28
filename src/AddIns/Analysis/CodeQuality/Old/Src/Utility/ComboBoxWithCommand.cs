// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.CodeQualityAnalysis.Utility
{
	/// <summary>
	/// Description of ComboBoxWithCommand.
	/// http://stackoverflow.com/questions/2222430/wpf-command-support-in-combobox
	/// </summary>
	public class ComboBoxWithCommand : ComboBox, ICommandSource
	{
		private static EventHandler canExecuteChangedHandler;
		
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command",
		                                                                                        typeof(ICommand),
		                                                                                        typeof(ComboBoxWithCommand),
		                                                                                        new PropertyMetadata((ICommand)null,
		                                                                                                             new PropertyChangedCallback(CommandChanged)));
		
		public ICommand Command
		{
			get
			{
				return (ICommand)GetValue(CommandProperty);
			}
			set
			{
				SetValue(CommandProperty, value);
			}
			
		}
		
		public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget",
		                                                                                              typeof(IInputElement),
		                                                                                              typeof(ComboBoxWithCommand),
		                                                                                              new PropertyMetadata((IInputElement)null));
		
		public IInputElement CommandTarget
		{
			get
			{
				return (IInputElement)GetValue(CommandTargetProperty);
			}
			set
			{
				SetValue(CommandTargetProperty, value);
			}
		}
		
		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter",
		                                                                                                 typeof(object),
		                                                                                                 typeof(ComboBoxWithCommand),
		                                                                                                 new PropertyMetadata((object)null));
		
		public object CommandParameter
		{
			get
			{
				return (object)GetValue(CommandParameterProperty);
			}
			set
			{
				SetValue(CommandParameterProperty, value);
			}
		}
		
		public ComboBoxWithCommand() : base() { }
		
		
		private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ComboBoxWithCommand cb = (ComboBoxWithCommand)d;
			cb.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
		}
		
		private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
		{
			if (oldCommand != null)
			{
				RemoveCommand(oldCommand, newCommand);
			}
			AddCommand(oldCommand, newCommand);
		}
		
		private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
		{
			EventHandler handler = CanExecuteChanged;
			oldCommand.CanExecuteChanged -= handler;
		}
		
		private void AddCommand(ICommand oldCommand, ICommand newCommand)
		{
			EventHandler handler = new EventHandler(CanExecuteChanged);
			canExecuteChangedHandler = handler;
			if (newCommand != null)
			{
				newCommand.CanExecuteChanged += canExecuteChangedHandler;
			}
		}
		private void CanExecuteChanged(object sender, EventArgs e)
		{
			
			if (this.Command != null)
			{
				RoutedCommand command = this.Command as RoutedCommand;
				
				// If a RoutedCommand.
				if (command != null)
				{
					if (command.CanExecute(this.CommandParameter, this.CommandTarget))
					{
						this.IsEnabled = true;
					}
					else
					{
						this.IsEnabled = false;
					}
				}
				// If a not RoutedCommand.
				else
				{
					if (Command.CanExecute(CommandParameter))
					{
						this.IsEnabled = true;
					}
					else
					{
						this.IsEnabled = false;
					}
				}
			}
		}
		
		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			base.OnSelectionChanged(e);
			
			if (this.Command != null)
			{
				RoutedCommand command = this.Command as RoutedCommand;
				
				if (command != null)
				{
					command.Execute(this.CommandParameter, this.CommandTarget);
				}
				else
				{
					((ICommand)Command).Execute(CommandParameter);
				}
			}
		}
	}

}
