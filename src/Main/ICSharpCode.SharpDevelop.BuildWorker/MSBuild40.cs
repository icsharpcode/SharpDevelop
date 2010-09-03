// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
