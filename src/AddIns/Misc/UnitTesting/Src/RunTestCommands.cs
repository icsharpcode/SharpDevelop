// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.UnitTesting
{
	public abstract class AbstractRunTestCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IMember m = TestableCondition.GetMember(Owner);
			IClass c = (m != null) ? m.DeclaringType : TestableCondition.GetClass(Owner);
			Run(TestableCondition.GetProject(Owner), c, m);
		}
		
		public void Run(IProject project, IClass fixture, IMember test)
		{
			if (project == null) {
				throw new ArgumentNullException("project");
			}
			BuildProject build = new BuildProject(project);
			build.BuildComplete += delegate {
				if (MSBuildEngine.LastErrorCount == 0) {
					RunTests(project, fixture, test);
				}
			};
			build.Run();
		}
		
		protected abstract void RunTests(IProject project, IClass fixture, IMember test);
	}
	
	public class RunTestInPadCommand : AbstractRunTestCommand
	{
		protected override void RunTests(IProject project, IClass fixture, IMember test)
		{
			PadContent.BringToFront();
			PadContent.Instance.RunTest(project, fixture, test);
		}
	}
	
	public class RunTestWithDebuggerCommand : AbstractRunTestCommand
	{
		public override void Run()
		{
			if (DebuggerService.IsDebuggerLoaded && DebuggerService.CurrentDebugger.IsDebugging) {
				MessageService.ShowMessage("The debugger is currently busy.");
			} else {
				base.Run();
			}
		}
		
		static string lastOutputFile;
		
		protected override void RunTests(IProject project, IClass fixture, IMember test)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo(UnitTestApplicationStartHelper.UnitTestConsoleApplication);
			UnitTestApplicationStartHelper helper = new UnitTestApplicationStartHelper();
			helper.Initialize(project, fixture, test);
			helper.XmlOutputFile = Path.GetTempFileName();
			try {
				File.Delete(helper.XmlOutputFile);
			} catch {}
			lastOutputFile = helper.XmlOutputFile;
			startInfo.Arguments = helper.GetArguments();
			startInfo.WorkingDirectory = UnitTestApplicationStartHelper.UnitTestApplicationDirectory;
			LoggingService.Info("Run " + startInfo.FileName + " " + startInfo.Arguments);
			
			// register event if it is not already registered
			DebuggerService.DebugStopped -= DebuggerFinished;
			DebuggerService.DebugStopped += DebuggerFinished;
			DebuggerService.CurrentDebugger.Start(startInfo);
		}
		
		static void DebuggerFinished(object sender, EventArgs e)
		{
			DebuggerService.DebugStopped -= DebuggerFinished;
			if (lastOutputFile != null) {
				UnitTestApplicationStartHelper.DisplayResults(lastOutputFile);
				try {
					File.Delete(lastOutputFile);
				} catch {}
				lastOutputFile = null;
			}
		}
	}
}
