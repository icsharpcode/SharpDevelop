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
using System.Linq;
using System.Windows.Markup;

namespace AvalonDock
{
    public abstract class PaneTabPanel : Panel
    {
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            ManagedContent mc = visualAdded as ManagedContent;
            if (mc != null)
            {
                mc.Style = TabItemStyle;
                mc.ApplyTemplate();
            }

        }

 
        internal PaneTabPanel()
        { 
            
        }

        #region TabItemStyle

        /// <summary>
        /// TabItemStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty TabItemStyleProperty =
            DependencyProperty.Register("TabItemStyle", typeof(Style), typeof(PaneTabPanel),
                new FrameworkPropertyMetadata((Style)null,
                    new PropertyChangedCallback(OnTabItemStyleChanged)));

        /// <summary>
        /// Gets or sets the TabItemStyle property.  This dependency property 
        /// indicates style to use for tabs.
        /// </summary>
        public Style TabItemStyle
        {
            get { return (Style)GetValue(TabItemStyleProperty); }
            set { SetValue(TabItemStyleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the TabItemStyle property.
        /// </summary>
        private static void OnTabItemStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PaneTabPanel)d).OnTabItemStyleChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the TabItemStyle property.
        /// </summary>
        protected virtual void OnTabItemStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            //Children.Cast<ManagedContent>().ForEach(c =>
            //    {
            //        Binding bnd = new Binding("TabItemStyle");
            //        bnd.Source = this;
            //        bnd.Mode = BindingMode.OneWay;

            //        c.SetBinding(StyleProperty, bnd);

            //        //c.Style = TabItemStyle;
            //    });
        }


        #endregion
    }
}
