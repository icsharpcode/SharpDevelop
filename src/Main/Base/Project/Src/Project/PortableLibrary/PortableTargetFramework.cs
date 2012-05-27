// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ICSharpCode.SharpDevelop.Project.PortableLibrary
{
	public class PortableTargetFramework : TargetFramework
	{
		public readonly string TargetFrameworkVersion;
		public readonly string TargetFrameworkProfile;
		
		public PortableTargetFramework(string targetFrameworkVersion, string targetFrameworkProfile)
			: base(targetFrameworkVersion + "-" + targetFrameworkProfile, Profile.PortableSubsetDisplayName + " (" + targetFrameworkVersion + "-" + targetFrameworkProfile + ")")
		{
			this.TargetFrameworkVersion = targetFrameworkVersion;
			this.TargetFrameworkProfile = targetFrameworkProfile;
			this.MinimumMSBuildVersion = new Version(4, 0);
		}
		
		public PortableTargetFramework(Profile profile)
			: base(profile.TargetFrameworkVersion + "-" + profile.TargetFrameworkProfile, profile.DisplayName)
		{
			this.TargetFrameworkVersion = profile.TargetFrameworkVersion;
			this.TargetFrameworkProfile = profile.TargetFrameworkProfile;
			this.MinimumMSBuildVersion = new Version(4, 0);
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
