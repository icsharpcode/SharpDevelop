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
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class TextEditorFontsAndColorsPropertyFactoryTests
	{
		TextEditorFontsAndColorsPropertyFactory propertyFactory;
		Properties properties;
		
		void CreateProperties()
		{
			propertyFactory = new TextEditorFontsAndColorsPropertyFactory();
			properties = new Properties(propertyFactory);
		}
		
		void AssertPropertiesContainProperty(string expectedPropertyName)
		{
			global::EnvDTE.Property property = PropertiesHelper.FindProperty(properties, expectedPropertyName);
			Assert.IsNotNull(property, "Unable to find property: " + expectedPropertyName);
		}
		
		[Test]
		public void Item_LocateFontsAndColorsItemsProperty_ReturnsFontsAndColorsItemsProperty()
		{
			CreateProperties();
			
			var property = properties.Item("FontsAndColorsItems") as TextEditorFontsAndColorsItemsProperty;
			string name = property.Name;
			
			Assert.AreEqual("FontsAndColorsItems", name);
		}
		
		[Test]
		public void GetEnumerator_GetAllPropertiesUsingEnumerator_HasFontsAndColorItemsProperty()
		{
			CreateProperties();
			
			AssertPropertiesContainProperty("FontsAndColorsItems");
		}
				
		[Test]
		public void Item_UnknownProperty_ReturnsNull()
		{
			CreateProperties();
			
			global::EnvDTE.Property property = properties.Item("UnknownPropertyName");
			Assert.IsNull(property);
		}
		
		[Test]
		public void Item_LocateFontsAndColorsItemsPropertyInUpperCase_ReturnsFontsAndColorsItemsProperty()
		{
			CreateProperties();
			
			global::EnvDTE.Property property = properties.Item("FONTSANDCOLORSITEMS");
			Assert.IsNotNull(property);
		}
		
		[Test]
		public void Item_LocateFontSizeProperty_ReturnsFontSizeProperty()
		{
			CreateProperties();
			
			var property = properties.Item("FontSize") as TextEditorFontSizeProperty;
			string name = property.Name;
			
			Assert.AreEqual("FontSize", name);
		}
		
		[Test]
		public void Item_LocateFontSizePropertyInUpperCase_ReturnsFontSizeProperty()
		{
			CreateProperties();
			
			global::EnvDTE.Property property = properties.Item("FONTSIZE");
			
			Assert.IsNotNull(property);
		}
		
		[Test]
		public void GetEnumerator_GetAllPropertiesUsingEnumerator_HasFontSizeProperty()
		{
			CreateProperties();
			
			AssertPropertiesContainProperty("FontSize");
		}
	}
}
