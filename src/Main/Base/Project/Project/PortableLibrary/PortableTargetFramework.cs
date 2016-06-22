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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	public class PortableTargetFramework : TargetFramework
	{
		readonly string targetFrameworkVersion;
		readonly string targetFrameworkProfile;
		readonly string displayName;
		readonly IReadOnlyList<DomAssemblyName> redistList;
		
		/// <summary>
		/// Creates a new PortableTargetFramework instance for the given profile.
		/// </summary>
		public PortableTargetFramework(Profile profile)
		{
			this.targetFrameworkVersion = profile.TargetFrameworkVersion;
			this.targetFrameworkProfile = profile.TargetFrameworkProfile;
			this.displayName = profile.DisplayName;
			this.redistList = profile.ReferenceAssemblies;
		}
		
		/// <summary>
		/// Creates a new PortableTargetFramework for a profile that is not installed.
		/// </summary>
		public PortableTargetFramework(string targetFrameworkVersion, string targetFrameworkProfile)
		{
			this.targetFrameworkVersion = targetFrameworkVersion;
			this.targetFrameworkProfile = targetFrameworkProfile;
			this.displayName = Profile.PortableSubsetDisplayName + " (" + targetFrameworkVersion + "-" + targetFrameworkProfile + ")";
			this.redistList = EmptyList<DomAssemblyName>.Instance;
		}
		
		public override string TargetFrameworkVersion {
			get { return targetFrameworkVersion; }
		}
		
		public override string TargetFrameworkProfile {
			get { return targetFrameworkProfile; }
		}
		
		public override string DisplayName {
			get { return displayName; }
		}
		
		public override Version Version {
			get {
				switch (targetFrameworkVersion) {
					case "v4.0":
						return Versions.V4_0;
					case "v4.5":
					default:
						return Versions.V4_5;
				}
			}
		}
		
		public override Version MinimumMSBuildVersion {
			get {
				return new Version(4, 0);
			}
		}
		
		public override bool Equals(object obj)
		{
			PortableTargetFramework other = obj as PortableTargetFramework;
			if (other == null)
				return false;
			return this.TargetFrameworkVersion == other.TargetFrameworkVersion && this.TargetFrameworkProfile == other.TargetFrameworkProfile;
		}
		
		public override int GetHashCode()
		{
			return this.TargetFrameworkVersion.GetHashCode() ^ this.TargetFrameworkProfile.GetHashCode();
		}
	}
}
