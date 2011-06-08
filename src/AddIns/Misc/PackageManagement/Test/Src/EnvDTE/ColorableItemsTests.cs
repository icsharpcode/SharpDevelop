// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class ColorableItemsTests
	{
		ColorableItems items;
		CustomizedHighlightingColor highlightingColor;
		FakeCustomizedHighlightingRules fakeHighlightingRules;
		FontsAndColorsItems fontsAndColorsItems;
		
		void CreateColorableItems()
		{
			highlightingColor = new CustomizedHighlightingColor();
			
			fakeHighlightingRules = new FakeCustomizedHighlightingRules();
			fakeHighlightingRules.Colors.Add(highlightingColor);
			
			fontsAndColorsItems = new FontsAndColorsItems(fakeHighlightingRules);
			
			items = new ColorableItems("Name", highlightingColor, fontsAndColorsItems);
		}
		
		[Test]
		public void Foreground_ForegroundHighlightingColorIsRed_ReturnsRed()
		{
			CreateColorableItems();
			highlightingColor.Foreground = Colors.Red;
			
			UInt32 foregroundOleColor = items.Foreground;
			Color foregroundColor = ColorHelper.ConvertToColor(foregroundOleColor);
			
			Assert.AreEqual(Colors.Red, foregroundColor);
		}
		
		[Test]
		public void Background_BackgroundHighlightingColorIsRed_ReturnsRed()
		{
			CreateColorableItems();
			highlightingColor.Background = Colors.Red;
			
			UInt32 backgroundOleColor = items.Background;
			Color backgroundColor = ColorHelper.ConvertToColor(backgroundOleColor);
			
			Assert.AreEqual(Colors.Red, backgroundColor);
		}
		
		[Test]
		public void Bold_HighlightingColorIsBold_ReturnsTrue()
		{
			CreateColorableItems();
			highlightingColor.Bold = true;
			
			bool bold = items.Bold;
			
			Assert.IsTrue(bold);
		}
		
		[Test]
		public void Bold_HighlightingColorIsNotBold_ReturnsFalse()
		{
			CreateColorableItems();
			highlightingColor.Bold = false;
			
			bool bold = items.Bold;
			
			Assert.IsFalse(bold);
		}
		
		[Test]
		public void Bold_SetBoldToTrue_HighlightingColorIsChangedToBold()
		{
			CreateColorableItems();
			highlightingColor.Bold = false;
			
			items.Bold = true;
			
			Assert.IsTrue(highlightingColor.Bold);
		}
		
		[Test]
		public void Foreground_ForegroundHighlightingColorIsNull_ReturnsWindowsTextSystemColor()
		{
			CreateColorableItems();
			highlightingColor.Foreground = null;
			
			UInt32 foregroundOleColor = items.Foreground;
			Color foregroundColor = ColorHelper.ConvertToColor(foregroundOleColor);
			
			Assert.AreEqual(SystemColors.WindowTextColor, foregroundColor);
		}
		
		[Test]
		public void Background_BackgroundHighlightingColorIsNull_ReturnsWindowSystemColor()
		{
			CreateColorableItems();
			highlightingColor.Background = null;
			
			UInt32 backgroundOleColor = items.Background;
			Color backgroundColor = ColorHelper.ConvertToColor(backgroundOleColor);
			
			Assert.AreEqual(SystemColors.WindowColor, backgroundColor);
		}
				
		[Test]
		public void Foreground_SetForegroundToRed_HighlightingColorIsChangedToRed()
		{
			CreateColorableItems();
			highlightingColor.Foreground = null;
			
			UInt32 oleColor = ColorHelper.ConvertToOleColor(System.Drawing.Color.Red);
			items.Foreground = oleColor;
			Color color = highlightingColor.Foreground.Value;
			
			Assert.AreEqual(Colors.Red, color);
		}
		
		[Test]
		public void Background_SetForegroundToGreen_HighlightingColorIsChangedToGreen()
		{
			CreateColorableItems();
			highlightingColor.Foreground = null;
			
			UInt32 oleColor = ColorHelper.ConvertToOleColor(System.Drawing.Color.Green);
			items.Background = oleColor;
			Color color = highlightingColor.Background.Value;
			
			Assert.AreEqual(Colors.Green, color);
		}
		
		[Test]
		public void Foreground_SetForegroundToBlue_HighlightingColorChangeIsSaved()
		{
			CreateColorableItems();
			
			items.Foreground = ColorHelper.ConvertToOleColor(System.Drawing.Color.Blue);
			
			bool contains = fakeHighlightingRules.ColorsSaved.Contains(highlightingColor);
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void Background_SetForegroundToBlue_HighlightingColorChangeIsSaved()
		{
			CreateColorableItems();
			
			items.Background = ColorHelper.ConvertToOleColor(System.Drawing.Color.Blue);
			
			bool contains = fakeHighlightingRules.ColorsSaved.Contains(highlightingColor);
			
			Assert.IsTrue(contains);
		}
		
		[Test]
		public void Background_BoldIsSetToTrue_HighlightingColorChangeIsSaved()
		{
			CreateColorableItems();
			highlightingColor.Bold = false;
			
			items.Bold = true;
			
			bool contains = fakeHighlightingRules.ColorsSaved.Contains(highlightingColor);
			
			Assert.IsTrue(contains);
		}
	}
}
