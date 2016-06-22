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
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Desktop Framework based on the .NET 4.0 runtime.
	/// The client profile framework derives from this class.
	/// </summary>
	class DotNet4x : TargetFramework
	{
		readonly Version version;
		readonly string targetFrameworkVersion;
		readonly string displayName;
		readonly IReadOnlyList<DomAssemblyName> redistList;
		readonly Func<bool> isAvailable;
		
		public DotNet4x(Version version, IReadOnlyList<DomAssemblyName> redistList, Func<bool> isAvailable)
		{
			this.version = version;
			this.targetFrameworkVersion = "v" + version;
			this.displayName = ".NET Framework " + version;
			this.redistList = redistList;
			this.isAvailable = isAvailable;
		}
		
		public override string TargetFrameworkVersion {
			get { return targetFrameworkVersion; }
		}
		
		public override string DisplayName {
			get { return displayName; }
		}
		
		public override Version Version {
			get { return version; }
		}
		
		public override bool IsAvailable()
		{
			return isAvailable();
		}
		
		public override Version MinimumMSBuildVersion {
			get { return Versions.V4_0; }
		}
		
		public override string SupportedRuntimeVersion {
			get { return "v4.0"; }
		}
		
		public override string SupportedSku {
			get {
				string sku = ".NETFramework,Version=" + this.TargetFrameworkVersion;
				if (!string.IsNullOrEmpty(this.TargetFrameworkProfile))
					sku += ",Profile=" + this.TargetFrameworkProfile;
				return sku;
			}
		}
		
		public override bool RequiresAppConfigEntry {
			get { return true; }
		}
		
		public override bool IsDesktopFramework {
			get { return true; }
		}
		
		public override IReadOnlyList<DomAssemblyName> ReferenceAssemblies {
			get { return redistList; }
		}
		
		public override bool IsBasedOn(TargetFramework fx)
		{
			// This implementation is used for all non-client versions of .NET 4.x
			return fx != null && fx.IsDesktopFramework && fx.Version <= this.Version;
		}
		
		public override SolutionFormatVersion MinimumSolutionVersion {
			get {
				if (this.Version <= Versions.V4_0)
					return SolutionFormatVersion.VS2010;
				else
					return SolutionFormatVersion.VS2012;
			}
		}
	}
	
	/// <summary>
	/// Desktop Framework based on the .NET 4.0 runtime, client profile.
	/// </summary>
	class DotNet4xClient : DotNet4x
	{
		public DotNet4xClient(Version version, IReadOnlyList<DomAssemblyName> redistList, Func<bool> isAvailable)
			: base(version, redistList, isAvailable)
		{
		}
		
		public override string DisplayName {
			get {
				return base.DisplayName + " Client Profile";
			}
		}
		
		public override string TargetFrameworkProfile {
			get { return "Client"; }
		}
		
		public override bool IsBasedOn(TargetFramework fx)
		{
			// This implementation is used for all non-client versions of .NET 4.x
			return fx != null && fx.IsDesktopFramework && fx.TargetFrameworkProfile == "Client" && fx.Version <= this.Version;
		}
	}
}
