// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class InvalidSequencePointDataTestFixture : CodeCoverageResultsTestsBase
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml = 
				"<PartCoverReport>\r\n" +
				"\t<File id=\"1\" url=\"c:\\Projects\\Foo\\SimpleTestFixture.cs\"/>\r\n" +
				"\t<Assembly id=\"1\" name=\"Foo.Tests\" module=\"C:\\Projects\\Test\\bin\\Foo.Tests.DLL\" domain=\"test-domain-Foo.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"\t<Type name=\"Foo.Tests.FooTestFixture\" asmref=\"1\">\r\n" +
				"\t\t<Method name=\"InvalidVisitCount\">\r\n" +
				"\t\t\t<pt visit=\"a\" fid=\"1\" sl=\"10\" sc=\"3\" el=\"20\" ec=\"4\"/>\r\n" +
				"\t\t</Method>\r\n" +
				"\t\t<Method name=\"InvalidLine\">\r\n" +
				"\t\t\t<pt visit=\"2\" fid=\"1\" sl=\"b\" sc=\"3\" el=\"20\" ec=\"4\"/>\r\n" +
				"\t\t</Method>\r\n" +
				"\t\t<Method name=\"InvalidColumn\">\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"20\" sc=\"c\" el=\"d\" ec=\"e\"/>\r\n" +
				"\t\t</Method>\r\n" +
				"\t</Type>\r\n" +
				"</PartCoverReport>";
			
			base.CreateCodeCoverageResults(xml);
		}
		
		CodeCoverageSequencePoint InvalidVisitCountSequencePoint {
			get { return FirstModuleFirstMethod.SequencePoints[0]; }
		}
		
		CodeCoverageSequencePoint InvalidLineCountSequencePoint {
			get { return FirstModuleSecondMethod.SequencePoints[0]; }
		}

		CodeCoverageSequencePoint InvalidColumnCountSequencePoint {
			get { return FirstModule.Methods[2].SequencePoints[0]; }
		}

		[Test]
		public void SequencePointVisitCount_InvalidVisitCount_ReturnsZero()
		{
			int count = InvalidVisitCountSequencePoint.VisitCount;
			Assert.AreEqual(0, count, "Should be set to zero since it is invalid.");
		}

		[Test]
		public void SequencePointLine_InvalidLineCount_ReturnsZero()
		{
			int line = InvalidLineCountSequencePoint.Line;
			Assert.AreEqual(0, line, "Should be set to zero since it is invalid.");
		}
		
		[Test]
		public void SequencePointColumn_InvalidColumnCount_ReturnsZero()
		{
			int col = InvalidColumnCountSequencePoint.Column;
			Assert.AreEqual(0, col, "Should be set to zero since it is invalid.");
		}
	}
}
