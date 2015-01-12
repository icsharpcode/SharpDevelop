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
	public class DrawPathExtension : BehaviorExtension, IDrawItemExtension
	{
		private ChangeGroup changeGroup;

		DesignItem CreateItem(DesignContext context, Type componentType)
		{
			object newInstance = context.Services.ExtensionManager.CreateInstanceWithCustomInstanceFactory(componentType, null);
			DesignItem item = context.Services.Component.RegisterComponentForDesigner(newInstance);
			changeGroup = item.OpenGroup("Draw Path");
			context.Services.ExtensionManager.ApplyDefaultInitializers(item);
			return item;
		}

		#region IDrawItemBehavior implementation

		public bool CanItemBeDrawn(Type createItemType)
		{
			return createItemType == typeof(Path);
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
			
			var figure = new PathFigure();
			var geometry = new PathGeometry();
			var geometryDesignItem = createdItem.Services.Component.RegisterComponentForDesigner(geometry);
			var figureDesignItem = createdItem.Services.Component.RegisterComponentForDesigner(figure);
			createdItem.Properties[Path.DataProperty].SetValue(geometry);
			//geometryDesignItem.Properties[PathGeometry.FiguresProperty].CollectionElements.Add(figureDesignItem);
			figureDesignItem.Properties[PathFigure.StartPointProperty].SetValue(new Point(0,0));
			
			new DrawPathMouseGesture(figure, createdItem, clickedOn.View, changeGroup, this.ExtendedItem.GetCompleteAppliedTransformationToView()).Start(panel, (MouseButtonEventArgs) e);
		}

		#endregion
		
		sealed class DrawPathMouseGesture : ClickOrDragMouseGesture
		{
			private ChangeGroup changeGroup;
			private DesignItem newLine;
			private Point sP;
			private PathFigure figure;
			private DesignItem geometry;
			private Matrix matrix;

			public DrawPathMouseGesture(PathFigure figure, DesignItem newLine, IInputElement relativeTo, ChangeGroup changeGroup, Transform transform)
			{
				this.newLine = newLine;
				this.positionRelativeTo = relativeTo;
				this.changeGroup = changeGroup;
				this.figure = figure;
				this.matrix = transform.Value;
				matrix.Invert();
				
				sP = Mouse.GetPosition(null);
				
				geometry = newLine.Properties[Path.DataProperty].Value;
			}
			
			protected override void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
			{
				e.Handled = true;
				base.OnPreviewMouseLeftButtonDown(sender, e);
			}
			
			protected override void OnMouseMove(object sender, MouseEventArgs e)
			{
				var delta = matrix.Transform(e.GetPosition(null) - sP);
				var point = new Point(Math.Round(delta.X, 0), Math.Round(delta.Y, 0));

				var segment = figure.Segments.LastOrDefault() as LineSegment;
				if (Mouse.LeftButton == MouseButtonState.Pressed)
				{
					if (segment == null || segment.Point != point)
					{
						figure.Segments.Add(new LineSegment(point, false));
						segment = figure.Segments.Last() as LineSegment;}
				}
					
				segment.Point = point;
				
				geometry.Properties[PathGeometry.FiguresProperty].SetValue(figure.ToString());
			}
			
			protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
			{
				var delta = matrix.Transform(e.GetPosition(null) - sP);
				var point = new Point(Math.Round(delta.X, 0), Math.Round(delta.Y,0));
				
				figure.Segments.Add(new LineSegment(point, false));
				geometry.Properties[PathGeometry.FiguresProperty].SetValue(figure.ToString());
			}
			
			protected override void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
			{
				base.OnMouseDoubleClick(sender, e);
				
				figure.Segments.RemoveAt(figure.Segments.Count - 1);
				geometry.Properties[PathGeometry.FiguresProperty].SetValue(figure.ToString());
				
				if (changeGroup != null) {
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
