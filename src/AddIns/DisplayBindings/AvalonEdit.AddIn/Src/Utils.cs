// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
