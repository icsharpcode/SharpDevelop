// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.SharpDevelop.AddIns.HighlightingEditor.Nodes
{
	public class EditorHighlightColor
	{
		bool hasForeColor = false;
		bool hasBackColor = false;
			
		Color  foreColor;
		bool   sysForeColor     = false;
		string sysForeColorName = "";
		
		Color  backColor;
		bool   sysBackColor     = false;
		string sysBackColorName = "";
		
		bool   bold   = false;
		bool   italic = false;
		
		bool noColor = false;
		
		public bool NoColor {
			get {
				return noColor;
			}
		}
		public bool HasForeColor {
			get {
				return hasForeColor;
			}
		}
		public bool HasBackColor {
			get {
				return hasBackColor;
			}
		}
		public Color ForeColor {
			get {
				return foreColor;
			}
		}
		public bool SysForeColor {
			get {
				return sysForeColor;
			}
		}
		public string SysForeColorName {
			get {
				return sysForeColorName;
			}
		}
		public Color BackColor {
			get {
				return backColor;
			}
		}
		public bool SysBackColor {
			get {
				return sysBackColor;
			}
		}
		public string SysBackColorName {
			get {
				return sysBackColorName;
			}
		}
		public bool Bold {
			get {
				return bold;
			}
		}
		public bool Italic {
			get {
				return italic;
			}
		}

		
		public EditorHighlightColor(XmlElement el)
		{
			if (el == null) {
				noColor = true;
				return;
			}
			
			if (el.Attributes["bold"] == null &&
			    el.Attributes["italic"] == null &&
			    el.Attributes["color"] == null &&
			    el.Attributes["bgcolor"] == null)
			{
			    noColor = true;
				return;
			}
			
			if (el.Attributes["bold"] != null) {
				bold = Boolean.Parse(el.Attributes["bold"].InnerText);
			}
			
			if (el.Attributes["italic"] != null) {
				italic = Boolean.Parse(el.Attributes["italic"].InnerText);
			}
			
			if (el.Attributes["color"] != null) {
				hasForeColor = true;
				string c = el.Attributes["color"].InnerText;
				if (c[0] == '#') {
					foreColor = ParseColor(c);
				} else if (c.StartsWith("SystemColors.")) {
					sysForeColor     = true;
					sysForeColorName = c.Substring("SystemColors.".Length);
				} else {
					PropertyInfo myPropInfo = typeof(Color).GetProperty(c, BindingFlags.Public | BindingFlags.Static);
					foreColor = (Color)myPropInfo.GetValue(null, null);
				}
			}
			
			if (el.Attributes["bgcolor"] != null) {
				hasBackColor = true;
				string c = el.Attributes["bgcolor"].InnerText;
				if (c[0] == '#') {
					backColor = ParseColor(c);
				} else if (c.StartsWith("SystemColors.")) {
					sysBackColor     = true;
					sysBackColorName = c.Substring("SystemColors.".Length);
				} else {
					PropertyInfo myPropInfo = typeof(Color).GetProperty(c, BindingFlags.Public | BindingFlags.Static);
					backColor = (Color)myPropInfo.GetValue(null, null);
				}
			}
		}
		
		public EditorHighlightColor(bool hascolor, Color Color, bool hasbackcolor, Color BackColor, bool bold, bool italic)
		{
			hasForeColor   = hascolor;
			hasBackColor   = hasbackcolor;
			this.backColor = BackColor;
			this.foreColor = Color;
			this.bold      = bold;
			this.italic    = italic;
		}
		
		public EditorHighlightColor(object ForeColor, object BackColor, bool Bold, bool Italic)
		{
			bold   = Bold;
			italic = Italic;
			
			if (ForeColor is Color) {
				hasForeColor = true;
				foreColor = (Color)ForeColor;
			} else if (ForeColor is string) {
				hasForeColor = true;
				sysForeColor = true;
				sysForeColorName = (string)ForeColor;
			}
			
			if (BackColor is Color) {
				hasBackColor = true;
				backColor = (Color)BackColor;
			} else if (BackColor is string) {
				hasBackColor = true;
				sysBackColor = true;
				sysBackColorName = (string)BackColor;
			}
		}
		
		public EditorHighlightColor()
		{
			bold = false;
			italic = false;
			hasForeColor = true;
			sysForeColor = true;
			sysForeColorName = "WindowText";
		}
		
		public EditorHighlightColor(bool NoColor) : this()
		{
			noColor = NoColor;
		}
		
		static Color ParseColor(string c)
		{
			int a = 255;
			int offset = 0;
			if (c.Length > 7) {
				offset = 2;
				a = Int32.Parse(c.Substring(1,2), NumberStyles.HexNumber);
			}
			
			int r = Int32.Parse(c.Substring(1 + offset,2), NumberStyles.HexNumber);
			int g = Int32.Parse(c.Substring(3 + offset,2), NumberStyles.HexNumber);
			int b = Int32.Parse(c.Substring(5 + offset,2), NumberStyles.HexNumber);
			return Color.FromArgb(a, r, g, b);
		}
		
		public string ToXml()
		{
			string str = "";
			str += "bold=\"" + bold.ToString().ToLower() + "\" ";
			str += "italic=\"" + italic.ToString().ToLower() + "\" ";
			if (hasForeColor) {
				str += "color=\"";
				if (sysForeColor) {
					str += "SystemColors." + sysForeColorName;
				} else {
					str += ReplaceColorName("#" + (foreColor.A != 255 ? foreColor.A.ToString("X2") : "") + 
								 foreColor.R.ToString("X2") +
								 foreColor.G.ToString("X2") + 
								 foreColor.B.ToString("X2"));
				}
					
				str += "\" ";
			}
			if (hasBackColor) {
				str += "bgcolor=\"";
				if (sysBackColor) {
					str += "SystemColors." + sysBackColorName;
				} else {
					str += ReplaceColorName("#" + (backColor.A != 255 ? backColor.A.ToString("X2") : "") + 
								 backColor.R.ToString("X2") + 
								 backColor.G.ToString("X2") + 
								 backColor.B.ToString("X2"));
				}
					
				str += "\" ";
			}
			return str;
		}
		
		Color ParseSysColor(string colorName)
		{
			string[] cNames = colorName.Split('*');
			PropertyInfo myPropInfo = typeof(System.Drawing.SystemColors).GetProperty(cNames[0], BindingFlags.Public | BindingFlags.Static);
			Color c = (Color)myPropInfo.GetValue(null, null);
			
			if (cNames.Length == 2) {
				// hack : can't figure out how to parse doubles with '.' (culture info might set the '.' to ',')
				double factor = Double.Parse(cNames[1]) / 100;
				c = Color.FromArgb((int)((double)c.R * factor), (int)((double)c.G * factor), (int)((double)c.B * factor));
			}
			
			return c;
		}
		
		public Color GetForeColor()
		{
			if (!hasForeColor) return Color.Transparent;
			
			if (sysForeColor) return ParseSysColor(sysForeColorName);
			
			return foreColor;
		}

		public Color GetBackColor()
		{
			if (!hasBackColor) return Color.Transparent;
			
			if (sysBackColor) return ParseSysColor(sysBackColorName);
			
			return backColor;
		}
		
		static Hashtable colorNames = new Hashtable();
		
		static string ReplaceColorName(string color)
		{
			if (colorNames.ContainsKey(color)) return (string)colorNames[color];
			return color;
		}
		
		static EditorHighlightColor()
		{
			PropertyInfo[] names = typeof(System.Drawing.Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
			
			foreach(PropertyInfo pi in names) {
				Color pcolor = (Color)pi.GetValue(null, null);
				string colorDesc = "#" + (pcolor.A != 255 ? pcolor.A.ToString("X2") : "") + pcolor.R.ToString("X2") + pcolor.G.ToString("X2") + pcolor.B.ToString("X2");
				try {
					colorNames.Add(colorDesc, pi.Name);
				} catch {}
			}
		}
	}
}
