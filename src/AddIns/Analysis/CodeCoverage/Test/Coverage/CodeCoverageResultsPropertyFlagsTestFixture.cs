// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	/// <summary>
	/// Tests that property methods (e.g. get_SomeValue) are identified by the CodeCoverageResults class
	/// via the flags attribute in the code coverage results file.
	/// </summary>
	[TestFixture]
	public class CodeCoverageResultsPropertyFlagsTestFixture : CodeCoverageResultsTestsBase
	{
		[SetUp]
		public void SetUpFixture()
		{
			string xml = 
				"<CoverageSession xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">\r\n" +
				"\t<Modules>\r\n" +
				"\t\t<Module hash=\"44-54-B6-13-97-49-45-F8-6A-74-9E-49-0C-77-87-C6-9C-54-47-7B\">\r\n" +
				"\t\t\t<FullName>C:\\Projects\\Test\\MyTests\\bin\\MyTests.DLL</FullName>\r\n" +
				"\t\t\t<ModuleName>MyTests</ModuleName>\r\n" +
				"\t\t\t<Files>\r\n" +
				"\t\t\t\t<File uid=\"1\" fullPath=\"d:\\Projects\\test\\TestFixture1.cs\" />\r\n" +
				"\t\t\t</Files>\r\n" +
				"\t\t\t<Classes>\r\n" +
				"\t\t\t\t<Class>\r\n" +
				"\t\t\t\t\t<FullName>MyTests.Class1</FullName>\r\n" +
				"\t\t\t\t\t<Methods>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"true\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Void MyTests.MyClass1::set_Count(System.Int32)</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"1\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"34\" sc=\"4\" el=\"34\" ec=\"5\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"33\" sc=\"5\" el=\"33\" ec=\"12\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"32\" sc=\"8\" el=\"32\" ec=\"9\" />\r\n" +
				"\t\t\t\t\t\t\t</SequencePoints>\r\n" +
				"\t\t\t\t\t\t</Method>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"true\" isSetter=\"false\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Int32 MyTests.MyClass1::get_Count())</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"1\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"31\" sc=\"4\" el=\"31\" ec=\"5\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"30\" sc=\"5\" el=\"30\" ec=\"15\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"29\" sc=\"8\" el=\"29\" ec=\"9\" />\r\n" +
				"\t\t\t\t\t\t\t</SequencePoints>\r\n" +
				"\t\t\t\t\t\t</Method>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"false\" isSetter=\"false\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Void MyTests.MyClass1::get_NotAProperty())</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"1\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"21\" sc=\"3\" el=\"21\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"19\" sc=\"3\" el=\"19\" ec=\"18\" />\r\n" +
				"\t\t\t\t\t\t\t</SequencePoints>\r\n" +
				"\t\t\t\t\t\t</Method>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"false\" isStatic=\"false\" isGetter=\"true\" isSetter=\"false\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Void MyTests.Tests.MyClass::PropertyFlagsButJustAMethod()</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"1\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"21\" sc=\"3\" el=\"21\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"19\" sc=\"3\" el=\"19\" ec=\"18\" />\r\n" +
				"\t\t\t\t\t\t\t</SequencePoints>\r\n" +
				"\t\t\t\t\t\t</Method>\r\n" +
				"\t\t\t\t\t\t<Method visited=\"true\" cyclomaticComplexity=\"1\" sequenceCoverage=\"100\" branchCoverage=\"100\" isConstructor=\"\" isStatic=\"false\" isGetter=\"\" isSetter=\"\">\r\n" +
				"\t\t\t\t\t\t\t<MetadataToken>100663297</MetadataToken>\r\n" +
				"\t\t\t\t\t\t\t<Name>System.Void MyTests.Tests.MyClass::InvalidFlags()</Name>\r\n" +
				"\t\t\t\t\t\t\t<FileRef uid=\"1\" />\r\n" +
				"\t\t\t\t\t\t\t<SequencePoints>\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"21\" sc=\"3\" el=\"21\" ec=\"4\" />\r\n" +
				"\t\t\t\t\t\t\t\t<SequencePoint vc=\"0\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"4\" />\r\n" +
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
		public void MethodName_GetterMethod_ReturnsExpectedGetterName()
		{
			string name = GetterMethod.Name;
			string expectedName = "get_Count";
			Assert.AreEqual(expectedName, name);
		}
		
		CodeCoverageMethod GetterMethod {
			get { return base.FirstModuleSecondMethod; }
		}
		
		[Test]
		public void MethodIsProperty_GetterMethod_ReturnsTrue()
		{
			bool result = GetterMethod.IsProperty;
			Assert.IsTrue(result);
		}
		
		[Test]
		public void MethodName_NameOfMethodWithPropertyFlagsButInvalidName_ReturnsMethodName()
		{
			string name = MethodWithPropertyFlagsButInvalidName.Name;
			string expectedName = "PropertyFlagsButJustAMethod";
			Assert.AreEqual(expectedName, name);
		}
		
		CodeCoverageMethod MethodWithPropertyFlagsButInvalidName {
			get { return FirstModule.Methods[3]; }
		}
		
		[Test]
		public void MethodName_SetterMethod_ReturnsSetterMethodName()
		{
			string name = SetterMethod.Name;
			string expectedName = "set_Count";
			Assert.AreEqual(expectedName, name);
		}
		
		CodeCoverageMethod SetterMethod {
			get { return FirstModuleFirstMethod; }
		}
		
		[Test]
		public void MethodIsProperty_SetterMethod_ReturnsTrue()
		{
			bool result = SetterMethod.IsProperty;
			Assert.IsTrue(result);
		}		
		
		[Test]
		public void MethodName_OrdinaryMethod_ReturnsMethodName()
		{
			string name = OrdinaryMethod.Name;
			string expectedName = "get_NotAProperty";
			Assert.AreEqual(expectedName, name);
		}
		
		CodeCoverageMethod OrdinaryMethod {
			get { return FirstModule.Methods[2]; }
		}
		
		[Test]
		public void MethodIsProperty_OrdinaryMethod_ReturnsFalse()
		{
			bool result = OrdinaryMethod.IsProperty;
			Assert.IsFalse(result);
		}
		
		/// <summary>
		/// Only methods with get_ or set_ as the start of the name are marked as properties.
		/// </summary>
		[Test]
		public void MethodIsProperty_MethodWithPropertyFlagButInvalidName_IsNotMarkedAsProperty()
		{
			bool result = MethodWithPropertyFlagsButInvalidName.IsProperty;
			Assert.IsFalse(result);
		}
	}
}
