// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Contains WPF routed commands for SharpDevelop-specific commands.
	/// </summary>
	public static class SharpDevelopRoutedCommands
	{
		public static readonly RoutedCommand SplitView = new RoutedCommand(
			"SplitView", typeof(SharpDevelopRoutedCommands));
	}
}
