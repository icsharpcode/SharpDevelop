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

namespace ICSharpCode.SharpDevelop.Project.TargetFrameworks
{
	/// <summary>
	/// Desktop Framework based on the .NET 2.0 runtime. (.NET 2.x/3.x)
	/// </summary>
	abstract class DotNet2x : TargetFramework
	{
		public override bool IsDesktopFramework {
			get { return true; }
		}
		
		public override string SupportedRuntimeVersion {
			get { return "v2.0.50727"; }
		}
		
		public override bool IsAvailable()
		{
			// .NET 2.0/3.0/3.5 can only be used if .NET 3.5 SP1 is installed
			return DotnetDetection.IsDotnet35SP1Installed();
		}
		
		public override bool IsBasedOn(TargetFramework fx)
		{
			// This implementation is used for all non-client versions of .NET 2.x/3.x
			return fx != null && fx.IsDesktopFramework && fx.Version <= this.Version;
		}
	}
	
	sealed class DotNet20 : DotNet2x
	{
		public override string TargetFrameworkVersion {
			get { return "v2.0"; }
		}
		
		public override string DisplayName {
			get { return ".NET Framework 2.0";}
		}
		
		public override Version Version {
			get { return Versions.V2_0; }
		}
		
		public override Version MinimumMSBuildVersion {
			get { return Versions.V2_0; }
		}
		
		public override IReadOnlyList<DomAssemblyName> ReferenceAssemblies {
			get { return RedistLists.Net20; }
		}
	}
	
	sealed class DotNet30 : DotNet2x
	{
		public override string TargetFrameworkVersion {
			get { return "v3.0"; }
		}
		
		public override string DisplayName {
			get { return ".NET Framework 3.0";}
		}
		
		public override Version Version {
			get { return Versions.V3_0; }
		}
		
		public override Version MinimumMSBuildVersion {
			get { return Versions.V3_5; }
		}
		
		public override IReadOnlyList<DomAssemblyName> ReferenceAssemblies {
			get { return RedistLists.Net30; }
		}
	}
	
	sealed class DotNet35 : DotNet2x
	{
		public override string TargetFrameworkVersion {
			get { return "v3.5"; }
		}
		
		public override string DisplayName {
			get { return ".NET Framework 3.5";}
		}
		
		public override Version Version {
			get { return Versions.V3_5; }
		}
		
		public override Version MinimumMSBuildVersion {
			get { return Versions.V3_5; }
		}
		
		public override IReadOnlyList<DomAssemblyName> ReferenceAssemblies {
			get { return RedistLists.Net35; }
		}
	}
	
	sealed class DotNet35Client : DotNet2x
	{
		public override string TargetFrameworkVersion {
			get { return "v3.5"; }
		}
		
		public override string TargetFrameworkProfile {
			get { return "Client"; }
		}
		
		public override string DisplayName {
			get { return ".NET Framework 3.5 Client Profile";}
		}
		
		public override string SupportedSku {
			get { return "Client"; }
		}
		
		public override bool RequiresAppConfigEntry {
			get { return true; }
		}
		
		public override Version Version {
			get { return Versions.V3_5; }
		}
		
		public override Version MinimumMSBuildVersion {
			get { return Versions.V3_5; }
		}
		
		public override IReadOnlyList<DomAssemblyName> ReferenceAssemblies {
			get { return RedistLists.Net35Client; }
		}
		
		public override bool IsBasedOn(TargetFramework fx)
		{
			// 3.5 Client was the first client profile
			return fx == this;
		}
	}
}
