// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>
 
using System;
using System.Windows.Input;

namespace ICSharpCode.AvalonEdit
{
	/// <summary>
	/// Custom commands for AvalonEdit.
	/// </summary>
	public static class AvalonEditCommands
	{
		/// <summary>
		/// Deletes the current line.
		/// The default shortcut is Ctrl+D.
		/// </summary>
		public static readonly RoutedUICommand DeleteLine = new RoutedUICommand(
			"${res:AcalonEditCommands.DeleteLine}", "DeleteLine", typeof(AvalonEditCommands),
			new InputGestureCollection {
				new KeyGesture(Key.D, ModifierKeys.Control)
			});
		
		/// <summary>
		/// Removes leading whitespace from the selected lines (or the whole document if the selection is empty).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace",
		                                                 Justification = "WPF uses 'Whitespace'")]
		public static readonly RoutedUICommand RemoveLeadingWhitespace = new RoutedUICommand("${res:XML.MainMenu.EditMenu.FormatMenu.RlWs}", "RemoveLeadingWhitespace", typeof(AvalonEditCommands));
				
		/// <summary>
		/// Removes trailing whitespace from the selected lines (or the whole document if the selection is empty).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Whitespace",
		                                                 Justification = "WPF uses 'Whitespace'")]
		public static readonly RoutedUICommand RemoveTrailingWhitespace = new RoutedUICommand("${res:XML.MainMenu.EditMenu.FormatMenu.RtWs}", "RemoveTrailingWhitespace", typeof(AvalonEditCommands));
		
		/// <summary>
		/// Converts the selected text to upper case.
		/// </summary>
		public static readonly RoutedUICommand ConvertToUppercase = new RoutedUICommand("${res:XML.MainMenu.EditMenu.FormatMenu.UpperCase}", "ConvertToUppercase", typeof(AvalonEditCommands));
		
		/// <summary>
		/// Converts the selected text to lower case.
		/// </summary>
		public static readonly RoutedUICommand ConvertToLowercase = new RoutedUICommand("${res:XML.MainMenu.EditMenu.FormatMenu.LowerCase}", "ConvertToLowercase", typeof(AvalonEditCommands));
		
		/// <summary>
		/// Converts the selected text to title case.
		/// </summary>
		public static readonly RoutedUICommand ConvertToTitleCase = new RoutedUICommand("${res:XML.MainMenu.EditMenu.FormatMenu.Capitalize}", "ConvertToTitleCase", typeof(AvalonEditCommands));
		
		/// <summary>
		/// Inverts the case of the selected text.
		/// </summary>
		public static readonly RoutedUICommand InvertCase = new RoutedUICommand("${res:XML.MainMenu.EditMenu.FormatMenu.InvertCase}", "InvertCase", typeof(AvalonEditCommands));
		
		/// <summary>
		/// Converts tabs to spaces in the selected text.
		/// </summary>
		public static readonly RoutedUICommand ConvertTabsToSpaces = new RoutedUICommand("${res:XML.MainMenu.EditMenu.FormatMenu.Tab2Space}", "ConvertTabsToSpaces", typeof(AvalonEditCommands));
		
		/// <summary>
		/// Converts spaces to tabs in the selected text.
		/// </summary>
		public static readonly RoutedUICommand ConvertSpacesToTabs = new RoutedUICommand("${res:XML.MainMenu.EditMenu.FormatMenu.Space2Tab}", "ConvertSpacesToTabs", typeof(AvalonEditCommands));
		
		/// <summary>
		/// Converts leading tabs to spaces in the selected lines (or the whole document if the selection is empty).
		/// </summary>
		public static readonly RoutedUICommand ConvertLeadingTabsToSpaces = new RoutedUICommand("${res:XML.MainMenu.EditMenu.FormatMenu.LdTab2Space}", "ConvertLeadingTabsToSpaces", typeof(AvalonEditCommands));
		
		/// <summary>
		/// Converts leading spaces to tabs in the selected lines (or the whole document if the selection is empty).
		/// </summary>
		public static readonly RoutedUICommand ConvertLeadingSpacesToTabs = new RoutedUICommand("${res:XML.MainMenu.EditMenu.FormatMenu.LdSpace2Tab}", "ConvertLeadingSpacesToTabs", typeof(AvalonEditCommands));
		
		/// <summary>
		/// Runs the IIndentationStrategy on the selected lines (or the whole document if the selection is empty).
		/// </summary>
		public static readonly RoutedUICommand IndentSelection = new RoutedUICommand("${res:AcalonEditCommands.IndentSelection}",
			"IndentSelection", typeof(AvalonEditCommands),
			new InputGestureCollection {
				new KeyGesture(Key.I, ModifierKeys.Control)
			});
	}
}
