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

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Suppresses the warning MSB3644.
	/// </summary>
	sealed class TargetingPackWarningWorkaround : IMSBuildLoggerFilter
	{
		/*
		 * Warning MSB3644: The reference assemblies for framework
		 * ".NETFramework,Version=v4.0,Profile=Client" were not found. To resolve this,
		 * install the SDK or Targeting Pack for this framework version or retarget
		 * your application to a version of the framework for which you have the SDK
		 * or Targeting Pack installed. Note that assemblies will be resolved from
		 * the Global Assembly Cache (GAC) and will be used in place of reference assemblies.
		 * Therefore your assembly may not be correctly targeted for the framework you intend.
		 */
		
		public IMSBuildChainedLoggerFilter CreateFilter(IMSBuildLoggerContext context, IMSBuildChainedLoggerFilter nextFilter)
		{
			if (context.Project.MinimumSolutionVersion >= SolutionFormatVersion.VS2010) {
				return new TargetingPackWarningWorkaroundChainEntry(nextFilter);
			} else {
				return nextFilter;
			}
		}
		
		sealed class TargetingPackWarningWorkaroundChainEntry : IMSBuildChainedLoggerFilter
		{
			readonly IMSBuildChainedLoggerFilter nextFilter;
			
			public TargetingPackWarningWorkaroundChainEntry(IMSBuildChainedLoggerFilter nextFilter)
			{
				this.nextFilter = nextFilter;
			}
			
			public void HandleError(BuildError error)
			{
				if (error.ErrorCode != "MSB3644")
					nextFilter.HandleError(error);
			}
			
			public void HandleBuildEvent(Microsoft.Build.Framework.BuildEventArgs e)
			{
				nextFilter.HandleBuildEvent(e);
			}
		}
	}
}
