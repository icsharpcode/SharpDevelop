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
	public class GallioTestDebugger : TestDebuggerBase
	{
		public GallioTestDebugger()
		{
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
		{
			GallioEchoConsoleApplicationProcessStartInfo startInfo = 
				new GallioEchoConsoleApplicationProcessStartInfo(selectedTests, base.TestResultsMonitor.FileName);
			startInfo.ProcessStartInfo.Arguments += " /d";
			return startInfo.ProcessStartInfo;
		}
		
		protected override TestResult CreateTestResultForTestFramework(TestResult testResult)
		{
			return new GallioTestResult(testResult);
		}
	}
}
