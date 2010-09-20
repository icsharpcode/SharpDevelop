// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.Converters
{
    public abstract class RelationToXYConverterBase : IValueConverter
    {
        protected const double MIN_SIZE = 30;
        protected const double MORE_SIZE_PER_ITEM = 5;
        protected const double ARROW_ANGLE = Math.PI / 6.0;
        protected const double ARROW_HEIGHT = 10;
        protected const double ARROW_WIDTH = 5.0;
        protected const double ARROW_LINE_MIN_ANGLE = Math.PI / 6.0;
        protected const double ARROW_LINE_MAX_ANGLE = Math.PI / 3.0;

        public abstract object Convert(object value, System.Type targetType, object parameter, CultureInfo culture);

        protected double GetAngle(double x1, double x2, double y1, double y2)
        {
            return (y2 - y1) / (Math.Abs(x2 - x1) + Math.Abs(y2 - y1)) * Math.PI / 2.0;
        }
        protected void FillArrow(double x1, double x2, double y1, double y2, out double x1Arrow, out double x2Arrow, out double y1Arrow, out double y2Arrow)
        {
            double angle = GetAngle(x1, x2, y1, y2);
            FillArrow(angle, x1, x2, y1, y2, out x1Arrow, out x2Arrow, out y1Arrow, out y2Arrow);
        }

        protected void FillArrow(double angle, double x1, double x2, double y1, double y2, out double x1Arrow, out double x2Arrow, out double y1Arrow, out double y2Arrow)
        {
            if (x1 > x2)
                angle = Math.PI - angle;
            x1Arrow = x2 - ARROW_HEIGHT * Math.Cos(angle - ARROW_ANGLE);
            x2Arrow = x2 - ARROW_HEIGHT * Math.Cos(angle + ARROW_ANGLE);
            y1Arrow = y2 - ARROW_HEIGHT * Math.Sin(angle - ARROW_ANGLE);
            y2Arrow = y2 - ARROW_HEIGHT * Math.Sin(angle + ARROW_ANGLE);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
