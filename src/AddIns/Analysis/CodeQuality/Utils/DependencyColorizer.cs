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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.CodeQuality.Engine.Dom;

namespace ICSharpCode.CodeQuality
{
	/// <summary>
	/// Description of DependencyColorizer.
	/// </summary>
	public class DependencyColorizer : IColorizer<Tuple<int, int>>
	{
		private Dictionary<Color, SolidColorBrush> cache;
		
		public DependencyColorizer()
		{
			cache = new Dictionary<Color, SolidColorBrush>();
		}
		
		public SolidColorBrush GetColorBrush(Tuple<int, int> value)
		{
			var color = GetColor(value);
			if (cache.ContainsKey(color))
				return cache[color];
			
			var brush = new SolidColorBrush(color);
			brush.Freeze();
			
			cache[color] = brush;
			
			return brush;
		}
		
		public Color GetColor(Tuple<int, int> value)
		{
			// null or both = 0 => None
			if (value == null)
				return Colors.LightGray;
			if (value.Item1 == 0 && value.Item2 == 0)
				return Colors.LightGray;
			// both = -1 => Same
			if (value.Item1 == -1 && value.Item2 == -1)
				return Colors.Gray;
			// both > 0 => UsesAndUsedBy
			if (value.Item1 > 0 && value.Item2 > 0)
				return Colors.Turquoise;
			// a > 0 => Uses
			if (value.Item1 > 0)
				return Colors.LightGreen;
			// b > 0 => UsedBy
			if (value.Item2 > 0)
				return Colors.LightBlue;
			
			return Colors.LightGray;
		}
		
		public SolidColorBrush GetColorBrushMixedWith(Color color, Tuple<int, int> value)
		{
			var mixedColor = GetColor(value);
			mixedColor = mixedColor.MixedWith(color);
			
			if (cache.ContainsKey(mixedColor))
				return cache[mixedColor];
			
			var brush = new SolidColorBrush(mixedColor);
			brush.Freeze();
			
			cache[mixedColor] = brush;
			
			return brush;
		}
		
		public string GetUsesText(Tuple<int, int> value)
		{
			// null or both = 0 => None
			if (value == null)
				return "";
			if (value.Item1 == 0 && value.Item2 == 0)
				return "";
			// both = -1 => Same
			if (value.Item1 == -1 && value.Item2 == -1)
				return "";
			// both > 0 => UsesAndUsedBy
			if (value.Item1 > 0 && value.Item2 > 0)
				return value.Item1.ToString(CultureInfo.InvariantCulture);
			// a > 0 => Uses
			if (value.Item1 > 0)
				return value.Item1.ToString(CultureInfo.InvariantCulture);
			// b > 0 => UsedBy
			if (value.Item2 > 0)
				return value.Item2.ToString(CultureInfo.InvariantCulture);
			
			return "";
		}
		
		public string GetDescription(Tuple<int, int> value)
		{
			if (value.Item1 == -1 && value.Item2 == -1)
				return "is the same as";
			if (value.Item1 > 0 && value.Item2 > 0)
				return "uses and is used by";
			if (value.Item1 > 0)
				return "uses";
			if (value.Item2 > 0)
				return "is used by";
			return "is not related to";
		}
	}
}
