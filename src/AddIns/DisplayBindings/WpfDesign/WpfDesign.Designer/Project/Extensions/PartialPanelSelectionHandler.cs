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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	public class PartialPanelSelectionHandler : BehaviorExtension, IHandlePointerToolMouseDown
	{
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.AddBehavior(typeof(IHandlePointerToolMouseDown), this);
		}
		
		#region IHandlePointerToolMouseDown

		public void HandleSelectionMouseDown(IDesignPanel designPanel, MouseButtonEventArgs e, DesignPanelHitTestResult result)
		{
			if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left))
			{
				e.Handled = true;
				new PartialRangeSelectionGesture(result.ModelHit).Start(designPanel, e);
			}
		}
		
		#endregion
	}

	/// <summary>
	/// 
	/// </summary>
	internal class PartialRangeSelectionGesture : RangeSelectionGesture
	{
		public PartialRangeSelectionGesture(DesignItem container)
			: base(container)
		{
		}

		protected override ICollection<DesignItem> GetChildDesignItemsInContainer(Geometry geometry)
		{
			HashSet<DesignItem> resultItems = new HashSet<DesignItem>();
			ViewService viewService = container.Services.View;

			HitTestFilterCallback filterCallback = delegate(DependencyObject potentialHitTestTarget)
			{
				FrameworkElement element = potentialHitTestTarget as FrameworkElement;
				if (element != null)
				{
					// ensure we are able to select elements with width/height=0
					if (element.ActualWidth == 0 || element.ActualHeight == 0)
					{
						DependencyObject tmp = element;
						DesignItem model = null;
						while (tmp != null)
						{
							model = viewService.GetModel(tmp);
							if (model != null) break;
							tmp = VisualTreeHelper.GetParent(tmp);
						}
						if (model != container)
						{
							resultItems.Add(model);
							return HitTestFilterBehavior.ContinueSkipChildren;
						}
					}
				}
				return HitTestFilterBehavior.Continue;
			};

			HitTestResultCallback resultCallback = delegate(HitTestResult result)
			{
				if (((GeometryHitTestResult)result).IntersectionDetail == IntersectionDetail.FullyInside || (Mouse.RightButton== MouseButtonState.Pressed && ((GeometryHitTestResult)result).IntersectionDetail == IntersectionDetail.Intersects))
				{
					// find the model for the visual contained in the selection area
					DependencyObject tmp = result.VisualHit;
					DesignItem model = null;
					while (tmp != null)
					{
						model = viewService.GetModel(tmp);
						if (model != null) break;
						tmp = VisualTreeHelper.GetParent(tmp);
					}
					if (model != container)
					{
						resultItems.Add(model);
					}
				}
				return HitTestResultBehavior.Continue;
			};

			VisualTreeHelper.HitTest(container.View, filterCallback, resultCallback, new GeometryHitTestParameters(geometry));
			return resultItems;
		}
	}
}
