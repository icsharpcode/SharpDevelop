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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.WpfDesign.Designer.UIExtensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Description of PathHandlerExtension.
	/// </summary>
	[ExtensionFor(typeof(Path))]
	internal class PathHandlerExtension : LineExtensionBase, IKeyDown, IKeyUp
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

			drag.MouseDown += drag_MouseDown;
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
			MultiPointResizeThumb mprt = sender as MultiPointResizeThumb;
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

		protected void drag_MouseDown(DragListener drag)
		{
			
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
//							if (rt is MultiPointResizeThumb)
//							{
//								MultiPointResizeThumb t = rt as MultiPointResizeThumb;
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

			var points = GetPoints();

			resizeThumbs = new List<ResizeThumb>();
			for (int i = 0; i < points.Count; i++)
			{
				CreateThumb(PlacementAlignment.BottomRight, Cursors.Cross, i);
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
		
		public static List<PathPoint> GetPoints(Path path)
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
						list.Add(new PathPoint(figure.StartPoint, figure, PointType.StartPoint, (p) => figure.StartPoint = p));
						foreach (var s in figure.Segments) {
							if (s is LineSegment)
								list.Add(new PathPoint(((LineSegment)s).Point, s, PointType.LineSegment, (p) => ((LineSegment)s).Point = p));
							else if (s is PolyLineSegment) {
								//list.AddRange(((PolyLineSegment)s).Points);
							}
							else if (s is BezierSegment) {
								list.Add(new PathPoint(((BezierSegment)s).Point1, s, PointType.BezierSegment, (p) => ((BezierSegment)s).Point1 = p));
								//list.Add(((BezierSegment)s).Point1);
								//list.Add(((BezierSegment)s).Point2);
								//list.Add(((BezierSegment)s).Point3);
							}
							else if (s is QuadraticBezierSegment) {
								list.Add(new PathPoint(((QuadraticBezierSegment)s).Point1, s, PointType.QuadricBezierSegment, (p) => ((QuadraticBezierSegment)s).Point1 = p));
								//list.Add(((QuadraticBezierSegment)s).Point1);
								//list.Add(((QuadraticBezierSegment)s).Point2);
							}
							else if (s is ArcSegment)
								list.Add(new PathPoint(((ArcSegment)s).Point, s, PointType.ArcSegment, (p) => ((ArcSegment)s).Point = p));
								//list.Add(((ArcSegment)s).Point);
						}
					}
				}
			} else if (geometry is RectangleGeometry) {
				var g = geometry as RectangleGeometry;
				list.Add(new PathPoint(g.Rect.TopLeft, geometry, PointType.RectangleGeometryP1, null)); //(p) => g.Rect.Left = p.X));
				list.Add(new PathPoint(g.Rect.TopRight, geometry, PointType.RectangleGeometryP2, null)); //(p) => g.Rect.Width = p.X));
				list.Add(new PathPoint(g.Rect.BottomLeft, geometry, PointType.RectangleGeometryP3, null)); //(p) => g.Rect.Top = p.Y));
				list.Add(new PathPoint(g.Rect.BottomRight, geometry, PointType.RectangleGeometryP4, null)); //(p) => g.Rect.Height = p.Y));
//				list.Add(new Point(g.Rect.Left, g.Rect.Top));
//				list.Add(new Point(g.Rect.Left, g.Rect.Top + g.Rect.Height));
//				list.Add(new Point(g.Rect.Left + g.Rect.Width, g.Rect.Top));
//				list.Add(new Point(g.Rect.Left + g.Rect.Width, g.Rect.Top + g.Rect.Height));
			} else if (geometry is EllipseGeometry) {
				var g = geometry as EllipseGeometry;
				list.Add(new PathPoint(g.Center, geometry, PointType.EllipseGeometryCenter, (p) => g.Center = p));
				//list.Add(g.Center);
			} else if (geometry is LineGeometry) {
				var g = geometry as LineGeometry;
				list.Add(new PathPoint(g.StartPoint, geometry, PointType.LineGeometryStart, (p) => g.StartPoint = p));
				list.Add(new PathPoint(g.EndPoint, geometry, PointType.LineGeometryEnd, (p) => g.EndPoint = p));
				//list.Add(g.StartPoint);
				//list.Add(g.EndPoint);
			}
		}
		
		public enum PointType{
			StartPoint,
			LineSegment,
			BezierSegment,
			ArcSegment,
			QuadricBezierSegment,
			
			LineGeometryStart,
			LineGeometryEnd,
			EllipseGeometryCenter,
			RectangleGeometryP1,
			RectangleGeometryP2,
			RectangleGeometryP3,
			RectangleGeometryP4,
		}
		
		public class PathPoint {
			public PathPoint(Point point, Object @object, PointType pointType, Action<Point> setLambda)
			{
				this._point = point;
				this._setLambda = setLambda;
				this.Object = @object;
			}
			
			private Point _point;
			Action<Point> _setLambda;
			
			public Point Point {
				get{return _point;} 
				set{_setLambda(value);}
			}
			public Point ReferencePoint {get; private set;}
			public PointType Start {get; private set;}
			public PointType End {get; private set;}
			public object Object {get; private set;}
		}
		
		//Should not return a List of Points, no a List of Point Object wich say what a Point is.
		//For Example: a Center Point of a Circle, should now it's a Center point!
		//When he is selected, he should show another drag point to change the radius!
		//a Combined Gemoetry should show a Adorner to change the combination mode!
		
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
