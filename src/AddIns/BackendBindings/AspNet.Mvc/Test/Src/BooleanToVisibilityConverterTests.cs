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
