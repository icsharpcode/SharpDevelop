// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
