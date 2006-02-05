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
	public class NUnitResultsTestFixture
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
			
			NUnitResults results = new NUnitResults(new StringReader(GetNUnitResultsXml()));
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
					{"TestCase", "NunitFoo.Tests.FooTest.Foo"},
					{"Message", "Foo failed"}
				});

			Assert.AreEqual(description, errorTask.Description);
		}
		
		string GetNUnitResultsXml()
		{
			return "<test-results name=\"C:\\test\\NunitFoo\\NunitFoo.Tests\\bin\\Debug\\NunitFoo.Tests.dll\" total=\"2\" failures=\"1\" not-run=\"0\" date=\"2006-01-31\" time=\"01:18:33\">\r\n" +
					"  <test-suite name=\"C:\\test\\NunitFoo\\NunitFoo.Tests\\bin\\Debug\\NunitFoo.Tests.dll\" success=\"False\" time=\"0.040\" asserts=\"0\">\r\n" +
					"    <results>\r\n" +
					"      <test-suite name=\"NunitFoo\" success=\"False\" time=\"0.040\" asserts=\"0\">\r\n" +
					"        <results>\r\n" +
					"          <test-suite name=\"Tests\" success=\"False\" time=\"0.040\" asserts=\"0\">\r\n" +
					"            <results>\r\n" +
					"              <test-suite name=\"FooTest\" success=\"False\" time=\"0.030\" asserts=\"0\">\r\n" +
					"                <results>\r\n" +
					"                  <test-case name=\"NunitFoo.Tests.FooTest.Foo\" executed=\"True\" success=\"False\" time=\"0.010\" asserts=\"0\">\r\n" +
					"                    <failure>\r\n" +
					"                      <message><![CDATA[Foo failed]]></message>\r\n" +
					"                      <stack-trace><![CDATA[   at NunitFoo.Tests.FooTest.Foo() in c:\\test\\NunitFoo\\NunitFoo.Tests\\FooTest.cs:line 22\r\n" +
					"]]></stack-trace>\r\n" +
					"                    </failure>\r\n" +
					"                  </test-case>\r\n" +
					"                </results>\r\n" +
					"              </test-suite>\r\n" +
					"            </results>\r\n" +
					"          </test-suite>\r\n" +
					"        </results>\r\n" +
					"      </test-suite>\r\n" +
					"    </results>\r\n" +
					"  </test-suite>\r\n" +
					"</test-results>";
		}
	}
}
