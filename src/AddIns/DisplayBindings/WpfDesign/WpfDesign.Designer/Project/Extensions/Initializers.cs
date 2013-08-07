// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.Extensions;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer.Extensions.Initializers
{
	[ExtensionFor(typeof(ContentControl))]
	public class ContentControlInitializer : DefaultInitializer
	{
		public override void InitializeDefaults(DesignItem item)
		{
            //Not every Content Control can have a text as Content (e.g. ZoomBox of WPF Toolkit)
		    if (item.Component is Button)
		    {
				DesignItemProperty contentProperty = item.Properties["Content"];
		        if (contentProperty.ValueOnInstance == null)
		        {
					contentProperty.SetValue(item.ComponentType.Name);
				}
			}

		    DesignItemProperty verticalAlignmentProperty = item.Properties["VerticalAlignment"];
            if (verticalAlignmentProperty.ValueOnInstance == null)
            {
                verticalAlignmentProperty.SetValue(VerticalAlignment.Center);
            }

            DesignItemProperty horizontalAlignmentProperty = item.Properties["HorizontalAlignment"];
            if (horizontalAlignmentProperty.ValueOnInstance == null)
            {
                horizontalAlignmentProperty.SetValue(HorizontalAlignment.Center);
            }
		}
	}

	[ExtensionFor(typeof(TextBlock))]
    public class TextBlockInitializer : DefaultInitializer
    {
        public override void InitializeDefaults(DesignItem item)
        {
            DesignItemProperty textProperty = item.Properties["Text"];
            if (textProperty.ValueOnInstance == null || textProperty.ValueOnInstance.ToString() == "")
            {
                textProperty.SetValue(item.ComponentType.Name);
            }

            DesignItemProperty verticalAlignmentProperty = item.Properties["VerticalAlignment"];
            if (verticalAlignmentProperty.ValueOnInstance == null)
            {
                verticalAlignmentProperty.SetValue(VerticalAlignment.Center);
            }

            DesignItemProperty horizontalAlignmentProperty = item.Properties["HorizontalAlignment"];
            if (horizontalAlignmentProperty.ValueOnInstance == null)
            {
                horizontalAlignmentProperty.SetValue(HorizontalAlignment.Center);
            }
        }
	}
    
	[ExtensionFor(typeof(HeaderedContentControl), OverrideExtension = typeof(ContentControlInitializer))]
	public class HeaderedContentControlInitializer : DefaultInitializer
	{
		public override void InitializeDefaults(DesignItem item)
		{
			DesignItemProperty headerProperty = item.Properties["Header"];
			if (headerProperty.ValueOnInstance == null) {
				headerProperty.SetValue(item.ComponentType.Name);
			}
			
			DesignItemProperty contentProperty = item.Properties["Content"];
			if (contentProperty.ValueOnInstance == null) {
				contentProperty.SetValue(new PanelInstanceFactory().CreateInstance(typeof(Canvas)));
			}
		}
	}

    [ExtensionFor(typeof(Shape))]
	public class ShapeInitializer : DefaultInitializer
	{
		public override void InitializeDefaults(DesignItem item)
		{
			DesignItemProperty fillProperty = item.Properties["Fill"];
			if (fillProperty.ValueOnInstance == null) {
				fillProperty.SetValue(Brushes.YellowGreen);
			}
		}
	}
}
