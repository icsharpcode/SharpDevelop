// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using Gallio.SharpDevelop;
using Gallio.SharpDevelop.Tests.Utils;
using ICSharpCode.Core;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace Gallio.SharpDevelop.Tests
{
	[TestFixture]
	public class GallioEchoConsoleProcessStartInfoTestFixture
	{
		ProcessStartInfo info;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string xml =
				"<AddIn name='Unit Testing Addin'\r\n" +
				"       author='Matt Ward'\r\n" +
				"       copyright='prj:///doc/copyright.txt'\r\n" +
				"       description='Runs unit tests with Gallio inside SharpDevelop'\r\n" +
				"       addInManagerHidden='preinstalled'>\r\n" +
				"    <Manifest>\r\n" +
				"        <Identity name='ICSharpCode.Gallio'/>\r\n" +
				"    </Manifest>\r\n" +
				"</AddIn>";
			AddIn addin = AddIn.Load(new StringReader(xml));
			addin.Enabled = false;
			addin.FileName = @"d:\sharpdevelop\addins\gallio\Gallio.SharpDevelop.dll";
			AddInTree.InsertAddIn(addin);
		}
		
		[SetUp]
		public void Init()
		{
			MockCSharpProject project = new MockCSharpProject();
			SelectedTests selectedTests = new SelectedTests(project);
			GallioEchoConsoleApplication app = new GallioEchoConsoleApplication(selectedTests, @"d:\gallio\Gallio.Echo.exe");
			info = app.GetProcessStartInfo();
		}
		
		[Test]
		public void GallioAddInPathIsConvertedByStringParser()
		{
			string expectedDirectory = @"d:\sharpdevelop\addins\gallio";
			string actualDirectory = StringParser.Parse("${addinpath:ICSharpCode.Gallio}");
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
		
		[Test]
		public void WorkingDirectoryIsGallioAddInDirectory()
		{
			string expectedDirectory = @"d:\sharpdevelop\addins\gallio";
			Assert.AreEqual(expectedDirectory, info.WorkingDirectory);
		}
		
		[Test]
		public void FileNameIsNUnitConsoleExe()
		{
			string expectedFileName = @"d:\gallio\Gallio.Echo.exe";
			Assert.AreEqual(expectedFileName, info.FileName);
		}
		
		[Test]
		public void CommandLineArgumentsAreNUnitConsoleExeCommandLineArguments()
		{
			string expectedCommandLine =
				"/rv:v4.0.30319 " +
				"\"c:\\projects\\MyTests\\bin\\Debug\\MyTests.dll\"";
			
			Assert.AreEqual(expectedCommandLine, info.Arguments);
		}
	}
}
