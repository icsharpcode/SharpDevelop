// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace Gallio.SharpDevelop.Tests
{
	[TestFixture]
	public class GallioEchoCommandLineTests
	{
		MockCSharpProject project;
		SelectedTests selectedTests;
		
		[SetUp]
		public void Init()
		{
			project = new MockCSharpProject();
			selectedTests = new SelectedTests(project);
		}
		
		[Test]
		public void CommandLineIncludesProjectOutputAssembly()
		{
			GallioEchoConsoleApplication app = new GallioEchoConsoleApplication(selectedTests);
			
			string expectedArgs =
				"/rv:v4.0.30319 " +
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\"";
			
			Assert.AreEqual(expectedArgs, app.GetArguments());
		}
		
		[Test]
		public void SharpDevelopTestRunnerExtensionAddedToCommandLine()
		{
			SharpDevelopTestRunnerExtensionCommandLineArgument arg =
				new SharpDevelopTestRunnerExtensionCommandLineArgument();
			arg.TestResultsFileName = @"c:\temp\tmp66.tmp";
			
			GallioEchoConsoleApplication app = new GallioEchoConsoleApplication(selectedTests);
			app.TestRunnerExtensions.Add(arg);
			
			string expectedArgs =
				"/rv:v4.0.30319 " +
				"/re:\"Gallio.Extension.SharpDevelopTestRunnerExtension,Gallio.Extension.dll;c:\\temp\\tmp66.tmp\" " +
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\"";
			
			Assert.AreEqual(expectedArgs, app.GetArguments());
		}
		
		[Test]
		public void TestRunnerExtensionWithoutParametersAddedToCommandLine()
		{
			TestRunnerExtensionCommandLineArgument arg =
				new TestRunnerExtensionCommandLineArgument("MyNamespace.MyTestExtension,MyTestRunner.dll");
			
			GallioEchoConsoleApplication app = new GallioEchoConsoleApplication(selectedTests);
			app.TestRunnerExtensions.Add(arg);
			
			string expectedArgs =
				"/rv:v4.0.30319 " +
				"/re:\"MyNamespace.MyTestExtension,MyTestRunner.dll\" " +
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\"";
			
			Assert.AreEqual(expectedArgs, app.GetArguments());
		}

	}
}
