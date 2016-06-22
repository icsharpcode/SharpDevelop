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
using ICSharpCode.PackageManagement;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class BooleanToFontWeightConverterTests
	{
		BooleanToFontWeightConverter converter;
		
		[SetUp]
		public void Init()
		{
			converter = new BooleanToFontWeightConverter();
		}
		
		object Convert(object value)
		{
			return converter.Convert(value, typeof(FontWeight), null, CultureInfo.InvariantCulture);
		}
		
		object ConvertBack(object value)
		{
			return converter.ConvertBack(value, typeof(Boolean), null, CultureInfo.InvariantCulture);
		}
		
		[Test]
		public void Convert_BooleanTruePassed_ReturnsFontWeightBold()
		{
			FontWeight result = (FontWeight)Convert(true);
			Assert.AreEqual(FontWeights.Bold, result);
		}
				
		[Test]
		public void Convert_BooleanFalsePassed_ReturnsFontWeightNormal()
		{
			FontWeight result = (FontWeight)Convert(false);
			Assert.AreEqual(FontWeights.Normal, result);
		}
		
		[Test]
		public void Convert_NullObjectPassed_ReturnsUnsetValue()
		{
			object result = Convert(null);
			Assert.AreEqual(DependencyProperty.UnsetValue, result);
		}
		
		[Test]
		public void ConvertBack_NullObjectPassed_ReturnsUnsetValue()
		{
			object result = ConvertBack(null);
			Assert.AreEqual(DependencyProperty.UnsetValue, result);
		}
		
		[Test]
		public void ConvertBack_FontWeightBoldPassed_ReturnsTrue()
		{
			bool result = (bool)ConvertBack(FontWeights.Bold);
			Assert.IsTrue(result);
		}
				
		[Test]
		public void ConvertBack_FontWeightNormalPassed_ReturnsFalse()
		{
			bool result = (bool)ConvertBack(FontWeights.Normal);
			Assert.IsFalse(result);
		}
	}
}
