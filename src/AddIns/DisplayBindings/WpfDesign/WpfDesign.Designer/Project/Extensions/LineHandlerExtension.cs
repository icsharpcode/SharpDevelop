/*
 * Created by SharpDevelop.
 * User: trubra
 * Date: 2014-12-22
 * Time: 10:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Description of LineHandlerExtension.
	/// </summary>
	[ExtensionFor(typeof(Line), OverrideExtensions = new Type[] { typeof(ResizeThumbExtension), typeof(SelectedElementRectangleExtension), typeof(CanvasPositionExtension), typeof(QuickOperationMenuExtension), typeof(RotateThumbExtension), typeof(RenderTransformOriginExtension), typeof(InPlaceEditorExtension) })]
    internal class LineHandlerExtension : LineExtensionBase
    {
        //
        private double CurrentX2;
        private double CurrentY2;
        private double CurrentLeft;
        private double CurrentTop;

        //Size oldSize;
        ZoomControl zoom;

        protected ResizeThumb CreateThumb(PlacementAlignment alignment, Cursor cursor)
        {
            ResizeThumb resizeThumb = new ResizeThumb { Alignment = alignment, Cursor = cursor, IsPrimarySelection = true};
            AdornerPanel.SetPlacement(resizeThumb, Place(ref resizeThumb, alignment));

            adornerPanel.Children.Add(resizeThumb);

            DragListener drag = new DragListener(resizeThumb);
            drag.Started += drag_Started;
            drag.Changed += drag_Changed;
            drag.Completed += drag_Completed;
            return resizeThumb;
        }

        protected Bounds CalculateDrawing(double x, double y, double left, double top, double xleft, double xtop)
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

            SetSurfaceInfo(0, 0, Math.Round((180 / Math.PI) * Math.Atan2(y, x), 0).ToString());
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
            if (designPanel != null)
            {
                var p = VisualTreeHelper.GetParent(designPanel);
                while (p != null && !(p is ZoomControl))
                {
                    p = VisualTreeHelper.GetParent(p);
                }
                zoom = p as ZoomControl;
            }

            if (resizeBehavior != null)
                operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
            else
            {
                changeGroup = this.ExtendedItem.Context.OpenGroup("Resize", extendedItemArray);
            }
            _isResizing = true;

            (drag.Target as ResizeThumb).IsPrimarySelection = false;
        }

        protected virtual void drag_Changed(DragListener drag)
        {
            Line al = ExtendedItem.View as Line;

            var alignment = (drag.Target as ResizeThumb).Alignment;
            var info = operation.PlacedItems[0];
            double dx = 0;
            double dy = 0;

            if (zoom != null)
            {
                dx = drag.Delta.X * (1 / zoom.CurrentZoom);
                dy = drag.Delta.Y * (1 / zoom.CurrentZoom);
            }
            double top, left, x, y, xtop, xleft;

            
            if (alignment == PlacementAlignment.TopLeft)
            {
                //normal values
                x = CurrentX2 - dx;
                y = CurrentY2 - dy;
                top = CurrentTop + dy;
                left = CurrentLeft + dx;

                //values to use when keys are pressed
                xtop = CurrentTop + CurrentY2;
                xleft = CurrentLeft + CurrentX2;

            }
            else
            {
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

            if (operation != null)
            {
                var result = info.OriginalBounds;
                result.X = position.Left;
                result.Y = position.Top;
                result.Width = Math.Abs(position.X);
                result.Height = Math.Abs(position.Y);

                info.Bounds = result.Round();
                operation.CurrentContainerBehavior.BeforeSetPosition(operation);
                operation.CurrentContainerBehavior.SetPosition(info);
            }
            (drag.Target as ResizeThumb).InvalidateArrange();
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
                if (drag.IsCanceled) changeGroup.Abort();
                else changeGroup.Commit();
                changeGroup = null;
            }

            _isResizing = false;
            (drag.Target as ResizeThumb).IsPrimarySelection = true;
            HideSizeAndShowHandles();
        }

        /// <summary>
        /// is invoked whenever a line is selected on the canvas, remember that the adorners are created for each line object and never destroyed
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (ExtendedItem.Properties.GetProperty(Line.StrokeProperty).ValueOnInstance == null)
            {
                ExtendedItem.Properties[Shape.StrokeProperty].SetValue(Colors.Black);
                ExtendedItem.Properties[Shape.StrokeThicknessProperty].SetValue(2d);
                ExtendedItem.Properties[Shape.StretchProperty].SetValue(Stretch.None);
                ExtendedItem.Properties.GetProperty(Line.X1Property).SetValue(0);
                ExtendedItem.Properties.GetProperty(Line.Y1Property).SetValue(0);
                ExtendedItem.Properties.GetProperty(Line.X2Property).SetValue(20);
                ExtendedItem.Properties.GetProperty(Line.Y2Property).SetValue(20);
                (ExtendedItem.View as Line).BringIntoView();
            }

            resizeThumbs = new ResizeThumb[]
                {
                    CreateThumb(PlacementAlignment.TopLeft, Cursors.SizeNWSE),
                    CreateThumb(PlacementAlignment.BottomRight, Cursors.SizeNWSE)
                };

            extendedItemArray[0] = this.ExtendedItem;

            Invalidate();
            //ResetWidthHeightProperties();

            this.ExtendedItem.PropertyChanged += OnPropertyChanged;
            this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
            resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);
            UpdateAdornerVisibility();
            OnPrimarySelectionChanged(null, null);
        }

        #endregion
    }
}
