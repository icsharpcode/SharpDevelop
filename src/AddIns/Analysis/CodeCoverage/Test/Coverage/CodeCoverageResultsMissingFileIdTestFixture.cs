// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// PartCover does not always put a file id in the code
	/// coverage report. This is typically for code that 
	/// has no source code. It can also be for code that
	/// is in a method that is not covered at all.
	/// </summary>
	[TestFixture]
	public class CodeCoverageResultsMissingFileIdTestFixture : CodeCoverageResultsTestsBase
	{
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

			base.CreateCodeCoverageResults(xml);
		}
		
		[Test]
		public void Modules_ModuleReadFromResults_HasTwoModules()
		{
			int count = results.Modules.Count;
			Assert.AreEqual(2, count);
		}
		
		[Test]
		public void ModulesName_FirstModuleNameReadFromResults_IsNUnitFrameworkAssembly()
		{
			string name = FirstModule.Name;
			string expectedName = "nunit.framework";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void ModulesName_SecondModuleNameReadFromResults_IsMyTestsAssembly()
		{
			string name = SecondModule.Name;
			string expectedName = "MyTests.Tests";
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void SequencePointsCount_NUnitNotEqualAssertFailMethod_ReturnsAllSequencePoints()
		{
			int sequencePointCount = FirstModuleFirstMethod.SequencePoints.Count;
			int expectedSequencePointCount = 3;
			Assert.AreEqual(expectedSequencePointCount, sequencePointCount);
		}
		
		[Test]
		public void SequencePointsCount_MyClassConstructorHasFourSequencePointsWithOneMissingFileId_ReturnsAllSequencePoints()
		{
			int sequencePointCount = SecondModule.Methods[0].SequencePoints.Count;
			int expectedSequencePointCount = 4;
			Assert.AreEqual(expectedSequencePointCount, sequencePointCount);
		}
	}
}
