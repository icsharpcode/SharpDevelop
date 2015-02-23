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

using System.Diagnostics;
using System.Windows;
using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Resize thumb that automatically disappears if the adornered element is too small.
	/// </summary>
	public sealed class ResizeThumb : DesignerThumb
	{
		bool checkWidth, checkHeight;

		internal ResizeThumb(bool checkWidth, bool checkHeight)
		{
			Debug.Assert((checkWidth && checkHeight) == false);
			this.checkWidth = checkWidth;
			this.checkHeight = checkHeight;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			AdornerPanel parent = this.Parent as AdornerPanel;
			if (parent != null && parent.AdornedElement != null)
			{
				if (checkWidth)
					this.ThumbVisible = PlacementOperation.GetRealElementSize(parent.AdornedElement).Width > 14;
				else if (checkHeight)
					this.ThumbVisible = PlacementOperation.GetRealElementSize(parent.AdornedElement).Height > 14;
			}
			return base.ArrangeOverride(arrangeBounds);
		}
	}
}
