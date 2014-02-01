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
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Project.TargetFrameworks;

namespace ICSharpCode.SharpDevelop.Project
{
	public abstract class TargetFramework
	{
		public static readonly TargetFramework Net20 = new DotNet20();
		public static readonly TargetFramework Net30 = new DotNet30();
		public static readonly TargetFramework Net35 = new DotNet35();
		public static readonly TargetFramework Net35Client = new DotNet35Client();
		public static readonly TargetFramework Net40 = new DotNet4x(Versions.V4_0, RedistLists.Net40, DotnetDetection.IsDotnet40Installed);
		public static readonly TargetFramework Net40Client = new DotNet4xClient(Versions.V4_0, RedistLists.Net40Client, DotnetDetection.IsDotnet40Installed);
		public static readonly TargetFramework Net45 = new DotNet4x(Versions.V4_5, RedistLists.Net45, DotnetDetection.IsDotnet45Installed);
		public static readonly TargetFramework Net451 = new DotNet4x(Versions.V4_5_1, RedistLists.Net45, DotnetDetection.IsDotnet451Installed);
		
		/// <summary>
		/// Retrieves a target framework by a 'name'.
		/// Used by the .xpt project system; please do not use anywhere else.
		/// </summary>
		internal static TargetFramework GetByName(string name)
		{
			foreach (var fx in SD.ProjectService.TargetFrameworks) {
				// Yes, put version + profile together without any separator... that's how we indicated the Client profiles in SD 4.x and the .xpt file format
				if (fx.TargetFrameworkVersion + fx.TargetFrameworkProfile == name)
					return fx;
			}
			return null;
		}
		
		internal const string DefaultTargetFrameworkVersion = "v4.0";
		internal const string DefaultTargetFrameworkProfile = "";
		
		/// <summary>
		/// Gets the name of the target framework.
		/// This is used in the project's &lt;TargetFrameworkVersion&gt; element.
		/// </summary>
		public abstract string TargetFrameworkVersion { get; }
		
		/// <summary>
		/// Gets the profile of the target framework.
		/// This is used in the project's &lt;TargetFrameworkProfile&gt; element.
		/// 
		/// Returns the empty string if no profile name is in use.
		/// </summary>
		public virtual string TargetFrameworkProfile {
			get { return string.Empty; }
		}
		
		/// <summary>
		/// Gets the corresponding .NET desktop framework version.
		/// If this target framework is not a .NET desktop version, gets the closest corresponding version.
		/// </summary>
		public abstract Version Version { get; }
		
		/// <summary>
		/// Gets the display name of the target framework.
		/// </summary>
		public virtual string DisplayName {
			get { return this.TargetFrameworkVersion; }
		}
		
		/// <summary>
		/// Gets the minimum MSBuild version required to build projects with this target framework.
		/// </summary>
		public abstract Version MinimumMSBuildVersion { get; }
		
		/// <summary>
		/// Gets whether this is the MS.NET Framework for the desktop. (not Mono, not WinPhone/Win8 app/portable library/...)
		/// </summary>
		public virtual bool IsDesktopFramework {
			get { return false; }
		}
		
		public virtual bool Supports32BitPreferredOption {
			get { return Version >= Versions.V4_5; }
		}
		
		/// <summary>
		/// Supported runtime version string for app.config
		/// </summary>
		public virtual string SupportedRuntimeVersion {
			get { return null; }
		}
		
		/// <summary>
		/// Supported SKU string for app.config.
		/// </summary>
		public virtual string SupportedSku {
			get { return null; }
		}
		
		/// <summary>
		/// Specifies whether this target framework requires an explicit app.config entry.
		/// </summary>
		public virtual bool RequiresAppConfigEntry {
			get { return false; }
		}
		
		/// <summary>
		/// Gets whether this target framework in an "improved version" of the specified framework.
		/// This should usually return whether this framework is (mostly) backward-compatible with the specified framework.
		/// </summary>
		/// <remarks>The 'IsBasedOn' relation should be reflexive and transitive.</remarks>
		public virtual bool IsBasedOn(TargetFramework fx)
		{
			return fx == this;
		}
		
		/// <summary>
		/// Gets whether the runtime for the specified target framework is available on this machine.
		/// This method controls whether the target framework is visible to the user.
		/// 
		/// Note: for the desktop frameworks, this method tests for run-time availability; it does not check if the reference assemblies are present.
		/// </summary>
		public virtual bool IsAvailable()
		{
			return true;
		}
		
		/// <summary>
		/// Gets the solution format version corresponding to the older version of Visual Studio that supports this target framework. 
		/// </summary>
		/// <remarks>The default implementation of this property is based on the <see cref="MinimumMSBuildVersion"/> property.</remarks>
		public virtual SolutionFormatVersion MinimumSolutionVersion {
			get {
				if (MinimumMSBuildVersion <= Versions.V2_0)
					return SolutionFormatVersion.VS2005;
				else if (MinimumMSBuildVersion <= Versions.V3_5)
					return SolutionFormatVersion.VS2008;
				else
					return SolutionFormatVersion.VS2010;
			}
		}
		
		/* We might implement+use this API in the future if we want to notify the user about missing reference assemblies.
		/// <summary>
		/// Tests whether the reference assemblies for this framework are installed on this machine.
		/// </summary>
		public virtual bool ReferenceAssembliesAvailable()
		{
			return true;
		}
		
		/// <summary>
		/// Returns the URL where the reference assemblies can be download.
		/// May return null if the download location is unknown.
		/// </summary>
		public virtual Uri ReferenceAssemblyDownloadLocation {
			get { return null; }
		}
		
		/// <summary>
		/// Returns the name of the product that contains the reference assemblies for this framework. (for example: "Windows SDK x.y")
		/// May return null if the source for the reference assemblies is unknown.
		/// </summary>
		public virtual string ReferenceAssemblyDownloadVehicle {
			get { return null; }
		}
		 */
		
		/// <summary>
		/// Retrieves the list of reference assemblies for this framework.
		/// May return an empty list if the reference assemblies are not installed.
		/// </summary>
		public virtual IReadOnlyList<DomAssemblyName> ReferenceAssemblies {
			get {
				return EmptyList<DomAssemblyName>.Instance;
			}
		}
		
		/// <summary>
		/// Reads the 'RedistList/FrameworkList.xml' file with the specified file name.
		/// </summary>
		public static IReadOnlyList<DomAssemblyName> ReadRedistList(string redistListFileName)
		{
			List<DomAssemblyName> list = new List<DomAssemblyName>();
			XDocument doc = XDocument.Load(redistListFileName);
			foreach (var file in doc.Root.Elements()) {
				string assemblyName = (string)file.Attribute("AssemblyName");
				string version = (string)file.Attribute("Version");
				string publicKeyToken = (string)file.Attribute("PublicKeyToken");
				string culture = (string)file.Attribute("Culture");
				//string processorArchitecture = (string)file.Attribute("ProcessorArchitecture");
				if ((string)file.Attribute("InGAC") == "false" || (string)file.Attribute("InGac") == "false") {
					// Ignore assemblies not in GAC.
					// Note that casing of 'InGAC'/'InGac' is inconsistent between different .NET versions
					continue;
				}
				list.Add(new DomAssemblyName(assemblyName, Version.Parse(version), publicKeyToken, culture));
			}
			return list;
		}
		
		/// <summary>
		/// Shows a dialog to pick the target framework.
		/// This method is called by the UpgradeView 'convert' button to retrieve the actual target framework
		/// </summary>
		public virtual TargetFramework PickFramework(IEnumerable<IUpgradableProject> selectedProjects)
		{
			return this;
		}
		
		public override string ToString()
		{
			return DisplayName;
		}
	}
	
	/*
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
	}*/
}
