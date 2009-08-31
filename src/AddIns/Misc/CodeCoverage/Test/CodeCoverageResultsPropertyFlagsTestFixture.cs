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
	/// Tests that property methods (e.g. get_SomeValue) are identified by the CodeCoverageResults class
	/// via the flags attribute in the code coverage results file.
	/// </summary>
	[TestFixture]
	public class CodeCoverageResultsPropertyFlagsTestFixture
	{
		CodeCoverageResults results;
		CodeCoverageMethod getterMethod;
		CodeCoverageMethod setterMethod;
		CodeCoverageMethod method;
		CodeCoverageMethod methodWithPropertyFlagsButInvalidName;
		
		[SetUp]
		public void SetUpFixture()
		{
			string xml = "<PartCoverReport ver=\"1.0.2796.35184\">\r\n" +
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

			results = new CodeCoverageResults(new StringReader(xml));
			if (results.Modules.Count > 0) {
				CodeCoverageModule module = results.Modules[0];
				if (module.Methods.Count > 2) {
					setterMethod = module.Methods[0];
					getterMethod = module.Methods[1];
					method = module.Methods[2];
					methodWithPropertyFlagsButInvalidName = module.Methods[3];
				}
			}
		}
		
		[Test]
		public void GetterMethodName()
		{
			Assert.AreEqual("get_Count", getterMethod.Name);
		}
		
		[Test]
		public void GetterMethodIsProperty()
		{
			Assert.IsTrue(getterMethod.IsProperty);
		}
		
		[Test]
		public void NameOfMethodWithPropertyFlagsButInvalidName()
		{
			Assert.AreEqual("PropertyFlagsButJustAMethod", methodWithPropertyFlagsButInvalidName.Name);
		}
		
		[Test]
		public void SetterMethodName()
		{
			Assert.AreEqual("set_Count", setterMethod.Name);
		}
		
		[Test]
		public void SetterMethodIsProperty()
		{
			Assert.IsTrue(setterMethod.IsProperty);
		}		
		
		[Test]
		public void OrdinaryMethodName()
		{
			Assert.AreEqual("get_NotAProperty", method.Name);
		}
		
		[Test]
		public void OrdinaryMethodIsNotProperty()
		{
			Assert.IsFalse(method.IsProperty);
		}
		
		/// <summary>
		/// Only methods with get_ or set_ as the start of the name are marked as properties.
		/// </summary>
		[Test]
		public void MethodWithPropertyFlagButInvalidNameIsNotMarkedAsProperty()
		{
			Assert.IsFalse(methodWithPropertyFlagsButInvalidName.IsProperty);
		}
	}
}
