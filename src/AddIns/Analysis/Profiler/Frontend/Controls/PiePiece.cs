// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

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
		private void DrawGeometry(StreamGeometryContext context)
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
