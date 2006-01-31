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
			string xml = "<coverage>\r\n" +
				"\t<module name=\"C:\\Projects\\Foo.Tests\\bin\\Debug\\Foo.Tests.dll\" assembly=\"Foo.Tests\">\r\n" +
				"\t\t<method name=\"SimpleTest\" class=\"Foo.Tests.FooTestFixture\">\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"20\" column=\"3\" endline=\"20\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"1\" line=\"21\" column=\"13\" endline=\"21\" endcolumn=\"32\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t\t<seqpnt visitcount=\"0\" line=\"24\" column=\"3\" endline=\"24\" endcolumn=\"4\" document=\"c:\\Projects\\Foo\\FooTestFixture.cs\" />\r\n" +
				"\t\t</method>\r\n" +
				"\t</module>\r\n" +
				"</coverage>";
			results = new CodeCoverageResults(new StringReader(xml));
			module = results.Modules[0];
			method = module.Methods[0];
			
			point1 = method.SequencePoints[0];
			point2 = method.SequencePoints[1];
			point3 = method.SequencePoints[2];
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
