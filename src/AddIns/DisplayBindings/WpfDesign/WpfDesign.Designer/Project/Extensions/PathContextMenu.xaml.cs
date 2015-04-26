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
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Designer.themes;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	public partial class PathContextMenu
	{
		private DesignItem designItem;

		public PathContextMenu(DesignItem designItem)
		{
			this.designItem = designItem;
			
			SpecialInitializeComponent();
		}
		
		/// <summary>
		/// Fixes InitializeComponent with multiple Versions of same Assembly loaded
		/// </summary>
		public void SpecialInitializeComponent()
		{
			if (!this._contentLoaded) {
				this._contentLoaded = true;
				Uri resourceLocator = new Uri(VersionedAssemblyResourceDictionary.GetXamlNameForType(this.GetType()), UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
			
			this.InitializeComponent();
		}

		void Click_ConvertToFigures(object sender, System.Windows.RoutedEventArgs e)
		{
			var path = this.designItem.Component as Path;
			
			if (path.Data is StreamGeometry) {
				var sg = path.Data as StreamGeometry;
				
				var pg = sg.GetFlattenedPathGeometry();
				
//				foreach (var g in parts) {
//					
//				}
				
				var pgDes = designItem.Services.Component.RegisterComponentForDesigner(pg);
				designItem.Properties[Path.DataProperty].SetValue(pgDes);
			}
			else if (path.Data is PathGeometry) {
				var pg = path.Data as PathGeometry;
				
				var figs = pg.Figures;
				
				var newPg = new PathGeometry();
				var newPgDes = designItem.Services.Component.RegisterComponentForDesigner(newPg);
				
				foreach (var fig in figs) {
					newPgDes.Properties[PathGeometry.FiguresProperty].CollectionElements.Add(FigureToDesignItem(fig));
				}
								
				designItem.Properties[Path.DataProperty].SetValue(newPg);
			}
			
		}
		
		private DesignItem FigureToDesignItem(PathFigure pf)
		{
			var pfDes = designItem.Services.Component.RegisterComponentForDesigner(new PathFigure());
			
			pfDes.Properties[PathFigure.StartPointProperty].SetValue(pf.StartPoint);
			pfDes.Properties[PathFigure.IsClosedProperty].SetValue(pf.IsClosed);
			
			foreach (var s in pf.Segments) {
					pfDes.Properties[PathFigure.SegmentsProperty].CollectionElements.Add(SegmentToDesignItem(s));
				}
			return pfDes;
		}
		
		private DesignItem SegmentToDesignItem(PathSegment s)
		{
			var sDes = designItem.Services.Component.RegisterComponentForDesigner(s.Clone());
			
			if (!((PathSegment)s).IsStroked)
				sDes.Properties[PathSegment.IsStrokedProperty].SetValue(((PathSegment)s).IsStroked);
			if (((PathSegment)s).IsSmoothJoin)
				sDes.Properties[PathSegment.IsSmoothJoinProperty].SetValue(((PathSegment)s).IsSmoothJoin);
				
			if (s is LineSegment) {
				sDes.Properties[LineSegment.PointProperty].SetValue(((LineSegment)s).Point);
			} else if (s is QuadraticBezierSegment) {
				sDes.Properties[QuadraticBezierSegment.Point1Property].SetValue(((QuadraticBezierSegment)s).Point1);
				sDes.Properties[QuadraticBezierSegment.Point2Property].SetValue(((QuadraticBezierSegment)s).Point2);
			} else if (s is BezierSegment) {
				sDes.Properties[BezierSegment.Point1Property].SetValue(((BezierSegment)s).Point1);
				sDes.Properties[BezierSegment.Point2Property].SetValue(((BezierSegment)s).Point2);
				sDes.Properties[BezierSegment.Point3Property].SetValue(((BezierSegment)s).Point3);
			} else if (s is ArcSegment) {
				sDes.Properties[ArcSegment.PointProperty].SetValue(((ArcSegment)s).Point);
				sDes.Properties[ArcSegment.IsLargeArcProperty].SetValue(((ArcSegment)s).IsLargeArc);
				sDes.Properties[ArcSegment.RotationAngleProperty].SetValue(((ArcSegment)s).RotationAngle);
				sDes.Properties[ArcSegment.SizeProperty].SetValue(((ArcSegment)s).Size);
				sDes.Properties[ArcSegment.SweepDirectionProperty].SetValue(((ArcSegment)s).SweepDirection);
			} else if (s is PolyLineSegment) {
				sDes.Properties[PolyLineSegment.PointsProperty].SetValue(((PolyLineSegment)s).Points);
			} else if (s is PolyQuadraticBezierSegment) {
				sDes.Properties[PolyQuadraticBezierSegment.PointsProperty].SetValue(((PolyQuadraticBezierSegment)s).Points);
			} else if (s is PolyBezierSegment) {
				sDes.Properties[PolyBezierSegment.PointsProperty].SetValue(((PolyBezierSegment)s).Points);
			}
			return sDes;
		}
	}
}

