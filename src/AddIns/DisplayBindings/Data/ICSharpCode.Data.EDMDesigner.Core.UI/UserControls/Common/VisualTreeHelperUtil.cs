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

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common
{
    public class VisualTreeHelperUtil
    {
        public static T GetControlAscendant<T>(object reference) where T : DependencyObject
        {
            return GetControlAscendant<T>(reference as DependencyObject);
        }
        public static T GetControlAscendant<T>(DependencyObject reference) where T : DependencyObject
        {
            if (reference == null || !(reference is Visual || reference is Visual3D))
                return null;
            DependencyObject parent = reference;
            try
            {
                while (!((parent = VisualTreeHelper.GetParent(parent)) == null || parent is T)) ;
                return (T)parent;
            }
            catch
            {
                return null;
            }
        }

        public static bool IsAscendant(DependencyObject parent, DependencyObject child)
        {
            if (parent == null || child == null)
                return false;
            DependencyObject parentTmp = child;
            while (!((parentTmp = VisualTreeHelper.GetParent(parentTmp)) == null || parentTmp == parent)) ;
            return parentTmp != null;
        }

        public static IEnumerable<T> GetControlsDecendant<T>(object reference) where T : DependencyObject
        {
            return GetControlsDecendant<T>(reference as DependencyObject);
        }
        public static IEnumerable<T> GetControlsDecendant<T>(DependencyObject reference) where T : DependencyObject
        {
            if (reference == null)
                yield break; 
            int nbChildren = VisualTreeHelper.GetChildrenCount(reference);
            for (int index = 0; index < nbChildren; index++)
            {
                var child = VisualTreeHelper.GetChild(reference, index);
                T value = child as T;
                if (value != null)
                    yield return value;
                foreach (var childValue in GetControlsDecendant<T>(child))
                    yield return childValue;
            }
            yield break;
        }

        public static FrameworkElement GetFocusedElement(DependencyObject reference)
        {
            return VisualTreeHelperUtil.GetControlsDecendant<FrameworkElement>(reference).FirstOrDefault(fe => fe.IsFocused);
        }
    }
}
