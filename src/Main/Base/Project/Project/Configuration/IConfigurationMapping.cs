// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Represents a mapping between solution and project configurations.
	/// </summary>
	public interface IConfigurationMapping
	{
		/// <summary>
		/// Gets the project configuration corresponding to the given solution configuration.
		/// </summary>
		ConfigurationAndPlatform GetProjectConfiguration(ConfigurationAndPlatform solutionConfiguration);
		
		/// <summary>
		/// Sets the project configuration corresponding to the given solution configuration.
		/// </summary>
		void SetProjectConfiguration(ConfigurationAndPlatform solutionConfiguration, ConfigurationAndPlatform projectConfiguration);
		
		/// <summary>
		/// Gets whether building the project is enabled in the given solution configuration.
		/// </summary>
		bool IsBuildEnabled(ConfigurationAndPlatform solutionConfiguration);
		
		/// <summary>
		/// Sets whether building the project is enabled in the given solution configuration.
		/// </summary>
		void SetBuildEnabled(ConfigurationAndPlatform solutionConfiguration, bool value);
	}
}
