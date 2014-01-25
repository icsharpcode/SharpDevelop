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
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Class containing static properties for the code completion options.
	/// </summary>
	public static class CodeCompletionOptions
	{
		static Properties properties = PropertyService.NestedProperties("CodeCompletionOptions");
		
		public static Properties Properties {
			get {
				return properties;
			}
		}
		
		/// <summary>
		/// Global option to turn all code-completion-related features off.
		/// </summary>
		public static bool EnableCodeCompletion {
			get { return properties.Get("EnableCC", true); }
			set { properties.Set("EnableCC", value); }
		}
		
		public static bool DataUsageCacheEnabled {
			get { return properties.Get("DataUsageCacheEnabled", true); }
			set { properties.Set("DataUsageCacheEnabled", value); }
		}
		
		public static int DataUsageCacheItemCount {
			get { return properties.Get("DataUsageCacheItemCount", 500); }
			set { properties.Set("DataUsageCacheItemCount", value); }
		}
		
		public static bool TooltipsEnabled {
			get { return properties.Get("TooltipsEnabled", true); }
			set { properties.Set("TooltipsEnabled", value); }
		}
		
		public static bool TooltipsOnlyWhenDebugging {
			get { return properties.Get("TooltipsOnlyWhenDebugging", false); }
			set { properties.Set("TooltipsOnlyWhenDebugging", value); }
		}
		
		public static bool KeywordCompletionEnabled {
			get { return properties.Get("KeywordCompletionEnabled", true); }
			set { properties.Set("KeywordCompletionEnabled", value); }
		}
		
		public static bool CompleteWhenTyping {
			get { return properties.Get("CompleteWhenTyping", true); }
			set { properties.Set("CompleteWhenTyping", value); }
		}
		
		public static bool InsightEnabled {
			get { return properties.Get("InsightEnabled", true); }
			set { properties.Set("InsightEnabled", value); }
		}
		
		public static bool InsightRefreshOnComma {
			get { return properties.Get("InsightRefreshOnComma", true); }
			set { properties.Set("InsightRefreshOnComma", value); }
		}
	}
}
