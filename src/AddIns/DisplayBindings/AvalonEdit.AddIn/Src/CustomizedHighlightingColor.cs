// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		public string Language;
		
		/// <summary>
		/// The name of the highlighting color being modified.
		/// </summary>
		public string Name;
		
		public bool Bold, Italic;
		public Color? Foreground, Background;
		
		public static List<CustomizedHighlightingColor> LoadColors()
		{
			var list = PropertyService.Get("CustomizedHighlightingRules", new List<CustomizedHighlightingColor>());
			// Always make a copy of the list so that the original list cannot be modified without using SaveColors().
			return new List<CustomizedHighlightingColor>(list);
		}
		
		/// <summary>
		/// Saves the set of colors.
		/// </summary>
		public static void SaveColors(IEnumerable<CustomizedHighlightingColor> colors)
		{
			lock (staticLockObj) {
				activeColors = null;
				PropertyService.Set("CustomizedHighlightingRules", colors.ToList());
			}
			EventHandler e = ActiveColorsChanged;
			if (e != null)
				e(null, EventArgs.Empty);
		}
		
		static ReadOnlyCollection<CustomizedHighlightingColor> activeColors;
		static readonly object staticLockObj = new object();
		
		public static ReadOnlyCollection<CustomizedHighlightingColor> ActiveColors {
			get {
				lock (staticLockObj) {
					if (activeColors == null)
						activeColors = LoadColors().AsReadOnly();
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
