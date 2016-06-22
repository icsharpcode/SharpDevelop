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
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// A project with support for the UpgradeView
	/// </summary>
	public interface IUpgradableProject
	{
		/// <summary>
		/// Gets the project name.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Gets whether an upgrade is desired (controls whether the upgrade view should pop
		/// up automatically)
		/// </summary>
		bool UpgradeDesired { get; }
		
		/// <summary>
		/// Gets the supported compiler versions.
		/// </summary>
		IEnumerable<CompilerVersion> GetAvailableCompilerVersions();
		
		/// <summary>
		/// Gets the supported target frameworks.
		/// </summary>
		IEnumerable<TargetFramework> GetAvailableTargetFrameworks();
		
		/// <summary>
		/// Gets the current compiler version.
		/// </summary>
		CompilerVersion CurrentCompilerVersion { get; }
		
		/// <summary>
		/// Gets the current target framework.
		/// </summary>
		TargetFramework CurrentTargetFramework { get; }
		
		/// <summary>
		/// Upgrades the selected compiler and target framework.
		/// </summary>
		/// <param name="newVersion">The new compiler version. If this property is null, the compiler version is not changed.</param>
		/// <param name="newFramework">The new target framework. If this property is null, the target framework is not changed.</param>
		/// <exception cref="ProjectUpgradeException">Upgrading the project failed.</exception>
		void UpgradeProject(CompilerVersion newVersion, TargetFramework newFramework);
	}
	
	public class CompilerVersion
	{
		public Version MSBuildVersion { get; private set; }
		public string DisplayName { get; private set; }
		
		public static readonly CompilerVersion MSBuild20 = new CompilerVersion(Versions.V2_0, "MSBuild 2.0");
		public static readonly CompilerVersion MSBuild35 = new CompilerVersion(Versions.V3_5, "MSBuild 3.5");
		public static readonly CompilerVersion MSBuild40 = new CompilerVersion(Versions.V4_0, "MSBuild 4.0");
		
		public CompilerVersion(Version msbuildVersion, string displayName)
		{
			if (msbuildVersion == null)
				throw new ArgumentNullException("msbuildVersion");
			this.MSBuildVersion = msbuildVersion;
			this.DisplayName = displayName;
		}
		
		public virtual bool CanTarget(TargetFramework targetFramework)
		{
			return targetFramework.MinimumMSBuildVersion <= this.MSBuildVersion;
		}
		
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (GetType() != obj.GetType())
				return false;
			CompilerVersion v = (CompilerVersion)obj;
			return this.MSBuildVersion == v.MSBuildVersion;
		}
		
		public override int GetHashCode()
		{
			return MSBuildVersion.GetHashCode();
		}
		
		public override string ToString()
		{
			return DisplayName;
		}
	}
}
