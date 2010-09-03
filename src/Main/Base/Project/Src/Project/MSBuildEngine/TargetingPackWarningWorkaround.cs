// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Suppresses the warning MSB3644.
	/// </summary>
	public sealed class TargetingPackWarningWorkaround : IMSBuildLoggerFilter
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
		
		public IMSBuildChainedLoggerFilter CreateFilter(MSBuildEngine engine, IMSBuildChainedLoggerFilter nextFilter)
		{
			if (engine.ProjectMinimumSolutionVersion >= Solution.SolutionVersionVS2010) {
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
