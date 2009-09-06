// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	using ICSharpCode.Profiler.Controller;
	
	/// <summary>
	/// Description of RunProject
	/// </summary>
	public class ProfileProject : ProfilerMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			IProject currentProj = ProjectService.CurrentProject;
			string path = currentProj.GetSessionFileName();
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			
			IProfilingDataWriter writer = new ProfilingDataSQLiteWriter(path, false, null);
			ProfilerRunner runner = ProfilerRunner.CreateRunner(writer);
			
			if (runner != null) {
				runner.RunFinished += delegate { currentProj.AddSessionToProject(path); };
				runner.Run();
			}
		}
	}
}
