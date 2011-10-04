// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Search
{
	/// <summary>
	/// Search commands for AvalonEdit.
	/// </summary>
	public static class SearchCommands
	{
		/// <summary>
		/// Finds the next occurrence in the file.
		/// </summary>
		public static readonly RoutedCommand FindNext = new RoutedCommand(
			"FindNext", typeof(SearchPanel),
			new InputGestureCollection { new KeyGesture(Key.F3) }
		);
		
		/// <summary>
		/// Finds the previous occurrence in the file.
		/// </summary>
		public static readonly RoutedCommand FindPrevious = new RoutedCommand(
			"FindPrevious", typeof(SearchPanel),
			new InputGestureCollection { new KeyGesture(Key.F3, ModifierKeys.Shift) }
		);
	}
	
	/// <summary>
	/// TextAreaInputHandler that registers all search-related commands.
	/// </summary>
	public class SearchInputHandler : TextAreaInputHandler
	{
		/// <summary>
		/// Creates a new SearchInputHandler and registers the search-related commands.
		/// </summary>
		public SearchInputHandler(TextArea textArea)
			: base(textArea)
		{
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, ExecuteFind));
			this.CommandBindings.Add(new CommandBinding(SearchCommands.FindNext, ExecuteFindNext));
			this.CommandBindings.Add(new CommandBinding(SearchCommands.FindPrevious, ExecuteFindPrevious));
		}
		
		void ExecuteFind(object sender, ExecutedRoutedEventArgs e)
		{
			var panel = TextArea.TextView.Layers.OfType<SearchPanel>().FirstOrDefault();
			if (panel == null)
				new SearchPanel(TextArea);
			else
				panel.Reactivate();
		}
		
		void ExecuteFindNext(object sender, ExecutedRoutedEventArgs e)
		{
			var panel = TextArea.TextView.Layers.OfType<SearchPanel>().FirstOrDefault();
			if (panel != null)
				panel.FindNext();
		}
		
		void ExecuteFindPrevious(object sender, ExecutedRoutedEventArgs e)
		{
			var panel = TextArea.TextView.Layers.OfType<SearchPanel>().FirstOrDefault();
			if (panel != null)
				panel.FindPrevious();
		}
	}
}
