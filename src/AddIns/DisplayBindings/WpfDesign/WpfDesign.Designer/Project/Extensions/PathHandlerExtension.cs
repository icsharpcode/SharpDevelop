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
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.WpfDesign.Designer.UIExtensions;
using DragListener = ICSharpCode.WpfDesign.Designer.Controls.DragListener;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Description of PathHandlerExtension.
	/// </summary>
	[ExtensionFor(typeof(Path))]
	internal class PathHandlerExtension : LineExtensionBase, IKeyDown, IKeyUp
	{
		enum PathPartConvertType
		{
			ToLineSegment,
			ToBezierSegment,
			ToQuadricBezierSegment,
		}

		protected class PathPoint
		{
			public PathPoint(Point point, Object @object, Object parentObject, Action<Point> setLambda)
			{
				this._point = point;
				this._setLambda = setLambda;
				this.Object = @object;
				this.ParentObject = parentObject;
			}

			private Point _point;
			Action<Point> _setLambda;

			public Point Point
			{
				get { return _point; }
				set { _setLambda(value); }
			}

			public Point ReferencePoint { get; private set; }

			public object Object { get; private set; }

			public object ParentObject { get; private set; }

			public int PolyLineIndex { get; set; }

			public PathPoint TargetPathPoint { get; set; }
		}

		protected class PathThumb : PointThumb
		{
			public PathThumb(Point point, int index, PathPoint pathpoint) : base(point)
			{
				this.Index = index;
				this.PathPoint = pathpoint;
			}

			public int Index { get; set; }

			public PathPoint PathPoint { get; set; }
		}

		private readonly Dictionary<int, Bounds> _selectedThumbs = new Dictionary<int, Bounds>();
		private bool _isDragging;
		ZoomControl _zoom;

		private MenuItem[] segmentContextMenu = null;
		

		public PathHandlerExtension()
		{
			var mnu1 = new MenuItem() { Header = "to Line Segment" };
			mnu1.Click += (s, e) => ConvertPart(((DependencyObject)s).TryFindParent<PathThumb>(), PathPartConvertType.ToLineSegment);
			var mnu2 = new MenuItem() {Header = "to Bezier Segment"};
			mnu2.Click += (s, e) => ConvertPart(((DependencyObject)s).TryFindParent<PathThumb>(), PathPartConvertType.ToBezierSegment);
			var mnu3 = new MenuItem() {Header = "to Quadric Bezier Segment"};
			mnu3.Click += (s, e) => ConvertPart(((DependencyObject)s).TryFindParent<PathThumb>(), PathPartConvertType.ToQuadricBezierSegment);

			segmentContextMenu = new[] {mnu1, mnu2, mnu3};
		}

		#region thumb methods
		protected DesignerThumb CreateThumb(PlacementAlignment alignment, Cursor cursor, int index, PathPoint pathpoint)
		{
			var point = pathpoint.Point;
			var transform = ((Shape)this.ExtendedItem.View).RenderedGeometry.Transform;
			point = transform.Transform(point);

			var designerThumb = new PathThumb(point, index, pathpoint) {Cursor = cursor};

			if (pathpoint.Object is LineSegment || pathpoint.Object is PolyLineSegment || pathpoint.Object is BezierSegment || pathpoint.Object is QuadraticBezierSegment) {
				designerThumb.OperationMenu = segmentContextMenu;
			}

			if (pathpoint.TargetPathPoint != null)
				designerThumb.IsEllipse = true;

			AdornerPanel.SetPlacement(designerThumb, designerThumb.AdornerPlacement);
			adornerPanel.Children.Add(designerThumb);

			DragListener drag = new DragListener(designerThumb);

			WeakEventManager<DesignerThumb, MouseButtonEventArgs>.AddHandler(designerThumb, "PreviewMouseLeftButtonDown", ResizeThumbOnMouseLeftButtonUp);

			drag.MouseDown += drag_MouseDown;
			drag.Started += drag_Started;
			drag.Changed += drag_Changed;
			drag.Completed += drag_Completed;
			return designerThumb;
		}

		private static void ConvertPart(PathThumb senderThumb, PathPartConvertType convertType)
		{
			if (senderThumb.PathPoint.ParentObject is PathFigure)
			{
				var pathFigure = senderThumb.PathPoint.ParentObject as PathFigure;
				var pathSegment = senderThumb.PathPoint.Object as PathSegment;

				var idx = pathFigure.Segments.IndexOf(pathSegment);

				var point = senderThumb.PathPoint.Point;

				if (pathSegment is PolyLineSegment)
				{
					var poly = pathSegment as PolyLineSegment;
					var lst = poly.Points.Take(senderThumb.PathPoint.PolyLineIndex);
					var lst2 = poly.Points.Skip(senderThumb.PathPoint.PolyLineIndex + 1);
					pathFigure.Segments.RemoveAt(idx);
					var p1 = new PolyLineSegment();
					p1.Points.AddRange(lst);
					pathFigure.Segments.Insert(idx, p1);
					pathFigure.Segments.Insert(idx+1, new LineSegment());
					var p2 = new PolyLineSegment();
					p2.Points.AddRange(lst2);
					pathFigure.Segments.Insert(idx+2, p2);
					idx++;
				}

				pathFigure.Segments.RemoveAt(idx);

				PathSegment newSegment = null;
				switch (convertType)
				{
					case PathPartConvertType.ToBezierSegment:
						newSegment = new BezierSegment() { Point1 = point - new Vector(40, 40), Point2 = point + new Vector(-40, 40), Point3 = point };
						break;
					case PathPartConvertType.ToQuadricBezierSegment:
						newSegment = new QuadraticBezierSegment() { Point1 = point - new Vector(40, 40), Point2 = point  };
						break;
					default:
						newSegment = new LineSegment() { Point = point };
						break;
				}

				pathFigure.Segments.Insert(idx, newSegment);
			}
		}

		private void ResetThumbs()
		{
			foreach (FrameworkElement rt in adornerPanel.Children)
			{
				if (rt is DesignerThumb)
					(rt as DesignerThumb).IsPrimarySelection = true;
			}
			_selectedThumbs.Clear();
		}

		private void SelectThumb(PathThumb mprt)
		{
			var points = GetPoints();
			Point p = points[mprt.Index].Point;
			_selectedThumbs.Add(mprt.Index, new Bounds { X = p.X, Y = p.Y });

			mprt.IsPrimarySelection = false;
		}

		#endregion

		#region eventhandlers

		private void ResizeThumbOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			//get current thumb
			var mprt = sender as PathThumb;
			if (mprt != null)
			{
				//shift+ctrl will remove selected point
				if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) &&
				    (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
				{
					//unselect all points
					ResetThumbs();
					var points = GetPoints();

					//iterate thumbs to lower index of remaining thumbs
					foreach (PathThumb m in adornerPanel.Children)
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

		protected void drag_MouseDown(DragListener drag)
		{
			
		}
		
		// TODO : Remove all hide/show extensions from here.
		protected void drag_Started(DragListener drag)
		{
			//get current thumb
			var mprt = (drag.Target as PathThumb);
			if (mprt != null)
			{
				SetOperation();
			}
		}

		void SetOperation()
		{
			var designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
			_zoom = designPanel.TryFindParent<ZoomControl>();
			
			if (resizeBehavior != null)
				operation = PlacementOperation.Start(extendedItemArray, PlacementType.Resize);
			else
			{
				changeGroup = ExtendedItem.Context.OpenGroup("Resize", extendedItemArray);
			}
			_isResizing = true;
		}

		void ChangeOperation(List<PathPoint> points)
		{
			//this is for SharpDevelop built in undo functionality
			if (operation != null)
			{
				var info = operation.PlacedItems[0];
				var result = info.OriginalBounds;

				IEnumerable<double> xs = points.Select(x => x.Point.X);
				IEnumerable<double> ys = points.Select(y => y.Point.Y);
				result.X = (double)(info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).ValueOnInstance);
				result.Y = (double)(info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).ValueOnInstance);
				result.Width = xs.Max() - xs.Min();
				result.Height = ys.Max() - ys.Min();

				info.Bounds = result.Round();
				operation.CurrentContainerBehavior.BeforeSetPosition(operation);
				operation.CurrentContainerBehavior.SetPosition(info);
			}
		}

		void CommitOperation()
		{
			if (operation != null)
			{
//				foreach (int i in _selectedThumbs.Keys)
//				{
//					_selectedThumbs[i].X = points[i].X;
//					_selectedThumbs[i].Y = points[i].Y;
//				}
				
				//ExtendedItem.Properties.GetProperty(pl != null ? Polyline.PointsProperty : Polygon.PointsProperty).SetValue(points);
				
				operation.Commit();

				operation = null;
			}
			else
			{
				if (changeGroup != null)
					changeGroup.Commit();
				changeGroup = null;
			}
			_isResizing = false;

			Invalidate();
		}

		protected void drag_Changed(DragListener drag)
		{
			var points = GetPoints();

			var mprt = drag.Target as PathThumb;
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
				if (_selectedThumbs.Count == 1 && mprt.Index > 0) {
					theta = (180 / Math.PI) * Math.Atan2(_selectedThumbs[mprt.Index].Y + dy - points[mprt.Index - 1].Point.Y, _selectedThumbs[mprt.Index].X + dx - points[mprt.Index - 1].Point.X);
				} else { //if multiple points snapping angle is calculated in relation to mouse dragging angle
					theta = (180 / Math.PI) * Math.Atan2(dy, dx);
				}

				//snappingAngle is used for snapping function to horizontal or vertical plane in line drawing, and is activated by pressing ctrl or shift button
				int? snapAngle = null;

				//shift+alt gives a new point
//				if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
//				{
//					//if dragging occurs on a point and that point is the only selected, a new node will be added.
//					//_isCtrlDragging is needed since this method is called for every x pixel that the mouse moves
//					//so it could be many thousands of times during a single dragging
//					if (!_isDragging && _selectedThumbs.Count == 1 && (Math.Abs(dx) > 0 || Math.Abs(dy) > 0))
//					{
//
//						//duplicate point that is selected
//						Point p = points[mprt.Index].Point;
//
//						//insert duplicate
//						points.Insert(mprt.Index, p);
//
//						//create adorner marker
//						CreateThumb(PlacementAlignment.BottomRight, Cursors.Cross, mprt.Index);
//
//						//set index of all points that had a higher index than selected to +1
//						foreach (FrameworkElement rt in adornerPanel.Children)
//						{
//							if (rt is MultiPathThumb)
//							{
//								MultiPathThumb t = rt as MultiPathThumb;
//								if (t.Index > mprt.Index)
//									t.Index++;
//							}
//						}
//
//						//set index of new point to old point index + 1
//						mprt.Index = mprt.Index + 1;
//						ResetThumbs();
//						SelectThumb(mprt);
//
//					}
//					snapAngle = 10;
//				}

				//snapping occurs when mouse is within 10 degrees from horizontal or vertical plane if shift is pressed
				/*else*/ if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
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
			(drag.Target as DesignerThumb).InvalidateArrange();
		}

		protected void drag_Completed(DragListener drag)
		{
			var mprt = drag.Target as PathThumb;
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

			var points = GetPoints();

			resizeThumbs = new List<DesignerThumb>();
			for (int i = 0; i < points.Count; i++)
			{
				CreateThumb(PlacementAlignment.BottomRight, Cursors.Cross, i, points[i]);
			}

			Invalidate();

			ResetThumbs();
			_isDragging = false;

			extendedItemArray[0] = ExtendedItem;
			ExtendedItem.PropertyChanged += OnPropertyChanged;
			resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);
			UpdateAdornerVisibility();
		}

		#endregion

		List<PathPoint> GetPoints()
		{
			return GetPoints(this.ExtendedItem.View as Path);
		}
		
		static List<PathPoint> GetPoints(Path path)
		{
			var retVal = new List<PathPoint>();
			AddGeometryPoints(retVal, path.Data);
			
			return retVal;
		}

		private static void AddGeometryPoints(List<PathPoint> list, Geometry geometry)
		{
			if (geometry is CombinedGeometry) {
				var g = geometry as CombinedGeometry;
				AddGeometryPoints(list, g.Geometry1);
				AddGeometryPoints(list, g.Geometry2);
			} else if (geometry is PathGeometry) {
				var g = geometry as PathGeometry;
				if (geometry!=null) {
					foreach(var figure in g.Figures) {
						list.Add(new PathPoint(figure.StartPoint, figure, null, (p) => figure.StartPoint = p));
						foreach (var s in figure.Segments) {
							if (s is LineSegment)
								list.Add(new PathPoint(((LineSegment)s).Point, s, figure, (p) => ((LineSegment)s).Point = p));
							else if (s is PolyLineSegment) {
								var poly = s as PolyLineSegment;
								for(int n=0; n<poly.Points.Count; n++)
								{
									var closure_n = n;
									list.Add(new PathPoint(poly.Points[closure_n], s, figure, (p) => poly.Points[closure_n] = p) { PolyLineIndex = closure_n });
								}
							}
							else if (s is BezierSegment) {
								var pathp = new PathPoint(((BezierSegment)s).Point3, s, figure, (p) => ((BezierSegment)s).Point3 = p);
								list.Add(new PathPoint(((BezierSegment)s).Point1, s, figure, (p) => ((BezierSegment)s).Point1 = p) { TargetPathPoint = pathp});
								list.Add(new PathPoint(((BezierSegment)s).Point2, s, figure, (p) => ((BezierSegment)s).Point2 = p) { TargetPathPoint = pathp });
								list.Add(pathp);
							}
							else if (s is QuadraticBezierSegment) {
								var pathp = new PathPoint(((QuadraticBezierSegment)s).Point2, s, figure, (p) => ((QuadraticBezierSegment)s).Point2 = p);
								list.Add(new PathPoint(((QuadraticBezierSegment)s).Point1, s, figure, (p) => ((QuadraticBezierSegment)s).Point1 = p) { TargetPathPoint = pathp });
								list.Add(pathp);
							}
							else if (s is ArcSegment)
								list.Add(new PathPoint(((ArcSegment)s).Point, s, figure, (p) => ((ArcSegment)s).Point = p));
						}
					}
				}
			} else if (geometry is RectangleGeometry) {
				var g = geometry as RectangleGeometry;
				list.Add(new PathPoint(g.Rect.TopLeft, geometry, null, null)); //(p) => g.Rect.Left = p.X));
				list.Add(new PathPoint(g.Rect.TopRight, geometry, null, null)); //(p) => g.Rect.Width = p.X));
				list.Add(new PathPoint(g.Rect.BottomLeft, geometry, null, null)); //(p) => g.Rect.Top = p.Y));
				list.Add(new PathPoint(g.Rect.BottomRight, geometry, null, null)); //(p) => g.Rect.Height = p.Y));
			} else if (geometry is EllipseGeometry) {
				var g = geometry as EllipseGeometry;
				list.Add(new PathPoint(g.Center, geometry, null, (p) => g.Center = p));
			} else if (geometry is LineGeometry) {
				var g = geometry as LineGeometry;
				list.Add(new PathPoint(g.StartPoint, geometry, null, (p) => g.StartPoint = p));
				list.Add(new PathPoint(g.EndPoint, geometry, null, (p) => g.EndPoint = p));
			}
		}
		
		List<PathPoint> MovePoints(List<PathPoint> pc, double displacementX, double displacementY, double theta, int? snapangle)
		{
			//iterate all selected points
			foreach (int i in _selectedThumbs.Keys)
			{
				Point p = pc[i].Point;

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
							y = _selectedThumbs.Count == 1 ? pc[i - 1].Point.Y : y - displacementY;
						}
						else if (Math.Abs(90 - Math.Abs(theta)) < snapangle)//vertical snap
						{
							//if one point selected use point before as snap point, else snap to movement
							x = _selectedThumbs.Count == 1 ? pc[i - 1].Point.X : x - displacementX;
						}
					}
				}

				p.X = x;
				p.Y = y;
				pc[i].Point = p;
			}
			return pc;
		}

		#region IKeyDown

		public bool InvokeDefaultAction
		{
			get { return _selectedThumbs.Count == 0 || _selectedThumbs.Count == GetPoints().Count - 1; }
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

			ChangeOperation(MovePoints(GetPoints(), dx1 + dx2, dy1 + dy2, 0, null));
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
