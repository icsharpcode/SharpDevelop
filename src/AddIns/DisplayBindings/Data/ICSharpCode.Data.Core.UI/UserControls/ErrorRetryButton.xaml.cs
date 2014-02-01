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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

#endregion

namespace ICSharpCode.Data.Core.UI.UserControls
{
    /// <summary>
    /// Interaction logic for ErrorRetryButton.xaml
    /// </summary>
    public partial class ErrorRetryButton : UserControl
    {
        #region Fields

        public static readonly DependencyProperty ExceptionProperty =
            DependencyProperty.Register("Exception", typeof(Exception), typeof(ErrorRetryButton), new FrameworkPropertyMetadata(null, ExceptionChanged));

        private bool _useExclamationMark = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the exception to display.
        /// </summary>
        public Exception Exception
        {
            get { return (Exception)GetValue(ErrorRetryButton.ExceptionProperty); }
            set { SetValue(ErrorRetryButton.ExceptionProperty, value); }
        }

        /// <summary>
        /// Gets or set if the control should use the exclamation mark for displaying the exception.
        /// </summary>
        public bool UseExclamationMark
        {
            get { return _useExclamationMark; }
            set { _useExclamationMark = value; }
        }

        #endregion

        #region Constructor

        public ErrorRetryButton()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private static void ExceptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ErrorRetryButton errorRetryButton = d as ErrorRetryButton;

            if (errorRetryButton == null)
                return;
            
            if (e.NewValue is Exception)
            {
                errorRetryButton.This.Visibility = Visibility.Visible;

                if (errorRetryButton._useExclamationMark)
                {
                    errorRetryButton.imgExclamation.Visibility = Visibility.Visible;
                    errorRetryButton.imgError.Visibility = Visibility.Collapsed;
                }
                else
                {
                    errorRetryButton.imgError.Visibility = Visibility.Visible;
                    errorRetryButton.imgExclamation.Visibility = Visibility.Collapsed;
                }
            }
            else
            { 
                errorRetryButton.This.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}
