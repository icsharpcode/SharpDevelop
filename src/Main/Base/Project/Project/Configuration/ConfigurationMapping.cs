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
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Represents a mapping between solution and project configurations.
	/// </summary>
	/// <remarks>
	/// This is a simple storage class - just a dictionary of the mappings.
	/// This class is thread-safe.
	/// </remarks>
	public class ConfigurationMapping
	{
		class Entry
		{
			public ConfigurationAndPlatform Config;
			public bool Build = true;
			public bool Deploy = false;
			
			public Entry(ConfigurationAndPlatform config)
			{
				this.Config = config;
			}
			
			public Entry Clone()
			{
				return (Entry)MemberwiseClone();
			}
		}
		
		/// <summary>
		/// The configuration mapping was changed.
		/// </summary>
		public event EventHandler Changed = delegate { };
		
		Dictionary<ConfigurationAndPlatform, Entry> dict = new Dictionary<ConfigurationAndPlatform, Entry>();
		
		Entry GetOrCreateEntry(ConfigurationAndPlatform solutionConfiguration)
		{
			Entry entry;
			if (!dict.TryGetValue(solutionConfiguration, out entry)) {
				var config = new ConfigurationAndPlatform(solutionConfiguration.Configuration, MSBuildInternals.FixPlatformNameForProject(solutionConfiguration.Platform));
				entry = new Entry(config);
				dict.Add(solutionConfiguration, entry);
			}
			return entry;
		}
		
		/// <summary>
		/// Gets the project configuration corresponding to the given solution configuration.
		/// </summary>
		public ConfigurationAndPlatform GetProjectConfiguration(ConfigurationAndPlatform solutionConfiguration)
		{
			lock (dict) {
				Entry entry;
				if (dict.TryGetValue(solutionConfiguration, out entry)) {
					return entry.Config;
				} else {
					return new ConfigurationAndPlatform(solutionConfiguration.Configuration, MSBuildInternals.FixPlatformNameForProject(solutionConfiguration.Platform));
				}
			}
		}
		
		/// <summary>
		/// Sets the project configuration corresponding to the given solution configuration.
		/// </summary>
		public void SetProjectConfiguration(ConfigurationAndPlatform solutionConfiguration, ConfigurationAndPlatform projectConfiguration)
		{
			if (string.IsNullOrEmpty(projectConfiguration.Configuration))
				throw new ArgumentException("Invalid project configuration");
			if (string.IsNullOrEmpty(projectConfiguration.Platform))
				throw new ArgumentException("Invalid project platform");
			lock (dict) {
				GetOrCreateEntry(solutionConfiguration).Config = new ConfigurationAndPlatform(
					projectConfiguration.Configuration,
					MSBuildInternals.FixPlatformNameForProject(projectConfiguration.Platform)
				);
			}
			Changed(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Gets whether building the project is enabled in the given solution configuration.
		/// </summary>
		public bool IsBuildEnabled(ConfigurationAndPlatform solutionConfiguration)
		{
			lock (dict) {
				Entry entry;
				if (dict.TryGetValue(solutionConfiguration, out entry)) {
					return entry.Build;
				} else {
					return true;
				}
			}
		}
		
		/// <summary>
		/// Sets whether building the project is enabled in the given solution configuration.
		/// </summary>
		public void SetBuildEnabled(ConfigurationAndPlatform solutionConfiguration, bool value)
		{
			lock (dict) {
				GetOrCreateEntry(solutionConfiguration).Build = value;
			}
			Changed(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Gets whether deploying the project is enabled in the given solution configuration.
		/// </summary>
		public bool IsDeployEnabled(ConfigurationAndPlatform solutionConfiguration)
		{
			lock (dict) {
				Entry entry;
				if (dict.TryGetValue(solutionConfiguration, out entry)) {
					return entry.Deploy;
				} else {
					return false;
				}
			}
		}
		
		/// <summary>
		/// Sets whether deploying the project is enabled in the given solution configuration.
		/// </summary>
		public void SetDeployEnabled(ConfigurationAndPlatform solutionConfiguration, bool value)
		{
			lock (dict) {
				GetOrCreateEntry(solutionConfiguration).Deploy = value;
			}
			Changed(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Removes all data stored about the specified solution configuration.
		/// </summary>
		public void Remove(ConfigurationAndPlatform solutionConfiguration)
		{
			bool result;
			lock (dict) {
				result = dict.Remove(solutionConfiguration);
			}
			if (result)
				Changed(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Copies an entry.
		/// </summary>
		public void CopySolutionConfiguration(ConfigurationAndPlatform sourceSolutionConfiguration, ConfigurationAndPlatform targetSolutionConfiguration)
		{
			lock (dict) {
				Entry entry;
				if (dict.TryGetValue(sourceSolutionConfiguration, out entry)) {
					dict[targetSolutionConfiguration] = entry.Clone();
				}
			}
			Changed(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Renames an entry.
		/// </summary>
		public void RenameSolutionConfiguration(ConfigurationAndPlatform sourceSolutionConfiguration, ConfigurationAndPlatform targetSolutionConfiguration)
		{
			lock (dict) {
				Entry entry;
				if (dict.TryGetValue(sourceSolutionConfiguration, out entry)) {
					dict.Remove(sourceSolutionConfiguration);
					dict.Add(targetSolutionConfiguration, entry);
				}
			}
			Changed(this, EventArgs.Empty);
		}
	}
}
