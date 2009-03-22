// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Utils
{
	static class ExtensionMethods
	{
		#region Epsilon / IsClose / CoerceValue
		public const double Epsilon = 1e-8;
		
		/// <summary>
		/// Returns true if the doubles are close (difference smaller than 10^-8).
		/// </summary>
		public static bool IsClose(this double d1, double d2)
		{
			if (d1 == d2) // required for infinities
				return true;
			return Math.Abs(d1 - d2) < Epsilon;
		}
		
		/// <summary>
		/// Returns true if the doubles are close (difference smaller than 10^-8).
		/// </summary>
		public static bool IsClose(this Size d1, Size d2)
		{
			return IsClose(d1.Width, d2.Width) && IsClose(d1.Height, d2.Height);
		}
		
		/// <summary>
		/// Returns true if the doubles are close (difference smaller than 10^-8).
		/// </summary>
		public static bool IsClose(this Vector d1, Vector d2)
		{
			return IsClose(d1.X, d2.X) && IsClose(d1.Y, d2.Y);
		}
		
		/// <summary>
		/// Forces the value to stay between mininum and maximum.
		/// </summary>
		/// <returns>minimum, if value is less than minimum.
		/// Maximum, if value is greater than maximum.
		/// Otherwise, value.</returns>
		public static double CoerceValue(this double value, double minimum, double maximum)
		{
			return Math.Max(Math.Min(value, maximum), minimum);
		}
		
		/// <summary>
		/// Forces the value to stay between mininum and maximum.
		/// </summary>
		/// <returns>minimum, if value is less than minimum.
		/// Maximum, if value is greater than maximum.
		/// Otherwise, value.</returns>
		public static int CoerceValue(this int value, int minimum, int maximum)
		{
			return Math.Max(Math.Min(value, maximum), minimum);
		}
		#endregion
		
		#region CreateTypeface
		/// <summary>
		/// Creates typeface from the framework element.
		/// </summary>
		public static Typeface CreateTypeface(this FrameworkElement fe)
		{
			return new Typeface((FontFamily)fe.GetValue(TextBlock.FontFamilyProperty),
			                    (FontStyle)fe.GetValue(TextBlock.FontStyleProperty),
			                    (FontWeight)fe.GetValue(TextBlock.FontWeightProperty),
			                    (FontStretch)fe.GetValue(TextBlock.FontStretchProperty));
		}
		#endregion
		
		#region AddRange / Sequence
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elements)
		{
			foreach (T e in elements)
				collection.Add(e);
		}
		
		/// <summary>
		/// Creates an IEnumerable with a single value.
		/// </summary>
		public static IEnumerable<T> Sequence<T>(T value)
		{
			yield return value;
		}
		#endregion
		
		#region XML reading
		/// <summary>
		/// Gets the value of the attribute, or null if the attribute does not exist.
		/// </summary>
		public static string GetAttributeOrNull(this XmlElement element, string attributeName)
		{
			XmlAttribute attr = element.GetAttributeNode(attributeName);
			return attr != null ? attr.Value : null;
		}
		
		/// <summary>
		/// Gets the value of the attribute as boolean, or null if the attribute does not exist.
		/// </summary>
		public static bool? GetBoolAttribute(this XmlElement element, string attributeName)
		{
			XmlAttribute attr = element.GetAttributeNode(attributeName);
			return attr != null ? (bool?)XmlConvert.ToBoolean(attr.Value) : null;
		}
		
		/// <summary>
		/// Gets the value of the attribute as boolean, or null if the attribute does not exist.
		/// </summary>
		public static bool? GetBoolAttribute(this XmlReader reader, string attributeName)
		{
			string attributeValue = reader.GetAttribute(attributeName);
			if (attributeValue == null)
				return null;
			else
				return XmlConvert.ToBoolean(attributeValue);
		}
		#endregion
		
		#region ISegment extensions
		/// <summary>
		/// Gets whether the segment contains the offset.
		/// </summary>
		/// <returns>
		/// True, if offset is between segment.Start and segment.End (inclusive); otherwise, false.
		/// </returns>
		public static bool Contains(this ISegment segment, int offset)
		{
			int start = segment.Offset;
			int end = start + segment.Length;
			return offset >= start && offset <= end;
		}
		
		/// <summary>
		/// Gets the overlapping portion of the segments.
		/// Returns SimpleSegment.Invalid if the segments don't overlap.
		/// </summary>
		public static SimpleSegment GetOverlap(this ISegment segment, ISegment other)
		{
			int start = Math.Max(segment.Offset, other.Offset);
			int end = Math.Min(segment.EndOffset, other.EndOffset);
			if (end < start)
				return SimpleSegment.Invalid;
			else
				return new SimpleSegment(start, end - start);
		}
		#endregion
		
		#region System.Drawing <-> WPF conversions
		public static System.Drawing.Point ToSystemDrawing(this Point p)
		{
			return new System.Drawing.Point((int)p.X, (int)p.Y);
		}
		
		public static Point ToWpf(this System.Drawing.Point p)
		{
			return new Point(p.X, p.Y);
		}
		
		public static Size ToWpf(this System.Drawing.Size s)
		{
			return new Size(s.Width, s.Height);
		}
		
		public static Rect ToWpf(this System.Drawing.Rectangle rect)
		{
			return new Rect(rect.Location.ToWpf(), rect.Size.ToWpf());
		}
		#endregion
	}
}
