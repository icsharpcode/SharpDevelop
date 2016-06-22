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
