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
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.UIExtensions;
namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Description of LineHandlerExtension.
	/// </summary>
	[ExtensionFor(typeof(Line), OverrideExtensions = new Type[] { typeof(ResizeThumbExtension), typeof(SelectedElementRectangleExtension), typeof(CanvasPositionExtension), typeof(QuickOperationMenuExtension), typeof(RotateThumbExtension), typeof(RenderTransformOriginExtension), typeof(InPlaceEditorExtension), typeof(SkewThumbExtension) })]
	public class LineHandlerExtension : LineExtensionBase
	{
		/// <summary>
		/// Used instead of Rect to allow negative values on "Width" and "Height" (here called X and Y).
		/// </summary>
		class Bounds
		{
			public double X, Y, Left, Top;
		}
		
		//
		private double CurrentX2;
		private double CurrentY2;
		private double CurrentLeft;
		private double CurrentTop;

		//Size oldSize;
		ZoomControl zoom;

		public DragListener DragListener {get; private set;}
		
		protected DesignerThumb CreateThumb(PlacementAlignment alignment, Cursor cursor)
		{
			DesignerThumb designerThumb = new DesignerThumb { Alignment = alignment, Cursor = cursor, IsPrimarySelection = true};
			AdornerPanel.SetPlacement(designerThumb, Place(ref designerThumb, alignment));

			adornerPanel.Children.Add(designerThumb);

			DragListener = new DragListener(designerThumb);
			DragListener.Started += drag_Started;
			DragListener.Changed += drag_Changed;
			DragListener.Completed += drag_Completed;
			
			return designerThumb;
		}

		Bounds CalculateDrawing(double x, double y, double left, double top, double xleft, double xtop)
		{

			Double theta = (180 / Math.PI) * Math.Atan2(y, x);
			double verticaloffset = Math.Abs(90 - Math.Abs(theta));
			if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
			{
				if (Math.Abs(theta) < 45 || Math.Abs(theta) > 135)
				{
					y = 0;
					top = xtop;
				}
				else if (verticaloffset < 45)
				{
					x = 0;
					left = xleft;
				}
			}
			else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
			{
				if (verticaloffset < 10)
				{
					x = 0;
					left = xleft;
				}
				else if (Math.Abs(theta) < 10 || Math.Abs(theta) > 170)
				{
					y = 0;
					top = xtop;
				}
			}

			SetSurfaceInfo(0, 3, Math.Round((180 / Math.PI) * Math.Atan2(y, x), 0).ToString());
			return new Bounds { X = Math.Round(x, 1), Y = Math.Round(y, 1), Left = Math.Round(left, 1), Top = Math.Round(top, 1) };
		}

		#region eventhandlers


		// TODO : Remove all hide/show extensions from here.
		protected virtual void drag_Started(DragListener drag)
		{
			Line al = ExtendedItem.View as Line;
			CurrentX2 = al.X2;
			CurrentY2 = al.Y2;
			CurrentLeft = (double)al.GetValue(Canvas.LeftProperty);
			CurrentTop = (double)al.GetValue(Canvas.TopProperty);

			var designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
			zoom = designPanel.TryFindParent<ZoomControl>();
			
			if (resizeBehavior != null)
				operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
			else
			{
				changeGroup = this.ExtendedItem.Context.OpenGroup("Resize", extendedItemArray);
			}
			_isResizing = true;

			(drag.Target as DesignerThumb).IsPrimarySelection = false;
		}

		protected virtual void drag_Changed(DragListener drag)
		{
			Line al = ExtendedItem.View as Line;

			var alignment = (drag.Target as DesignerThumb).Alignment;
			var info = operation.PlacedItems[0];
			double dx = 0;
			double dy = 0;

			if (zoom != null) {
				dx = drag.Delta.X * (1 / zoom.CurrentZoom);
				dy = drag.Delta.Y * (1 / zoom.CurrentZoom);
			}
			
			double top, left, x, y, xtop, xleft;

			if (alignment == PlacementAlignment.TopLeft) {
				
				//normal values
				x = CurrentX2 - dx;
				y = CurrentY2 - dy;
				top = CurrentTop + dy;
				left = CurrentLeft + dx;

				//values to use when keys are pressed
				xtop = CurrentTop + CurrentY2;
				xleft = CurrentLeft + CurrentX2;

			} else {
				x = CurrentX2 + dx;
				y = CurrentY2 + dy;
				top = xtop = CurrentTop;
				left = xleft = CurrentLeft;
			}
			
			Bounds position = CalculateDrawing(x, y, left, top, xleft, xtop);

			ExtendedItem.Properties.GetProperty(Line.X1Property).SetValue(0);
			ExtendedItem.Properties.GetProperty(Line.Y1Property).SetValue(0);
			ExtendedItem.Properties.GetProperty(Line.X2Property).SetValue(position.X);
			ExtendedItem.Properties.GetProperty(Line.Y2Property).SetValue(position.Y);

			if (operation != null) {
				var result = info.OriginalBounds;
				result.X = position.Left;
				result.Y = position.Top;
				result.Width = Math.Abs(position.X);
				result.Height = Math.Abs(position.Y);

				info.Bounds = result.Round();
				operation.CurrentContainerBehavior.BeforeSetPosition(operation);
				operation.CurrentContainerBehavior.SetPosition(info);
				
//				var p = operation.CurrentContainerBehavior.PlacePoint(new Point(position.X, position.Y));
//				ExtendedItem.Properties.GetProperty(Line.X2Property).SetValue(p.X);
//				ExtendedItem.Properties.GetProperty(Line.Y2Property).SetValue(p.Y);
			}
			
			(drag.Target as DesignerThumb).InvalidateArrange();
			ResetWidthHeightProperties();
		}

		protected virtual void drag_Completed(DragListener drag)
		{
			if (operation != null)
			{
				if (drag.IsCanceled) operation.Abort();
				else
				{
					ResetWidthHeightProperties();

					operation.Commit();
				}
				operation = null;
			}
			else
			{
				if (drag.IsCanceled)
					changeGroup.Abort();
				else
					changeGroup.Commit();
				changeGroup = null;
			}

			_isResizing = false;
			(drag.Target as DesignerThumb).IsPrimarySelection = true;
			HideSizeAndShowHandles();
		}

		/// <summary>
		/// is invoked whenever a line is selected on the canvas, remember that the adorners are created for each line object and never destroyed
		/// </summary>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			
			resizeThumbs = new DesignerThumb[]
			{
				CreateThumb(PlacementAlignment.TopLeft, Cursors.Cross),
				CreateThumb(PlacementAlignment.BottomRight, Cursors.Cross)
			};

			extendedItemArray[0] = this.ExtendedItem;

			Invalidate();
			
			this.ExtendedItem.PropertyChanged += OnPropertyChanged;
			resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);
			UpdateAdornerVisibility();
		}

		#endregion
	}
}
