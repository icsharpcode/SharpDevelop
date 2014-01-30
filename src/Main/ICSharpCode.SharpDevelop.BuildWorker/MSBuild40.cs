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
