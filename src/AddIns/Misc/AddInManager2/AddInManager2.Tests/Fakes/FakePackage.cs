// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	public class FakePackage : IPackage
	{
		public FakePackage()
		{
		}
		
		public bool IsAbsoluteLatestVersion
		{
			get;
			set;
		}
		
		public bool IsLatestVersion
		{
			get;
			set;
		}
		
		public bool Listed
		{
			get;
			set;
		}
		
		public Nullable<DateTimeOffset> Published
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<IPackageAssemblyReference> AssemblyReferences
		{
			get;
			set;
		}
		
		public string Id
		{
			get;
			set;
		}
		
		public SemanticVersion Version
		{
			get;
			set;
		}
		
		public string Title
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<string> Authors
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<string> Owners
		{
			get;
			set;
		}
		
		public Uri IconUrl
		{
			get;
			set;
		}
		
		public Uri LicenseUrl
		{
			get;
			set;
		}
		
		public Uri ProjectUrl
		{
			get;
			set;
		}
		
		public bool RequireLicenseAcceptance
		{
			get;
			set;
		}
		
		public string Description
		{
			get;
			set;
		}
		
		public string Summary
		{
			get;
			set;
		}
		
		public string ReleaseNotes
		{
			get;
			set;
		}
		
		public string Language
		{
			get;
			set;
		}
		
		public string Tags
		{
			get;
			set;
		}
		
		public string Copyright
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<PackageDependencySet> DependencySets
		{
			get;
			set;
		}
		
		public Uri ReportAbuseUrl
		{
			get;
			set;
		}
		
		public int DownloadCount
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<IPackageFile> GetFiles()
		{
			return null;
		}
		
		public System.Collections.Generic.IEnumerable<System.Runtime.Versioning.FrameworkName> GetSupportedFrameworks()
		{
			return null;
		}
		
		public System.IO.Stream GetStream()
		{
			return null;
		}
		
		public System.Collections.Generic.ICollection<PackageReferenceSet> PackageAssemblyReferences {
			get {
				throw new NotImplementedException();
			}
		}
		
		public Version MinClientVersion {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override string ToString()
		{
			return string.Format("[FakePackage Id={0}, Version={1}]", Id, Version);
		}
	}
}
