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
				"\t<file id=\"1\" url=\"c:\\Projects\\Foo\\SimpleTestFixture.cs\"/>\r\n" +
				"\t<type name=\"Foo.Tests.FooTestFixture\" asm=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"InvalidVisitCount\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t<pt visit=\"a\" fid=\"1\" sl=\"10\" sc=\"3\" el=\"20\" ec=\"4\"/>\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t\t<method name=\"InvalidLine\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t<pt visit=\"2\" fid=\"1\" sl=\"b\" sc=\"3\" el=\"20\" ec=\"4\"/>\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t\t<method name=\"InvalidColumn\">\r\n" +
				"\t\t\t<code>\r\n" +
				"\t\t\t<pt visit=\"1\" fid=\"1\" sl=\"20\" sc=\"c\" el=\"d\" ec=\"e\"/>\r\n" +
				"\t\t\t</code>\r\n" +
				"\t\t</method>\r\n" +
				"\t</type>\r\n" +
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
