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
	public static class SearchCommands
	{
		public static readonly RoutedCommand FindNext = new RoutedCommand(
			"FindNext", typeof(SearchPanel),
			new InputGestureCollection { new KeyGesture(Key.F3) }
		);
		public static readonly RoutedCommand FindPrevious = new RoutedCommand(
			"FindPrevious", typeof(SearchPanel),
			new InputGestureCollection { new KeyGesture(Key.F3, ModifierKeys.Shift) }
		);
	}
	
	class SearchResult : TextSegment, ISearchResult
	{
	}
}
