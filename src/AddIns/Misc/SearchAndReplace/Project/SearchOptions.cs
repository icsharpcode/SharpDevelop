// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
					string[] oldPatterns = FindPatterns;
					string[] newPatterns = new string[oldPatterns.Length + 1];
					oldPatterns.CopyTo(newPatterns, 1);
					newPatterns[0] = value;
					FindPatterns = newPatterns;
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
		
		public static string[] FindPatterns {
			get {
				return properties.Get("FindPatterns", new string[0]);
			}
			set {
				properties.Set("FindPatterns", value);
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
					string[] oldPatterns = ReplacePatterns;
					string[] newPatterns = new string[oldPatterns.Length + 1];
					oldPatterns.CopyTo(newPatterns, 1);
					newPatterns[0] = value;
					ReplacePatterns = newPatterns;
					replacePattern = value;
				}
			}
		}
		
		public static string[] ReplacePatterns {
			get {
				return properties.Get("ReplacePatterns", new string[0]);
			}
			set {
				properties.Set("ReplacePatterns", value);
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
			properties = PropertyService.Get(searchPropertyKey, new Properties());
		}
	}
	
	public class SearchAndReplaceBinding : DefaultLanguageBinding
	{
		TextArea textArea;
		SearchInputHandler handler;
		
		public override void Attach(ITextEditor editor)
		{
			base.Attach(editor);
			textArea = editor.GetService(typeof(TextArea)) as TextArea;
			if (textArea != null) {
				handler = new SearchInputHandler(textArea);
				textArea.DefaultInputHandler.NestedInputHandlers.Add(handler);
				handler.SearchOptionsChanged += SearchOptionsChanged;
			}
		}

		void SearchOptionsChanged(object sender, SearchOptionsChangedEventArgs e)
		{
			SearchOptions.CurrentFindPattern = e.SearchPattern;
			SearchOptions.MatchCase = e.MatchCase;
			SearchOptions.MatchWholeWord = e.WholeWords;
			SearchOptions.SearchMode = e.UseRegex ? SearchMode.RegEx : SearchMode.Normal;
		}
		
		public override void Detach()
		{
			base.Detach();
			if (textArea != null) {
				textArea.DefaultInputHandler.NestedInputHandlers.Remove(handler);
				textArea = null;
				handler = null;
			}
		}
	}
}
