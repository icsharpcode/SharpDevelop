// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.SharpDevelop.Widgets.MyersDiff;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public static class Utils
	{
		public static OffsetChangeMap ToOffsetChangeMap(this IEnumerable<Edit> edits)
		{
			var map = new OffsetChangeMap();
			int diff = 0;
			foreach (var edit in edits) {
				Debug.Assert(edit.EditType != ChangeType.None && edit.EditType != ChangeType.Unsaved);
				int offset = edit.BeginA + diff;
				int removalLength = edit.EndA - edit.BeginA;
				int insertionLength = edit.EndB - edit.BeginB;
				
				diff += (insertionLength - removalLength);
				map.Add(new OffsetChangeMapEntry(offset, removalLength, insertionLength));
			}
			return map;
		}
		
		/// <summary>
		/// Copies editor options and default element customizations.
		/// Does not copy the syntax highlighting.
		/// </summary>
		public static void CopySettingsFrom(this TextEditor editor, TextEditor source)
		{
			editor.Options = source.Options;
			string language = source.SyntaxHighlighting != null ? source.SyntaxHighlighting.Name : null;
			CustomizingHighlighter.ApplyCustomizationsToDefaultElements(editor, CustomizedHighlightingColor.FetchCustomizations(language));
			HighlightingOptions.ApplyToRendering(editor, CustomizedHighlightingColor.FetchCustomizations(language));
		}
		
		static IEnumerable<CustomizedHighlightingColor> FetchCustomizations(string languageName)
		{
			return CustomizedHighlightingColor.FetchCustomizations(languageName);
		}
	}
}
