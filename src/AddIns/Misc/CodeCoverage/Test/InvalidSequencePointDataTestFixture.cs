// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class InvalidSequencePointDataTestFixture
	{
		CodeCoverageResults results;
		CodeCoverageSequencePoint invalidVisitCountSequencePoint;
		CodeCoverageSequencePoint invalidLineCountSequencePoint;
		CodeCoverageSequencePoint invalidColumnCountSequencePoint;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml = "<PartCoverReport>\r\n" +
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
			
			results = new CodeCoverageResults(new StringReader(xml));
			invalidVisitCountSequencePoint = results.Modules[0].Methods[0].SequencePoints[0];
			invalidLineCountSequencePoint = results.Modules[0].Methods[1].SequencePoints[0];
			invalidColumnCountSequencePoint = results.Modules[0].Methods[2].SequencePoints[0];
		}
		
		[Test]
		public void InvalidVisitCount()
		{
			Assert.AreEqual(0, invalidVisitCountSequencePoint.VisitCount, "Should be set to zero since it is invalid.");
		}

		[Test]
		public void InvalidLineCount()
		{
			Assert.AreEqual(0, invalidLineCountSequencePoint.Line, "Should be set to zero since it is invalid.");
		}
		
		[Test]
		public void InvalidColumnCount()
		{
			Assert.AreEqual(0, invalidColumnCountSequencePoint.Column, "Should be set to zero since it is invalid.");
		}
	}
}
