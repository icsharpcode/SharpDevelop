// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace SearchAndReplace
{
	public enum SearchStrategyType {
		Normal,
		RegEx,
		Wildcard
	}
	
	public static class SearchOptions
	{
		const string searchPropertyKey = "SearchAndReplaceProperties";
		
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
				if (!properties.Contains("FindPatterns")) {
					return new string[] {};
				}
				return properties.Get("FindPatterns", "").Split('\xFF');
			}
			set {
				properties.Set("FindPatterns", String.Join("\xFF", value));
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
				if (!properties.Contains("ReplacePatterns")) {
					return new string[] {};
				}
				
				return properties.Get("ReplacePatterns", "").Split('\xFF');
			}
			set {
				properties.Set("ReplacePatterns", String.Join("\xFF", value));
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
		
		public static DocumentIteratorType DocumentIteratorType {
			get {
				return properties.Get("DocumentIteratorType", DocumentIteratorType.CurrentDocument);
			}
			set {
				if (!Enum.IsDefined(typeof(DocumentIteratorType), value))
					throw new ArgumentException("invalid enum value");
				properties.Set("DocumentIteratorType", value);
			}
		}
		
		public static SearchStrategyType SearchStrategyType {
			get {
				return properties.Get("SearchStrategyType", SearchStrategyType.Normal);
			}
			set {
				properties.Set("SearchStrategyType", value);
			}
		}
		#endregion
		
		static SearchOptions()
		{
			properties = PropertyService.Get(searchPropertyKey, new Properties());
		}
	}
}
