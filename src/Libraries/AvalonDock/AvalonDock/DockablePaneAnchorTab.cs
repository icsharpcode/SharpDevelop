//Copyright (c) 2007-2010, Adolfo Marinucci
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification, 
//are permitted provided that the following conditions are met:
//
//* Redistributions of source code must retain the above copyright notice, 
//  this list of conditions and the following disclaimer.
//* Redistributions in binary form must reproduce the above copyright notice, 
//  this list of conditions and the following disclaimer in the documentation 
//  and/or other materials provided with the distribution.
//* Neither the name of Adolfo Marinucci nor the names of its contributors may 
//  be used to endorse or promote products derived from this software without 
//  specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
//AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
//IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
//INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
//HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
//EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;

namespace AvalonDock
{
    /// <summary>
    /// Represents a tab displayed in a border of the docking manager
    /// </summary>
    /// <remarks></remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DockablePaneAnchorTab : System.Windows.Controls.Control//, INotifyPropertyChanged
    {
        static DockablePaneAnchorTab()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockablePaneAnchorTab), new FrameworkPropertyMetadata(typeof(DockablePaneAnchorTab)));
        }

        public DockablePaneAnchorTab()
        { 
            
        }

        /// <summary>
        /// Gets or sets the referenced content
        /// </summary>
        public DockableContent ReferencedContent
        {
            get { return (DockableContent)GetValue(ReferencedContentPropertyKey.DependencyProperty); }
            set { SetValue(ReferencedContentPropertyKey, value); }
        }

        // Using a DependencyProperty as the backing store for DockableContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyPropertyKey ReferencedContentPropertyKey =
            DependencyProperty.RegisterReadOnly("ReferencedContent", typeof(DockableContent), typeof(DockablePaneAnchorTab), new UIPropertyMetadata(null, new PropertyChangedCallback(OnPaneAttached)));

        /// <summary>
        /// Handles the referencedContent property changes in order to update the Anchor property
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="e"></param>
        static void OnPaneAttached(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            //Update Anchor, Title and Icon property 
            DockablePaneAnchorTab _this = depObj as DockablePaneAnchorTab;
            _this.SetAnchor(((DockablePane)_this.ReferencedContent.ContainerPane).Anchor);
            _this.SetIcon(_this.ReferencedContent.Icon);
            _this.SetTitle(_this.ReferencedContent.Title);

        }

        ///// <summary>
        ///// Gets anchor style of the referenced content
        ///// </summary>
        ///// <remarks>This proprety is exposed to facilitate the control template binding.</remarks>
        //public AnchorStyle Anchor
        //{
        //    get { return (AnchorStyle)GetValue(AnchorPropertyKey.DependencyProperty); }
        //    protected set { SetValue(AnchorPropertyKey, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        //public static readonly DependencyPropertyKey AnchorPropertyKey =
        //    DependencyProperty.RegisterAttachedReadOnly("Anchor", typeof(AnchorStyle), typeof(DockablePaneAnchorTab), new PropertyMetadata(AnchorStyle.Left));

        ///// <summary>
        ///// Gets icon of the referenced content
        ///// </summary>
        ///// <remarks>This proprety is exposed to facilitate the control template binding.</remarks>
        //public object Icon
        //{
        //    get { return (object)GetValue(IconPropertyKey.DependencyProperty); }
        //    protected set { SetValue(IconPropertyKey, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        //public static readonly DependencyPropertyKey IconPropertyKey =
        //    DependencyProperty.RegisterAttachedReadOnly("Icon", typeof(object), typeof(DockablePaneAnchorTab), new PropertyMetadata(null));

        #region Anchor

        /// <summary>
        /// Anchor Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey AnchorPropertyKey
            = DependencyProperty.RegisterReadOnly("Anchor", typeof(AnchorStyle), typeof(DockablePaneAnchorTab),
                new FrameworkPropertyMetadata((AnchorStyle)AnchorStyle.None));

        public static readonly DependencyProperty AnchorProperty
            = AnchorPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the Anchor property.  This dependency property 
        /// indicates the achor style of referenced content that is in autohidden state.
        /// </summary>
        public AnchorStyle Anchor
        {
            get { return (AnchorStyle)GetValue(AnchorProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the Anchor property.  
        /// This dependency property indicates the achor style of referenced content that is in autohidden state.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetAnchor(AnchorStyle value)
        {
            SetValue(AnchorPropertyKey, value);
        }

        #endregion

        #region Icon

        /// <summary>
        /// Icon Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey IconPropertyKey
            = DependencyProperty.RegisterReadOnly("Icon", typeof(object), typeof(DockablePaneAnchorTab),
                new FrameworkPropertyMetadata((object)null));

        public static readonly DependencyProperty IconProperty
            = IconPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the Icon property.  This dependency property 
        /// indicates icon of the referenced content in autohidden state.
        /// </summary>
        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the Icon property.  
        /// This dependency property indicates icon of the referenced content in autohidden state.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetIcon(object value)
        {
            SetValue(IconPropertyKey, value);
        }

        #endregion

        #region Title

        /// <summary>
        /// Title Read-Only Dependency Property
        /// </summary>
        private static readonly DependencyPropertyKey TitlePropertyKey
            = DependencyProperty.RegisterReadOnly("Title", typeof(object), typeof(DockablePaneAnchorTab),
                new FrameworkPropertyMetadata((string)null));

        public static readonly DependencyProperty TitleProperty
            = TitlePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the Title property.  This dependency property 
        /// indicates title of the content that is hosted in autohidden pane.
        /// </summary>
        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
        }

        /// <summary>
        /// Provides a secure method for setting the Title property.  
        /// This dependency property indicates title of the content that is hosted in autohidden pane.
        /// </summary>
        /// <param name="value">The new value for the property.</param>
        protected void SetTitle(object value)
        {
            SetValue(TitlePropertyKey, value);
        }

        #endregion

        /// <summary>
        /// Handles the MouseMove event
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>Notify the docking manager that the referenced content should appears</remarks>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (ReferencedContent != null)
                ReferencedContent.Manager.ShowFlyoutWindow(ReferencedContent, this);

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handles the MouseDown event
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>Notify the docking manager that the referenced content should appears and should be activated</remarks>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (ReferencedContent != null)
            {
                ReferencedContent.Manager.ShowFlyoutWindow(ReferencedContent, this);
                ReferencedContent.Activate();
            }

            base.OnMouseLeftButtonDown(e);
        }

    }

}
