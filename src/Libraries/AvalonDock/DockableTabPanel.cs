//Copyright (c) 2007-2009, Adolfo Marinucci
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

namespace AvalonDock
{
    public class DockableTabPanel : PaneTabPanel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double totWidth = 0;
            //double maxHeight = 0;

            if (Children.Count == 0)
                return base.MeasureOverride(availableSize);


            List<UIElement> childsOrderedByWidth = new List<UIElement>();

            foreach (FrameworkElement child in Children)
            {
                child.Width = double.NaN;
                child.Height = double.NaN;

                child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                totWidth += child.DesiredSize.Width;
                childsOrderedByWidth.Add(child);
            }

            if (totWidth > availableSize.Width)
            {
                childsOrderedByWidth.Sort(delegate(UIElement elem1, UIElement elem2) { return elem2.DesiredSize.Width.CompareTo(elem1.DesiredSize.Width); });


                int i = childsOrderedByWidth.Count - 1;
                double sumWidth = 0;

                while (childsOrderedByWidth[i].DesiredSize.Width * (i + 1) + sumWidth < availableSize.Width)
                {
                    sumWidth += childsOrderedByWidth[i].DesiredSize.Width;

                    i--;

                    if (i < 0)
                        break;

                }

                double shWidth = (availableSize.Width - sumWidth) / (i + 1);


                foreach (UIElement child in Children)
                {
                    if (shWidth < child.DesiredSize.Width)
                        child.Measure(new Size(shWidth, availableSize.Height));
                }

            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double offsetX = 0;

            foreach (FrameworkElement child in Children)
            {
                double childFinalWidth = child.DesiredSize.Width;
                child.Arrange(new Rect(offsetX, 0, childFinalWidth, finalSize.Height));

                offsetX += childFinalWidth;
            }

            return base.ArrangeOverride(finalSize);
        }

    }
}
