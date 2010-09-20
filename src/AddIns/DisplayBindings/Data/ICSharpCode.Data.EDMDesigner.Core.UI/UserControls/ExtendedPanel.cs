// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    public class ExtendedPanel : Panel
    {
        public ExtendedPanel()
        {
            Focusable = true;
        }

        private List<UIElement> _subElements;
        public List<UIElement> SubElements
        {
            get 
            {
                if (_subElements == null)
                    _subElements = new List<UIElement>();
                return _subElements; 
            }
        }

        public void AddLogicalChild(UIElement uiElement)
        {
            SubElements.Add(uiElement);
            uiElement.GotFocus += delegate { IsFocusedIncludingChildren = true; };
            uiElement.LostFocus += delegate { IsFocusedIncludingChildren = IsFocusedIncludingChildren; };
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            e.Handled = true;
        }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            SubElements.First(uie => uie.Focusable).Focus();
        }

        public bool IsFocusedIncludingChildren
        {
            get 
            { 
                return IsFocused || VisualTreeHelperUtil.GetFocusedElement(this) != null; 
            }
            set { SetValue(IsFocusedIncludingChildrenProperty, value); }
        }
        public static readonly DependencyProperty IsFocusedIncludingChildrenProperty =
            DependencyProperty.Register("IsFocusedIncludingChildren", typeof(bool), typeof(ExtendedPanel), new UIPropertyMetadata(false));
    }
}
