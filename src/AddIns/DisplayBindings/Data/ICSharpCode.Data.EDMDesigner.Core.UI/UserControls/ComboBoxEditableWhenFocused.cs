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

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;
using System.Windows.Data;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    public class ComboBoxEditableWhenFocused : ComboBox
    {
        public ComboBoxEditableWhenFocused()
        {
            SetBinding(IsEditableProperty, new Binding("IsOrHisChildrenAreFocused") { Source = this });
            GotFocus += delegate { IsOrHisChildrenAreFocused = true; };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBox textBox = null;
            foreach (var tb in VisualTreeHelperUtil.GetControlsDecendant<TextBox>(this).Where(uie => uie.Focusable))
            {
                textBox = tb;
                tb.GotFocus += (sender, e) => OnControlGotFous((UIElement)sender);
                tb.LostFocus +=
                    delegate
                    {
                        IsOrHisChildrenAreFocused = IsOrHisChildrenAreFocused;
                        if (SelectedItem == null)
                            SelectedItem = Items[0];
                    };
            }
            if (textBox != null)
            {
                textBox.Focus();
                textBox.SelectAll();
            }
        }

        internal void SelectNullItemItemIfSelectedItemIsNull()
        {
            if (SelectedItem == null && Items.Count > 0)
                SelectedItem = Items[0];
        }

        public bool IsOrHisChildrenAreFocused
        {
            get { return IsFocused || VisualTreeHelperUtil.GetControlsDecendant<UIElement>(this).Any(uie => uie.IsFocused); }
            set { SetValue(IsOrHisChildrenAreFocusedProperty, value); }
        }
        public static readonly DependencyProperty IsOrHisChildrenAreFocusedProperty =
            DependencyProperty.Register("IsOrHisChildrenAreFocused", typeof(bool), typeof(ComboBoxEditableWhenFocused), new UIPropertyMetadata(false));

        protected virtual void OnControlGotFous(UIElement control)
        {
            if (ControlGotFocus != null)
                ControlGotFocus(control);
        }
        public event Action<UIElement> ControlGotFocus;
    }
}
