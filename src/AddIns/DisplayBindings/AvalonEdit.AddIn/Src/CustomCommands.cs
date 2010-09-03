// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Custom commands for CodeEditor.
	/// </summary>
	public static class CustomCommands
	{
		public static readonly RoutedCommand CtrlSpaceCompletion = new RoutedCommand(
			"CtrlSpaceCompletion", typeof(CodeEditor),
			new InputGestureCollection {
				new KeyGesture(Key.Space, ModifierKeys.Control)
			});
	}
}
