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

#region Usings

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class InheritanceRelationToXYConverter : RelationToXYConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var inheritanceRelation = (InheritanceRelation)value;
            var fromY = -((MatrixTransform)inheritanceRelation.Canvas.TransformToVisual(inheritanceRelation.FromTypeDesigner)).Matrix.OffsetY;
            var toY = -((MatrixTransform)inheritanceRelation.Canvas.TransformToVisual(inheritanceRelation.ToTypeDesigner)).Matrix.OffsetY;
            var yDiff = toY - fromY;
            double y1, y2, y3, y4;
            double yCanvas;
            if (yDiff > 0)
            {
                y1 = 0;
                y2 = yDiff;
                yCanvas = fromY;
                if (yDiff > MIN_SIZE)
                {
                    var moreY = inheritanceRelation.FromTypeDesigner.ActualHeight;
                    yCanvas += moreY;
                    y2 -= moreY;
                }
            }
            else
            {
                y1 = -yDiff;
                y2 = 0;
                yCanvas = toY;
                if (yDiff < -MIN_SIZE)
                {
                    double moreY = inheritanceRelation.ToTypeDesigner.ActualHeight;
                    yCanvas += moreY;
                    y1 -= moreY;
                }
            }

            var x1 = inheritanceRelation.FromTypeDesigner.ActualWidth / 2.0;
            var x2 = inheritanceRelation.ToTypeDesigner.ActualWidth / 2.0;
            var fromX = -((MatrixTransform)inheritanceRelation.Canvas.TransformToVisual(inheritanceRelation.FromTypeDesigner)).Matrix.OffsetX + x1;
            var toX = -((MatrixTransform)inheritanceRelation.Canvas.TransformToVisual(inheritanceRelation.ToTypeDesigner)).Matrix.OffsetX + x2;
            var xDiff = toX - fromX;
            double x3;
            double xCanvas;
            xCanvas = Math.Min(fromX, toX);
            if (xDiff > 0)
            {
                x2 = xDiff;
                x1 = 0;
            }
            else
            {
                x1 = -xDiff;
                x2 = 0;
            }

            double x1Arrow, x2Arrow, x3Arrow, y1Arrow, y2Arrow, y3Arrow;

            double fromBottom = fromY + inheritanceRelation.FromTypeDesigner.ActualHeight;
            double toBottom = toY + inheritanceRelation.ToTypeDesigner.ActualHeight;
            var yDiff1 = toY - fromBottom;
            var yDiff2 = toBottom - fromY;
            var fromDesignerLeft = -((MatrixTransform)inheritanceRelation.Canvas.TransformToVisual(inheritanceRelation.FromTypeDesigner)).Matrix.OffsetX;
            var fromDesignerRight = fromDesignerLeft + inheritanceRelation.FromTypeDesigner.ActualWidth;
            var toDesignerLeft = -((MatrixTransform)inheritanceRelation.Canvas.TransformToVisual(inheritanceRelation.ToTypeDesigner)).Matrix.OffsetX;
            var toDesignerRight = toDesignerLeft + inheritanceRelation.ToTypeDesigner.ActualWidth;
            var angle = GetAngle(x1, x2, y1, y2);
            var angleAbs = Math.Abs(angle);
            bool twoPoints = (angleAbs >= ARROW_LINE_MIN_ANGLE && angleAbs <= Math.PI / 2.0 + ARROW_LINE_MAX_ANGLE) && (Math.Min(Math.Abs(yDiff1), Math.Abs(yDiff2)) > MIN_SIZE && yDiff1 < 0 == yDiff2 < 0 || !(fromDesignerRight <= toDesignerLeft || toDesignerRight <= fromDesignerLeft)) && inheritanceRelation.FromTypeDesigner != inheritanceRelation.ToTypeDesigner;
            x3Arrow = x2;
            y3Arrow = y2;
            if (twoPoints)
            {
                y3 = 0;
                y4 = 0;
                x3 = 0;
                FillArrow(x1, x2, y1, y2, out x1Arrow, out x2Arrow, out y1Arrow, out y2Arrow);
                var translateXCanvas = Math.Min(x1Arrow, x2Arrow);
                if (translateXCanvas < 0)
                {
                    x1 -= translateXCanvas;
                    x1Arrow -= translateXCanvas;
                    x2Arrow -= translateXCanvas;
                    x3Arrow -= translateXCanvas;
                    xCanvas += translateXCanvas;
                }
                var translateYCanvas = Math.Min(y1Arrow, y2Arrow);
                if (translateYCanvas < 0)
                {
                    y1 -= translateYCanvas;
                    y1Arrow -= translateYCanvas;
                    y2Arrow -= translateYCanvas;
                    y3Arrow -= translateYCanvas;
                    yCanvas += translateYCanvas;
                }
                x2 = (x1Arrow + x2Arrow) / 2.0;
                y2 = (y1Arrow + y2Arrow) / 2.0;
            }
            else
            {
                x3 = x2;
                x1Arrow = x2 - ARROW_WIDTH;
                x2Arrow = x2 + ARROW_WIDTH;
                x3Arrow = x2;
                x2 = x1;
                y4 = 0;
                var top = Math.Abs(toY - fromY) > Math.Abs(toBottom - fromBottom);
                if (top)
                {
                    y1 = y4 = MIN_SIZE;
                    if (fromY > toY)
                        y1 += fromY - toY;
                    else
                        y4 += toY - fromY;
                    yCanvas = Math.Min(fromY - MIN_SIZE, toY - MIN_SIZE);
                    y2 = 0;
                    y3Arrow = y4;
                    y4 -= ARROW_HEIGHT;
                }
                if (! top || yCanvas < 0)
                {
                    y1 = fromBottom;
                    y4 = toBottom;
                    yCanvas = Math.Min(fromBottom, toBottom);
                    y1 -= yCanvas;
                    y4 -= yCanvas;
                    y2 = Math.Max(y1, y4) + MIN_SIZE;
                    y3Arrow = y4;
                    y4 += ARROW_HEIGHT;
                }
                y1Arrow = y2Arrow = y4;
                y3 = y2;
            }

            return new double[][] { new[] { xCanvas, yCanvas }, new[] { x1, y1 }, new[] { x2, y2 }, new[] { x3, y3 }, new[] { x3, y4 }, new[] { x1Arrow, y1Arrow }, new[] { x2Arrow, y2Arrow }, new[] { x3Arrow, y3Arrow } };
        }
    }
}
