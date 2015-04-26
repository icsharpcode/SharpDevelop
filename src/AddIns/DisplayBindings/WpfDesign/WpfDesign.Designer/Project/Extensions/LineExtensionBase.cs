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
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Designer.Extensions;
using System.Windows.Shapes;

using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Adorners;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.ComponentModel;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// base class for the Line, Polyline and Polygon extension classes
	/// </summary>
	public class LineExtensionBase : SelectionAdornerProvider
	{
		

		internal AdornerPanel adornerPanel;
		internal IEnumerable resizeThumbs;

		/// <summary>An array containing this.ExtendedItem as only element</summary>
		internal readonly DesignItem[] extendedItemArray = new DesignItem[1];

		internal IPlacementBehavior resizeBehavior;
		internal PlacementOperation operation;
		internal ChangeGroup changeGroup;
		private Canvas _surface;
		internal bool _isResizing;
		private TextBlock _text;
		//private DesignPanel designPanel;

		/// <summary>
		/// Gets whether this extension is resizing any element.
		/// </summary>
		public bool IsResizing
		{
			get { return _isResizing; }
		}

		/// <summary>
		/// on creation add adornerlayer
		/// </summary>
		public LineExtensionBase()
		{
			_surface = new Canvas();
			adornerPanel = new AdornerPanel(){ MinWidth = 10, MinHeight = 10 };
			adornerPanel.Order = AdornerOrder.Foreground;
			adornerPanel.Children.Add(_surface);
			Adorners.Add(adornerPanel);
		}

		#region eventhandlers


		protected void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			UpdateAdornerVisibility();
		}

		
		protected override void OnRemove()
		{
			this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
			base.OnRemove();
		}

		#endregion

		protected void UpdateAdornerVisibility()
		{
			FrameworkElement fe = this.ExtendedItem.View as FrameworkElement;
			foreach (DesignerThumb r in resizeThumbs)
			{
				bool isVisible = resizeBehavior != null &&
					resizeBehavior.CanPlace(extendedItemArray, PlacementType.Resize, r.Alignment);
				r.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
			}
		}

		/// <summary>
		/// Places resize thumbs at their respective positions
		/// and streches out thumbs which are at the center of outline to extend resizability across the whole outline
		/// </summary>
		/// <param name="designerThumb"></param>
		/// <param name="alignment"></param>
		/// <param name="index">if using a polygon or multipoint adorner this is the index of the point in the Points array</param>
		/// <returns></returns>
		protected PointTrackerPlacementSupport Place(ref DesignerThumb designerThumb, PlacementAlignment alignment, int index = -1)
		{
			PointTrackerPlacementSupport placement = new PointTrackerPlacementSupport(ExtendedItem.View as Shape, alignment, index);
			return placement;
		}

		/// <summary>
		/// forces redraw of shape
		/// </summary>
		protected void Invalidate()
		{
			Shape s = ExtendedItem.View as Shape;
			if (s != null)
			{
				s.InvalidateVisual();
				s.BringIntoView();
			}
		}

		protected void SetSurfaceInfo(int x, int y, string s)
		{
			if (_text == null) {
				_text = new TextBlock(){ FontSize = 8, FontStyle = FontStyles.Italic };
				_surface.Children.Add(_text);
			}
			
			AdornerPanel ap = _surface.Parent as AdornerPanel;
			
			_surface.Width = ap.Width;
			_surface.Height = ap.Height;

			_text.Text = s;
			Canvas.SetLeft(_text, x);
			Canvas.SetTop(_text, y);
		}

		protected void HideSizeAndShowHandles()
		{
			SizeDisplayExtension sizeDisplay = null;
			MarginHandleExtension marginDisplay = null;
			foreach (var extension in ExtendedItem.Extensions)
			{
				if (extension is SizeDisplayExtension)
					sizeDisplay = extension as SizeDisplayExtension;
				if (extension is MarginHandleExtension)
					marginDisplay = extension as MarginHandleExtension;
			}

			if (sizeDisplay != null)
			{
				sizeDisplay.HeightDisplay.Visibility = Visibility.Hidden;
				sizeDisplay.WidthDisplay.Visibility = Visibility.Hidden;
			}
			if (marginDisplay != null)
			{
				marginDisplay.ShowHandles();
			}
		}

		protected void ResetWidthHeightProperties()
		{
			ExtendedItem.Properties.GetProperty(FrameworkElement.HeightProperty).Reset();
			ExtendedItem.Properties.GetProperty(FrameworkElement.WidthProperty).Reset();
		}
	}
}
