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
using System.Drawing;
using Wpf = System.Windows.Media;
using ICSharpCode.AvalonEdit.AddIn;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ColorableItems : MarshalByRefObject, global::EnvDTE.ColorableItems
	{
		public static readonly Wpf.Color DefaultForegroundColor = System.Windows.SystemColors.WindowTextColor;
		public static readonly Wpf.Color DefaultBackgroundColor = System.Windows.SystemColors.WindowColor;
		
		CustomizedHighlightingColor color;
		FontsAndColorsItems fontsAndColorsItems;
		
		public ColorableItems(
			string name,
			CustomizedHighlightingColor color,
			FontsAndColorsItems fontsAndColorsItems)
		{
			this.Name = name;
			this.color = color;
			this.fontsAndColorsItems = fontsAndColorsItems;
		}
		
		public string Name { get; private set; }
		
		public bool Bold {
			get { return color.Bold; }
			set {
				color.Bold = value;
				SaveChanges();
			}
		}
		
		public uint Foreground {
			get { return GetOleColor(color.Foreground, DefaultForegroundColor); }
			set {
				SetForegroundColor(value);
				SaveChanges();
			}
		}
		
		public UInt32 Background {
			get { return GetOleColor(color.Background, DefaultBackgroundColor); }
			set { 
				SetBackgroundColor(value);
				SaveChanges();
			}
		}
		
		UInt32 GetOleColor(Wpf.Color? color, Wpf.Color defaultColor)
		{
			var convertedColor = ConvertToSystemDrawingColor(color, defaultColor);
			return (UInt32)ColorTranslator.ToOle(convertedColor);
		}
		
		Color ConvertToSystemDrawingColor(Wpf.Color? color, Wpf.Color defaultColor)
		{
			Wpf.Color colorValue = defaultColor;
			if (color.HasValue) {
				colorValue = color.Value;
			}
			return Color.FromArgb(colorValue.A, colorValue.R, colorValue.G, colorValue.B);
		}
		
		void SetForegroundColor(UInt32 oleColor)
		{
			Wpf.Color updatedColor = GetColorFromOleColor(oleColor);
			color.Foreground = updatedColor;
		}
		
		Wpf.Color GetColorFromOleColor(UInt32 oleColor)
		{
			Color convertedColor = ColorTranslator.FromOle((int)oleColor);
			return Wpf.Color.FromArgb(convertedColor.A, convertedColor.R, convertedColor.G, convertedColor.B);
		}
		
		void SetBackgroundColor(UInt32 oleColor)
		{
			Wpf.Color updatedColor = GetColorFromOleColor(oleColor);
			color.Background = updatedColor;
		}
		
		void SaveChanges()
		{
			fontsAndColorsItems.Save();
		}
	}
}
