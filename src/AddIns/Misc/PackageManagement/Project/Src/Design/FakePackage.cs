// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackage : IPackageFromRepository
	{		
		public Stream Stream = null;
		public List<string> AuthorsList = new List<string>();
		public List<string> OwnersList = new List<string>();
		public List<IPackageFile> FilesList = new List<IPackageFile>();
		
		public List<PackageDependency> DependenciesList = 
			new List<PackageDependency>();
		
		public List<IPackageAssemblyReference> AssemblyReferenceList =
			new List<IPackageAssemblyReference>();
		
		public FakePackage()
			: this(String.Empty)
		{
		}
		
		public FakePackage(string id)
			: this(id, "1.0.0.0")
		{
		}
		
		public FakePackage(string id, string version)
		{
			this.Id = id;
			this.Description = String.Empty;
			this.Version = new SemanticVersion(version);
			this.Listed = true;
			this.IsLatestVersion = true;
		}
		
		public static FakePackage CreatePackageWithVersion(string version)
		{
			return CreatePackageWithVersion("Test", version);
		}
		
		public static FakePackage CreatePackageWithVersion(string id, string version)
		{
			return new FakePackage(id, version);
		}
		
		public string Id { get; set; }
		public SemanticVersion Version { get; set; }
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
			get { return AssemblyReferenceList; }
		}
		
		public IEnumerable<string> Authors {
			get { return AuthorsList; }
		}
		
		public IEnumerable<string> Owners {
			get { return OwnersList; }
		}
		
		public IEnumerable<IPackageFile> GetFiles()
		{
			return FilesList;
		}
		
		public Stream GetStream()
		{
			return Stream;
		}
		
		public override string ToString()
		{
			return String.Format("{0} {1}", Id, Version);
		}
		
		public override bool Equals(object obj)
		{
			IPackage rhs = obj as IPackage;
			if (rhs != null) {
				return (Id == rhs.Id) && (Version == rhs.Version);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public void AddAuthor(string author)
		{
			AuthorsList.Add(author);
		}
		
		public void AddDependency(string id, SemanticVersion minVersion, SemanticVersion maxVersion)
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
		
		public List<FrameworkAssemblyReference> FrameworkAssembliesList = 
			new List<FrameworkAssemblyReference>();
		
		public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies {
			get { return FrameworkAssembliesList; }
		}
		
		public FakePackageRepository FakePackageRepository = new FakePackageRepository();
		
		public IPackageRepository Repository {
			get { return FakePackageRepository; }
		}
		
		public bool HasDependencies { get; set; }
		
		public void AddFile(string fileName)
		{
			var file = new PhysicalPackageFile();
			file.TargetPath = fileName;
			FilesList.Add(file);
		}
		
		public DateTime? LastUpdated { get; set; }
		public bool IsLatestVersion { get; set; }
		public Nullable<DateTimeOffset> Published { get; set; }
		public string ReleaseNotes { get; set; }
		public string Copyright { get; set; }
		public bool IsAbsoluteLatestVersion { get; set; }
		public bool Listed { get; set; }
		
		public IEnumerable<PackageDependencySet> DependencySets {
			get {
				return new PackageDependencySet[] {
					new PackageDependencySet(null, DependenciesList)
				};
			}
		}
		
		public List<FrameworkName> SupportedFrameworks = new List<FrameworkName>();
		
		public FrameworkName AddSupportedFramework(string identifier)
		{
			var framework = new FrameworkName(identifier);
			SupportedFrameworks.Add(framework);
			return framework;
		}
		
		public IEnumerable<FrameworkName> GetSupportedFrameworks()
		{
			return SupportedFrameworks;
		}
	}
}
