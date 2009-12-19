// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
