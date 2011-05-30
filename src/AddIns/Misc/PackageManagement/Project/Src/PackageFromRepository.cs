// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageFromRepository : IPackageFromRepository
	{
		IPackage package;
		bool? hasDependencies;
		
		public PackageFromRepository(IPackage package, IPackageRepository repository)
		{
			this.package = package;
			this.Repository = repository;
		}
		
		public IPackageRepository Repository { get; private set; }
		
		public IEnumerable<IPackageAssemblyReference> AssemblyReferences {
			get { return package.AssemblyReferences; }
		}
		
		public string Id {
			get { return package.Id; }
		}
		
		public Version Version {
			get { return package.Version; }
		}
		
		public string Title {
			get { return package.Title; }
		}
		
		public IEnumerable<string> Authors {
			get { return package.Authors; }
		}
		
		public IEnumerable<string> Owners {
			get { return package.Owners; }
		}
		
		public Uri IconUrl {
			get { return package.IconUrl; }
		}
		
		public Uri LicenseUrl {
			get { return package.LicenseUrl; }
		}
		
		public Uri ProjectUrl {
			get { return package.ProjectUrl;}
		}
		
		public bool RequireLicenseAcceptance {
			get { return package.RequireLicenseAcceptance; }
		}
		
		public string Description {
			get { return package.Description; }
		}
		
		public string Summary {
			get { return package.Summary; }
		}
		
		public string Language {
			get { return package.Language; }
		}
		
		public string Tags {
			get { return package.Tags; }
		}
		
		public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies {
			get { return package.FrameworkAssemblies; }
		}
		
		public IEnumerable<PackageDependency> Dependencies {
			get { return package.Dependencies; }
		}
		
		public Uri ReportAbuseUrl {
			get { return package.ReportAbuseUrl; }
		}
		
		public int DownloadCount {
			get { return package.DownloadCount; }
		}
		
		public int RatingsCount {
			get { return package.RatingsCount; }
		}
		
		public double Rating {
			get { return package.Rating; }
		}
		
		public IEnumerable<IPackageFile> GetFiles()
		{
			return package.GetFiles();
		}
		
		public Stream GetStream()
		{
			return package.GetStream();
		}
		
		public bool HasDependencies {
			get {
				if (!hasDependencies.HasValue) {
					IEnumerator<PackageDependency> enumerator = Dependencies.GetEnumerator();
					hasDependencies = enumerator.MoveNext();
				}
				return hasDependencies.Value;
			}
		}
	}
}
