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
using System.Windows.Media;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Relations;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public class AssociationToXYConverter : RelationToXYConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var association = (Association)value;
            var fromX = -((MatrixTransform)association.Canvas.TransformToVisual(association.FromTypeDesigner)).Matrix.OffsetX;
            var toX = -((MatrixTransform)association.Canvas.TransformToVisual(association.ToTypeDesigner)).Matrix.OffsetX;            
            var xDiff = toX - fromX;
            double x1, x2, x3, x4;
            double xCanvas;
            if (xDiff > 0)
            {
                x1 = 0;
                x2 = xDiff;
                xCanvas = fromX;
                if (xDiff > MIN_SIZE)
                {
                    var moreX = association.FromTypeDesigner.ActualWidth;
                    xCanvas += moreX;
                    x2 -= moreX;
                }
            }
            else
            {
                x1 = -xDiff;
                x2 = 0;
                xCanvas = toX;
                if (xDiff < -MIN_SIZE)
                {
                    var moreX = association.ToTypeDesigner.ActualWidth;
                    xCanvas += moreX;
                    x1 -= moreX;
                }
            }

            var fromItem = association.FromTypeDesigner.IsExpanded ? association.FromItem : association.FromTypeDesigner;
            var toItem = association.ToTypeDesigner.IsExpanded ? association.ToItem : association.ToTypeDesigner;
            var fromY = -((MatrixTransform)association.Canvas.TransformToVisual(fromItem)).Matrix.OffsetY;
            var toY = -((MatrixTransform)association.Canvas.TransformToVisual(toItem)).Matrix.OffsetY;
            var yDiff = toY - fromY;
            var y1 = fromItem.ActualHeight / 2.0;
            var y2 = toItem.ActualHeight / 2.0;
            double y3;
            double yCanvas;
            if (yDiff > 0)
            {
                y2 += yDiff - y1;
                yCanvas = fromY + y1;
                y1 = 0;
            }
            else
            {
                y1 += -yDiff - y2;
                yCanvas = toY + y2;
                y2 = 0;
            }

            double fromRight = fromX + association.FromTypeDesigner.ActualWidth;
            double toRight = toX + association.ToTypeDesigner.ActualWidth;
            var xDiff1 = toX - fromRight;
            var xDiff2 = toRight - fromX;
            var fromDesignerTop = -((MatrixTransform)association.Canvas.TransformToVisual(association.FromTypeDesigner)).Matrix.OffsetY;
            var fromDesignerBottom = fromDesignerTop + association.FromTypeDesigner.ActualHeight;
            var toDesignerTop = -((MatrixTransform)association.Canvas.TransformToVisual(association.ToTypeDesigner)).Matrix.OffsetY;
            var toDesignerBottom = toDesignerTop + association.ToTypeDesigner.ActualHeight;
            bool twoPoints = (Math.Min(Math.Abs(xDiff1), Math.Abs(xDiff2)) > MIN_SIZE && xDiff1 < 0 == xDiff2 < 0 || !(fromDesignerBottom <= toDesignerTop || toDesignerBottom <= fromDesignerTop)) && association.FromTypeDesigner != association.ToTypeDesigner;
            if (twoPoints)
            {
                x3 = 0;
                x4 = 0;
                y3 = 0;
            }
            else
            {
                y3 = y2;
                y2 = y1;
                x4 = 0;
                bool left = Math.Abs(toX - fromX) < Math.Abs(toRight - fromRight);
                if (left)
                {
                    x1 = MIN_SIZE + (double)association.FromItemIndex * MORE_SIZE_PER_ITEM;
                    x4 = MIN_SIZE + (double)association.ToItemIndex * MORE_SIZE_PER_ITEM;
                    if (fromX > toX)
                        x1 += fromX - toX;
                    else
                        x4 += toX - fromX;
                    xCanvas = Math.Min(fromX - x1, toX - x4);
                    x2 = 0;
                }
                if (!left || xCanvas < 0)
                {
                    x1 = fromRight;
                    x4 = toRight;
                    xCanvas = Math.Min(fromRight, toRight);
                    x1 -= xCanvas;
                    x4 -= xCanvas;
                    x2 = Math.Max(x1 + (double)association.FromItemIndex * MORE_SIZE_PER_ITEM, x4 + (double)association.ToItemIndex * MORE_SIZE_PER_ITEM) + MIN_SIZE;
                }
                x3 = x2;
            }

            return new double[][] { new[] { xCanvas, yCanvas }, new[] { x1, y1 }, new[] { x2, y2 }, new[] { x3, y3 }, new[] { x4, y3 } };
        }
    }
}
