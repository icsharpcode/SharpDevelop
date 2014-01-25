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
		
		[Test]
		public void SetContentToStaticResource()
		{
			DesignItem button = CreateCanvasContext(@"<Button Width='100' Height='200'/>");
			button.Properties.GetProperty("Content").SetValue(new StaticResourceExtension());
			button.Properties.GetProperty("Content").Value.Properties["ResourceKey"].SetValue("MyBrush");
			// TODO : maybe we should support positional arguments from ctors as well => {StaticResource MyBrush}?
			AssertCanvasDesignerOutput("<Button Width=\"100\" Height=\"200\" Content=\"{StaticResource ResourceKey=MyBrush}\" />", button.Context);
		}
		
		[Test]
		public void SetContentToXStatic()
		{
			DesignItem button = CreateCanvasContext("<Button Width='100' Height='200'/>");
			button.Properties.GetProperty("Content").SetValue(new StaticExtension());
			button.Properties.GetProperty("Content").Value.Properties["Member"].SetValue("Button.ClickModeProperty");
			AssertCanvasDesignerOutput("<Button Width=\"100\" Height=\"200\" Content=\"{x:Static Member=Button.ClickModeProperty}\" />", button.Context);
		}
		
		[Test]
		public void SetContentToString()
		{
			DesignItem button = CreateCanvasContext("<Button Width='100' Height='200'/>");
			button.Properties.GetProperty("Content").SetValue("Hello World!");
			AssertCanvasDesignerOutput("<Button Width=\"100\" Height=\"200\" Content=\"Hello World!\" />", button.Context);
		}
		
		[Test]
		public void SetAttachedProperties()
		{
			DesignItem button = CreateCanvasContext("<Button />");
			
			button.Properties.GetAttachedProperty(Grid.ColumnProperty).SetValue(0);
			button.Properties.GetAttachedProperty(CustomButton.TestAttachedProperty).SetValue(0);
			button.Properties.GetAttachedProperty(ICSharpCode.WpfDesign.Tests.Controls.CustomButton.TestAttachedProperty).SetValue(0);
			button.Properties.GetAttachedProperty(ICSharpCode.WpfDesign.Tests.OtherControls.CustomButton.TestAttachedProperty).SetValue(0);
			
			AssertCanvasDesignerOutput("<Button Grid.Column=\"0\" t:CustomButton.TestAttached=\"0\" sdtcontrols:CustomButton.TestAttached=\"0\" Controls0:CustomButton.TestAttached=\"0\" />",
			                           button.Context,
			                           "xmlns:sdtcontrols=\"http://sharpdevelop.net/WpfDesign/Tests/Controls\"",
			                           "xmlns:Controls0=\"clr-namespace:ICSharpCode.WpfDesign.Tests.OtherControls;assembly=ICSharpCode.WpfDesign.Tests\"");
		}
		
		[Test]
		public void SetInstanceProperty()
		{
			DesignItem button = CreateCanvasContext("<Button />");
			button.Properties.GetProperty("Width").SetValue(10);
			AssertCanvasDesignerOutput("<Button Width=\"10\" />", button.Context);
		}
		
		[Test]
		public void SetInstancePropertyElement()
		{
			DesignItem button = CreateCanvasContext("<Button />");
			DesignItem canvas = button.Parent;
			
			canvas.Properties.GetProperty(Canvas.TagProperty).SetValue(new ExampleClass());
			
			DesignItem customControl = canvas.Services.Component.RegisterComponentForDesigner(new ICSharpCode.WpfDesign.Tests.Controls.CustomButton());
			canvas.Properties["Children"].CollectionElements.Add(customControl);
			customControl.Properties.GetProperty(ICSharpCode.WpfDesign.Tests.Controls.CustomButton.TagProperty).SetValue(new ExampleClass());
			
			AssertCanvasDesignerOutput("<Canvas.Tag>\n" +
    	                             "  <t:ExampleClass />\n" +
    	                             "</Canvas.Tag>\n" +
    	                             "<Button />\n" +
    	                             "<sdtcontrols:CustomButton>\n" +
    	                             "  <sdtcontrols:CustomButton.Tag>\n" +
    	                             "    <t:ExampleClass />\n" +
    	                             "  </sdtcontrols:CustomButton.Tag>\n" +
    	                             "</sdtcontrols:CustomButton>",
    	                             canvas.Context,
    	                            "xmlns:sdtcontrols=\"http://sharpdevelop.net/WpfDesign/Tests/Controls\"");
		}
	}
}
