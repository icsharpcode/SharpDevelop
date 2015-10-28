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
				item.Properties[FrameworkElement.WidthProperty].Reset();
				item.Properties[FrameworkElement.HeightProperty].Reset();
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
}
