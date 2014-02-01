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
