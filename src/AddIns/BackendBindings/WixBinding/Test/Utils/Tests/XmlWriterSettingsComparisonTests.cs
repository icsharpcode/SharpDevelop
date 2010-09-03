// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
