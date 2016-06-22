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
using System.Xml;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class XmlWriterSettingsComparisonTests
	{
		XmlWriterSettings lhs;
		XmlWriterSettings rhs;
		XmlWriterSettingsComparison comparison;
		
		[SetUp]
		public void Init()
		{
			lhs = new XmlWriterSettings();
			rhs = new XmlWriterSettings();
			comparison = new XmlWriterSettingsComparison();
		}
		
		[Test]
		public void XmlWriterSettingsAreEqualByDefault()
		{
			Assert.IsTrue(comparison.AreEqual(lhs, rhs));
		}
		
		[Test]
		public void AreEqualReturnsFalseWhenXmlWriterSettingsCloseOutputAreDifferent()
		{
			lhs.CloseOutput = true;
			rhs.CloseOutput = false;
			Assert.IsFalse(comparison.AreEqual(lhs, rhs));
		}
		
		[Test]
		public void PropertyNameIsCloseOutputWhenXmlWriterSettingsCloseOutputAreDifferent()
		{
			AreEqualReturnsFalseWhenXmlWriterSettingsCloseOutputAreDifferent();
			Assert.AreEqual("CloseOutput", comparison.PropertyName);
		}		
		
		[Test]
		public void AreEqualReturnsFalseWhenXmlWriterSettingsNewLineCharsAreDifferent()
		{
			lhs.NewLineChars = "a";
			rhs.NewLineChars = "b";
			Assert.IsFalse(comparison.AreEqual(lhs, rhs));
		}
		
		[Test]
		public void PropertyNameIsNewLineCharsWhenXmlWriterSettingsNewLineCharsAreDifferent()
		{
			AreEqualReturnsFalseWhenXmlWriterSettingsNewLineCharsAreDifferent();
			Assert.AreEqual("NewLineChars", comparison.PropertyName);
		}
		
		[Test]
		public void ComparisonToStringHasNewLineCharsWhenXmlWriterSettingsNewLineCharsAreDifferent()
		{
			AreEqualReturnsFalseWhenXmlWriterSettingsNewLineCharsAreDifferent();
			Assert.AreEqual("Property NewLineChars is different.", comparison.ToString());
		}
		
		[Test]
		public void ComparisonToStringIsEmptyStringWhenXmlWriterSettingsAreSame()
		{
			Assert.AreEqual(String.Empty, comparison.ToString());
		}
		
		[Test]
		public void AreEqualReturnsFalseIfRhsIsNull()
		{
			Assert.IsFalse(comparison.AreEqual(lhs, null));
		}
		
		[Test]
		public void AreEqualReturnsFalseIfLhsIsNull()
		{
			Assert.IsFalse(comparison.AreEqual(null, rhs));
		}
		
		[Test]
		public void ComparisonToStringHasNullComparisionFailureWhenAreEqualRhsIsNull()
		{
			AreEqualReturnsFalseIfRhsIsNull();
			Assert.AreEqual("XmlWriterSetting is null.", comparison.ToString());
		}
	}
}
