// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
