// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Custom commands for AvalonEdit.
	/// </summary>
	public static class CustomCommands
	{
		public static readonly RoutedCommand CtrlSpaceCompletion = new RoutedCommand(
			"CtrlSpaceCompletion", typeof(CodeEditor),
			new InputGestureCollection {
				new KeyGesture(Key.Space, ModifierKeys.Control)
			});
		
		public static readonly RoutedCommand DeleteLine = new RoutedCommand(
			"DeleteLine", typeof(CodeEditor),
			new InputGestureCollection {
				new KeyGesture(Key.D, ModifierKeys.Control)
			});
	}
}
