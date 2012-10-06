// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.Profiler.AddIn
{
	public class ProfilerTestRunner : TestRunnerBase
	{
		ProfilerRunner runner;
		UnitTestingOptions options = new UnitTestingOptions();
		TestResultsMonitor testResultsMonitor;
		
		public ProfilerTestRunner()
		{
		}
		
		public override void Start(SelectedTests selectedTests)
		{
			ProcessStartInfo startInfo = GetProcessStartInfo(selectedTests);
			Start(startInfo, selectedTests);
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(SelectedTests selectedTests)
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests, options);
			testResultsMonitor = new TestResultsMonitor();
			app.Results = testResultsMonitor.FileName;
			return app.GetProcessStartInfo();
		}
		
		void Start(ProcessStartInfo startInfo, SelectedTests selectedTests)
		{
			LogCommandLine(startInfo);
			
			string path = selectedTests.Project.GetSessionFileName();
			
			LoggingService.Info("starting profiler...");
			
			runner = new ProfilerRunner(startInfo, true, new UnitTestWriter(new ProfilingDataSQLiteWriter(path), GetUnitTestNames(selectedTests).ToArray()));
			
			runner.RunFinished += delegate {
				WorkbenchSingleton.SafeThreadCall(() => FileService.OpenFile(path));
				AfterFinish(selectedTests, path);
			};
			
			testResultsMonitor.TestFinished += OnTestFinished;
			testResultsMonitor.Start();
			runner.Run();
		}
				
		IEnumerable<string> GetUnitTestNames(SelectedTests selectedTests)
		{
			IProjectContent content = ParserService.GetProjectContent(selectedTests.Project);
			
			if (selectedTests.Class == null) {
				var testClasses = content.Classes
					.Where(c => c.Attributes.Any(a => a.AttributeType.FullyQualifiedName == "NUnit.Framework.TestFixtureAttribute"));
				return testClasses
					.SelectMany(c2 => c2.Methods)
					.Where(m => m.Attributes.Any(a2 => a2.AttributeType.FullyQualifiedName == "NUnit.Framework.TestAttribute"))
					.Select(m2 => m2.FullyQualifiedName);
			}
			
			if (selectedTests.Member == null) {
				return content.Classes.First(c => c.FullyQualifiedName == selectedTests.Class.DotNetName).Methods
					.Where(m => m.Attributes.Any(a2 => a2.AttributeType.FullyQualifiedName == "NUnit.Framework.TestAttribute"))
					.Select(m2 => m2.FullyQualifiedName);
			}
			
			return new[] { selectedTests.Class.DotNetName + "." + selectedTests.Member.Name };
		}
		
		void AfterFinish(SelectedTests selectedTests, string path)
		{
			selectedTests.Project.AddSessionToProject(path);
			OnAllTestsFinished(this, new EventArgs());
			LoggingService.Info("shutting profiler down...");
		}
		
		public override void Stop()
		{
			if (this.runner != null && this.runner.Profiler.IsRunning) {
				LoggingService.Info("stopping profiler...");
				runner.Stop();
			}
			
			if (testResultsMonitor != null) {
				testResultsMonitor.Stop();
				testResultsMonitor.Read();
				testResultsMonitor = null;
			}
		}
		
		public override void Dispose()
		{
			Stop();
		}
	}
}
