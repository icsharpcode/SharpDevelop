// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RecentPackageInfo
	{
		SemanticVersion version;
		
		public RecentPackageInfo()
		{
		}
		
		public RecentPackageInfo(IPackage package)
			: this(package.Id, package.Version)
		{
		}
		
		public RecentPackageInfo(string id, SemanticVersion version)
		{
			this.Id = id;
			this.version = version;
		}
		
		public string Id { get; set; }
		
		public string Version {
			get { return version.ToString(); }
			set { version = new SemanticVersion(value); }
		}
		
		public override string ToString()
		{
			return String.Format("[RecentPackageInfo Id={0}, Version={1}]", Id, Version);
		}
		
		public bool IsMatch(IPackage package)
		{
			return (package.Version.ToString() == Version) && (package.Id == Id);
		}
	}
}
