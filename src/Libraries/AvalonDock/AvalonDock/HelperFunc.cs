using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;

namespace AvalonDock
{
    internal static class HelperFunc
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T v in collection)
                action(v);
        }

        public static bool AreClose(double v1, double v2)
        {
            if (v1 == v2)
            {
                return true;
            }
            double num = ((Math.Abs(v1) + Math.Abs(v2)) + 10.0) * 2.2204460492503131E-16;
            double num2 = v1 - v2;
            return ((-num < num2) && (num > num2));
        }

        public static double MultiplyCheckNaN(double v1, double v2)
        {
            //inf * 0 = 1
            if (double.IsInfinity(v1) &&
                v2 == 0.0)
                return 1.0;
            if (double.IsInfinity(v2) &&
                v1 == 0.0)
                return 1.0;

            return v1 * v2;
        }


        public static bool IsLessThen(double v1, double v2)
        {
            if (AreClose(v1, v2))
                return false;

            return v1 < v2;
        }

        public static Point PointToScreenWithoutFlowDirection(FrameworkElement element, Point point)
        { 
            if (FrameworkElement.GetFlowDirection(element) == FlowDirection.RightToLeft)
            {
                var actualSize = element.TransformedActualSize();
                Point leftToRightPoint = new Point(
                    actualSize.Width - point.X,
                    point.Y);
                return element.PointToScreenDPI(leftToRightPoint);
            }

            return element.PointToScreenDPI(point);
        }

        public static T FindVisualAncestor<T>(this DependencyObject obj, bool includeThis) where T : DependencyObject
        {
            if (!includeThis)
                obj = VisualTreeHelper.GetParent(obj);

            while (obj != null && (!(obj is T)))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            return obj as T;
        }

        public static bool IsLogicalChildContained<T>(this DependencyObject obj) where T : DependencyObject
        {
            foreach (object child in LogicalTreeHelper.GetChildren(obj))
            {
                if (child is T)
                    return true;

                if (child is DependencyObject)
                {

                    bool res = (child as DependencyObject).IsLogicalChildContained<T>();
                    if (res)
                        return true;
                }
            }

            return false;
        }

        public static T GetLogicalChildContained<T>(this DependencyObject obj) where T : DependencyObject
        {
            foreach (object child in LogicalTreeHelper.GetChildren(obj))
            {
                if (child is T)
                    return child as T;

                if (child is DependencyObject)
                {
                    T childFound = (child as DependencyObject).GetLogicalChildContained<T>();
                    if (childFound != null)
                        return childFound;
                }
            }

            return null;
        }

        public static T FindAnotherLogicalChildContained<T>(this DependencyObject obj, UIElement childToExclude) where T : DependencyObject
        {
            foreach (object child in LogicalTreeHelper.GetChildren(obj))
            {
                if (child is T && child != childToExclude)
                    return child as T;

                if (child is DependencyObject)
                {
                    T childFound = (child as DependencyObject).FindAnotherLogicalChildContained<T>(childToExclude);
                    if (childFound != null)
                        return childFound;
                }
            }

            return null;
        }

        public static IEnumerable<DockablePane> DockablePanes(this UIElement element)
        {
            if (element is DockablePane)
                yield return element as DockablePane;

            foreach (UIElement childObject in LogicalTreeHelper.GetChildren(element))
            {
                if (element is DockablePane)
                    yield return element as DockablePane;

                yield return FindChildDockablePane(childObject);
            }
        }

        static DockablePane FindChildDockablePane(UIElement parent)
        {
            if (parent is ResizingPanel)
            {
                foreach (UIElement childObject in ((ResizingPanel)parent).Children)
                {
                    DockablePane foundPane = FindChildDockablePane(childObject);
                    if (foundPane != null)
                        return foundPane;
                }
            }

            return null;
        }


        public static Point PointToScreenDPI(this Visual visual, Point pt)
        {
            Point resultPt = visual.PointToScreen(pt);
            return TransformToDeviceDPI(visual, resultPt);
        }

        public static Point TransformToDeviceDPI(this Visual visual, Point pt)
        {
            Matrix m = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Point(pt.X / m.M11, pt.Y /m.M22);
        }

        public static Size TransformFromDeviceDPI(this Visual visual, Size size)
        {
            Matrix m = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Size(size.Width * m.M11, size.Height * m.M22);
        }

        public static Point TransformFromDeviceDPI(this Visual visual, Point pt)
        {
            Matrix m = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Point(pt.X * m.M11, pt.Y * m.M22);
        }

        public static bool CanTransform(this Visual visual)
        {
            return PresentationSource.FromVisual(visual) != null;
        }

        public static Size TransformedActualSize(this FrameworkElement element)
        {
            if (PresentationSource.FromVisual(element) == null)
                return new Size(element.ActualWidth, element.ActualHeight);

            var parentWindow = PresentationSource.FromVisual(element).RootVisual;
            var transformToWindow = element.TransformToAncestor(parentWindow);
            return transformToWindow.TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight)).Size;
        }

        public static Size TransformSize(this FrameworkElement element, Size sizeToTransform)
        {
            if (PresentationSource.FromVisual(element) == null)
                return sizeToTransform;

            var parentWindow = PresentationSource.FromVisual(element).RootVisual;
            var transformToWindow = element.TransformToAncestor(parentWindow);
            return transformToWindow.TransformBounds(new Rect(0, 0, sizeToTransform.Width, sizeToTransform.Height)).Size;
        }

        public static GeneralTransform TansformToAncestor(this FrameworkElement element)
        {
            if (PresentationSource.FromVisual(element) == null)
                return new MatrixTransform(Matrix.Identity);

            var parentWindow = PresentationSource.FromVisual(element).RootVisual;
            return element.TransformToAncestor(parentWindow);
        }

        public static void CallMethod(this object o, string methodName, object[] args)
        {
            o.GetType().GetMethod(methodName).Invoke(o, null);            
        }

        public static T GetPropertyValue<T>(this object o, string propertyName)
        {
            return (T)o.GetType().GetProperty(propertyName).GetValue(o, null);
        }
    }
}
