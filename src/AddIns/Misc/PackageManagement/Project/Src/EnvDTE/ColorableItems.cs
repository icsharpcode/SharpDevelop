// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
