// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// Contains brushes/pens for the text editor to speed up drawing. Re-Creation of brushes and pens
	/// seems too costly.
	/// </summary>
	public class BrushRegistry
	{
		static Hashtable brushes = new Hashtable();
		static Hashtable pens    = new Hashtable();
		static Hashtable dotPens = new Hashtable();
		
		public static Brush GetBrush(Color color)
		{
			if (!brushes.Contains(color)) {
				Brush newBrush = new SolidBrush(color);
				brushes.Add(color, newBrush);
				return newBrush;
			}
			return brushes[color] as Brush;
		}
		
		public static Pen GetPen(Color color)
		{
			if (!pens.Contains(color)) {
				Pen newPen = new Pen(color);
				pens.Add(color, newPen);
				return newPen;
			}
			return pens[color] as Pen;
		}
		
		public static Pen GetDotPen(Color bgColor, Color fgColor)
		{
			bool containsBgColor = dotPens.Contains(bgColor);
			if (!containsBgColor || !((Hashtable)dotPens[bgColor]).Contains(fgColor)) {
				if (!containsBgColor) {
					dotPens[bgColor] = new Hashtable();
				}
				
				HatchBrush hb = new HatchBrush(HatchStyle.Percent50, bgColor, fgColor);
				Pen newPen = new Pen(hb);
				((Hashtable)dotPens[bgColor])[fgColor] = newPen;
				return newPen;
			}
			return ((Hashtable)dotPens[bgColor])[fgColor] as Pen;
		}
	}
}
