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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

using ICSharpCode.Core;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Holds a customized highlighting color.
	/// </summary>
	public class CustomizedHighlightingColor
	{
		/// <summary>
		/// The language to which this customization applies. null==all languages.
		/// </summary>
		public string Language { get; set; }
		
		/// <summary>
		/// The name of the highlighting color being modified.
		/// </summary>
		public string Name { get; set; }
		
		public bool Bold { get; set; }
		public bool Italic { get; set; }
		public Color? Foreground { get; set; }
		public Color? Background { get; set; }
		
		public static IReadOnlyList<CustomizedHighlightingColor> LoadColors()
		{
			return PropertyService.GetList<CustomizedHighlightingColor>("CustomizedHighlightingRules");
		}
		
		/// <summary>
		/// Saves the set of colors.
		/// </summary>
		public static void SaveColors(IEnumerable<CustomizedHighlightingColor> colors)
		{
			lock (staticLockObj) {
				activeColors = null;
				PropertyService.SetList("CustomizedHighlightingRules", colors);
			}
			EventHandler e = ActiveColorsChanged;
			if (e != null)
				e(null, EventArgs.Empty);
		}
		
		static IReadOnlyList<CustomizedHighlightingColor> activeColors;
		static readonly object staticLockObj = new object();
		
		public static IReadOnlyList<CustomizedHighlightingColor> ActiveColors {
			get {
				lock (staticLockObj) {
					if (activeColors == null)
						activeColors = LoadColors();
					return activeColors;
				}
			}
		}
		
		public static IEnumerable<CustomizedHighlightingColor> FetchCustomizations(string languageName)
		{
			// Access CustomizedHighlightingColor.ActiveColors within enumerator so that always the latest version is used.
			// Using CustomizedHighlightingColor.ActiveColors.Where(...) would not work correctly!
			foreach (CustomizedHighlightingColor color in CustomizedHighlightingColor.ActiveColors) {
				if (color.Language == null || color.Language == languageName)
					yield return color;
			}
		}
		
		/// <summary>
		/// Occurs when the set of customized highlighting colors was changed.
		/// </summary>
		public static EventHandler ActiveColorsChanged;
	}
}
