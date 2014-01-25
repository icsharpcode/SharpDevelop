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
using System.Linq;

using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Search;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace SearchAndReplace
{
	public static class SearchOptions
	{
		const string searchPropertyKey = "SearchAndReplaceProperties2";
		
		static Properties properties;
		
		public static Properties Properties {
			get {
				return properties;
			}
		}
		static string findPattern    = "";
		static string replacePattern = "";
		
		#region Search & Replace properties
		public static string FindPattern {
			get {
				return findPattern;
			}
			set {
				if (value != FindPattern) {
					findPattern = value;
					List<string> patterns = FindPatterns.ToList();
					patterns.Insert(0, value);
					FindPatterns = patterns;
				}
			}
		}
		
		public static string CurrentFindPattern {
			get {
				return findPattern;
			}
			set {
				findPattern = value;
			}
		}
		
		public static IReadOnlyList<string> FindPatterns {
			get {
				return properties.GetList<string>("FindPatterns");
			}
			set {
				properties.SetList("FindPatterns", value);
			}
		}
		
		public static string ReplacePattern {
			get {
				if (!properties.Contains("ReplacePatterns")) {
					return "";
				}
				return replacePattern;
			}
			set {
				if (value != ReplacePattern) {
					List<string> patterns = ReplacePatterns.ToList();
					patterns.Insert(0, value);
					ReplacePatterns = patterns;
					replacePattern = value;
				}
			}
		}
		
		public static IReadOnlyList<string> ReplacePatterns {
			get {
				return properties.GetList<string>("ReplacePatterns");
			}
			set {
				properties.SetList("ReplacePatterns", value);
			}
		}
		
		public static bool MatchCase {
			get {
				return properties.Get("MatchCase", false);
			}
			set {
				properties.Set("MatchCase", value);
			}
		}
		
		public static bool IncludeSubdirectories {
			get {
				return properties.Get("IncludeSubdirectories", false);
			}
			set {
				properties.Set("IncludeSubdirectories", value);
			}
		}
		
		public static bool MatchWholeWord {
			get {
				return properties.Get("MatchWholeWord", false);
			}
			set {
				properties.Set("MatchWholeWord", value);
			}
		}
		
		public static string LookIn {
			get {
				return properties.Get("LookIn", @"C:\");
			}
			set {
				properties.Set("LookIn", value);
			}
		}
		
		public static string LookInFiletypes {
			get {
				return properties.Get("LookInFiletypes", "*.*");
			}
			set {
				properties.Set("LookInFiletypes", value);
			}
		}
		
		public static SearchTarget SearchTarget {
			get {
				return properties.Get("SearchTarget", SearchTarget.CurrentDocument);
			}
			set {
				properties.Set("SearchTarget", value);
			}
		}
		
		public static SearchMode SearchMode {
			get {
				return properties.Get("SearchMode", SearchMode.Normal);
			}
			set {
				properties.Set("SearchMode", value);
			}
		}
		#endregion
		
		static SearchOptions()
		{
			properties = PropertyService.NestedProperties(searchPropertyKey);
		}
	}
	
	public class SearchAndReplaceTextEditorExtension : ITextEditorExtension
	{
		SearchPanel panel;
		
		public void Attach(ITextEditor editor)
		{
			TextArea textArea = editor.GetService(typeof(TextArea)) as TextArea;
			if (textArea != null) {
				panel = SearchPanel.Install(textArea);
				panel.SearchOptionsChanged += SearchOptionsChanged;
			}
		}

		void SearchOptionsChanged(object sender, SearchOptionsChangedEventArgs e)
		{
			SearchOptions.CurrentFindPattern = e.SearchPattern;
			SearchOptions.MatchCase = e.MatchCase;
			SearchOptions.MatchWholeWord = e.WholeWords;
			SearchOptions.SearchMode = e.UseRegex ? SearchMode.RegEx : SearchMode.Normal;
		}
		
		public void Detach()
		{
			if (panel != null) {
				panel.SearchOptionsChanged -= SearchOptionsChanged;
				panel.Uninstall();
				panel = null;
			}
		}
	}
}
