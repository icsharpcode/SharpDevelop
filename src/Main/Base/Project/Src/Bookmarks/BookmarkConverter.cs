// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Bookmarks
{
	public sealed class BookmarkConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string)) {
				return true;
			} else {
				return base.CanConvertFrom(context, sourceType);
			}
		}
		
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string) {
				string[] v = ((string)value).Split('|');
				
				FileName fileName = FileName.Create(v[1]);
				int lineNumber = int.Parse(v[2], culture);
				int columnNumber = int.Parse(v[3], culture);
				if (lineNumber < 0)
					return null;
				if (columnNumber < 0)
					return null;
				SDBookmark bookmark;
				switch (v[0]) {
					case "Breakpoint":
						Debugging.BreakpointAction action = Debugging.BreakpointAction.Break;
						string scriptLanguage = "";
						string script = "";
						action = (Debugging.BreakpointAction)Enum.Parse(typeof(Debugging.BreakpointAction), v[5]);
						scriptLanguage = v[6];
						script = v[7];
						
						var bbm = new Debugging.BreakpointBookmark(fileName, new Location(columnNumber, lineNumber), action, scriptLanguage, script);
						bbm.IsEnabled = bool.Parse(v[4]);
						bbm.Action = action;
						bbm.ScriptLanguage = scriptLanguage;
						bbm.Condition = script;
						bookmark = bbm;
						break;
					case "DecompiledBreakpointBookmark":
						action = (Debugging.BreakpointAction)Enum.Parse(typeof(Debugging.BreakpointAction), v[5]);
						scriptLanguage = v[6];
						script = v[7];
						
						bbm = new DecompiledBreakpointBookmark(fileName, new Location(columnNumber, lineNumber), action, scriptLanguage, script);
						bbm.IsEnabled = bool.Parse(v[4]);
						bbm.Action = action;
						bbm.ScriptLanguage = scriptLanguage;
						bbm.Condition = script;
						bookmark = bbm;
						break;
					case "PinBookmark":
						var pin = new PinBookmark(fileName, new Location(columnNumber, lineNumber));
						pin.Comment = v[4];
						pin.PinPosition =
							new Point
						{
							X = double.Parse(v[5], culture),
							Y = double.Parse(v[6], culture)
						};
						
						// pop-up nodes
						pin.SavedNodes = new System.Collections.Generic.List<Tuple<string, string, string>>();
						for (int i = 7; i < v.Length; i+=3) {
							pin.SavedNodes.Add(new Tuple<string, string, string>(v[i], v[i+1], v[i+2]));
						}
						
						bookmark = pin;
						break;
					default:
						bookmark = new Bookmark(fileName, new Location(columnNumber, lineNumber));
						break;
				}
				return bookmark;
			} else {
				return base.ConvertFrom(context, culture, value);
			}
		}
		
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			SDBookmark bookmark = value as SDBookmark;
			if (destinationType == typeof(string) && bookmark != null) {
				StringBuilder b = new StringBuilder();
				if (bookmark is DecompiledBreakpointBookmark) {
					b.Append("DecompiledBreakpointBookmark");
				} else if (bookmark is Debugging.BreakpointBookmark) {
					b.Append("Breakpoint");
				} else {
					if (bookmark is PinBookmark)
						b.Append("PinBookmark");
					else
						b.Append("Bookmark");
				}
				b.Append('|');
				b.Append(bookmark.FileName);
				b.Append('|');
				b.Append(bookmark.LineNumber);
				b.Append('|');
				b.Append(bookmark.ColumnNumber);
				
				if (bookmark is DecompiledBreakpointBookmark) {
					var bbm = (DecompiledBreakpointBookmark)bookmark;
					b.Append('|');
					b.Append(bbm.IsEnabled.ToString());
					b.Append('|');
					b.Append(bbm.Action.ToString());
					b.Append('|');
					b.Append(bbm.ScriptLanguage);
					b.Append('|');
					b.Append(bbm.Condition);
				} else if (bookmark is Debugging.BreakpointBookmark) {
					var bbm = (Debugging.BreakpointBookmark)bookmark;
					b.Append('|');
					b.Append(bbm.IsEnabled.ToString());
					b.Append('|');
					b.Append(bbm.Action.ToString());
					b.Append('|');
					b.Append(bbm.ScriptLanguage);
					b.Append('|');
					b.Append(bbm.Condition);
				}
				
				if (bookmark is PinBookmark) {
					var pin = (PinBookmark)bookmark;
					b.Append('|');
					b.Append(pin.Comment ?? string.Empty);
					
					// popup position
					b.Append('|');
					b.Append(pin.PinPosition.Value.X);
					b.Append('|');
					b.Append(pin.PinPosition.Value.Y);
					
					//popup nodes
					foreach(var node in pin.Nodes) {
						b.Append('|');
						b.Append(node.ImageName);
						b.Append('|');
						b.Append(node.FullName);
						b.Append('|');
						b.Append(node.Text);
					}
				}
				return b.ToString();
			} else {
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
}
