// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	sealed class MSBuildWrapper
	{
		public void Cancel()
		{
		}
		
		readonly Engine engine;
		
		public MSBuildWrapper()
		{
			engine = new Engine(ToolsetDefinitionLocations.Registry | ToolsetDefinitionLocations.ConfigurationFile);
		}
		
		public bool DoBuild(BuildJob job, ILogger logger)
		{
			engine.RegisterLogger(logger);
			
			Program.Log("Building target '" + job.Target + "' in " + job.ProjectFileName);
			string[] targets = job.Target.Split(';');
			
			BuildPropertyGroup globalProperties = new BuildPropertyGroup();
			foreach (var pair in job.Properties) {
				globalProperties.SetProperty(pair.Key, pair.Value, true);
			}
			
			try {
				return engine.BuildProjectFile(job.ProjectFileName, targets, globalProperties);
			} finally {
				engine.UnregisterAllLoggers();
			}
		}
	}
}
