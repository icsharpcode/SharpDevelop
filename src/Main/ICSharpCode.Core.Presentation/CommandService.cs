// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Provides access to the RoutedUICommand objects for commands defined in the addin-tree.
	/// </summary>
	public static class CommandService
	{
		static Dictionary<string, RoutedUICommand> dict = new Dictionary<string, RoutedUICommand>();
		
		public static RoutedUICommand GetCommand(string className)
		{
			if (className == null)
				throw new ArgumentNullException("className");
			if (className.Length == 0)
				throw new ArgumentException("className must not be the empty string");
			lock (dict) {
				RoutedUICommand cmd;
				if (!dict.TryGetValue(className, out cmd)) {
					dict[className] = cmd = new RoutedUICommand(className, className, typeof(CommandService));
				}
				return cmd;
			}
		}
		
		public static RoutedUICommand GetCommand(Type commandClass)
		{
			if (commandClass == null)
				throw new ArgumentNullException("commandClass");
			if (!typeof(ICommand).IsAssignableFrom(commandClass))
				throw new ArgumentException("commandClass must implement ICommand", "commandClass");
			return GetCommand(commandClass.FullName);
		}
	}
}