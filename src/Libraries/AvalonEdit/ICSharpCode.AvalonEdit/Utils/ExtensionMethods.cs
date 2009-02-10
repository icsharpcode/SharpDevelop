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
		/// Creates typeface from the framework element.
		/// </summary>
		public static Typeface CreateTypeface(this FrameworkElement fe)
		{
			return new Typeface((FontFamily)fe.GetValue(TextBlock.FontFamilyProperty),
			                    (FontStyle)fe.GetValue(TextBlock.FontStyleProperty),
			                    (FontWeight)fe.GetValue(TextBlock.FontWeightProperty),
			                    (FontStretch)fe.GetValue(TextBlock.FontStretchProperty));
		}
		
		/// <summary>
		/// Runs all outstanding dispatcher tasks with a priority higher or equal to <paramref name="priority"/>.
		/// </summary>
		public static void DoEvents(this Dispatcher dispatcher, DispatcherPriority priority)
		{
			dispatcher.VerifyAccess();
			DispatcherFrame frame = new DispatcherFrame();
			dispatcher.BeginInvoke(
				priority, new DispatcherOperationCallback(
					delegate {
						frame.Continue = false;
						return null;
					}), null);
			Dispatcher.PushFrame(frame);
		}
		
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
		
		/// <summary>
		/// Gets the end offset of the segment.
		/// </summary>
		public static int GetEndOffset(this ISegment segment)
		{
			return segment.Offset + segment.Length;
		}
	}
}
