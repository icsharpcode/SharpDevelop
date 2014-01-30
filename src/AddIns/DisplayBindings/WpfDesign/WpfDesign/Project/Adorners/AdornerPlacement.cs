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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Adorners
{
	/// <summary>
	/// Defines how a design-time adorner is placed.
	/// </summary>
	public abstract class AdornerPlacement
	{
		/// <summary>
		/// A placement instance that places the adorner above the content, using the same bounds as the content.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly AdornerPlacement FillContent = new FillContentPlacement();
		
		/// <summary>
		/// Arranges the adorner element on the specified adorner panel.
		/// </summary>
		public abstract void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize);
		
		sealed class FillContentPlacement : AdornerPlacement
		{
			public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
			{
				adorner.Arrange(new Rect(adornedElementSize));
			}
		}
	}
}
