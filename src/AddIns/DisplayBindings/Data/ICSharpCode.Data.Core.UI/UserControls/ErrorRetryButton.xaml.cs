// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
