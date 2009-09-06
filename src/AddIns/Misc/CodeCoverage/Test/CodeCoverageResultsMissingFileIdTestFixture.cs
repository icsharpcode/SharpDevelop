// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests
{
	/// <summary>
	/// PartCover does not always put a file id in the code
	/// coverage report. We also ensure that a module that has no
	/// associated source code is not included in the final list
	/// of modules.
	/// </summary>
	[TestFixture]
	public class CodeCoverageResultsMissingFileIdTestFixture
	{
		CodeCoverageResults results;
		
		[SetUp]
		public void SetUpFixture()
		{
			string xml = "<PartCoverReport ver=\"1.0.2796.35184\">\r\n" +
				"<File id=\"1\" url=\"c:\\test\\MyTests\\Class1.cs\" />\r\n" +
				"<Assembly id=\"1\" name=\"MyTests.Tests\" module=\"C:\\Projects\\Test\\MyTests\\bin\\MyTests.DLL\" domain=\"test-domain-MyTests.dll\" domainIdx=\"1\" />\r\n" +
				"<Assembly id=\"2\" name=\"nunit.framework\" module=\"C:\\Projects\\Test\\MyTests\\bin\\nunit.framework.DLL\" domain=\"test-domain-nunit.framework.dll\" domainIdx=\"1\" />\r\n" +
				"<Type asmref=\"2\" name=\"NUnit.Framework.NotEqualAsserter\" flags=\"1233408\">\r\n" +
				"  <Method name=\"Fail\" sig=\"bool  ()\" flags=\"134\" iflags=\"0\">\r\n" +
				"    <pt visit=\"1\" pos=\"11\" len=\"1\" />\r\n" +
				"    <pt visit=\"0\" pos=\"6\" len=\"5\" />\r\n" +
				"    <pt visit=\"0\" pos=\"0\" len=\"6\" />\r\n" +
				"  </Method>\r\n" +
				"</Type>\r\n" +
				"<Type asmref=\"1\" name=\"MyClass\" flags=\"1233248\">\r\n" +
				"  <Method name=\".ctor\" sig=\"void  ()\" flags=\"6278\" iflags=\"0\">\r\n" +
				"    <pt visit=\"0\" pos=\"8\" len=\"2\" fid=\"1\" sl=\"21\" sc=\"3\" el=\"21\" ec=\"4\" />\r\n" +
				"    <pt visit=\"0\" pos=\"7\" len=\"1\" fid=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"    <pt visit=\"0\" pos=\"0\" len=\"7\" fid=\"1\" sl=\"19\" sc=\"3\" el=\"19\" ec=\"18\" />\r\n" +
				"    <pt visit=\"0\" pos=\"0\" len=\"7\" sl=\"19\" sc=\"3\" el=\"19\" ec=\"18\" />\r\n" +
				"  </Method>\r\n" +
				"</Type>\r\n" +
				"</PartCoverReport>";
		
			results = new CodeCoverageResults(new StringReader(xml));
		}
		
		[Test]
		public void OneModule()
		{
			Assert.AreEqual(1, results.Modules.Count);
		}
		
		[Test]
		public void ModuleName()
		{
			Assert.AreEqual("MyTests.Tests", results.Modules[0].Name);
		}
		
		[Test]
		public void MyTestsCtorHasThreeSequencePoints()
		{
			Assert.AreEqual(3, results.Modules[0].Methods[0].SequencePoints.Count);
		}
	}
}
