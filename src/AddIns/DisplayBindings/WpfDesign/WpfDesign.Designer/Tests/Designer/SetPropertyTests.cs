// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	[TestFixture]
	public class SetPropertyTests : ModelTestHelper
	{
		[Test]
		public void SetContentToBinding()
		{
			DesignItem button = CreateCanvasContext("<Button Width='100' Height='200'/>");
			button.Properties.GetProperty("Content").SetValue(new Binding());
			AssertCanvasDesignerOutput("<Button Width=\"100\" Height=\"200\" Content=\"{Binding}\" />", button.Context);
		}
		
		[Test, Ignore("Properties are not present in XAML DOM")]
		public void SetContentToStaticResource()
		{
			DesignItem button = CreateCanvasContext(@"<Button Width='100' Height='200'/>");
			button.Properties.GetProperty("Content").SetValue(new StaticResourceExtension("MyBrush"));
			// TODO : maybe we should support positional arguments from ctors as well => {StaticResource MyBrush}?
			AssertCanvasDesignerOutput("<Button Width=\"100\" Height=\"200\" Content=\"{StaticResource ResourceKey=MyBrush}\" />", button.Context);
		}
		
		[Test, Ignore("Properties are not present in XAML DOM")]
		public void SetContentToXStatic()
		{
			DesignItem button = CreateCanvasContext("<Button Width='100' Height='200'/>");
			button.Properties.GetProperty("Content").SetValue(new StaticExtension("Button.ClickModeProperty"));
			AssertCanvasDesignerOutput("<Button Width=\"100\" Height=\"200\" Content=\"{x:Static Member=Button.ClickModeProperty}\" />", button.Context);
		}
		
		[Test]
		public void SetContentToString()
		{
			DesignItem button = CreateCanvasContext("<Button Width='100' Height='200'/>");
			button.Properties.GetProperty("Content").SetValue("Hello World!");
			AssertCanvasDesignerOutput("<Button Width=\"100\" Height=\"200\" Content=\"Hello World!\" />", button.Context);
		}
	}
}
