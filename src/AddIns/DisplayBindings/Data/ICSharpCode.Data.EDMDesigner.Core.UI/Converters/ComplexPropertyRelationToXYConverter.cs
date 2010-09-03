// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations;
using System.Windows.Media;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class ComplexPropertyRelationToXYConverter : RelationToXYConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var complexPropertyRelation = (ComplexPropertyRelation)value;
            var fromX = -((MatrixTransform)complexPropertyRelation.Canvas.TransformToVisual(complexPropertyRelation.FromTypeDesigner)).Matrix.OffsetX;
            var toMiddleWidth = complexPropertyRelation.ToTypeDesigner.ActualWidth / 2.0;
            var toX = -((MatrixTransform)complexPropertyRelation.Canvas.TransformToVisual(complexPropertyRelation.ToTypeDesigner)).Matrix.OffsetX + toMiddleWidth;
            var xDiff = toX - fromX;
            double x1, x2, x3, x4, x1Arrow, x2Arrow, xCanvas;
            var fromByLeft = true;
            if (xDiff > 0)
            {
                x1 = 0;
                x2 = xDiff;
                xCanvas = fromX;
                if (xDiff > complexPropertyRelation.FromTypeDesigner.ActualWidth / 2.0)
                {
                    double moreX = complexPropertyRelation.FromTypeDesigner.ActualWidth;
                    xCanvas += moreX;
                    x2 -= moreX;
                    fromByLeft = false;
                }
            }
            else
            {
                x1 = -xDiff;
                x2 = 0;
                xCanvas = toX;
            }

            var fromItem = complexPropertyRelation.FromTypeDesigner.IsExpanded ? complexPropertyRelation.FromItem : complexPropertyRelation.FromTypeDesigner;
            var fromY = -((MatrixTransform)complexPropertyRelation.Canvas.TransformToVisual(fromItem)).Matrix.OffsetY;
            var toY = -((MatrixTransform)complexPropertyRelation.Canvas.TransformToVisual(complexPropertyRelation.ToTypeDesigner)).Matrix.OffsetY;
            var yDiff = toY - fromY;
            var itemMiddleHeight = fromItem.ActualHeight / 2.0;
            var y1 = itemMiddleHeight;
            double y2, y3, y4, y1Arrow, y2Arrow, yCanvas;
            bool toByTop = true;
            if (yDiff > 0)
            {
                y2 = yDiff - y1;
                yCanvas = fromY + y1;
                y1 = 0;
            }
            else
            {
                y1 -= yDiff;
                yCanvas = toY;
                y2 = 0;
                if (yDiff < -complexPropertyRelation.ToTypeDesigner.ActualHeight / 2.0 || toY < MIN_SIZE)
                {
                    double moreY = complexPropertyRelation.ToTypeDesigner.ActualHeight;
                    yCanvas += moreY;
                    y1 -= moreY;
                    toByTop = false;
                }
            }

            var angle = GetAngle(x1, x2, y1, y2);
            var angleAbs = Math.Abs(angle);
            yDiff = y2 - y1;
            xDiff = x2 - x1;
            bool twoPoints, threePoints, fivePoints, fivePointsV, fivePointsH;
            if (angleAbs >= ARROW_LINE_MIN_ANGLE && angleAbs <= ARROW_LINE_MAX_ANGLE && (toByTop ? angle > 0 : angle < 0))
            {
                twoPoints = true;
                threePoints = fivePoints = fivePointsH = fivePointsV = false;
            }
            else
            {
                threePoints = ((y2 > y1) ? toByTop && yDiff > MIN_SIZE : !toByTop && yDiff < -MIN_SIZE) && (xDiff > MIN_SIZE ? !fromByLeft && x2 - x1 > MIN_SIZE : fromByLeft && x2 - x1 < -MIN_SIZE);
                if (threePoints)
                {
                    fivePointsH = false;
                    fivePointsV = false;
                    fivePoints = false;
                }
                else
                {
                    fromY += fromItem.ActualHeight / 2.0;
                    fivePointsV = fromX - MIN_SIZE * 2.0 < toX && fromX + complexPropertyRelation.ToTypeDesigner.ActualWidth + MIN_SIZE * (double)2 > toX && (fromY > toY + complexPropertyRelation.ToTypeDesigner.ActualHeight + MIN_SIZE || fromY < toY + complexPropertyRelation.ToTypeDesigner.ActualHeight + MIN_SIZE);
                    fivePointsH = (toByTop ? toY - MIN_SIZE <= fromY : toY + complexPropertyRelation.ToTypeDesigner.ActualHeight + MIN_SIZE > fromY) && (fromX - MIN_SIZE * 2.0 > toX || fromX + complexPropertyRelation.FromTypeDesigner.ActualWidth + MIN_SIZE * (double)2 < toX);
                    fivePoints = fivePointsH || fivePointsV;
                }
                twoPoints = !(fivePoints || threePoints);
            }

            if (twoPoints)
            {
                x3 = y3 = x4 = y4 = 0;
                FillArrow(angle, x1, x2, y1, y2, out x1Arrow, out x2Arrow, out y1Arrow, out y2Arrow);
                var translateXCanvas = Math.Min(x1Arrow, x2Arrow);
                if (translateXCanvas < 0)
                {
                    x1 -= translateXCanvas;
                    x2 -= translateXCanvas;
                    x1Arrow -= translateXCanvas;
                    x2Arrow -= translateXCanvas;
                    xCanvas += translateXCanvas;
                }
                var translateYCanvas = Math.Min(y1Arrow, y2Arrow);
                if (translateYCanvas < 0)
                {
                    y1 -= translateYCanvas;
                    y2 -= translateYCanvas;
                    y1Arrow -= translateYCanvas;
                    y2Arrow -= translateYCanvas;
                    yCanvas += translateYCanvas;
                }
            }
            else
            {
                var xTranslate = x2 - ARROW_WIDTH;
                if (xTranslate < 0)
                {
                    xCanvas += xTranslate;
                    x1 -= xTranslate;
                    x2 -= xTranslate;
                }
                x1Arrow = x2 - ARROW_WIDTH;
                x2Arrow = x2 + ARROW_WIDTH;
                if (threePoints)
                {
                    x4 = y4 = 0;
                    y3 = y2;
                    y2 = y1;
                    x3 = x2;
                    if (y3 > y1)
                        y1Arrow = y3 - ARROW_HEIGHT;
                    else
                        y1Arrow = y3 + ARROW_HEIGHT;
                }
                else if (fivePoints)
                {
                    x4 = x2;
                    y4 = y2;
                    if (fivePointsH)
                    {
                        x2 = Math.Abs(fromByLeft ? x1 - x2 + toMiddleWidth : x2 - toMiddleWidth - x1 ) / 2.0;
                        if (fromByLeft)
                        {
                            x3 = x1 + MIN_SIZE + (double)complexPropertyRelation.FromItemIndex * MORE_SIZE_PER_ITEM;
                            if (x3 < x4 - MIN_SIZE)
                                x2 = Math.Max(x2, x3);
                        }
                        else
                        {
                            x3 = x1 - MIN_SIZE - (double)complexPropertyRelation.FromItemIndex * MORE_SIZE_PER_ITEM;
                            if (x3 > x4 + MIN_SIZE)
                                x2 = Math.Min(x2, x3);
                        }
                    }
                    else if (fivePointsV)
                    {
                        var fromTypeY = -((MatrixTransform)complexPropertyRelation.Canvas.TransformToVisual(complexPropertyRelation.FromTypeDesigner)).Matrix.OffsetY;
                        if (toByTop)
                            y2 = Math.Abs(y2 - fromTypeY + yCanvas) / 2.0;
                        else
                        {
                            fromTypeY += complexPropertyRelation.FromTypeDesigner.ActualHeight;
                            y2 = Math.Abs(y2 - fromTypeY + yCanvas) / 2.0;
                        }
                        if (fromByLeft)
                            x2 = x1 - MIN_SIZE - (double)complexPropertyRelation.FromItemIndex * MORE_SIZE_PER_ITEM;
                        else
                            x2 = x1 + MIN_SIZE + (double)complexPropertyRelation.FromItemIndex * MORE_SIZE_PER_ITEM;
                    }
                    else
                        throw new NotImplementedException();
                    x3 = x2;
                    y2 = y1;
                    if (toByTop)
                    {
                        y3 = y4 - MIN_SIZE;
                        y1Arrow = y4 - ARROW_HEIGHT;
                    }
                    else
                    {
                        y3 = y4 + MIN_SIZE;
                        y1Arrow = y4 + ARROW_HEIGHT;
                    }
                }
                else
                    throw new InvalidOperationException();
                y2Arrow = y1Arrow;
            }

            return new double[][] { new[] { xCanvas, yCanvas }, new[] { x1, y1 }, new[] { x2, y2 }, new[] { x3, y3 }, new[] { x4, y3 }, new[] { x4, y4 }, new[] { x1Arrow, y1Arrow }, new[] { x2Arrow, y2Arrow } };
        }
    }
}
