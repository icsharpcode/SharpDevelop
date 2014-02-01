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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.WpfDesign.XamlDom
{
	/// <summary>
	/// Static class that can generate XAML markup extension code ("{Binding Path=...}").
	/// </summary>
	public static class MarkupExtensionPrinter
	{
		/// <summary>
		/// Gets whether shorthand XAML markup extension code can be generated for the object.
		/// </summary>
		public static bool CanPrint(XamlObject obj)
		{
			if (obj.ElementType == typeof(System.Windows.Data.MultiBinding) ||
				obj.ElementType == typeof(System.Windows.Data.PriorityBinding)) {
				return false;
			}
			
			foreach (var property in obj.Properties.Where((prop) => prop.IsSet))
			{
				var value = property.PropertyValue;
				if (value is XamlTextValue)
					continue;
				else
				{
					XamlObject xamlObject = value as XamlObject;
					if (xamlObject == null || !xamlObject.IsMarkupExtension)
						return false;
					else {
						var staticResource = xamlObject.Instance as System.Windows.StaticResourceExtension;
						if (staticResource != null &&
							staticResource.ResourceKey != null) {
							XamlObject parent = GetNonMarkupExtensionParent(xamlObject);
							
							if (parent != null) {
								var parentLocalResource = parent.ServiceProvider.Resolver.FindLocalResource(staticResource.ResourceKey);
								
								// If resource with the specified key is declared locally on the same object as the StaticResource is being used the markup extension
								// must be printed as element to find the resource, otherwise it will search from parent-parent and find none or another resource.
								if (parentLocalResource != null)
									return false;
							}
						}
					}
				}
			}

			return true;
		}
		
		/// <summary>
		/// Generates XAML markup extension code for the object.
		/// </summary>
		public static string Print(XamlObject obj)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			sb.Append(obj.GetNameForMarkupExtension());

			bool first = true;
			foreach (var property in obj.Properties) {
				if (!property.IsSet) continue;

				if (first)
					sb.Append(" ");
				else
					sb.Append(", ");
				first = false;

				sb.Append(property.GetNameForMarkupExtension());
				sb.Append("=");

				var value = property.PropertyValue;
				if (value is XamlTextValue) {
					sb.Append((value as XamlTextValue).Text);
				} else if (value is XamlObject) {
					sb.Append(Print(value as XamlObject));
				}
			}
			sb.Append("}");
			return sb.ToString();
		}
		
		private static XamlObject GetNonMarkupExtensionParent(XamlObject markupExtensionObject)
		{
			System.Diagnostics.Debug.Assert(markupExtensionObject.IsMarkupExtension);
			
			XamlObject obj = markupExtensionObject;
			while (obj != null && obj.IsMarkupExtension) {
				obj = obj.ParentObject;
			}
			return obj;
		}
	}
}
