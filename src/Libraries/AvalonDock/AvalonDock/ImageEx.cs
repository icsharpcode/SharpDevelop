using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace AvalonDock
{
    public class ImageEx : Image
    {
        static ImageEx()
        { 
#if NET4
            UseLayoutRoundingProperty.OverrideMetadata(typeof(ImageEx), new FrameworkPropertyMetadata(true));
#endif
        }
    }
}
