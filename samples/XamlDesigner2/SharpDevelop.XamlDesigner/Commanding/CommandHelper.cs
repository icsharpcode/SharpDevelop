using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Media;

namespace SharpDevelop.XamlDesigner.Commanding
{
	public static class CommandHelper
	{
		static CommandHelper()
		{
			CommandManager.RequerySuggested += RequerySuggestedDelegate;
		}

		static EventHandler RequerySuggestedDelegate = RequerySuggested;
		static DefaultContainerStyles DefaultContainerStyles = new DefaultContainerStyles();
		static List<WeakReference> TrackingElements = new List<WeakReference>();

		static MethodInfo CommandConverter_GetKnownCommand = typeof(CommandConverter)
			.GetMethod("GetKnownCommand", BindingFlags.NonPublic | BindingFlags.Static);

		public static string GetCommand(DependencyObject obj)
		{
			return (string)obj.GetValue(CommandProperty);
		}

		public static void SetCommand(DependencyObject obj, object value)
		{
			obj.SetValue(CommandProperty, value);
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached("Command", typeof(string), typeof(CommandHelper),
			new FrameworkPropertyMetadata(CommandPropertyChanged));

		//public static object GetCommandTarget(DependencyObject obj)
		//{
		//    return (object)obj.GetValue(CommandTargetProperty);
		//}

		//public static void SetCommandTarget(DependencyObject obj, object value)
		//{
		//    obj.SetValue(CommandTargetProperty, value);
		//}

		//public static readonly DependencyProperty CommandTargetProperty =
		//    DependencyProperty.RegisterAttached("CommandTarget", typeof(object), typeof(CommandHelper),
		//    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static CommandView GetCommandView(DependencyObject obj)
		{
			return (CommandView)obj.GetValue(CommandViewProperty);
		}

		public static void SetCommandView(DependencyObject obj, CommandView value)
		{
			obj.SetValue(CommandViewProperty, value);
		}

		public static readonly DependencyProperty CommandViewProperty =
			DependencyProperty.RegisterAttached("CommandView", typeof(CommandView), typeof(CommandHelper));

		static void CommandPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
		{
			var commandName = (string)e.NewValue;
			var element = target as FrameworkElement;
			var view = element.TryFindResource(commandName) as CommandView;

			if (view == null) {
				view = new CommandView();
				view.Text = commandName;
			}

			view.CommandName = commandName;

			SetCommandView(element, view);

			TrackingElements.Add(new WeakReference(element));

			var routedCommand = CommandConverter_GetKnownCommand.Invoke(null, new[] { commandName, null }) as RoutedCommand;
			if (routedCommand != null) {
				view.InitializeFromRoutedCommand(routedCommand);
			}

			var key = new ContainerStyleKey(element.GetType());
			var style = element.TryFindResource(key) as Style;
			if (style == null) {
				style = DefaultContainerStyles[key] as Style;
			}

			style.BasedOn = element.FindResource(element.GetType()) as Style;
			element.Style = style;

			if (view.Shortcut != null) {
				Application.Current.MainWindow.InputBindings.Add(
					new InputBinding(new ShortcutCommand() { Sender = element }, view.Shortcut));
			}
		}

		public static void RequerySuggested(object sender, EventArgs e)
		{
			foreach (var weak in TrackingElements) {
				if (weak.IsAlive) {
					var element = weak.Target as FrameworkElement;
					if (element.IsLoaded) {
						var handler = FindCommandHandler(element);
						var view = GetCommandView(element);
						if (handler != null) {
							view.IsEnabled = handler.CanExecute(null);
						}
						else {
							view.IsEnabled = false;
						}
					}
				}
			}
		}

		public static void TryExecuteCommand(object sender)
		{
			var handler = FindCommandHandler(sender);
			if (handler != null) {
				handler.Execute(null);
			}
		}

		public static CommandBase FindCommandHandler(object sender)
		{
			CommandBase handler = null;
			var d = sender as DependencyObject;
			if (d != null) {
				var commandView = GetCommandView(d);
				if (commandView != null) {
					//var target = GetCommandTarget(d);
					//if (target != null) {
					//    handler = FindCommandHandler(target, commandView.CommandName);
					//}
					if (handler == null) {
						handler = FindCommandHandler(sender, commandView.CommandName);
					}
				}
			}
			return handler;
		}

		static CommandBase FindCommandHandler(object start, string commandName)
		{
			foreach (var step in GetCommandChain(start)) {
				var hasCommands = step as IHasCommands;
				if (hasCommands != null) {
					foreach (var commandHandler in hasCommands.Commands) {
						if (commandHandler.CommandName == commandName) {
							return commandHandler;
						}
					}
				}
			}
			return null;
		}

		static IEnumerable<object> GetCommandChain(object start)
		{
			yield return start;
			var d = start as DependencyObject;
			if (d != null) {
				if (d != start) {
					yield return d;
				}
				var element = d as FrameworkElement;
				if (element != null) {
					yield return element.DataContext;
				}
				d = VisualTreeHelper.GetParent(d);
			}
		}

		class ShortcutCommand : ICommand
		{
			public FrameworkElement Sender;

			public bool CanExecute(object parameter)
			{
				var handler = FindCommandHandler(Sender);
				if (handler != null) {
					return handler.CanExecute(parameter);
				}
				return false;
			}

			public event EventHandler CanExecuteChanged;

			public void Execute(object parameter)
			{
				CommandHelper.TryExecuteCommand(Sender);
			}
		}
	}
}
