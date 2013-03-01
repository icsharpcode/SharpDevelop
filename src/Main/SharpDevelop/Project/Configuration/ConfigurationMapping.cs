// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Project
{
	class ConfigurationMapping : IConfigurationMapping
	{
		class Entry
		{
			public ConfigurationAndPlatform Config;
			public bool Build = true;
			
			public Entry(ConfigurationAndPlatform config)
			{
				this.Config = config;
			}
		}
		
		readonly Solution parentSolution;
		Dictionary<ConfigurationAndPlatform, Entry> dict = new Dictionary<ConfigurationAndPlatform, Entry>();
		
		public ConfigurationMapping(Solution parentSolution)
		{
			this.parentSolution = parentSolution;
		}
		
		Entry GetEntry(ConfigurationAndPlatform solutionConfiguration)
		{
			Entry entry;
			lock (dict) {
				if (!dict.TryGetValue(solutionConfiguration, out entry)) {
					var config = new ConfigurationAndPlatform(solutionConfiguration.Configuration, MSBuildInternals.FixPlatformNameForProject(solutionConfiguration.Platform));
					entry = new Entry(config);
					dict.Add(solutionConfiguration, entry);
				}
			}
			return entry;
		}
		
		#region IConfigurationMapping implementation
		public ConfigurationAndPlatform GetProjectConfiguration(ConfigurationAndPlatform solutionConfiguration)
		{
			return GetEntry(solutionConfiguration).Config;
		}
		
		public void SetProjectConfiguration(ConfigurationAndPlatform solutionConfiguration, ConfigurationAndPlatform projectConfiguration)
		{
			Debug.Assert(projectConfiguration.Platform != "Any CPU");
			GetEntry(solutionConfiguration).Config = projectConfiguration;
			if (parentSolution != null)
				parentSolution.IsDirty = true;
		}
		
		public bool IsBuildEnabled(ConfigurationAndPlatform solutionConfiguration)
		{
			return GetEntry(solutionConfiguration).Build;
		}
		
		public void SetBuildEnabled(ConfigurationAndPlatform solutionConfiguration, bool value)
		{
			GetEntry(solutionConfiguration).Build = value;
			if (parentSolution != null)
				parentSolution.IsDirty = true;
		}
		#endregion
		
		public void RenameSolutionConfig(string oldName, string newName, bool isPlatform)
		{
			throw new NotImplementedException();
			lock (dict) {
				foreach (var pair in dict.ToArray()) {
					if (oldName == (isPlatform ? pair.Key.Platform : pair.Key.Configuration)) {
						
					}
				}
			}
		}
	}
}
