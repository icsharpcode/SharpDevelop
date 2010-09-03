// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
    /// <summary>
    /// Display height of the element.
    /// </summary>
    class HeightDisplay : Control
    {
        static HeightDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeightDisplay), new FrameworkPropertyMetadata(typeof(HeightDisplay)));
        }
    }

    /// <summary>
    /// Display width of the element.
    /// </summary>
    class WidthDisplay : Control
    {
        static WidthDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WidthDisplay), new FrameworkPropertyMetadata(typeof(WidthDisplay)));
        }
    }
}
