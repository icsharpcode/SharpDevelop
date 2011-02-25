// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackage : IPackage
	{		
		List<string> authors = new List<string>();
		List<string> owners = new List<string>();
		Stream stream = null;
		List<IPackageFile> files = new List<IPackageFile>();
		List<IPackageAssemblyReference> assemblyReferences = 
			new List<IPackageAssemblyReference>();
		
		public List<PackageDependency> DependenciesList = 
			new List<PackageDependency>();
		
		public FakePackage()
			: this(String.Empty)
		{
		}
		
		public FakePackage(string id)
		{
			this.Id = id;
			this.Description = String.Empty;
			this.Version = new Version(1, 0, 0, 0);
		}
		
		public string Id { get; set; }
		public Version Version { get; set; }
		public string Title { get; set; }		
		public Uri IconUrl { get; set; }
		public Uri LicenseUrl { get; set; }
		public Uri ProjectUrl { get; set; }
		public bool RequireLicenseAcceptance { get; set; }
		public string Description { get; set; }
		public string Summary { get; set; }
		public string Language { get; set; }
		public string Tags { get; set; }
		public Uri ReportAbuseUrl { get; set; }
		public int DownloadCount { get; set; }
		public int RatingsCount { get; set; }
		public double Rating { get; set; }
		
		public IEnumerable<IPackageAssemblyReference> AssemblyReferences {
			get { return assemblyReferences; }
		}
		
		public IEnumerable<string> Authors {
			get { return authors; }
		}
		
		public IEnumerable<string> Owners {
			get { return owners; }
		}
		
		public IEnumerable<PackageDependency> Dependencies {
			get { return DependenciesList; }
		}
		
		public IEnumerable<IPackageFile> GetFiles()
		{
			return files;
		}
		
		public Stream GetStream()
		{
			return stream;
		}
		
		public override string ToString()
		{
			return String.Format("{0} {1}", Id, Version);
		}
		
		public override bool Equals(object obj)
		{
			FakePackage rhs = obj as FakePackage;
			return (Id == rhs.Id) && (Version == rhs.Version);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public void AddAuthor(string author)
		{
			authors.Add(author);
		}
		
		public void AddDependency(string id, Version minVersion, Version maxVersion)
		{
			var versionSpec = new VersionSpec();
			versionSpec.MinVersion = minVersion;
			versionSpec.MaxVersion = maxVersion;
			var dependency = new PackageDependency(id, versionSpec);
			DependenciesList.Add(dependency);
		}
		
		public void AddDependency(string id)
		{
			DependenciesList.Add(new PackageDependency(id));
		}
	}
}
