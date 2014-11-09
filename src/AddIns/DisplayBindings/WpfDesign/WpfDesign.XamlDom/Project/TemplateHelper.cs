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
using System.Linq;
using System.Windows;

namespace ICSharpCode.WpfDesign.XamlDom
{
	public static class TemplateHelper
	{
		public static FrameworkElementFactory XamlObjectToFrameworkElementFactory(XamlObject xamlObject)
		{
			var factory = new FrameworkElementFactory(xamlObject.ElementType);
			foreach (var prop in xamlObject.Properties)
			{
				if (prop.IsCollection)
				{
					foreach (var propertyValue in prop.CollectionElements)
					{
						if (propertyValue is XamlObject && !((XamlObject)propertyValue).IsMarkupExtension)
						{
							factory.AppendChild(XamlObjectToFrameworkElementFactory((XamlObject)propertyValue));
						}
						else if (propertyValue is XamlObject)
						{
							factory.SetValue(prop.DependencyProperty, ((XamlObject)propertyValue).Instance);
						}
						else
						{
							factory.SetValue(prop.DependencyProperty, prop.ValueOnInstance);
						}
					}
				}
				else
				{
					if (prop.PropertyValue is XamlObject && !((XamlObject)prop.PropertyValue).IsMarkupExtension)
					{
						factory.AppendChild(XamlObjectToFrameworkElementFactory((XamlObject)prop.PropertyValue));
					}
					else if (prop.PropertyValue is XamlObject)
					{
						factory.SetValue(prop.DependencyProperty, ((XamlObject)prop.PropertyValue).Instance);
					}
					else
					{
						factory.SetValue(prop.DependencyProperty, prop.ValueOnInstance);
					}
				}
			}

			return factory;
		}
	}
}
