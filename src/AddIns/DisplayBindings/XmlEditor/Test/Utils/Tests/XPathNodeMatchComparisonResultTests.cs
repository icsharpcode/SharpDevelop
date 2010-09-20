// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using XmlEditor.Tests.Utils;
using NUnit.Framework;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class XPathNodeMatchComparisonResultTests
	{
		[Test]
		public void ComparisonResultsAreEqualWhenResultAndMessageAreSame()
		{
			XPathNodeMatchComparisonResult lhs = new XPathNodeMatchComparisonResult(true, "abc");
			XPathNodeMatchComparisonResult rhs = new XPathNodeMatchComparisonResult(true, "abc");
			Assert.AreEqual(lhs, rhs);
		}
		
		[Test]
		public void ComparisonResultsAreNotEqualWhenResultIsDifferent()
		{
			XPathNodeMatchComparisonResult lhs = new XPathNodeMatchComparisonResult(false, "abc");
			XPathNodeMatchComparisonResult rhs = new XPathNodeMatchComparisonResult(true, "abc");
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void ComparisonResultsAreNotEqualWhenMessageIsDifferent()
		{
			XPathNodeMatchComparisonResult lhs = new XPathNodeMatchComparisonResult(false, "aaa");
			XPathNodeMatchComparisonResult rhs = new XPathNodeMatchComparisonResult(false, "bbb");
			Assert.AreNotEqual(lhs, rhs);
		}
		
		[Test]
		public void ComparisonResultToString()
		{
			XPathNodeMatchComparisonResult lhs = new XPathNodeMatchComparisonResult(true, "message");
			Assert.AreEqual("Result: True, Message: 'message'", lhs.ToString());
		}
		
		[Test]
		public void ComparisonResultEqualsReturnsFalseForNull()
		{
			XPathNodeMatchComparisonResult result = new XPathNodeMatchComparisonResult();
			Assert.IsFalse(result.Equals(null));
		}
	}
}
