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
			highlightingColor.Name = CustomizingHighlighter.DefaultTextAndBackground;
			
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
					.Find(c => c.Name == CustomizingHighlighter.DefaultTextAndBackground);
			
			Assert.IsNotNull(color);
		}
	}
}
