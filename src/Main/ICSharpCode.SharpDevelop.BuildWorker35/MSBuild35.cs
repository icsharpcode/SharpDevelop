// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
