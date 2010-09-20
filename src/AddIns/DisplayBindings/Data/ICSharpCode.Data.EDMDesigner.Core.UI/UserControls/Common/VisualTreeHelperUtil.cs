// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
