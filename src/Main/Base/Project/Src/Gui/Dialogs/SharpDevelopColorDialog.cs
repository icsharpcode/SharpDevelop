// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// A ColorDialog that remembers any custom colors defined.
	/// </summary>
	public class SharpDevelopColorDialog : ColorDialog
	{
		const string CustomColorsPropertyName = "SharpDevelopColorDialog.CustomColors";
		
		public SharpDevelopColorDialog()
		{
			LoadCustomColors();
		}
		
		/// <summary>
		/// Converts a string of colors separated by the '|' character
		/// into an array of colors.
		/// </summary>
		public static int[] CustomColorsFromString(string s)
		{
			if (String.IsNullOrEmpty(s)) {
				return null;
			}
			
			string[] items = s.Split('|');
			List<int> colors = new List<int>();
			foreach (string item in items) {
				int color;
				if (Int32.TryParse(item, out color)) {
					colors.Add(color);
				}
			}
			return colors.ToArray();
		}
		
		/// <summary>
		/// Converts an integer array of colors into a string.
		/// </summary>
		public static string CustomColorsToString(int[] colors)
		{
			if (colors == null) {
				return String.Empty;
			} 
			
			StringBuilder s = new StringBuilder();
			for (int i = 0; i < colors.Length; ++i) {
				if (i != 0) {
					s.Append('|');
				}
				s.Append(colors[i]);
			}
			return s.ToString();
		}
		
		protected override bool RunDialog(IntPtr hwndOwner)
		{
			bool result = base.RunDialog(hwndOwner);
			SaveCustomColors();
			return result;
		}
		
		void LoadCustomColors()
		{
			CustomColors = CustomColorsFromString(PropertyService.Get(CustomColorsPropertyName));
		}
		
		void SaveCustomColors()
		{
			PropertyService.Set(CustomColorsPropertyName, CustomColorsToString(CustomColors));
		}
		
		public bool? ShowWpfDialog()
		{
			return ShowDialog() == DialogResult.OK;
		}
		
		public System.Windows.Media.Color WpfColor {
			get {
				var c = this.Color;
				return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
			}
			set {
				this.Color = System.Drawing.Color.FromArgb(value.A, value.R, value.G, value.B);
			}
		}
	}
}
