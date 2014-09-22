/*
 * Created by SharpDevelop.
 * User: Abdelkarim
 * Date: 9/22/2014
 * Time: 8:07 AM
 * 
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public static class EditorSplitContainerCommands
	{
		private static RoutedUICommand _splitEditorCommand;

		private static RoutedUICommand _removeSplitCommand;

		/// <summary>
		/// 
		/// </summary>
		public static RoutedUICommand SplitEditorCommand {
			get {
				if (_splitEditorCommand == null)
					_splitEditorCommand = new RoutedUICommand("Split", "SplitEditor", typeof(EditorSplitContainerCommands));
				return _splitEditorCommand;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static RoutedUICommand RemoveSplitCommand {
			get {
				if (_removeSplitCommand == null) {
					_removeSplitCommand = new RoutedUICommand("Remove Split", "RemoveSplitEditor", typeof(EditorSplitContainerCommands));
				}
				return _removeSplitCommand;
			}
		}
	}
}


