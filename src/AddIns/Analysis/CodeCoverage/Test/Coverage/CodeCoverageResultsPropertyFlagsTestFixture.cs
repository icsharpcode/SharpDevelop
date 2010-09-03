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
				"<PartCoverReport ver=\"1.0.2796.35184\">\r\n" +
				"  <File id=\"1\" url=\"d:\\Projects\\test\\TestFixture1.cs\" />\r\n" +
				"  <Assembly id=\"1\" name=\"MyTests\" module=\"C:\\Projects\\Test\\MyTests\\bin\\MyTests.DLL\" domain=\"test-domain-MyTests.dll\" domainIdx=\"1\" />\r\n" +
				"  <Type asmref=\"1\" name=\"MyTests.Class1\" flags=\"2606412\">\r\n" +
				"    <Method name=\"set_Count\" sig=\"void  (int)\" flags=\"2182\" iflags=\"0\">\r\n" +
				"      <pt visit=\"0\" pos=\"9\" len=\"1\" fid=\"1\" sl=\"34\" sc=\"4\" el=\"34\" ec=\"5\" />\r\n" +
				"      <pt visit=\"0\" pos=\"1\" len=\"8\" fid=\"1\" sl=\"33\" sc=\"5\" el=\"33\" ec=\"12\" />\r\n" +
				"      <pt visit=\"0\" pos=\"0\" len=\"1\" fid=\"1\" sl=\"32\" sc=\"8\" el=\"32\" ec=\"9\" />\r\n" +
				"    </Method>\r\n" +
				"    <Method name=\"get_Count\" sig=\"int  ()\" flags=\"2182\" iflags=\"0\">\r\n" +
				"      <pt visit=\"0\" pos=\"6\" len=\"2\" fid=\"1\" sl=\"31\" sc=\"4\" el=\"31\" ec=\"5\" />\r\n" +
				"      <pt visit=\"0\" pos=\"1\" len=\"5\" fid=\"1\" sl=\"30\" sc=\"5\" el=\"30\" ec=\"15\" />\r\n" +
				"      <pt visit=\"0\" pos=\"0\" len=\"1\" fid=\"1\" sl=\"29\" sc=\"8\" el=\"29\" ec=\"9\" />\r\n" +
				"    </Method>\r\n" +
				"    <Method name=\"get_NotAProperty\" sig=\"void  ()\" flags=\"134\" iflags=\"0\">\r\n" +
				"      <pt visit=\"0\" pos=\"1\" len=\"1\" fid=\"1\" sl=\"26\" sc=\"3\" el=\"26\" ec=\"4\" />\r\n" +
				"      <pt visit=\"0\" pos=\"0\" len=\"1\" fid=\"1\" sl=\"25\" sc=\"3\" el=\"25\" ec=\"4\" />\r\n" +
				"    </Method>\r\n" +
				"    <Method name=\"PropertyFlagsButJustAMethod\" sig=\"void  ()\" flags=\"2182\" iflags=\"0\">\r\n" +
				"      <pt visit=\"0\" pos=\"8\" len=\"2\" fid=\"1\" sl=\"22\" sc=\"3\" el=\"22\" ec=\"4\" />\r\n" +
				"      <pt visit=\"0\" pos=\"7\" len=\"1\" fid=\"1\" sl=\"21\" sc=\"3\" el=\"21\" ec=\"4\" />\r\n" +
				"      <pt visit=\"0\" pos=\"0\" len=\"7\" fid=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"18\" />\r\n" +
				"    </Method>\r\n" +
				"    <Method name=\"InvalidFlags\" sig=\"void  ()\" flags=\"\" iflags=\"0\">\r\n" +
				"      <pt visit=\"0\" pos=\"8\" len=\"2\" fid=\"1\" sl=\"22\" sc=\"3\" el=\"22\" ec=\"4\" />\r\n" +
				"      <pt visit=\"0\" pos=\"7\" len=\"1\" fid=\"1\" sl=\"21\" sc=\"3\" el=\"21\" ec=\"4\" />\r\n" +
				"      <pt visit=\"0\" pos=\"0\" len=\"7\" fid=\"1\" sl=\"20\" sc=\"3\" el=\"20\" ec=\"18\" />\r\n" +
				"    </Method>\r\n" +
				"  </Type>\r\n" +
				"</PartCoverReport>";

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
