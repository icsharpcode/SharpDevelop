// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// This class is used to generate bold, italic and bold/italic fonts out
	/// of a base font.
	/// </summary>
	public class FontContainer
	{
		static Font defaultfont    = null;
		static Font boldfont       = null;
		static Font italicfont     = null;
		static Font bolditalicfont = null;
		
		/// <value>
		/// The bold version of the base font
		/// </value>
		public static Font BoldFont {
			get {
				Debug.Assert(boldfont != null, "ICSharpCode.TextEditor.Document.FontContainer : boldfont == null");
				return boldfont;
			}
		}
		
		/// <value>
		/// The italic version of the base font
		/// </value>
		public static Font ItalicFont {
			get {
				Debug.Assert(italicfont != null, "ICSharpCode.TextEditor.Document.FontContainer : italicfont == null");
				return italicfont;
			}
		}
		
		/// <value>
		/// The bold/italic version of the base font
		/// </value>
		public static Font BoldItalicFont {
			get {
				Debug.Assert(bolditalicfont != null, "ICSharpCode.TextEditor.Document.FontContainer : bolditalicfont == null");
				return bolditalicfont;
			}
		}
		
		/// <value>
		/// The base font
		/// </value>
		public static Font DefaultFont {
			get {
				return defaultfont;
			}
			set {
////// Alex: free resources properly
//				if (defaultfont!=null) defaultfont.Dispose();
				defaultfont    = value;
//				if (boldfont!=null) boldfont.Dispose();
				boldfont       = new Font(defaultfont, FontStyle.Bold);
//				if (italicfont!=null) italicfont.Dispose();
				italicfont     = new Font(defaultfont, FontStyle.Italic);
//				if (bolditalicfont!=null) bolditalicfont.Dispose();
				bolditalicfont = new Font(defaultfont, FontStyle.Bold | FontStyle.Italic);
			}
		}
		
//		static void CheckFontChange(object sender, PropertyEventArgs e)
//		{
//			if (e.Key == "DefaultFont") {
//				DefaultFont = ParseFont(e.NewValue.ToString());
//			}
//		}
		
		public static Font ParseFont(string font)
		{
			string[] descr = font.Split(new char[]{',', '='});
			return new Font(descr[1], Single.Parse(descr[3]));
		}
		
		static FontContainer()
		{
			DefaultFont = new Font("Courier New", 10);
		}
	}
}
