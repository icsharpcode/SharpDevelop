// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class FontsAndColorItemsTests
	{
		FontsAndColorsItems items;
		FakeCustomizedHighlightingRules fakeHighlightingRules;
		
		void CreateFontsAndColorItems()
		{
			fakeHighlightingRules = new FakeCustomizedHighlightingRules();
			items = new FontsAndColorsItems(fakeHighlightingRules);
		}
		
		CustomizedHighlightingColor AddDefaultTextAndBackgroundColorToRules()
		{
			var highlightingColor = new CustomizedHighlightingColor();
			highlightingColor.Name = CustomizableHighlightingColorizer.DefaultTextAndBackground;
			
			fakeHighlightingRules.Colors.Add(highlightingColor);
			
			return highlightingColor;
		}
		
		[Test]
		public void Item_GetPlainTextItem_ReturnsColorableItems()
		{
			CreateFontsAndColorItems();
			AddDefaultTextAndBackgroundColorToRules();
			
			global::EnvDTE.ColorableItems colorableItems = items.Item("Plain Text");
			string name = colorableItems.Name;
			
			Assert.AreEqual("Plain Text", name);
		}
		
		[Test]
		public void Item_GetUnknownItem_ReturnsNull()
		{
			CreateFontsAndColorItems();
			AddDefaultTextAndBackgroundColorToRules();
			
			global::EnvDTE.ColorableItems colorableItems = items.Item("Unknown item");
			
			Assert.IsNull(colorableItems);
		}
		
		[Test]
		public void Item_GetPlainTextItemInUpperCase_ReturnsColorableItems()
		{
			CreateFontsAndColorItems();
			AddDefaultTextAndBackgroundColorToRules();
			
			global::EnvDTE.ColorableItems colorableItems = items.Item("PLAIN TEXT");
			string name = colorableItems.Name;
			
			Assert.AreEqual("Plain Text", name);
		}
		
		[Test]
		public void Item_DefaultTextBackgroundColorIsBlue_PlainTextItemBackgroundIsBlue()
		{
			CreateFontsAndColorItems();
			CustomizedHighlightingColor defaultColor = AddDefaultTextAndBackgroundColorToRules();
			defaultColor.Background = Colors.Blue;
			
			global::EnvDTE.ColorableItems colorableItems = items.Item("Plain Text");
			uint backgroundOleColor = colorableItems.Background;
			Color backgroundColor = ColorHelper.ConvertToColor(backgroundOleColor);
			
			Assert.AreEqual(Colors.Blue, backgroundColor);
		}
		
		[Test]
		public void Item_GetPlainTextItemWhenCustomHighlightingColorDoesNotExist_ReturnsPlainTextItemWithDefaultBackgroundColor()
		{
			CreateFontsAndColorItems();
			
			global::EnvDTE.ColorableItems colorableItems = items.Item("Plain Text");
			uint backgroundOleColor = colorableItems.Background;
			Color backgroundColor = ColorHelper.ConvertToColor(backgroundOleColor);
			
			Assert.AreEqual(ColorableItems.DefaultBackgroundColor, backgroundColor);
		}
		
		[Test]
		public void Item_GetPlainTextItemWhenCustomHighlightingColorDoesNotExist_PlainTextItemAddedToCustomizationColorsWhenBoldIsSetToTrue()
		{
			CreateFontsAndColorItems();
			
			global::EnvDTE.ColorableItems colorableItems = items.Item("Plain Text");
			colorableItems.Bold = true;
			
			CustomizedHighlightingColor color = 
				fakeHighlightingRules
					.ColorsSaved
					.Find(c => c.Name == CustomizableHighlightingColorizer.DefaultTextAndBackground);
			
			Assert.IsNotNull(color);
		}
	}
}
