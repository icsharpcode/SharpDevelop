using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    public class InputBindingsContener : ContentControl
    {
        public static InputBindingCollection GetInputBindingsCollection(DependencyObject obj)
        {
            return (InputBindingCollection)obj.GetValue(InputBindingsProperty);
        }
        public static void SetInputBindingsCollection(DependencyObject obj, InputBindingCollection value)
        {
            obj.SetValue(InputBindingsProperty, value);
        }
        public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached("InputBindingsCollection", typeof(InputBindingCollection), typeof(UIElement), new UIPropertyMetadata(null, (sender, e) => ((UIElement)sender).InputBindings.AddRange((InputBindingCollection)e.NewValue)));
    }
}
