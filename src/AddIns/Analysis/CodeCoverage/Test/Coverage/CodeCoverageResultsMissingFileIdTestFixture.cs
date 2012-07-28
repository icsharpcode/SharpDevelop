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
			string xml = "<CoverageSession xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
				"\t<Modules>\r\n" +
				"\t\t<Module hash=\"44-54-B6-13-97-49-45-F8-6A-74-9E-49-0C-77-87-C6-9C-54-47-7B\">\r\n" +
				"\t\t\t<FullName>C:\\Projects\\Test\\MyTests\\bin\\nunit.framework.DLL</FullName>\r\n" +
				"\t\t\t<ModuleName>nunit.framework</ModuleName>\r\n" +
				"\t\t\t<Classes>\r\n" +
				"\t\t\t\t<Class>\r\n" +
				"\t\t\t\t\t<FullName>NUnit.Framework.NotEqualAsserter</FullName>\r\n" +
				"\t\t\t\t\t<Methods>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Boolean NUnit.Framework.NotEqualAsserter::Fail()</Name>\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"1\" sl=\"21\" sc=\"3\" el=\"21\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"19\" sc=\"3\" el=\"19\" ec=\"18\" />\r\n" +
				"\t\t\t\t\t\t\t</SequencePoints>\r\n" +
				"\t\t\t\t\t\t</Method>\r\n" +
				"\t\t\t\t\t</Methods>\r\n" +
				"\t\t\t\t</Class>\r\n" +
				"\t\t\t</Classes>\r\n" +
				"\t\t</Module>\r\n" +
				"\t\t<Module hash=\"44-54-B6-13-97-49-45-F8-6A-74-9E-49-0C-77-87-C6-9C-54-47-7A\">\r\n" +
				"\t\t\t<FullName>C:\\Projects\\Test\\MyTests\\bin\\MyTests.DLL</FullName>\r\n" +
				"\t\t\t<ModuleName>MyTests.Tests</ModuleName>\r\n" +
				"\t\t\t<Files>\r\n" +
				"\t\t\t\t<File uid=\"1\" fullPath=\"c:\\test\\MyTests\\Class1.cs\" />\r\n" +
				"\t\t\t</Files>\r\n" +
				"\t\t\t<Classes>\r\n" +
				"\t\t\t\t<Class>\r\n" +
				"\t\t\t\t\t<FullName>MyTests.Tests.MyClass</FullName>\r\n" +
				"\t\t\t\t\t<Methods>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Void MyTests.Tests.MyClass::.ctor()</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"1\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"21\" sc=\"3\" el=\"21\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"19\" sc=\"3\" el=\"19\" ec=\"18\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"19\" sc=\"3\" el=\"19\" ec=\"18\" />\r\n" +
				"\t\t\t\t\t\t\t</SequencePoints>\r\n" +
				"\t\t\t\t\t\t</Method>\r\n" +
				"\t\t\t\t\t</Methods>\r\n" +
				"\t\t\t\t</Class>\r\n" +
				"\t\t\t</Classes>\r\n" +
				"\t\t</Module>\r\n" +
				"\t</Modules>\r\n" +
				"</CoverageSession>";

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
