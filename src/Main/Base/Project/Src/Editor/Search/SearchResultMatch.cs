// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Editor.Search
{
	public class SearchResultMatch
	{
		FileName fileName;
		Location startLocation;
		Location endLocation;
		HighlightedInlineBuilder builder;
		
		public FileName FileName {
			get { return fileName; }
		}
		
		public Location StartLocation {
			get { return startLocation; }
		}
		
		public Location EndLocation {
			get { return endLocation; }
		}
		
		public HighlightedInlineBuilder Builder {
			get { return builder; }
		}
		
		public virtual string TransformReplacePattern(string pattern)
		{
			return pattern;
		}
		
		public SearchResultMatch(FileName fileName, Location startLocation, Location endLocation, HighlightedInlineBuilder builder)
		{
			this.fileName = fileName;
			this.startLocation = startLocation;
			this.endLocation = endLocation;
			this.builder = builder;
		}
		
		/// <summary>
		/// Gets a special text to display, or null to display the line's content.
		/// </summary>
		public virtual string DisplayText {
			get {
				return null;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[{3}: FileName={0}, StartLocation={1}, EndLocation={2}]",
			                     fileName, startLocation, endLocation,
			                     GetType().Name);
		}
	}
	
	public class SimpleSearchResultMatch : SearchResultMatch
	{
		string displayText;
		
		public override string DisplayText {
			get {
				return displayText;
			}
		}
		
		public SimpleSearchResultMatch(FileName fileName, Location position, string displayText)
			: base(fileName, position, position, new HighlightedInlineBuilder(displayText))
		{
			this.displayText = displayText;
		}
	}
}
