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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.WpfDesign.UIExtensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Description of PolyLineHandlerExtension.
	/// </summary>
	[ExtensionFor(typeof(Polyline))]
	[ExtensionFor(typeof(Polygon))]
	public class PolyLineHandlerExtension : LineExtensionBase, IKeyDown, IKeyUp
	{
		private readonly Dictionary<int, Point> _selectedPoints = new Dictionary<int, Point>();
		private bool _isDragging;
		ZoomControl _zoom;

		#region thumb methods
		protected DesignerThumb CreateThumb(PlacementAlignment alignment, Cursor cursor, int index)
		{
			DesignerThumb designerThumb = new MultiPointThumb { Index = index, Alignment = alignment, Cursor = cursor, IsPrimarySelection = true };
			AdornerPlacement ap = Place(ref designerThumb, alignment, index);
			(designerThumb as MultiPointThumb).AdornerPlacement = ap;

			AdornerPanel.SetPlacement(designerThumb, ap);
			adornerPanel.Children.Add(designerThumb);

			DragListener drag = new DragListener(designerThumb);

			WeakEventManager<DesignerThumb, MouseButtonEventArgs>.AddHandler(designerThumb, "PreviewMouseLeftButtonDown", ResizeThumbOnMouseLeftButtonUp);

			drag.Started += drag_Started;
			drag.Changed += drag_Changed;
			drag.Completed += drag_Completed;
			return designerThumb;
		}

		private void ResetThumbs()
		{
			foreach (FrameworkElement rt in adornerPanel.Children)
			{
				if (rt is DesignerThumb)
					(rt as DesignerThumb).IsPrimarySelection = true;
			}
			_selectedPoints.Clear();
		}

		private void SelectThumb(MultiPointThumb mprt)
		{
			PointCollection points = GetPointCollection();
			Point p = points[mprt.Index];
			_selectedPoints.Add(mprt.Index, p);

			mprt.IsPrimarySelection = false;
		}

		#endregion

		#region eventhandlers

		private void ResizeThumbOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			//get current thumb
			MultiPointThumb mprt = sender as MultiPointThumb;
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
					foreach (MultiPointThumb m in adornerPanel.Children)
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
					if (!_selectedPoints.ContainsKey(mprt.Index) & !Keyboard.IsKeyDown(Key.LeftCtrl) &
					    !Keyboard.IsKeyDown(Key.RightCtrl))
					{
						ResetThumbs();
					}
					//add selected thumb, if ctrl pressed this could be all points in poly
					if (!_selectedPoints.ContainsKey(mprt.Index))
						SelectThumb(mprt);
					_isDragging = false;
				}
			}
		}

		// TODO : Remove all hide/show extensions from here.
		protected void drag_Started(DragListener drag)
		{
			//get current thumb
			MultiPointThumb mprt = (drag.Target as MultiPointThumb);
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

				foreach (int i in _selectedPoints.Keys.ToList())
				{
					_selectedPoints[i] = points[i];
				}
				ExtendedItem.Properties.GetProperty(pl != null ? Polyline.PointsProperty : Polygon.PointsProperty).SetValue(points);
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
			PointCollection points = GetPointCollection();

			MultiPointThumb mprt = drag.Target as MultiPointThumb;
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
				if (_selectedPoints.Count == 1 && mprt.Index > 0) {
					theta = (180 / Math.PI) * Math.Atan2(_selectedPoints[mprt.Index].Y + dy - points[mprt.Index - 1].Y, _selectedPoints[mprt.Index].X + dx - points[mprt.Index - 1].X);
				} else { //if multiple points snapping angle is calculated in relation to mouse dragging angle
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
					if (!_isDragging && _selectedPoints.Count == 1 && (Math.Abs(dx) > 0 || Math.Abs(dy) > 0))
					{

						//duplicate point that is selected
						Point p = points[mprt.Index];

						//insert duplicate
						points.Insert(mprt.Index, p);

						//create adorner marker
						CreateThumb(PlacementAlignment.BottomRight, Cursors.Cross, mprt.Index);

						//set index of all points that had a higher index than selected to +1
						foreach (FrameworkElement rt in adornerPanel.Children)
						{
							if (rt is MultiPointThumb)
							{
								MultiPointThumb t = rt as MultiPointThumb;
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
			
			(drag.Target as DesignerThumb).InvalidateArrange();
		}

		protected void drag_Completed(DragListener drag)
		{
			MultiPointThumb mprt = drag.Target as MultiPointThumb;
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

			resizeThumbs = new List<DesignerThumb>();
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

		PointCollection GetPointCollection()
		{
			Polygon pg = ExtendedItem.View as Polygon;
			Polyline pl = ExtendedItem.View as Polyline;

			return pl == null ? pg.Points : pl.Points;
		}

		PointCollection MovePoints(PointCollection pc, double displacementX, double displacementY, double theta, int? snapangle)
		{
			//iterate all selected points
			foreach (int i in _selectedPoints.Keys)
			{
				Point p = pc[i];

				//x and y is calculated from the currentl point
				double x = _selectedPoints[i].X + displacementX;
				double y = _selectedPoints[i].Y + displacementY;

				//if snap is applied
				if (snapangle != null)
				{
					if (_selectedPoints.Count > 0)
					{
						//horizontal snap
						if (Math.Abs(theta) < snapangle || 180 - Math.Abs(theta) < snapangle)
						{
							//if one point selected use point before as snap point, else snap to movement
							y = _selectedPoints.Count == 1 ? pc[i - 1].Y : y - displacementY;
						}
						else if (Math.Abs(90 - Math.Abs(theta)) < snapangle)//vertical snap
						{
							//if one point selected use point before as snap point, else snap to movement
							x = _selectedPoints.Count == 1 ? pc[i - 1].X : x - displacementX;
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
			get { return _selectedPoints.Count == 0 || _selectedPoints.Count == GetPointCollection().Count - 1; }
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
