// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	public class RunTestWithProfilerCommand : AbstractRunTestCommand
	{
		protected override void RunTests(UnitTestApplicationStartHelper helper)
		{
			TestRunnerCategory.AppendLine(helper.GetCommandLine());
			
			ProcessStartInfo startInfo = new ProcessStartInfo(helper.UnitTestApplication);
			
			string path = helper.Project.GetSessionFileName();
			
			startInfo.Arguments = helper.GetArguments();
			startInfo.WorkingDirectory = UnitTestApplicationStartHelper.UnitTestApplicationDirectory;
			LoggingService.Info("starting profiler...");
			
			ProfilerRunner runner = new ProfilerRunner(startInfo, true, new ProfilingDataSQLiteWriter(path, true, GetUnitTestNames(helper).ToArray()));
			
			runner.RunFinished += delegate {
				WorkbenchSingleton.SafeThreadCall(() => FileService.OpenFile(path));
				AfterFinish(helper, path);
			};
			
			runner.Run();
		}
		
		IEnumerable<string> GetUnitTestNames(UnitTestApplicationStartHelper helper)
		{
			IProjectContent content = ParserService.GetProjectContent(helper.Project);
			
			if (helper.Fixture == null) {
				var testClasses = content.Classes
					.Where(c => c.Attributes.Any(a => a.AttributeType.FullyQualifiedName == "NUnit.Framework.TestFixtureAttribute"));
				return testClasses
					.SelectMany(c2 => c2.Methods)
					.Where(m => m.Attributes.Any(a2 => a2.AttributeType.FullyQualifiedName == "NUnit.Framework.TestAttribute"))
					.Select(m2 => m2.FullyQualifiedName);
			}
			
			if (helper.Test == null) {
				return content.Classes
					.Where(c => c.FullyQualifiedName == helper.Fixture).First().Methods
					.Where(m => m.Attributes.Any(a2 => a2.AttributeType.FullyQualifiedName == "NUnit.Framework.TestAttribute"))
					.Select(m2 => m2.FullyQualifiedName);
			}
			
			return new[] { helper.Fixture + "." + helper.Test };
		}
		
		void AfterFinish(UnitTestApplicationStartHelper helper, string path)
		{
			helper.Project.AddSessionToProject(path);
			WorkbenchSingleton.SafeThreadAsyncCall(TestsFinished);
			LoggingService.Info("shutting profiler down...");
		}
		
		public override void Run()
		{
//			if (ProfilerService.IsProfilerLoaded && ProfilerService.CurrentProfiler.IsRunning) {
//				MessageService.ShowError("Currently there is a profiling session in progress. " +
//				                         "Please finish the current session before starting a new one.");
//			} else {
			base.Run();
//			}
		}
		
		protected override void OnStop()
		{
//			if (ProfilerService.CurrentProfiler.IsRunning) {
//				LoggingService.Info("stopping profiler...");
//				ProfilerService.CurrentProfiler.Stop();
//			}
		}
	}
}
