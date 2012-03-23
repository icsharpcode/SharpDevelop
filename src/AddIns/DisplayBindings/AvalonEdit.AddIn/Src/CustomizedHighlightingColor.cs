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
		
		/// <summary>
		/// Occurs when the set of customized highlighting colors was changed.
		/// </summary>
		public static EventHandler ActiveColorsChanged;
	}
}
