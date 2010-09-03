// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml.XPath;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using XmlEditor.Tests.Utils;

namespace XmlEditor.Tests.Utils.Tests
{
	[TestFixture]
	public class XPathNodeMatchComparisonTests
	{
		[Test]
		public void ResultEqualsReturnsTrueWhenXPathNodesAreSame()
		{
			XPathNodeMatch lhs = new XPathNodeMatch("nodeValue", "DisplayValue", 1, 2, XPathNodeType.Text);
			XPathNodeMatch rhs = new XPathNodeMatch("nodeValue", "DisplayValue", 1, 2, XPathNodeType.Text);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			
			XPathNodeMatchComparisonResult result = new XPathNodeMatchComparisonResult();
			result.Result = comparison.AreEqual(lhs, rhs);
			result.Message = comparison.GetReasonForNotMatching();
			
			XPathNodeMatchComparisonResult expectedResult = new XPathNodeMatchComparisonResult(true, String.Empty);
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void ResultEqualsReturnsFalseWhenXPathNodeDisplayValueAreDifferent()
		{
			XPathNodeMatch lhs = new XPathNodeMatch("nodeValue", "DisplayValue1", 1, 2, XPathNodeType.Text);
			XPathNodeMatch rhs = new XPathNodeMatch("nodeValue", "DisplayValue2", 1, 2, XPathNodeType.Text);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			
			XPathNodeMatchComparisonResult result = new XPathNodeMatchComparisonResult();
			result.Result = comparison.AreEqual(lhs, rhs);
			result.Message = comparison.GetReasonForNotMatching();
			
			string expectedReason = "DisplayValues do not match. Expected 'DisplayValue1' but was 'DisplayValue2'.";
			XPathNodeMatchComparisonResult expectedResult = new XPathNodeMatchComparisonResult(false, expectedReason);
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void ResultEqualsReturnsFalseWhenXPathNodeLineNumbersAreDifferent()
		{
			XPathNodeMatch lhs = new XPathNodeMatch("nodeValue", "DisplayValue", 1, 2, XPathNodeType.Text);
			XPathNodeMatch rhs = new XPathNodeMatch("nodeValue", "DisplayValue", 3, 2, XPathNodeType.Text);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			
			XPathNodeMatchComparisonResult result = new XPathNodeMatchComparisonResult();
			result.Result = comparison.AreEqual(lhs, rhs);
			result.Message = comparison.GetReasonForNotMatching();
			
			string expectedReason = "LineNumbers do not match. Expected '1' but was '3'.";
			XPathNodeMatchComparisonResult expectedResult = new XPathNodeMatchComparisonResult(false, expectedReason);
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void ResultEqualsReturnsFalseWhenXPathNodeValuesAreDifferent()
		{
			XPathNodeMatch lhs = new XPathNodeMatch("nodeValue1", "DisplayValue", 1, 2, XPathNodeType.Text);
			XPathNodeMatch rhs = new XPathNodeMatch("nodeValue2", "DisplayValue", 1, 2, XPathNodeType.Text);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			
			XPathNodeMatchComparisonResult result = new XPathNodeMatchComparisonResult();
			result.Result = comparison.AreEqual(lhs, rhs);
			result.Message = comparison.GetReasonForNotMatching();
			
			string expectedReason = "Values do not match. Expected 'nodeValue1' but was 'nodeValue2'.";
			XPathNodeMatchComparisonResult expectedResult = new XPathNodeMatchComparisonResult(false, expectedReason);
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void ResultEqualsReturnsFalseWhenXPathNodeLinePositionsAreDifferent()
		{
			XPathNodeMatch lhs = new XPathNodeMatch("nodeValue", "DisplayValue", 1, 2,  XPathNodeType.Text);
			XPathNodeMatch rhs = new XPathNodeMatch("nodeValue", "DisplayValue", 1, 3, XPathNodeType.Text);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			
			XPathNodeMatchComparisonResult result = new XPathNodeMatchComparisonResult();
			result.Result = comparison.AreEqual(lhs, rhs);
			result.Message = comparison.GetReasonForNotMatching();
			
			string expectedReason = "LinePositions do not match. Expected '2' but was '3'.";
			XPathNodeMatchComparisonResult expectedResult = new XPathNodeMatchComparisonResult(false, expectedReason);
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void ResultEqualsReturnsFalseWhenXPathNodeNodeTypesAreDifferent()
		{
			XPathNodeMatch lhs = new XPathNodeMatch("nodeValue", "DisplayValue", 1, 2,  XPathNodeType.Text);
			XPathNodeMatch rhs = new XPathNodeMatch("nodeValue", "DisplayValue", 1, 2, XPathNodeType.Element);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			
			XPathNodeMatchComparisonResult result = new XPathNodeMatchComparisonResult();
			result.Result = comparison.AreEqual(lhs, rhs);
			result.Message = comparison.GetReasonForNotMatching();
			
			string expectedReason = "NodeTypes do not match. Expected 'Text' but was 'Element'.";
			XPathNodeMatchComparisonResult expectedResult = new XPathNodeMatchComparisonResult(false, expectedReason);
			Assert.AreEqual(expectedResult, result);
		}
		
		[Test]
		public void ResultEqualsReturnsFalseWhenOneXPathNodeLineNumberIsNull()
		{
			int? lineNumber = null;
			XPathNodeMatch lhs = new XPathNodeMatch("nodeValue", "DisplayValue", lineNumber, 2,  XPathNodeType.Text);
			XPathNodeMatch rhs = new XPathNodeMatch("nodeValue", "DisplayValue", 0, 2, XPathNodeType.Text);
			XPathNodeMatchComparison comparison = new XPathNodeMatchComparison();
			
			XPathNodeMatchComparisonResult result = new XPathNodeMatchComparisonResult();
			result.Result = comparison.AreEqual(lhs, rhs);
			result.Message = comparison.GetReasonForNotMatching();
			
			string expectedReason = "LineNumbers do not match. Expected 'null' but was '0'.";
			XPathNodeMatchComparisonResult expectedResult = new XPathNodeMatchComparisonResult(false, expectedReason);
			Assert.AreEqual(expectedResult, result);
		}
	}
}
