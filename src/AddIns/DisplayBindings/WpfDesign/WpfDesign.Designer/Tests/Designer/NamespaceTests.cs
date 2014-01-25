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
using System.Windows;
using System.Windows.Controls;

using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	[TestFixture]
	public class NamespaceTests : ModelTestHelper
	{
	
		[Test]
		public void AddControlFromTestNamespace()
		{
			DesignItem button = CreateCanvasContext("<Button />");
			
			DesignItem canvas = button.Parent;
			
			DesignItem customButton = canvas.Services.Component.RegisterComponentForDesigner(new CustomButton());
			canvas.Properties["Children"].CollectionElements.Add(customButton);

			AssertCanvasDesignerOutput("<Button />\n" +
			                           "<t:CustomButton />", canvas.Context);
		}
		
		[Test]
		public void AddControlWithUndeclaredNamespace()
		{
			DesignItem button = CreateCanvasContext("<Button />");
			
			DesignItem canvas = button.Parent;
			
			DesignItem customButton = canvas.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.OtherControls.CustomButton());
			canvas.Properties["Children"].CollectionElements.Add(customButton);

			AssertCanvasDesignerOutput("<Button />\n" +
			                           "<Controls0:CustomButton />",
			                           canvas.Context,
			                           "xmlns:Controls0=\"clr-namespace:ICSharpCode.WpfDesign.Tests.OtherControls;assembly=ICSharpCode.WpfDesign.Tests\"");
		}
		
		[Test]
		public void AddControlWithUndeclaredNamespaceThatUsesXmlnsPrefixAttribute()
		{
			DesignItem button = CreateCanvasContext("<Button />");
			
			DesignItem canvas = button.Parent;
			
			DesignItem customButton = canvas.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.Controls.CustomButton());
			canvas.Properties["Children"].CollectionElements.Add(customButton);

			AssertCanvasDesignerOutput("<Button />\n" +
			                           "<sdtcontrols:CustomButton />",
			                           canvas.Context,
			                           "xmlns:sdtcontrols=\"http://sharpdevelop.net/WpfDesign/Tests/Controls\"");
		}
		
		[Test]
		public void AddMultipleControls()
		{
			DesignItem button = CreateCanvasContext("<Button />");
			
			DesignItem canvas = button.Parent;
			
			DesignItem customControl = canvas.Services.Component.RegisterComponentForDesigner(new CustomButton());
			canvas.Properties["Children"].CollectionElements.Add(customControl);
			
			customControl = canvas.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.Controls.CustomButton());
			canvas.Properties["Children"].CollectionElements.Add(customControl);
			
			customControl = canvas.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.OtherControls.CustomButton());
			canvas.Properties["Children"].CollectionElements.Add(customControl);
			
			customControl = canvas.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.SpecialControls.CustomButton());
			canvas.Properties["Children"].CollectionElements.Add(customControl);
			
			customControl = canvas.Services.Component.RegisterComponentForDesigner(new CustomCheckBox());
			canvas.Properties["Children"].CollectionElements.Add(customControl);
			
			customControl = canvas.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.Controls.CustomCheckBox());
			canvas.Properties["Children"].CollectionElements.Add(customControl);
			
			customControl = canvas.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.OtherControls.CustomCheckBox());
			canvas.Properties["Children"].CollectionElements.Add(customControl);
			
			customControl = canvas.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.SpecialControls.CustomCheckBox());
			canvas.Properties["Children"].CollectionElements.Add(customControl);

			AssertCanvasDesignerOutput("<Button />\n" +
			                           "<t:CustomButton />\n" +
			                           "<sdtcontrols:CustomButton />\n" +
			                           "<Controls0:CustomButton />\n" +
			                           "<Controls1:CustomButton />\n" +
			                           "<t:CustomCheckBox />\n" +
			                           "<sdtcontrols:CustomCheckBox />\n" +
			                           "<Controls0:CustomCheckBox />\n" +
			                           "<Controls1:CustomCheckBox />",
			                           canvas.Context,
			                           "xmlns:sdtcontrols=\"http://sharpdevelop.net/WpfDesign/Tests/Controls\"",
			                           "xmlns:Controls0=\"clr-namespace:ICSharpCode.WpfDesign.Tests.OtherControls;assembly=ICSharpCode.WpfDesign.Tests\"",
			                           "xmlns:Controls1=\"clr-namespace:ICSharpCode.WpfDesign.Tests.SpecialControls;assembly=ICSharpCode.WpfDesign.Tests\"");
		}
	}
	
	public class CustomButton : Button
	{
		public static readonly DependencyProperty TestAttachedProperty = DependencyProperty.RegisterAttached("TestAttached", typeof(double), typeof(CustomButton),
		                                                                                                     new FrameworkPropertyMetadata(Double.NaN));

		public static double GetTestAttached(UIElement element)
		{
			return (double)element.GetValue(TestAttachedProperty);
		}

		public static void SetTestAttached(UIElement element, double value)
		{
			element.SetValue(TestAttachedProperty, value);
		}
	}
	
	public class CustomCheckBox : CheckBox
	{
	}
}

namespace ICSharpCode.WpfDesign.Tests.Controls
{
	public class CustomButton : Button
	{
		public static readonly DependencyProperty TestAttachedProperty = DependencyProperty.RegisterAttached("TestAttached", typeof(double), typeof(CustomButton),
		                                                                                                     new FrameworkPropertyMetadata(Double.NaN));

		public static double GetTestAttached(UIElement element)
		{
			return (double)element.GetValue(TestAttachedProperty);
		}

		public static void SetTestAttached(UIElement element, double value)
		{
			element.SetValue(TestAttachedProperty, value);
		}
	}
	
	public class CustomCheckBox : CheckBox
	{
	}
}

namespace ICSharpCode.WpfDesign.Tests.OtherControls
{
	public class CustomButton : Button
	{
		public static readonly DependencyProperty TestAttachedProperty = DependencyProperty.RegisterAttached("TestAttached", typeof(double), typeof(CustomButton),
		                                                                                                     new FrameworkPropertyMetadata(Double.NaN));

		public static double GetTestAttached(UIElement element)
		{
			return (double)element.GetValue(TestAttachedProperty);
		}

		public static void SetTestAttached(UIElement element, double value)
		{
			element.SetValue(TestAttachedProperty, value);
		}
	}
	
	public class CustomCheckBox : CheckBox
	{
	}
}

namespace ICSharpCode.WpfDesign.Tests.SpecialControls
{
	public class CustomButton : Button
	{
	}
	
	public class CustomCheckBox : CheckBox
	{
	}
}
