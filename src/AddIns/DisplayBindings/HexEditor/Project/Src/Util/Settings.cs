// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using WPF = System.Windows.Media;
using System.Xml;

using ICSharpCode.Core;

namespace HexEditor.Util
{
	/// <summary>
	/// Description of Settings.
	/// </summary>
	public class Settings
	{
		static Properties properties = PropertyService.Get("HexEditorOptions", new Properties());
		
		public static Properties Properties {
			get {
				return properties;
			}
		}
		
		public static Settings CreateDefault()
		{
			Settings settings = new Settings();
			
			Settings.BytesPerLine = 16;
			Settings.FitToWidth = false;
			Settings.ViewMode = ViewMode.Hexadecimal;
			
			Settings.DataForeColor = WPF.Colors.Black;
			Settings.OffsetForeColor = WPF.Colors.Blue;
			
			Settings.OffsetFont = Settings.DataFont = new Font("Courier New", 9.5f, FontStyle.Regular);
			
			return settings;
		}
		
		public static WPF.Color OffsetForeColor {
			get { return properties.Get("OffsetForeColor", WPF.Colors.Blue); }
			set { properties.Set("OffsetForeColor", value); }
		}
		
		public static WPF.Color DataForeColor {
			get { return properties.Get("DataForeColor", WPF.Colors.Black); }
			set { properties.Set("DataForeColor", value); }
		}

		public static Font OffsetFont {
			get { return properties.Get("OffsetFont", new Font("Courier New", 9.5f, FontStyle.Regular)); }
			set { properties.Set("OffsetFont", value); }
		}
		
		public static Font DataFont {
			get { return properties.Get("DataFont", new Font("Courier New", 9.5f, FontStyle.Regular)); }
			set { properties.Set("DataFont", value); }
		}
		
		public static bool FitToWidth {
			get { return properties.Get("FitToWidth", false); }
			set { properties.Set("FitToWidth", value); }
		}
		
		public static int BytesPerLine {
			get { return properties.Get("BytesPerLine", 16); }
			set { properties.Set("BytesPerLine", value); }
		}
		
		public static ViewMode ViewMode {
			get { return properties.Get("ViewMode", ViewMode.Hexadecimal); }
			set { properties.Set("ViewMode", value); }
		}
	}
}
