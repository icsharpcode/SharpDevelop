// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	sealed class MSBuildWrapper
	{
		public void Cancel()
		{
			BuildManager.DefaultBuildManager.CancelAllSubmissions();
		}
		
		public MSBuildWrapper()
		{
		}
		
		public bool DoBuild(BuildJob job, ILogger logger)
		{
			BuildParameters parameters = new BuildParameters();
			parameters.MaxNodeCount = 1;
			parameters.Loggers = new ILogger[] { logger };
			
			Program.Log("Building target '" + job.Target + "' in " + job.ProjectFileName);
			string[] targets = job.Target.Split(';');
			BuildRequestData request = new BuildRequestData(job.ProjectFileName, job.Properties, null, targets, null);
			
			BuildResult result = BuildManager.DefaultBuildManager.Build(parameters, request);
			return result.OverallResult == BuildResultCode.Success;
		}
	}
}
