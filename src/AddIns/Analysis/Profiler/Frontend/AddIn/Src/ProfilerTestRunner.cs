// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.Profiler.AddIn;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.Profiler.AddIn
{
	public class ProfilerTestRunner : TestRunnerBase
	{
		ProfilerRunner runner;
		//UnitTestingOptions options = new UnitTestingOptions();
		TestResultsReader testResultsReader;
		
		public ProfilerTestRunner()
		{
		}
		
		public override void Start(IEnumerable<ITest> selectedTests)
		{
			ProcessStartInfo startInfo = GetProcessStartInfo(selectedTests);
			Start(startInfo, selectedTests);
		}
		
		protected override ProcessStartInfo GetProcessStartInfo(IEnumerable<ITest> selectedTests)
		{
			NUnitConsoleApplication app = new NUnitConsoleApplication(selectedTests, UnitTestingOptions.Instance);
			testResultsReader = new TestResultsReader();
			app.ResultsPipe = testResultsReader.PipeName;
			return app.GetProcessStartInfo();
		}
		
		void Start(ProcessStartInfo startInfo, IEnumerable<ITest> selectedTests)
		{
			LogCommandLine(startInfo);

			string path = selectedTests.FirstOrDefault().ParentProject.Project.GetSessionFileName();
			
			LoggingService.Info("starting profiler...");
			
			runner = new ProfilerRunner(startInfo, true, new UnitTestWriter(new ProfilingDataSQLiteWriter(path), GetUnitTestNames(selectedTests).ToArray()));
			
			runner.RunFinished += delegate {
				SD.MainThread.InvokeIfRequired(() => FileService.OpenFile(path));
				AfterFinish(selectedTests, path);
			};
			
			testResultsReader.TestFinished += OnTestFinished;
			testResultsReader.Start();
			runner.Run();
		}
				
		IEnumerable<string> GetUnitTestNames(IEnumerable<ITest> selectedTests)
		{
			foreach (var project in selectedTests.Select(l => l.ParentProject)) {
				foreach (var name in project.GetUnitTestNames()) {
					yield return name;
				}
			}
			/*
			foreach (ITest test in selectedTests) {
				IProject project = test.ParentProject.Project;
				ICompilation compilation = SD.ParserService.GetCompilation(project);
				compilation.FindType
			}
			IProjectContent content = selectedTests.FirstOrDefault().ParentProject.Project.ProjectContent;
			IEnumerable<NUnitTestClass> selectedNunitTests = (IEnumerable<NUnitTestClass>) selectedTests;
			if (selectedNunitTests.FirstOrDefault() == null) {
				var testClasses = content.Classes
					.Where(c => c.Attributes.Any(a => a.AttributeType.FullyQualifiedName == "NUnit.Framework.TestFixtureAttribute"));
				return testClasses
					.SelectMany(c2 => c2.Methods)
					.Where(m => m.Attributes.Any(a2 => a2.AttributeType.FullyQualifiedName == "NUnit.Framework.TestAttribute"))
					.Select(m2 => m2.FullyQualifiedName);
			}
			if (selectedTests.Member == null) {
				return content.Classes.First(c => c.FullyQualifiedName == selectedNunitTests.FirstOrDefault().ClassName).Methods
					.Where(m => m.Attributes.Any(a2 => a2.AttributeType.FullyQualifiedName == "NUnit.Framework.TestAttribute"))
					.Select(m2 => m2.FullyQualifiedName);
			}
			return new[] { selectedTests.Class.DotNetName + "." + selectedTests.Member.Name };*/
		}
		
		void AfterFinish(IEnumerable<ITest> selectedTests, string path)
		{
			selectedTests.FirstOrDefault().ParentProject.Project.AddSessionToProject(path);
			OnAllTestsFinished();
			LoggingService.Info("shutting profiler down...");
		}
		
		/*public override void Stop()
		{
			if (this.runner != null && this.runner.Profiler.IsRunning) {
				LoggingService.Info("stopping profiler...");
				runner.Stop();
			}
			
			if (testResultsReader != null) {
				testResultsReader.Dispose();
				//testResultsReader.Stop();
				//testResultsReader.Read();
				testResultsReader = null;
			}
		}
		
		public override void Dispose()
		{
			Stop();
		}*/
		
		protected override void OnAllTestsFinished()
		{
			testResultsReader.Join();
			base.OnAllTestsFinished();
		}
		
		public override void Stop()
		{
			if (this.runner != null && this.runner.Profiler.IsRunning) {
				LoggingService.Info("stopping profiler...");
				runner.Stop();
			}
			if (testResultsReader != null) {
				testResultsReader.Dispose();
				testResultsReader = null;
			}
		}
		
		public override void Dispose()
		{
			testResultsReader.Dispose();
			testResultsReader.TestFinished -= OnTestFinished;
		}
		
		public override int GetExpectedNumberOfTestResults(IEnumerable<ITest> selectedTests)
		{
			return GetNumberOfTestMethods(selectedTests);
		}
		
		public static int GetNumberOfTestMethods(IEnumerable<ITest> selectedTests)
		{
			int count = 0;
			foreach (ITest test in selectedTests) {
				if (test is NUnitTestMethod)
					count++;
				else
					count += GetNumberOfTestMethods(test.NestedTests);
			}
			return count;
		}
	}
}
