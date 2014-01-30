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
		static Properties properties = PropertyService.NestedProperties("HexEditorOptions");
		
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
