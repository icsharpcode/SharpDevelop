// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
