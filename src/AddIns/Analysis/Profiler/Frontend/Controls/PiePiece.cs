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
using System.Windows.Media;
using System.Windows.Shapes;

namespace ICSharpCode.Profiler.Controls
{
	/// <summary>
	/// A pie piece shape
	/// </summary>
	class PiePiece : Shape
	{
		#region dependency properties

		public static readonly DependencyProperty RadiusProperty =
			DependencyProperty.Register("RadiusProperty", typeof(double), typeof(PiePiece),
			                            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		/// <summary>
		/// The radius of this pie piece
		/// </summary>
		public double Radius
		{
			get { return (double)GetValue(RadiusProperty); }
			set { SetValue(RadiusProperty, value); }
		}

		public static readonly DependencyProperty InnerRadiusProperty =
			DependencyProperty.Register("InnerRadiusProperty", typeof(double), typeof(PiePiece),
			                            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		/// <summary>
		/// The inner radius of this pie piece
		/// </summary>
		public double InnerRadius
		{
			get { return (double)GetValue(InnerRadiusProperty); }
			set { SetValue(InnerRadiusProperty, value); }
		}

		public static readonly DependencyProperty WedgeAngleProperty =
			DependencyProperty.Register("WedgeAngleProperty", typeof(double), typeof(PiePiece),
			                            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		/// <summary>
		/// The wedge angle of this pie piece in degrees
		/// </summary>
		public double WedgeAngle
		{
			get { return (double)GetValue(WedgeAngleProperty); }
			set { SetValue(WedgeAngleProperty, value); }
		}

		public static readonly DependencyProperty RotationAngleProperty =
			DependencyProperty.Register("RotationAngleProperty", typeof(double), typeof(PiePiece),
			                            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

		/// <summary>
		/// The rotation, in degrees, from the Y axis vector of this pie piece.
		/// </summary>
		public double RotationAngle
		{
			get { return (double)GetValue(RotationAngleProperty); }
			set { SetValue(RotationAngleProperty, value); }
		}
		#endregion
		
		protected override Size MeasureOverride(Size constraint)
		{
			double size = 2 * Radius;
			return new Size(size, size);
		}
		
		protected override Geometry DefiningGeometry
		{
			get
			{
				// Create a StreamGeometry for describing the shape
				StreamGeometry geometry = new StreamGeometry();
				geometry.FillRule = FillRule.EvenOdd;

				using (StreamGeometryContext context = geometry.Open())
				{
					DrawGeometry(context);
				}

				// Freeze the geometry for performance benefits
				geometry.Freeze();

				return geometry;
			}
		}

		/// <summary>
		/// Draws the pie piece
		/// </summary>
		void DrawGeometry(StreamGeometryContext context)
		{
			double centreX = Radius;
			double centreY = Radius;
			
			Point startPoint = new Point(centreX, centreY);

			Point innerArcStartPoint = Utils.ComputeCartesianCoordinate(RotationAngle, InnerRadius);
			innerArcStartPoint.Offset(centreX, centreY);

			Point innerArcEndPoint = Utils.ComputeCartesianCoordinate(RotationAngle + WedgeAngle, InnerRadius);
			innerArcEndPoint.Offset(centreX, centreY);

			Point outerArcStartPoint = Utils.ComputeCartesianCoordinate(RotationAngle, Radius);
			outerArcStartPoint.Offset(centreX, centreY);

			Point outerArcEndPoint = Utils.ComputeCartesianCoordinate(RotationAngle + WedgeAngle, Radius);
			outerArcEndPoint.Offset(centreX, centreY);

			bool largeArc = WedgeAngle>180.0;

			Size outerArcSize = new Size(Radius, Radius);
			Size innerArcSize = new Size(InnerRadius, InnerRadius);

			context.BeginFigure(innerArcStartPoint, true, true);
			context.LineTo(outerArcStartPoint, true, true);
			context.ArcTo(outerArcEndPoint, outerArcSize, 0, largeArc, SweepDirection.Clockwise, true, true);
			context.LineTo(innerArcEndPoint, true, true);
			context.ArcTo(innerArcStartPoint, innerArcSize, 0, largeArc, SweepDirection.Counterclockwise, true, true);
		}
	}

	public static class Utils
	{

		/// <summary>
		/// Converts a coordinate from the polar coordinate system to the cartesian coordinate system.
		/// </summary>
		/// <param name="angle"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Point ComputeCartesianCoordinate(double angle, double radius)
		{
			// convert to radians
			double angleRad = (Math.PI / 180.0) * (angle - 90);

			double x = radius * Math.Cos(angleRad);
			double y = radius * Math.Sin(angleRad);

			return new Point(x, y);
		}
	}
}
