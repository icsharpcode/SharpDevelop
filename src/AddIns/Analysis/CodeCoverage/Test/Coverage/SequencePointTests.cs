// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class SequencePointTests : CodeCoverageResultsTestsBase
	{
		CodeCoverageSequencePoint lhs;
		CodeCoverageSequencePoint rhs;
		
		[SetUp]
		public void Init()
		{
			lhs = base.CreateSequencePoint();
			rhs = base.CreateSequencePoint();
		}
		
		[Test]
		public void Equals_HaveSameValuesForAllProperties_ReturnsTrue()
		{
			AssertSequencePointsAreEqual();
		}
		
		void AssertSequencePointsAreEqual()
		{
			bool result = Compare();
			Assert.IsTrue(result);
		}
		
		void AssertSequencePointsAreNotEqual()
		{
			bool result = Compare();
			Assert.IsFalse(result);
		}
		
		bool Compare()
		{
			return lhs.Equals(rhs);
		}
		
		[Test]
		public void Equals_NullParameterPassed_ReturnsFalse()
		{
			bool result = lhs.Equals(null);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Equals_DocumentsAreDifferent_ReturnsFalse()
		{
			rhs.Document = "abc";
			AssertSequencePointsAreNotEqual();
		}
		
		[Test]
		public void Equals_ColumnsAreDifferent_ReturnsFalse()
		{
			rhs.Column = 4;
			AssertSequencePointsAreNotEqual();
		}
		
		[Test]
		public void Equals_EndColumnsAreDifferent_ReturnsFalse()
		{
			rhs.EndColumn = 4;
			AssertSequencePointsAreNotEqual();
		}
		
		[Test]
		public void Equals_EndLinesAreDifferent_ReturnsFalse()
		{
			rhs.EndLine = 4;
			AssertSequencePointsAreNotEqual();
		}
		
		[Test]
		public void Equals_LinesAreDifferent_ReturnsFalse()
		{
			rhs.Line = 4;
			AssertSequencePointsAreNotEqual();
		}
		
		[Test]
		public void Equals_VisitCountsAreDifferent_ReturnsFalse()
		{
			rhs.VisitCount = 4;
			AssertSequencePointsAreNotEqual();
		}
		
		[Test]
		public void Equals_LengthsAreDifferent_ReturnsFalse()
		{
			rhs.Length = 4;
			AssertSequencePointsAreNotEqual();
		}
		
		[Test]
		public void ToString_HasDocument_ReturnsExpectedStringContainingAllPropertyValues()
		{
			lhs.Document = "test";
			lhs.VisitCount = 2;
			lhs.Column = 1;
			lhs.Line = 3;
			lhs.EndLine = 4;
			lhs.EndColumn = 5;
			lhs.Length = 6;
			
			string toString = lhs.ToString();
			string expectedString = "Document: 'test' VisitCount: 2 Line: 3 Col: 1 EndLine: 4 EndCol: 5 Length: 6";
			
			Assert.AreEqual(expectedString, toString);
		}
		
		[Test]
		public void HasDocument_WhenDocumentIsNull_ReturnsFalse()
		{
			lhs.Document = null;
			bool result = lhs.HasDocument();
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasDocument_WhenDocumentIsEmptyString_ReturnsFalse()
		{
			lhs.Document = String.Empty;
			bool result = lhs.HasDocument();
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasDocument_WhenDocumentIsNotEmptyString_ReturnsTrue()
		{
			lhs.Document = "abc";
			bool result = lhs.HasDocument();
			Assert.IsTrue(result);
		}
	}
}
