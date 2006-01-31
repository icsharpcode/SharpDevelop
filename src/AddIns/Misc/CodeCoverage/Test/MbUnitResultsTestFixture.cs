// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.IO;
using System.Resources;

namespace ICSharpCode.CodeCoverage.Tests
{
	[TestFixture]
	public class MbUnitResultsTestFixture
	{
		Task errorTask;
		
		[SetUp]
		public void Init()
		{
			// Add NUnitPad TestFailedMessage string resource since this resource
			// contains a format string that has parameters that are otherwise not 
			// set.
			ResourceManager resourceManager = new ResourceManager("ICSharpCode.CodeCoverage.Tests.Strings", GetType().Assembly);
			ResourceService.RegisterNeutralStrings(resourceManager);
			
			MbUnitResults results = new MbUnitResults(new StringReader(GetMbUnitResultsXml()));
			errorTask = results.Tasks[0];
		}
		
		[Test]
		public void IsErrorTask()
		{
			Assert.AreEqual(TaskType.Error, errorTask.TaskType);
		}
		
		[Test]
		public void ErrorTaskFileName()
		{
			Assert.IsTrue(FileUtility.IsEqualFileName(@"c:\test\NunitFoo\NunitFoo.Tests\FooTest.cs", errorTask.FileName));
		}
		
		[Test]
		public void ErrorTaskLine()
		{
			Assert.AreEqual(21, errorTask.Line);
		}
		
		[Test]
		public void ErrorTaskColumn()
		{
			Assert.AreEqual(0, errorTask.Column);
		}
		
		[Test]
		public void TaskDescription()
		{
			string description = StringParser.Parse("${res:NUnitPad.NUnitPadContent.TestTreeView.TestFailedMessage}", new string[,] {
					{"TestCase", "FooTest.Foo"},
					{"Message", "Foo failed"}
				});

			Assert.AreEqual(description, errorTask.Description);
		}
		
		string GetMbUnitResultsXml()
		{
			return "<report-result xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" date=\"2006-01-29T23:00:55.8368928+00:00\">\r\n" +
				"  <counter duration=\"0.0300432\" run-count=\"2\" success-count=\"1\" failure-count=\"1\" ignore-count=\"0\" skip-count=\"0\" assert-count=\"0\" />\r\n" +
				"  <assemblies>\r\n" +
				"    <assembly name=\"NunitFoo.Tests\" location=\"file:///C:/test/NunitFoo/NunitFoo.Tests/bin/Debug/NunitFoo.Tests.DLL\" full-name=\"NunitFoo.Tests, Version=1.0.2220.32532, Culture=neutral, PublicKeyToken=null\">\r\n" +
				"      <counter duration=\"0.0300432\" run-count=\"2\" success-count=\"1\" failure-count=\"1\" ignore-count=\"0\" skip-count=\"0\" assert-count=\"0\" />\r\n" +
				"      <version major=\"1\" minor=\"0\" build=\"2220\" revision=\"32532\" />\r\n" +
				"      <namespaces>\r\n" +
				"        <namespace name=\"NunitFoo\">\r\n" +
				"          <counter duration=\"0.0300432\" run-count=\"2\" success-count=\"1\" failure-count=\"1\" ignore-count=\"0\" skip-count=\"0\" assert-count=\"0\" />\r\n" +
				"          <namespaces>\r\n" +
				"            <namespace name=\"NunitFoo.Tests\">\r\n" +
				"              <counter duration=\"0.0300432\" run-count=\"2\" success-count=\"1\" failure-count=\"1\" ignore-count=\"0\" skip-count=\"0\" assert-count=\"0\" />\r\n" +
				"              <namespaces />\r\n" +
				"              <fixtures>\r\n" +
				"                <fixture name=\"FooTest\" type=\"NunitFoo.Tests.FooTest\">\r\n" +
				"                  <counter duration=\"0.0300432\" run-count=\"2\" success-count=\"1\" failure-count=\"1\" ignore-count=\"0\" skip-count=\"0\" assert-count=\"0\" />\r\n" +
				"                  <description />\r\n" +
				"                  <runs>\r\n" +
				"                    <run name=\"FooTest.Foo\" result=\"failure\" assert-count=\"0\" duration=\"0.0300432\" memory=\"8192\">\r\n" +
				"                      <invokers />\r\n" +
				"                      <warnings />\r\n" +
				"                      <asserts />\r\n" +
				"                      <Description />\r\n" +
				"                      <console-out />\r\n" +
				"                      <console-error />\r\n" +
				"                      <exception type=\"NUnit.Framework.AssertionException\">\r\n" +
				"                        <properties>\r\n" +
				"                          <property name=\"TargetSite\" value=\"Void Fail(System.String, System.Object[])\" />\r\n" +
				"                          <property name=\"HelpLink\" value=\"null\" />\r\n" +
				"                        </properties>\r\n" +
				"                        <message>Foo failed</message>\r\n" +
				"                        <source>nunit.framework</source>\r\n" +
				"                        <stack-trace>   at NUnit.Framework.Assert.Fail(String message, Object[] args)\r\n" +
				"   at NUnit.Framework.Assert.Fail(String message)\r\n" +
				"   at NunitFoo.Tests.FooTest.Foo() in c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs:line 22</stack-trace>\r\n" +
				"                      </exception>\r\n" +
				"                    </run>\r\n" +
				"                  </runs>\r\n" +
				"                </fixture>\r\n" +
				"              </fixtures>\r\n" +
				"            </namespace>\r\n" +
				"          </namespaces>\r\n" +
				"        </namespace>\r\n" +
				"      </namespaces>\r\n" +
				"    </assembly>\r\n" +
				"  </assemblies>\r\n" +
				"</report-result>";
		}
	}
}
