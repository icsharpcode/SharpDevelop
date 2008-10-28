// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using ICSharpCode.TextEditor;

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
				if (v.Length != 8)
					return null;
				string fileName = v[1];
				int lineNumber = int.Parse(v[2], culture);
				int columnNumber = int.Parse(v[3], culture);
				Debugging.BreakpointAction action = Debugging.BreakpointAction.Break;
				string scriptLanguage = "";
				string script = "";
				if (v[0] == "Breakpoint") {
					action = (Debugging.BreakpointAction)Enum.Parse(typeof(Debugging.BreakpointAction), v[5]);
					scriptLanguage = v[6];
					script = v[7];
				}
				if (lineNumber < 0)
					return null;
				if (columnNumber < 0)
					return null;
				SDBookmark bookmark;
				switch (v[0]) {
					case "Breakpoint":
						bookmark = new Debugging.BreakpointBookmark(fileName, null, new TextLocation(columnNumber, lineNumber), action, scriptLanguage, script);
						break;
					default:
						bookmark = new SDBookmark(fileName, null, new TextLocation(columnNumber, lineNumber));
						break;
				}
				bookmark.IsEnabled = bool.Parse(v[4]);
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
				if (bookmark is Debugging.BreakpointBookmark) {
					b.Append("Breakpoint");
				} else {
					b.Append("Bookmark");
				}
				b.Append('|');
				b.Append(bookmark.FileName);
				b.Append('|');
				b.Append(bookmark.LineNumber);
				b.Append('|');
				b.Append(bookmark.ColumnNumber);
				b.Append('|');
				b.Append(bookmark.IsEnabled.ToString());
				if (bookmark is Debugging.BreakpointBookmark) {
					Debugging.BreakpointBookmark bbm = (Debugging.BreakpointBookmark)bookmark;
					b.Append('|');
					b.Append(bbm.Action);
					b.Append('|');
					b.Append(bbm.ScriptLanguage);
					b.Append('|');
					b.Append(bbm.Condition);
				}
				return b.ToString();
			} else {
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
}
