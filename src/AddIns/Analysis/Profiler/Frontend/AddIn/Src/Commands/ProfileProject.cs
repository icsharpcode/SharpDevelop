// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			
			IProfilingDataWriter writer = new ProfilingDataSQLiteWriter(path);
			ProfilerRunner runner = ProfilerRunner.CreateRunner(writer);
			
			if (runner != null) {
				runner.RunFinished += delegate { currentProj.AddSessionToProject(path); };
				runner.Run();
			}
		}
	}
}
