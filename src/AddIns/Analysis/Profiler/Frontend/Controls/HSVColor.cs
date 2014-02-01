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
using System.Windows.Media;

namespace ICSharpCode.Profiler.Controls
{
	public struct HSVColor
	{
		Color color;
		float hue;
		
		public float Hue {
			get { return hue; }
			set { hue = value; }
		}
		
		float saturation;
		
		public float Saturation {
			get { return saturation; }
			set { saturation = value; }
		}
		
		float value;
		
		public float Value {
			get { return value; }
			set { this.value = value; }
		}
	
		public HSVColor(Color color)
		{
			this.color = color;
			this.hue = 0;
			this.saturation = 0;
			this.value = 0;
			
			float r = color.R / 255.0f;
			float g = color.G / 255.0f;
			float b = color.B / 255.0f;
	
			float max = Math.Max(r, Math.Max(g, b));
			float min = Math.Min(r, Math.Min(g, b));
	
			// calculation of Hue
			if (min == max)
				this.hue = 0;
			if (max == r)
				this.hue = 60.0f * (0 + (g - b) / (max - min));
			if (max == g)
				this.hue = 60.0f * (2 + (b - r) / (max - min));
			if (max == b)
				this.hue = 60.0f * (4 + (r - g) / (max - min));
			
			if (this.hue < 0)
				this.hue += 360.0f;
	
			// calculation of Saturation
			if (max == 0)
				this.saturation = 0;
			else
				this.saturation = (max - min) / max;
	
			// calculation of value
			this.value = max;
		}
	
		public static HSVColor FromColor(Color color)
		{
			return new HSVColor(color);
		}
	
		public Color ToColor()
		{
			if (!InInterval(hue, 0, 360))
				throw new ArgumentException("Hue is not between 0 and 360 degrees!");
			if (!InInterval(saturation, 0, 1))
				throw new ArgumentException("Saturation is not between 0 and 1!");
			if (!InInterval(value, 0, 1))
				throw new ArgumentException("Value is not between 0 and 1!");
	
			int hi = (int)Math.Floor(hue / 60.0f);
			float f = hue / 60.0f - hi;
			
			float p = value * (1 - saturation);
			float q = value * (1 - saturation * f);
			float t = value * (1 - saturation * (1 - f));
			
			switch (hi)
			{
				case 0:
					return Color.FromRgb((byte)(value * 255.0f), (byte)(t * 255.0f), (byte)(p * 255.0f));
				case 1:
					return Color.FromRgb((byte)(q * 255.0f), (byte)(value * 255.0f), (byte)(p * 255.0f));
				case 2:
					return Color.FromRgb((byte)(p * 255.0f), (byte)(value * 255.0f), (byte)(t * 255.0f));
				case 3:
					return Color.FromRgb((byte)(p * 255.0f), (byte)(q * 255.0f), (byte)(value * 255.0f));
				case 4:
					return Color.FromRgb((byte)(t * 255.0f), (byte)(p * 255.0f), (byte)(value * 255.0f));
				case 5:
					return Color.FromRgb((byte)(value * 255.0f), (byte)(p * 255.0f), (byte)(q * 255.0f));
			}
			
			return Colors.Black;
		}
	
		bool InInterval(float value, float lower, float upper)
		{
			return (value >= lower) && (value <= upper);
		}
	}
}
