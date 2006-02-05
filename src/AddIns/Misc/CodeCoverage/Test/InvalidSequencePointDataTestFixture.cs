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
			string xml = "<coverage>\r\n" +
				"\t<module name=\"C:\\Projects\\Foo.Tests\\bin\\Debug\\Foo.Tests.dll\" assembly=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"InvalidVisitCount\" class=\"Foo.Tests.FooTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"a\" line=\"10\" column=\"3\" endline=\"20\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t\t<method name=\"InvalidLine\" class=\"Foo.Tests.FooTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"2\" line=\"b\" column=\"3\" endline=\"20\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\SimpleTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t\t<method name=\"InvalidColumn\" class=\"Foo.Tests.FooTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"20\" column=\"c\" endline=\"d\" endcolumn=\"e\" document=\"c:\\Projects\\Foo\\SimpleTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t</module>\r\n" +
				"</coverage>";
			
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
