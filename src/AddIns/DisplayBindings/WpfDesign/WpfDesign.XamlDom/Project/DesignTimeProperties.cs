using System.Security.Cryptography;
using System.Windows;

namespace ICSharpCode.WpfDesign.XamlDom
{
    public class DesignTimeProperties : FrameworkElement
    {
        #region IsHidden
        
        public static bool GetIsHidden(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHiddenProperty);
        }

        public static void SetIsHidden(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHiddenProperty, value);
        }
        
        public static readonly DependencyProperty IsHiddenProperty =
            DependencyProperty.RegisterAttached("IsHidden", typeof(bool), typeof(DesignTimeProperties));

        #endregion

        #region DesignWidth
        
        public static double GetDesignWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(DesignWidthProperty);
        }

        public static void SetDesignWidth(DependencyObject obj, double value)
        {
            obj.SetValue(DesignWidthProperty, value);
        }

        public static readonly DependencyProperty DesignWidthProperty =
            DependencyProperty.RegisterAttached("DesignWidth", typeof(double), typeof(DesignTimeProperties));

        #endregion

        #region DesignHeight

        public static double GetDesignHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(DesignHeightProperty);
        }

        public static void SetDesignHeight(DependencyObject obj, double value)
        {
            obj.SetValue(DesignHeightProperty, value);
        }

        public static readonly DependencyProperty DesignHeightProperty =
            DependencyProperty.RegisterAttached("DesignHeight", typeof(double), typeof(DesignTimeProperties));
        
        #endregion

        #region Ignorable

        public static string GetIgnorable(DependencyObject obj)
        {
            return (string)obj.GetValue(IgnorableProperty);
        }

        public static void SetIgnorable(DependencyObject obj, string value)
        {
            obj.SetValue(IgnorableProperty, value);
        }

        public static readonly DependencyProperty IgnorableProperty =
            DependencyProperty.RegisterAttached("Ignorable", typeof(string), typeof(DesignTimeProperties));
        
        #endregion
    }
}
