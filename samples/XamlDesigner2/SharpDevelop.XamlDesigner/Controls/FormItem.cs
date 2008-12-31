using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class FormItem : HeaderedContentControl
	{
		static FormItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FormItem),
				new FrameworkPropertyMetadata(typeof(FormItem)));

			DefaultStyleKey = new ComponentResourceKey(typeof(FormItem), "DefaultStyle");
			VerticalStyleKey = new ComponentResourceKey(typeof(FormItem), "VerticalStyle");
		}

		public static ResourceKey DefaultStyleKey { get; private set; }
		public static ResourceKey VerticalStyleKey { get; private set; }
	}
}
