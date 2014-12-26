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
	/// Description of PolyLineHandlerExtension.
	/// </summary>
	[ExtensionFor(typeof(Polyline), OverrideExtensions = new Type[] { typeof(ResizeThumbExtension), typeof(SelectedElementRectangleExtension), typeof(CanvasPositionExtension), typeof(QuickOperationMenuExtension), typeof(RotateThumbExtension), typeof(RenderTransformOriginExtension), typeof(InPlaceEditorExtension), typeof(SizeDisplayExtension) })]
    internal class PolyLineHandlerExtension : LineExtensionBase, IKeyDown, IKeyUp
    {
        private readonly Dictionary<int, Bounds> _selectedThumbs = new Dictionary<int, Bounds>();
        private bool _isDragging;
        ZoomControl _zoom;

        #region thumb methods
        protected ResizeThumb CreateThumb(PlacementAlignment alignment, Cursor cursor, int index)
        {
            ResizeThumb resizeThumb = new MultiPointResizeThumb { Index = index, Alignment = alignment, Cursor = cursor, IsPrimarySelection = true };
            AdornerPlacement ap = Place(ref resizeThumb, alignment, index);
            (resizeThumb as MultiPointResizeThumb).AdornerPlacement = ap;

            AdornerPanel.SetPlacement(resizeThumb, ap);
            adornerPanel.Children.Add(resizeThumb);

            DragListener drag = new DragListener(resizeThumb);

            WeakEventManager<ResizeThumb, MouseButtonEventArgs>.AddHandler(resizeThumb, "PreviewMouseLeftButtonDown", ResizeThumbOnMouseLeftButtonUp);

            drag.Started += drag_Started;
            drag.Changed += drag_Changed;
            drag.Completed += drag_Completed;
            return resizeThumb;
        }

        private void ResetThumbs()
        {
            foreach (FrameworkElement rt in adornerPanel.Children)
            {
                if (rt is ResizeThumb)
                    (rt as ResizeThumb).IsPrimarySelection = true;
            }
            _selectedThumbs.Clear();
        }

        private void SelectThumb(MultiPointResizeThumb mprt)
        {
            PointCollection points = GetPointCollection();
            Point p = points[mprt.Index];
            _selectedThumbs.Add(mprt.Index, new Bounds { X = p.X, Y = p.Y });

            mprt.IsPrimarySelection = false;
        }

        #endregion

        #region eventhandlers

        private void ResizeThumbOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            //get current thumb
            MultiPointResizeThumb mprt = sender as MultiPointResizeThumb;
            if (mprt != null)
            {
                //shift+ctrl will remove selected point 
                if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) &&
                    (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                {
                    //unselect all points
                    ResetThumbs();
                    PointCollection points = GetPointCollection();

                    //iterate thumbs to lower index of remaining thumbs
                    foreach (MultiPointResizeThumb m in adornerPanel.Children)
                    {
                        if (m.Index > mprt.Index)
                            m.Index--;
                    }

                    //remove point and thumb
                    points.RemoveAt(mprt.Index);
                    adornerPanel.Children.Remove(mprt);

                    Invalidate();
                }
                else
                {
                    //if not keyboard ctrl is pressed and selected point is not previously selected, clear selection
                    if (!_selectedThumbs.ContainsKey(mprt.Index) & !Keyboard.IsKeyDown(Key.LeftCtrl) &
                        !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        ResetThumbs();
                    }
                    //add selected thumb, if ctrl pressed this could be all points in poly
                    if (!_selectedThumbs.ContainsKey(mprt.Index))
                        SelectThumb(mprt);
                    _isDragging = false;
                }
            }
        }

        // TODO : Remove all hide/show extensions from here.
        protected void drag_Started(DragListener drag)
        {
            //get current thumb
            MultiPointResizeThumb mprt = (drag.Target as MultiPointResizeThumb);
            if (mprt != null)
            {
                SetOperation();
            }
        }

        void SetOperation()
        {
            var designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
            if (designPanel != null)
            {
                var p = VisualTreeHelper.GetParent(designPanel);
                while (p != null && !(p is ZoomControl))
                {
                    p = VisualTreeHelper.GetParent(p);
                }
                _zoom = p as ZoomControl;
            }

            if (resizeBehavior != null)
                operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
            else
            {
                changeGroup = ExtendedItem.Context.OpenGroup("Resize", extendedItemArray);
            }
            _isResizing = true;
        }

        void ChangeOperation(PointCollection points)
        {
            //this is for SharpDevelop built in undo functionality
            if (operation != null)
            {
                var info = operation.PlacedItems[0];
                var result = info.OriginalBounds;

                IEnumerable<double> xs = points.Select(x => x.X);
                IEnumerable<double> ys = points.Select(y => y.Y);
                result.X = (double)(info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).ValueOnInstance);
                result.Y = (double)(info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).ValueOnInstance);
                result.Width = xs.Max() - xs.Min();
                result.Height = ys.Max() - ys.Min();

                info.Bounds = result.Round();
                operation.CurrentContainerBehavior.BeforeSetPosition(operation);
                operation.CurrentContainerBehavior.SetPosition(info);
            }

            ResetWidthHeightProperties();
            
        }

        void CommitOperation()
        {
            if (operation != null)
            {
                PointCollection points;
                Polygon pg = ExtendedItem.View as Polygon;
                Polyline pl = ExtendedItem.View as Polyline;
                if (pl == null)
                {
                    points = pg.Points;

                }
                else
                {
                    points = pl.Points;
                }

                foreach (int i in _selectedThumbs.Keys)
                {
                    _selectedThumbs[i].X = points[i].X;
                    _selectedThumbs[i].Y = points[i].Y;
                }
                ExtendedItem.Properties.GetProperty(pl != null ? Polyline.PointsProperty : Polygon.PointsProperty).SetValue(points);
                ResetWidthHeightProperties();
                operation.Commit();

                operation = null;
            }
            else
            {
                changeGroup.Commit();
                changeGroup = null;
            }
            _isResizing = false;

            ResetWidthHeightProperties();
            Invalidate();
        }

        protected void drag_Changed(DragListener drag)
        {
            PointCollection points = GetPointCollection();

            MultiPointResizeThumb mprt = drag.Target as MultiPointResizeThumb;
            if (mprt != null)
            {
                double dx = 0;
                double dy = 0;
                //if has zoomed
                if (_zoom != null)
                {
                    dx = drag.Delta.X * (1 / _zoom.CurrentZoom);
                    dy = drag.Delta.Y * (1 / _zoom.CurrentZoom);
                }

                Double theta;
                //if one point selected snapping angle is calculated in relation to previous point
                if (_selectedThumbs.Count == 1)
                {
                    theta = (180 / Math.PI) * Math.Atan2(_selectedThumbs[mprt.Index].Y + dy - points[mprt.Index - 1].Y, _selectedThumbs[mprt.Index].X + dx - points[mprt.Index - 1].X);
                }
                else//if multiple points snapping angle is calculated in relation to mouse dragging angle
                {
                    theta = (180 / Math.PI) * Math.Atan2(dy, dx);
                }

                //snappingAngle is used for snapping function to horizontal or vertical plane in line drawing, and is activated by pressing ctrl or shift button
                int? snapAngle = null;

                //shift+alt gives a new point
                if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
                {
                    //if dragging occurs on a point and that point is the only selected, a new node will be added.
                    //_isCtrlDragging is needed since this method is called for every x pixel that the mouse moves
                    //so it could be many thousands of times during a single dragging
                    if (!_isDragging && _selectedThumbs.Count == 1 && (Math.Abs(dx) > 0 || Math.Abs(dy) > 0))
                    {

                        //duplicate point that is selected
                        Point p = points[mprt.Index];

                        //insert duplicate
                        points.Insert(mprt.Index, p);

                        //create adorner marker
                        CreateThumb(PlacementAlignment.BottomRight, Cursors.SizeNWSE, mprt.Index);

                        //set index of all points that had a higher index than selected to +1
                        foreach (FrameworkElement rt in adornerPanel.Children)
                        {
                            if (rt is MultiPointResizeThumb)
                            {
                                MultiPointResizeThumb t = rt as MultiPointResizeThumb;
                                if (t.Index > mprt.Index)
                                    t.Index++;
                            }
                        }

                        //set index of new point to old point index + 1
                        mprt.Index = mprt.Index + 1;
                        ResetThumbs();
                        SelectThumb(mprt);

                    }
                    snapAngle = 10;
                }

                //snapping occurs when mouse is within 10 degrees from horizontal or vertical plane if shift is pressed
                else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    snapAngle = 10;
                }
                //snapping occurs within 45 degree intervals that is line will always be horizontal or vertical if alt is pressed
                else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                {
                    snapAngle = 45;
                }
                _isDragging = true;
                points = MovePoints(points, dx, dy, theta, snapAngle);

            }
            ChangeOperation(points);
            (drag.Target as ResizeThumb).InvalidateArrange();
            

        }

        protected void drag_Completed(DragListener drag)
        {
            MultiPointResizeThumb mprt = drag.Target as MultiPointResizeThumb;
            if (mprt != null)
            {
                if (operation != null && drag.IsCanceled)
                {
                    operation.Abort();
                }
                else if (drag.IsCanceled)
                {
                    changeGroup.Abort();
                }
                CommitOperation();
            }
        }



        protected override void OnInitialized()
        {
            base.OnInitialized();

            PointCollection points = GetPointCollection();

            if (ExtendedItem.Properties[Shape.StrokeProperty].ValueOnInstance == null)
            {
                ExtendedItem.Properties[Shape.StrokeProperty].SetValue(Colors.Black);
                ExtendedItem.Properties[Shape.StrokeThicknessProperty].SetValue(2d);
                ExtendedItem.Properties[Shape.StretchProperty].SetValue(Stretch.None);
                points.AddRange(new List<Point> { new Point(0, 0), new Point(20, 20) });
                ExtendedItem.Properties.GetProperty(ExtendedItem.View as Polyline != null ? Polyline.PointsProperty : Polygon.PointsProperty).SetValue(points);
            }

            resizeThumbs = new List<ResizeThumb>();
            for (int i = 1; i < points.Count; i++)
            {
                CreateThumb(PlacementAlignment.BottomRight, Cursors.SizeNWSE, i);
            }

            Invalidate();

            ResetWidthHeightProperties();

            ResetThumbs();
            _isDragging = false;

            extendedItemArray[0] = ExtendedItem;
            ExtendedItem.PropertyChanged += OnPropertyChanged;
            Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
            resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);
            UpdateAdornerVisibility();
            OnPrimarySelectionChanged(null, null);
        }

        #endregion

        PointCollection GetPointCollection()
        {
            Polygon pg = ExtendedItem.View as Polygon;
            Polyline pl = ExtendedItem.View as Polyline;

            return pl == null ? pg.Points : pl.Points;
        }

        PointCollection MovePoints(PointCollection pc, double displacementX, double displacementY, double theta, int? snapangle)
        {
            //iterate all selected points
            foreach (int i in _selectedThumbs.Keys)
            {
                Point p = pc[i];

                //x and y is calculated from the currentl point  
                double x = _selectedThumbs[i].X + displacementX;
                double y = _selectedThumbs[i].Y + displacementY;

                //if snap is applied
                if (snapangle != null)
                {
                    if (_selectedThumbs.Count > 0)
                    {
                        //horizontal snap
                        if (Math.Abs(theta) < snapangle || 180 - Math.Abs(theta) < snapangle)
                        {
                            //if one point selected use point before as snap point, else snap to movement
                            y = _selectedThumbs.Count == 1 ? pc[i - 1].Y : y - displacementY;
                        }
                        else if (Math.Abs(90 - Math.Abs(theta)) < snapangle)//vertical snap
                        {
                            //if one point selected use point before as snap point, else snap to movement
                            x = _selectedThumbs.Count == 1 ? pc[i - 1].X : x - displacementX;
                        }
                    }
                }

                p.X = x;
                p.Y = y;
                pc[i] = p;
            }
            return pc;
        }

        #region IKeyDown

        public bool InvokeDefaultAction
        {
            get { return _selectedThumbs.Count == 0 || _selectedThumbs.Count == GetPointCollection().Count - 1; }
        }

        int _movingDistance;
        public void KeyDownAction(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("KeyDown");
            if (IsArrowKey(e.Key))
                if (operation == null)
                {
                    SetOperation();
                    _movingDistance = 0;
                }


            var dx1 = (e.Key == Key.Left) ? Keyboard.IsKeyDown(Key.LeftShift) ? _movingDistance - 10 : _movingDistance - 1 : 0;
            var dy1 = (e.Key == Key.Up) ? Keyboard.IsKeyDown(Key.LeftShift) ? _movingDistance - 10 : _movingDistance - 1 : 0;
            var dx2 = (e.Key == Key.Right) ? Keyboard.IsKeyDown(Key.LeftShift) ? _movingDistance + 10 : _movingDistance + 1 : 0;
            var dy2 = (e.Key == Key.Down) ? Keyboard.IsKeyDown(Key.LeftShift) ? _movingDistance + 10 : _movingDistance + 1 : 0;

            ChangeOperation(MovePoints(GetPointCollection(), dx1 + dx2, dy1 + dy2, 0, null));
            _movingDistance = (dx1 + dx2 + dy1 + dy2);
        }

        public void KeyUpAction(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("Keyup");
            if (IsArrowKey(e.Key)) 
                CommitOperation();
        }

        bool IsArrowKey(Key key)
        {
            return (key == Key.Left || key == Key.Right || key == Key.Up || key == Key.Down);
        }
        #endregion
    }
}
