// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.IO;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class CodeCoverageResultsTestFixture
	{
		CodeCoverageModule module;
		CodeCoverageResults results;
		CodeCoverageMethod method;
		CodeCoverageSequencePoint point1;
		CodeCoverageSequencePoint point2;
		CodeCoverageSequencePoint point3;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml = "<PartCoverReport date=\"2008-07-10T02:59:13.7198656+01:00\">\r\n" +
				"    <File id=\"1\" url=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"    <Assembly id=\"1\" name=\"Foo.Tests\" module=\"C:\\Projects\\Test\\Foo.Tests\\bin\\Foo.Tests.DLL\" domain=\"test-domain-Foo.Tests.dll\" domainIdx=\"1\" />\r\n" +
				"    <Type asmref=\"1\" name=\"Foo.Tests.FooTestFixture\" flags=\"1232592\">\r\n" +
				"        <Method name=\"SimpleTest\" sig=\"void  ()\" bodysize=\"42\" flags=\"2182\" iflags=\"0\">\r\n" +
				"            <pt visit=\"1\" pos=\"0\" len=\"1\" fid=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"            <pt visit=\"1\" pos=\"0\" len=\"1\" fid=\"1\" sl=\"21\" sc=\"13\" el=\"21\" ec=\"32\" />\r\n" +
				"            <pt visit=\"0\" pos=\"0\" len=\"1\" fid=\"1\" sl=\"24\" sc=\"3\" el=\"24\" ec=\"4\" />\r\n" +
				"        </Method>\r\n" +
				"    </Type>\r\n" +
				"</PartCoverReport>";
			results = new CodeCoverageResults(new StringReader(xml));
			if (results.Modules.Count > 0) {
				module = results.Modules[0];
				if (module.Methods.Count > 0) {
					method = module.Methods[0];
					if (method.SequencePoints.Count == 3) {
						point1 = method.SequencePoints[0];
						point2 = method.SequencePoints[1];
						point3 = method.SequencePoints[2];
					}
				}			
			}
		}
		
		[Test]
		public void AssemblyName()
		{
			Assert.AreEqual("Foo.Tests", module.Name);
		}
		
		[Test]
		public void ModuleCount()
		{
			Assert.AreEqual(1, results.Modules.Count);
		}
		
		[Test]
		public void MethodCount()
		{
			Assert.AreEqual(1, module.Methods.Count);
		}
		
		[Test]
		public void MethodName()
		{
			Assert.AreEqual("SimpleTest", method.Name);
		}
		
		[Test]
		public void FullClassName()
		{
			Assert.AreEqual("Foo.Tests.FooTestFixture", method.FullClassName);
		}
		
		[Test]
		public void ClassName()
		{
			Assert.AreEqual("FooTestFixture", method.ClassName);
		}
		
		[Test]
		public void ClassNamespace()
		{
			Assert.AreEqual("Foo.Tests", method.ClassNamespace);
		}
		
		[Test]
		public void SequencePointCount()
		{
			Assert.AreEqual(3, method.SequencePoints.Count);
		}
		
		[Test]
		public void SequencePointDocument()
		{
			Assert.AreEqual("c:\\Projects\\Foo\\FooTestFixture.cs", point1.Document);
		}
		
		[Test]
		public void SequencePoint1VisitCount()
		{
			Assert.AreEqual(1, point1.VisitCount);
		}
		
		[Test]
		public void SequencePoint3VisitCount()
		{
			Assert.AreEqual(0, point3.VisitCount);
		}
		
		[Test]
		public void SequencePoint1Line()
		{
			Assert.AreEqual(20, point1.Line);
		}
		
		[Test]
		public void SequencePoint1Column()
		{
			Assert.AreEqual(3, point1.Column);
		}
		
		[Test]
		public void SequencePoint1EndLine()
		{
			Assert.AreEqual(20, point1.EndLine);
		}
		
		[Test]
		public void SequencePoint1EndColumn()
		{
			Assert.AreEqual(4, point1.EndColumn);
		}
		
		[Test]
		public void MethodVisitedCount()
		{
			Assert.AreEqual(2, method.VisitedSequencePointsCount);
		}
		
		[Test]
		public void MethodNotVisitedCount()
		{
			Assert.AreEqual(1, method.NotVisitedSequencePointsCount);
		}
	}
}
