// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
