// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 2984 $</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Drawing;
using System.Xml;

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
			Settings.FileTypes = new string[] {".exe", ".dll"};
			Settings.ViewMode = ViewMode.Hexadecimal;
			
			Settings.DataForeColor = Color.Black;
			Settings.OffsetForeColor = Color.Blue;
			
			Settings.OffsetFont = Settings.DataFont = new Font("Courier New", 9.5f, FontStyle.Regular);
			
			return settings;
		}
		
		public static Color OffsetForeColor {
			get { return properties.Get("OffsetForeColor", Color.Blue); }
			set { properties.Set("OffsetForeColor", value); }
		}
		
		public static Color DataForeColor {
			get { return properties.Get("DataForeColor", Color.Black); }
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
		
		public static string[] FileTypes {
			get { return properties.Get("FileTypes", new string[] {".exe", ".dll"}); }
			set {
				properties.Set("FileTypes", value);
				properties.Set("FileTypesAsRegexString", "(" + string.Join("|", value) + ")$");
			}
		}
	}
}
