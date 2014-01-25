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
