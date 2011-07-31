// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using ICSharpCode.PackageManagement;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class StringToStringCollectionConverterTests
	{
		StringToStringCollectionConverter converter;
		
		[SetUp]
		public void Init()
		{
			converter = new StringToStringCollectionConverter();
		}
		
		object ConvertBack(object value)
		{
			return converter.ConvertBack(value, null, null, CultureInfo.InvariantCulture);
		}
		
		object Convert(object value)
		{
			return converter.Convert(value, null, null, CultureInfo.InvariantCulture);
		}
		
		[Test]
		public void ConvertBack_ObjectPassed_ReturnsUnsetValue()
		{
			object result = ConvertBack(null);
			Assert.AreEqual(DependencyProperty.UnsetValue, result);
		}
		
		[Test]
		public void Convert_StringArrayPassed_ReturnsIEnumerableStrings()
		{
			IEnumerable<string> value = new string[] { "a", "b" };
			IEnumerable<string> result = Convert(value) as IEnumerable<string>;
			
			Assert.IsNotNull(result);
			CollectionAssert.AreEqual(value, result);
		}
		
		[Test]
		public void Convert_StringPassed_ReturnsIEnumerableStrings()
		{
			string value = "a";
			IEnumerable<string> result = Convert(value) as IEnumerable<string>;
			
			string[] expectedResult = { "a" };
			CollectionAssert.AreEqual(expectedResult, result);
		}
	}
}
