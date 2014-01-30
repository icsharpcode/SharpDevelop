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
