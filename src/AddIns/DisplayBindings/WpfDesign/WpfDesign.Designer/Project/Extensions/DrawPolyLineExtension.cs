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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionFor(typeof(Canvas))]
	[ExtensionFor(typeof(Grid))]
	public class DrawPolyLineExtension : BehaviorExtension, IDrawItemExtension
	{
		private ChangeGroup changeGroup;

		DesignItem CreateItem(DesignContext context, Type componentType)
		{
			object newInstance = context.Services.ExtensionManager.CreateInstanceWithCustomInstanceFactory(componentType, null);
			DesignItem item = context.Services.Component.RegisterComponentForDesigner(newInstance);
			changeGroup = item.OpenGroup("Draw Polyline");
			context.Services.ExtensionManager.ApplyDefaultInitializers(item);
			return item;
		}

		#region IDrawItemBehavior implementation

		public bool CanItemBeDrawn(Type createItemType)
		{
			return createItemType == typeof(Polyline) || createItemType == typeof(Polygon);
		}

		public void StartDrawItem(DesignItem clickedOn, Type createItemType, IDesignPanel panel, System.Windows.Input.MouseEventArgs e)
		{
			var createdItem = CreateItem(panel.Context, createItemType);

			var startPoint = e.GetPosition(clickedOn.View);
			var operation = PlacementOperation.TryStartInsertNewComponents(clickedOn,
			                                                               new DesignItem[] { createdItem },
			                                                               new Rect[] { new Rect(startPoint.X, startPoint.Y, double.NaN, double.NaN) },
			                                                               PlacementType.AddItem);
			if (operation != null) {
				createdItem.Services.Selection.SetSelectedComponents(new DesignItem[] { createdItem });
				operation.Commit();
			}
			
			createdItem.Properties[Shape.StrokeProperty].SetValue(Brushes.Black);
			createdItem.Properties[Shape.StrokeThicknessProperty].SetValue(2d);
			createdItem.Properties[Shape.StretchProperty].SetValue(Stretch.None);
			
			if (createItemType == typeof(Polyline))
				createdItem.Properties[Polyline.PointsProperty].CollectionElements.Add(createdItem.Services.Component.RegisterComponentForDesigner(new Point(0,0)));
			else
				createdItem.Properties[Polygon.PointsProperty].CollectionElements.Add(createdItem.Services.Component.RegisterComponentForDesigner(new Point(0,0)));
			
			new DrawPolylineMouseGesture(createdItem, clickedOn.View, changeGroup).Start(panel, (MouseButtonEventArgs) e);
		}

		#endregion
		
		sealed class DrawPolylineMouseGesture : ClickOrDragMouseGesture
		{
			private ChangeGroup changeGroup;
			private DesignItem newLine;
			private new Point startPoint;

			public DrawPolylineMouseGesture(DesignItem newLine, IInputElement relativeTo, ChangeGroup changeGroup)
			{
				this.newLine = newLine;
				this.positionRelativeTo = relativeTo;
				this.changeGroup = changeGroup;
				
				startPoint = Mouse.GetPosition(null);
			}
			
			protected override void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
			{
				e.Handled = true;
				base.OnPreviewMouseLeftButtonDown(sender, e);
			}
			
			protected override void OnMouseMove(object sender, MouseEventArgs e)
			{
				var delta = e.GetPosition(null) - startPoint;
				var point = new Point(delta.X, delta.Y);

				if (newLine.View is Polyline) {
					if (((Polyline)newLine.View).Points.Count <= 1)
						((Polyline)newLine.View).Points.Add(point);
					if (Mouse.LeftButton != MouseButtonState.Pressed)
						((Polyline)newLine.View).Points.RemoveAt(((Polyline)newLine.View).Points.Count - 1);
					if (((Polyline)newLine.View).Points.Last() != point)
						((Polyline)newLine.View).Points.Add(point);
				} else {
					if (((Polygon)newLine.View).Points.Count <= 1)
						((Polygon)newLine.View).Points.Add(point);
					if (Mouse.LeftButton != MouseButtonState.Pressed)
						((Polygon)newLine.View).Points.RemoveAt(((Polygon)newLine.View).Points.Count - 1);
					if (((Polygon)newLine.View).Points.Last() != point)
						((Polygon)newLine.View).Points.Add(point);
				}
			}
			
			protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
			{
				var delta = e.GetPosition(null) - startPoint;
				var point = new Point(delta.X, delta.Y);
				
				if (newLine.View is Polyline)
					((Polyline)newLine.View).Points.Add(point);
				else
					((Polygon)newLine.View).Points.Add(point);
			}
			
			protected override void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
			{
				base.OnMouseDoubleClick(sender, e);
				
				if (newLine.View is Polyline) {
					((Polyline)newLine.View).Points.RemoveAt(((Polyline)newLine.View).Points.Count - 1);
					newLine.Properties[Polyline.PointsProperty].SetValue(string.Join(",", ((Polyline)newLine.View).Points));
				} else {
					((Polygon)newLine.View).Points.RemoveAt(((Polygon)newLine.View).Points.Count - 1);
					newLine.Properties[Polygon.PointsProperty].SetValue(string.Join(",", ((Polygon)newLine.View).Points));
				}
				
				if (changeGroup != null)
				{
					changeGroup.Commit();
					changeGroup = null;
				}
				
				Stop();
			}

			protected override void OnStopped()
			{
				if (changeGroup != null) {
					changeGroup.Abort();
					changeGroup = null;
				}
				if (services.Tool.CurrentTool is CreateComponentTool) {
					services.Tool.CurrentTool = services.Tool.PointerTool;
				}

				newLine.ReapplyAllExtensions();

				base.OnStopped();
			}
			
		}
	}
}
