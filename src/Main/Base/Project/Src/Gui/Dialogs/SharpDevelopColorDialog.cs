// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
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
		
		protected override bool RunDialog(IntPtr hwndOwner)
		{
			bool result = base.RunDialog(hwndOwner);
			SaveCustomColors();
			return result;
		}
		
		void LoadCustomColors()
		{
			CustomColors = PropertyService.GetList<int>(CustomColorsPropertyName).ToArray();
		}
		
		void SaveCustomColors()
		{
			PropertyService.SetList(CustomColorsPropertyName, CustomColors);
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
