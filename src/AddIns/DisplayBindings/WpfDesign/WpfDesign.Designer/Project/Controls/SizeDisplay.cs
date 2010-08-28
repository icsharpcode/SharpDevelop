// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>
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