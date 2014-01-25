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
using ICSharpCode.SharpDevelop.Project.Converter;

namespace ICSharpCode.SharpDevelop.Project
{
	public class TargetFramework
	{
		public readonly static TargetFramework Net20 = new TargetFramework("v2.0", ".NET Framework 2.0") {
			SupportedRuntimeVersion = "v2.0.50727",
			MinimumMSBuildVersion = new Version(2, 0),
			// .NET 2.0/3.0/3.5 can only be used if .NET 3.5 SP1 is installed
			IsAvailable = DotnetDetection.IsDotnet35SP1Installed
		};
		public readonly static TargetFramework Net30 = new TargetFramework("v3.0", ".NET Framework 3.0") {
			SupportedRuntimeVersion = "v2.0.50727",
			BasedOn = Net20,
			MinimumMSBuildVersion = new Version(3, 5)
		};
		public readonly static TargetFramework Net35 = new TargetFramework("v3.5", ".NET Framework 3.5") {
			SupportedRuntimeVersion = "v2.0.50727",
			BasedOn = Net30,
			MinimumMSBuildVersion = new Version(3, 5)
		};
		public readonly static TargetFramework Net35Client = new ClientProfileTargetFramework(Net35) {
			RequiresAppConfigEntry = true
		};
		public readonly static TargetFramework Net40 = new TargetFramework("v4.0", ".NET Framework 4.0") {
			BasedOn = Net35,
			MinimumMSBuildVersion = new Version(4, 0),
			SupportedSku = ".NETFramework,Version=v4.0",
			RequiresAppConfigEntry = true,
			IsAvailable = DotnetDetection.IsDotnet40Installed
		};
		public readonly static TargetFramework Net40Client = new ClientProfileTargetFramework(Net40) {
			BasedOn = Net35Client
		};
		public readonly static TargetFramework Net45 = new TargetFramework("v4.5", ".NET Framework 4.5") {
			BasedOn = Net40,
			MinimumMSBuildVersion = new Version(4, 0),
			SupportedRuntimeVersion = "v4.0",
			SupportedSku = ".NETFramework,Version=v4.5",
			RequiresAppConfigEntry = true,
			IsAvailable = DotnetDetection.IsDotnet45Installed
		};
		public readonly static TargetFramework Net451 = new TargetFramework("v4.5.1", ".NET Framework 4.5.1") {
			BasedOn = Net45,
			MinimumMSBuildVersion = new Version(4, 0),
			SupportedRuntimeVersion = "v4.0",
			SupportedSku = ".NETFramework,Version=v4.5.1",
			RequiresAppConfigEntry = true,
			IsAvailable = DotnetDetection.IsDotnet451Installed
		};
		
		public readonly static TargetFramework[] TargetFrameworks = {
			Net451, Net45, Net40, Net40Client, Net35, Net35Client, Net30, Net20
		};
		
		public readonly static TargetFramework DefaultTargetFramework = Net40Client;
		
		public static TargetFramework GetByName(string name)
		{
			foreach (TargetFramework tf in TargetFrameworks) {
				if (tf.Name == name)
					return tf;
			}
			throw new ArgumentException("No target framework '" + name + "' exists");
		}
		
		string name, displayName;
		
		public TargetFramework(string name, string displayName)
		{
			this.name = name;
			this.displayName = displayName;
			this.SupportedRuntimeVersion = name;
			this.IsAvailable = delegate {
				if (this.BasedOn != null)
					return this.BasedOn.IsAvailable();
				else
					return true;
			};
		}
		
		public string Name {
			get { return name; }
		}
		
		public string DisplayName {
			get { return displayName; }
		}
		
		/// <summary>
		/// Function that determines if this target framework is available.
		/// </summary>
		public Func<bool> IsAvailable { get; set; }
		
		/// <summary>
		/// Supported runtime version string for app.config
		/// </summary>
		public string SupportedRuntimeVersion { get; set; }
		
		/// <summary>
		/// Supported SKU string for app.config.
		/// </summary>
		public string SupportedSku { get; set; }
		
		/// <summary>
		/// Specifies whether this target framework requires an explicit app.config entry.
		/// </summary>
		public bool RequiresAppConfigEntry { get; set; }
		
		/// <summary>
		/// Gets the minimum MSBuild version required to build projects with this target framework.
		/// </summary>
		public Version MinimumMSBuildVersion { get; set; }
		
		/// <summary>
		/// Gets the previous release of this target framework.
		/// </summary>
		public TargetFramework BasedOn { get; set; }
		
		public virtual bool IsCompatibleWith(CompilerVersion compilerVersion)
		{
			return MinimumMSBuildVersion <= compilerVersion.MSBuildVersion;
		}
		
		public bool IsBasedOn(TargetFramework potentialBase)
		{
			TargetFramework tmp = this;
			while (tmp != null) {
				if (tmp == potentialBase)
					return true;
				tmp = tmp.BasedOn;
			}
			return false;
		}
		
		public override string ToString()
		{
			return DisplayName;
		}
		
		/// <summary>
		/// Shows a dialog to pick the target framework.
		/// This method is called by the UpgradeView 'convert' button to retrieve the actual target framework
		/// </summary>
		public virtual TargetFramework PickFramework(IEnumerable<IUpgradableProject> selectedProjects)
		{
			return this;
		}
	}
	
	public class ClientProfileTargetFramework : TargetFramework
	{
		public TargetFramework FullFramework { get; private set; }
		
		public ClientProfileTargetFramework(TargetFramework fullFramework)
			: base(fullFramework.Name + "Client", fullFramework.DisplayName + " Client Profile")
		{
			this.FullFramework = fullFramework;
			this.SupportedRuntimeVersion = fullFramework.SupportedRuntimeVersion;
			this.MinimumMSBuildVersion = fullFramework.MinimumMSBuildVersion;
			this.IsAvailable = fullFramework.IsAvailable;
			if (fullFramework.SupportedSku != null)
				this.SupportedSku = fullFramework.SupportedSku + ",Profile=Client";
			else
				this.SupportedSku = "Client";
		}
	}
}
