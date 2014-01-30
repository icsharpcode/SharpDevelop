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
using System.Windows;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// A combination of HorizontalAlignment/VerticalAlignment.
	/// </summary>
	public struct PlacementAlignment : IEquatable<PlacementAlignment>
	{
		readonly HorizontalAlignment horizontal;
		readonly VerticalAlignment vertical;
		
		/// <summary>
		/// Gets the horizontal component.
		/// </summary>
		public HorizontalAlignment Horizontal {
			get { return horizontal; }
		}
		
		/// <summary>
		/// Gets the vertical component.
		/// </summary>
		public VerticalAlignment Vertical {
			get { return vertical; }
		}
		
		/// <summary>
		/// Creates a new instance of the PlacementAlignment structure.
		/// </summary>
		public PlacementAlignment(HorizontalAlignment horizontal, VerticalAlignment vertical)
		{
			if (horizontal == HorizontalAlignment.Stretch)
				throw new ArgumentException("Strech is not a valid value", "horizontal");
			if (vertical == VerticalAlignment.Stretch)
				throw new ArgumentException("Strech is not a valid value", "vertical");
			this.horizontal = horizontal;
			this.vertical = vertical;
		}
		
		#region Equals and GetHashCode implementation
		/// <summary>Compares this to <paramref name="obj"/>.</summary>
		public override bool Equals(object obj)
		{
			if (obj is PlacementAlignment)
				return Equals((PlacementAlignment)obj); // use Equals method below
			else
				return false;
		}
		
		/// <summary>Compares this to <paramref name="other"/>.</summary>
		public bool Equals(PlacementAlignment other)
		{
			return this.horizontal == other.horizontal && this.vertical == other.vertical;
		}
		
		/// <summary>
		/// Gets the hash code.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked {
				return horizontal.GetHashCode() * 27 + vertical.GetHashCode();
			}
		}
		
		/// <summary>Compares <paramref name="lhs"/> to <paramref name="rhs"/>.</summary>
		public static bool operator ==(PlacementAlignment lhs, PlacementAlignment rhs)
		{
			return lhs.Equals(rhs);
		}
		
		/// <summary>Compares <paramref name="lhs"/> to <paramref name="rhs"/>.</summary>
		public static bool operator !=(PlacementAlignment lhs, PlacementAlignment rhs)
		{
			return !(lhs.Equals(rhs));
		}
		#endregion
		
		
		/// <summary>Top left</summary>
		public static readonly PlacementAlignment TopLeft = new PlacementAlignment(HorizontalAlignment.Left, VerticalAlignment.Top);
		/// <summary>Top center</summary>
		public static readonly PlacementAlignment Top = new PlacementAlignment(HorizontalAlignment.Center, VerticalAlignment.Top);
		/// <summary>Top right</summary>
		public static readonly PlacementAlignment TopRight = new PlacementAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);
		/// <summary>Center left</summary>
		public static readonly PlacementAlignment Left = new PlacementAlignment(HorizontalAlignment.Left, VerticalAlignment.Center);
		/// <summary>Center center</summary>
		public static readonly PlacementAlignment Center = new PlacementAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
		/// <summary>Center right</summary>
		public static readonly PlacementAlignment Right = new PlacementAlignment(HorizontalAlignment.Right, VerticalAlignment.Center);
		/// <summary>Bottom left</summary>
		public static readonly PlacementAlignment BottomLeft = new PlacementAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
		/// <summary>Bottom center</summary>
		public static readonly PlacementAlignment Bottom = new PlacementAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
		/// <summary>Bottom right</summary>
		public static readonly PlacementAlignment BottomRight = new PlacementAlignment(HorizontalAlignment.Right, VerticalAlignment.Bottom);
	}
}
