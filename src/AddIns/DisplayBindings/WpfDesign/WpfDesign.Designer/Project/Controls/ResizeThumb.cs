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
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// A thumb where the look can depend on the IsPrimarySelection property.
	/// </summary>
	public class ResizeThumb : Thumb
	{
		/// <summary>
		/// Dependency property for <see cref="IsPrimarySelection"/>.
		/// </summary>
		public static readonly DependencyProperty IsPrimarySelectionProperty
			= DependencyProperty.Register("IsPrimarySelection", typeof(bool), typeof(ResizeThumb));
		
		/// <summary>
		/// Dependency property for <see cref="IsPrimarySelection"/>.
		/// </summary>
		public static readonly DependencyProperty ResizeThumbVisibleProperty
			= DependencyProperty.Register("ResizeThumbVisible", typeof(bool), typeof(ResizeThumb), new FrameworkPropertyMetadata(true));
		
		internal PlacementAlignment Alignment;
		
		static ResizeThumb()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeThumb), new FrameworkPropertyMetadata(typeof(ResizeThumb)));
		}
		
		/// <summary>
		/// Gets/Sets if the resize thumb is attached to the primary selection.
		/// </summary>
		public bool IsPrimarySelection {
			get { return (bool)GetValue(IsPrimarySelectionProperty); }
			set { SetValue(IsPrimarySelectionProperty, value); }
		}
		
		/// <summary>
		/// Gets/Sets if the resize thumb is visible.
		/// </summary>
		public bool ResizeThumbVisible {
			get { return (bool)GetValue(ResizeThumbVisibleProperty); }
			set { SetValue(ResizeThumbVisibleProperty, value); }
		}
	}
	
	/// <summary>
	/// Resize thumb that automatically disappears if the adornered element is too small.
	/// </summary>
	sealed class ResizeThumbImpl : ResizeThumb
	{
		bool checkWidth, checkHeight;
		
		internal ResizeThumbImpl(bool checkWidth, bool checkHeight)
		{
			Debug.Assert((checkWidth && checkHeight) == false);
			this.checkWidth = checkWidth;
			this.checkHeight = checkHeight;
		}
		
		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			AdornerPanel parent = this.Parent as AdornerPanel;
			if (parent != null && parent.AdornedElement != null) {
				if (checkWidth)
					this.ResizeThumbVisible = parent.AdornedElement.RenderSize.Width > 14;
				else if (checkHeight)
					this.ResizeThumbVisible = parent.AdornedElement.RenderSize.Height > 14;
			}
			return base.ArrangeOverride(arrangeBounds);
		}
	}
}
