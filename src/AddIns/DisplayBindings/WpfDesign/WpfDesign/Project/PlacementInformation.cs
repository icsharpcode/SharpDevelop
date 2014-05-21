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
	/// Stores information about the placement of an individual item during a <see cref="PlacementOperation"/>.
	/// </summary>
	public sealed class PlacementInformation
	{
		/// <summary>
		/// The designer rounds bounds to this number of digits to avoid floating point errors.
		/// Value: 1
		/// </summary>
		public const int BoundsPrecision = 1;
		
		Rect originalBounds, bounds;
		readonly DesignItem item;
		readonly PlacementOperation operation;
		
		internal PlacementInformation(DesignItem item, PlacementOperation operation)
		{
			this.item = item;
			this.operation = operation;
		}
		
		/// <summary>
		/// The item being placed.
		/// </summary>
		public DesignItem Item {
			get { return item; }
		}
		
		/// <summary>
		/// The <see cref="PlacementOperation"/> to which this PlacementInformation belongs.
		/// </summary>
		public PlacementOperation Operation {
			get { return operation; }
		}
		
		/// <summary>
		/// Gets/sets the original bounds.
		/// </summary>
		public Rect OriginalBounds {
			get { return originalBounds; }
			set { originalBounds = value; }
		}
		
		/// <summary>
		/// Gets/sets the current bounds of the item.
		/// </summary>
		public Rect Bounds {
			get { return bounds; }
			set { bounds = value; }
		}

		/// <summary>
		/// Gets/sets the alignment of the resize thumb used to start the operation.
		/// </summary>
		public PlacementAlignment? ResizeThumbAlignment { get; set; }
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return "[PlacementInformation OriginalBounds=" + originalBounds + " Bounds=" + bounds + " Item=" + item + "]";
		}
	}
}
