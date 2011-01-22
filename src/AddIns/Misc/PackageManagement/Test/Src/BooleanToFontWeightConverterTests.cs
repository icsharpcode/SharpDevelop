// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
