// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using ICSharpCode.Data.EDMDesigner.Core.UI.Helpers;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    /// <summary>
    /// <remarks>
    /// This class is done using Thomas' one
    /// </remarks>
    /// </summary>
    /// <seealso cref="http://blogs.codes-sources.com/tom/archive/2008/12/16/wpf-d-velopper-un-textblock-ditable.aspx"/>
    [TemplatePart(Type = typeof(Grid), Name = EditableTextBlock.GRID_NAME)]
    [TemplatePart(Type = typeof(TextBlock), Name = EditableTextBlock.TEXTBLOCK_DISPLAYTEXT_NAME)]
    [TemplatePart(Type = typeof(TextBox), Name = EditableTextBlock.TEXTBOX_EDITTEXT_NAME)]
    public class EditableTextBlock : Control
    {
        private const string GRID_NAME = "PART_GridContainer";
        private const string TEXTBLOCK_DISPLAYTEXT_NAME = "PART_TbDisplayText";
        private const string TEXTBOX_EDITTEXT_NAME = "PART_TbEditText";

        private Grid _gridContainer;
        private TextBlock _textBlockDisplayText;
        private TextBox _textBoxEditText;

        static EditableTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditableTextBlock), new FrameworkPropertyMetadata(typeof(EditableTextBlock)));
        }

        public EditableTextBlock()
        {
            Focusable = true;
            ResourceDictionaryLoader.LoadResourceDictionary("/UserControls/EditableTextBlockResourceDictionary.xaml");
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock), new UIPropertyMetadata(string.Empty));

        public Brush TextBlockForegroundColor
        {
            get { return (Brush)GetValue(TextBlockForegroundColorProperty); }
            set { SetValue(TextBlockForegroundColorProperty, value); }
        }
        public static readonly DependencyProperty TextBlockForegroundColorProperty = DependencyProperty.Register("TextBlockForegroundColor", typeof(Brush), typeof(EditableTextBlock), new UIPropertyMetadata(Brushes.Black));

        public Brush TextBlockBackgroundColor
        {
            get { return (Brush)GetValue(TextBlockBackgroundColorProperty); }
            set { SetValue(TextBlockBackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty TextBlockBackgroundColorProperty = DependencyProperty.Register("TextBlockBackgroundColor", typeof(Brush), typeof(EditableTextBlock), new UIPropertyMetadata(null));

        public Brush TextBoxForegroundColor
        {
            get { return (Brush)GetValue(TextBoxForegroundColorProperty); }
            set { SetValue(TextBoxForegroundColorProperty, value); }
        }
        public static readonly DependencyProperty TextBoxForegroundColorProperty = DependencyProperty.Register("TextBoxForegroundColor", typeof(Brush), typeof(EditableTextBlock), new UIPropertyMetadata(Brushes.Black));

        public Brush TextBoxBackgroundColor
        {
            get { return (Brush)GetValue(TextBoxBackgroundColorProperty); }
            set { SetValue(TextBoxBackgroundColorProperty, value); }
        }
        public static readonly DependencyProperty TextBoxBackgroundColorProperty = DependencyProperty.Register("TextBoxBackgroundColor", typeof(Brush), typeof(EditableTextBlock), new UIPropertyMetadata(Brushes.White));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _gridContainer = Template.FindName(GRID_NAME, this) as Grid;
            if (_gridContainer != null)
            {
                _textBlockDisplayText = _gridContainer.Children[0] as TextBlock;
                _textBoxEditText = _gridContainer.Children[1] as TextBox;
                _textBoxEditText.LostFocus += delegate { EndEdit(); };
                _textBoxEditText.KeyDown +=
                    (sender, e) =>
                    {
                        switch (e.Key)
                        {
                            case Key.Enter:
                                EndEdit();
                                break;
                            case Key.Escape:
                                _textBoxEditText.Text = _oldText;
                                EndEdit();
                                break;
                        }
                        //e.Handled = true;
                    };
            }
        }

        private string _oldText;
        private bool _isEditing;

        public void Edit()
        {
            if (! _isEditing)
            {
                _isEditing = true;
                _oldText = Text;
                _textBlockDisplayText.Visibility = Visibility.Hidden;
                _textBoxEditText.Visibility = Visibility.Visible;
                _textBoxEditText.Focus();
            }
        }
        private void EndEdit()
        {
            _textBlockDisplayText.Visibility = Visibility.Visible;
            _textBoxEditText.Visibility = Visibility.Hidden;
            _isEditing = false;
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            Edit();
            e.Handled = true;
        }
    }
}
