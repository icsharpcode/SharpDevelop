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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls
{
    public class ToolBoxGrid : Grid
    {
        public ToolBoxGrid()
        {
            SizeChanged +=
                delegate
                {
                    var lastRowDefinition = RowDefinitions.Last();
                    var rowsHeight = RowDefinitions.Sum(rd => rd.ActualHeight);
                    lastRowDefinition.Height = new GridLength(Math.Max(LastRowMinHeight, ActualHeight - rowsHeight + lastRowDefinition.ActualHeight));
                };
        }

        protected override void OnInitialized(EventArgs e0)
        {
            base.OnInitialized(e0);
            var parent = Parent as FrameworkElement;
            parent.SizeChanged +=
                (sender, e) =>
                {
                    var heightDiff = e.NewSize.Height - e.PreviousSize.Height;
                    if (heightDiff < 0)
                        RowDefinitions.Last().Height = new GridLength(LastRowMinHeight);
                };
        }

        public double LastRowMinHeight
        {
            get { return (double)GetValue(LastRowMinHeightProperty); }
            set { SetValue(LastRowMinHeightProperty, value); }
        }
        public static readonly DependencyProperty LastRowMinHeightProperty =
            DependencyProperty.Register("LastRowMinHeight", typeof(double), typeof(ToolBoxGrid), new UIPropertyMetadata((double)0));
    }
}
