// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.Windows;

using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class BooleanToVisibilityConverterTests
	{
		BooleanToVisibilityConverter converter;
		
		void CreateConverter()
		{
			converter = new BooleanToVisibilityConverter();
		}
		
		Visibility Convert(bool value)
		{
			return (Visibility)converter.Convert(value, null, null, CultureInfo.InvariantCulture);
		}
		
		bool ConvertBack(Visibility visibility)
		{
			return (bool)converter.ConvertBack(visibility, null, null, CultureInfo.InvariantCulture);
		}
		
		[Test]
		public void Convert_FalseWithDefaultBehaviour_ReturnsCollapsed()
		{
			CreateConverter();
			Visibility visibility = Convert(false);
			
			Assert.AreEqual(Visibility.Collapsed, visibility);
		}
		
		[Test]
		public void Convert_TrueWithDefaultBehaviour_ReturnsVisible()
		{
			CreateConverter();
			Visibility visibility = Convert(true);
			
			Assert.AreEqual(Visibility.Visible, visibility);
		}
		
		[Test]
		public void Convert_FalseWithHiddenSetToTrue_ReturnsHidden()
		{
			CreateConverter();
			converter.Hidden = true;
			Visibility visibility = Convert(false);
			
			Assert.AreEqual(Visibility.Hidden, visibility);
		}
		
		[Test]
		public void ConvertBack_VisibleWithDefaultBehaviour_ReturnsTrue()
		{
			CreateConverter();
			bool value = ConvertBack(Visibility.Visible);
			
			Assert.IsTrue(value);
		}
		
		[Test]
		public void ConvertBack_HiddenWithDefaultBehaviour_ReturnsFalse()
		{
			CreateConverter();
			bool value = ConvertBack(Visibility.Hidden);
			
			Assert.IsFalse(value);
		}
		
		[Test]
		public void ConvertBack_CollapsedWithDefaultBehaviour_ReturnsFalse()
		{
			CreateConverter();
			bool value = ConvertBack(Visibility.Collapsed);
			
			Assert.IsFalse(value);
		}
		
		[Test]
		public void Convert_FalseWithIsReversedSetToTrue_ReturnsVisible()
		{
			CreateConverter();
			converter.IsReversed = true;
			Visibility visibility = Convert(false);
			
			Assert.AreEqual(Visibility.Visible, visibility);
		}
		
		[Test]
		public void Convert_TrueWithIsReversedSetToTrue_ReturnsCollapsed()
		{
			CreateConverter();
			converter.IsReversed = true;
			Visibility visibility = Convert(true);
			
			Assert.AreEqual(Visibility.Collapsed, visibility);
		}
		
		[Test]
		public void ConvertBack_VisibleWithIsReversedSetToTrue_ReturnsFalse()
		{
			CreateConverter();
			converter.IsReversed = true;
			bool value = ConvertBack(Visibility.Visible);
			
			Assert.IsFalse(value);
		}
		
		[Test]
		public void ConvertBack_CollapsedWithIsReversedSetToTrue_ReturnsTrue()
		{
			CreateConverter();
			converter.IsReversed = true;
			bool value = ConvertBack(Visibility.Collapsed);
			
			Assert.IsTrue(value);
		}
	}
}
