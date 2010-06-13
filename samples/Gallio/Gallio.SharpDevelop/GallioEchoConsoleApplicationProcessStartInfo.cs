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
	public class GallioEchoConsoleApplicationProcessStartInfo
	{
		ProcessStartInfo processStartInfo = new ProcessStartInfo();
		
		public GallioEchoConsoleApplicationProcessStartInfo(SelectedTests selectedTests, string testResultsFileName)
		{
			GallioEchoConsoleApplicationFactory factory = new GallioEchoConsoleApplicationFactory();
			GallioEchoConsoleApplication app = factory.Create(selectedTests);
			SharpDevelopTestRunnerExtensionCommandLineArgument argument = new SharpDevelopTestRunnerExtensionCommandLineArgument();
			argument.TestResultsFileName = testResultsFileName;
			app.TestRunnerExtensions.Add(argument);
			processStartInfo = app.GetProcessStartInfo();
		}
		
		public ProcessStartInfo ProcessStartInfo {
			get { return processStartInfo; }
		}
	}
}
