// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.UnitTesting;

namespace Gallio.SharpDevelop
{
	public class GallioTestRunner : TestProcessRunnerBase
	{
		public GallioTestRunner()
		{
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
		{
			GallioEchoConsoleApplicationProcessStartInfo startInfo = 
				new GallioEchoConsoleApplicationProcessStartInfo(selectedTests, base.TestResultsMonitor.FileName);
			return startInfo.ProcessStartInfo;
		}
		
		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return new GallioTestResult(testResult);
		}
	}
}
