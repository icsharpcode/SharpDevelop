// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Editor.Search
{
	public class SearchResultMatch
	{
		FileName fileName;
		int offset;
		int length;
		TextLocation startLocation;
		TextLocation endLocation;
		RichText displayText;
		HighlightingColor defaultTextColor;
		
		public FileName FileName {
			get { return fileName; }
		}
		
		public TextLocation StartLocation {
			get { return startLocation; }
		}
		
		public TextLocation EndLocation {
			get { return endLocation; }
		}
		
		public HighlightingColor DefaultTextColor {
			get { return defaultTextColor; }
		}
		
		public int StartOffset {
			get { return offset; }
		}
		
		public int Length {
			get { return length; }
		}
		
		public int EndOffset {
			get { return offset + length; }
		}
		
		public virtual string TransformReplacePattern(string pattern)
		{
			return pattern;
		}
		
		public SearchResultMatch(FileName fileName, TextLocation startLocation, TextLocation endLocation, int offset, int length, RichText displayText, HighlightingColor defaultTextColor)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (startLocation.IsEmpty)
				throw new ArgumentOutOfRangeException("startLocation");
			if (endLocation.IsEmpty)
				throw new ArgumentOutOfRangeException("endLocation");
			this.fileName = fileName;
			this.startLocation = startLocation;
			this.endLocation = endLocation;
			this.offset = offset;
			this.length = length;
			this.displayText = displayText;
			this.defaultTextColor = defaultTextColor;
		}
		
		public static SearchResultMatch Create(IDocument document, TextLocation startLocation, TextLocation endLocation, IHighlighter highlighter)
		{
			int startOffset = document.GetOffset(startLocation);
			int endOffset = document.GetOffset(endLocation);
			var inlineBuilder = SearchResultsPad.CreateInlineBuilder(startLocation, endLocation, document, highlighter);
			var defaultTextColor = highlighter.DefaultTextColor;
			return new SearchResultMatch(FileName.Create(document.FileName),
			                             startLocation, endLocation,
			                             startOffset, endOffset - startOffset,
			                             inlineBuilder, defaultTextColor);
		}
		
		/// <summary>
		/// Gets a special text to display, or null to display the line's content.
		/// </summary>
		public RichText DisplayText {
			get {
				return displayText;
			}
		}
		
		public override string ToString()
		{
			return String.Format("[{3}: FileName={0}, StartLocation={1}, EndLocation={2}]",
			                     fileName, startLocation, endLocation,
			                     GetType().Name);
		}
	}
	
	public class AvalonEditSearchResultMatch : SearchResultMatch
	{
		ICSharpCode.AvalonEdit.Search.ISearchResult match;
		
		public AvalonEditSearchResultMatch(FileName fileName, TextLocation startLocation, TextLocation endLocation, int offset, int length, RichText richText, HighlightingColor defaultTextColor, ICSharpCode.AvalonEdit.Search.ISearchResult match)
			: base(fileName, startLocation, endLocation, offset, length, richText, defaultTextColor)
		{
			this.match = match;
		}
		
		public override string TransformReplacePattern(string pattern)
		{
			return match.ReplaceWith(pattern);
		}
	}
	
	public class RenameResultMatch : SearchResultMatch
	{
		public readonly string NewCode;
		
		public RenameResultMatch(FileName fileName, TextLocation startLocation, TextLocation endLocation, int offset, int length, string newCode, RichText richText = null, HighlightingColor defaultTextColor = null)
			: base(fileName, startLocation, endLocation, offset, length, richText, defaultTextColor)
		{
			this.NewCode = newCode;
		}
	}
	
	public class SearchedFile
	{
		public FileName FileName { get; private set; }
		
		public IReadOnlyList<SearchResultMatch> Matches { get; private set; }
		
		public SearchedFile(FileName fileName, IReadOnlyList<SearchResultMatch> matches)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (matches == null)
				throw new ArgumentNullException("matches");
			this.FileName = fileName;
			this.Matches = matches;
		}
	}
	
	public class PatchedFile : SearchedFile
	{
		ITextSourceVersion oldVersion;
		ITextSourceVersion newVersion;
		
		public PatchedFile(FileName fileName, IReadOnlyList<SearchResultMatch> matches, ITextSourceVersion oldVersion, ITextSourceVersion newVersion)
			: base(fileName, matches)
		{
			if (oldVersion == null)
				throw new ArgumentNullException("oldVersion");
			if (newVersion == null)
				throw new ArgumentNullException("newVersion");
			this.oldVersion = oldVersion;
			this.newVersion = newVersion;
		}
		
		public void Apply(IDocument document)
		{
			using (document.OpenUndoGroup()) {
				var changes = oldVersion.GetChangesTo(newVersion);
				foreach (var change in changes)
					document.Replace(change.Offset, change.RemovalLength, change.InsertedText);
			}
		}
	}
}
