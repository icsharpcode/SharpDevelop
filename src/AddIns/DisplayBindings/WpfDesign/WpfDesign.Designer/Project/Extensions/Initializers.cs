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
			DesignItemProperty contentProperty = item.Properties["Content"];
			if (contentProperty.ValueOnInstance == null) {
				contentProperty.SetValue(item.ComponentType.Name);
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
