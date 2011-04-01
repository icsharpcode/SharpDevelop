// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet(VerbsCommon.Get, "Package", DefaultParameterSetName = ParameterAttribute.AllParameterSets)]
	public class GetPackageCmdlet : PackageManagementCmdlet
	{
		int? skip;
		int? first;
		
		public GetPackageCmdlet()
			: this(
				ServiceLocator.PackageManagementService,
				ServiceLocator.PackageManagementConsoleHost,
				null)
		{
		}
		
		public GetPackageCmdlet(
			IPackageManagementService packageManagementService,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(packageManagementService, consoleHost, terminatingError)
		{
		}
		
		[Alias("Online", "Remote")]
		[Parameter(ParameterSetName = "Available")]
		public SwitchParameter ListAvailable { get; set; }
		
		[Parameter(ParameterSetName = "Updated")]
		public SwitchParameter Updates { get; set; }
		
		[Parameter(Position = 0)]
		public string Filter { get; set; }
		
		[Parameter(ParameterSetName = "Available")]
		[Parameter(ParameterSetName = "Updated")]
		public string Source { get; set; }
		
		[Parameter(ParameterSetName = "Recent")]
		public SwitchParameter Recent { get; set; }
		
		[Parameter]
		[ValidateRange(0, Int32.MaxValue)]
		public int Skip {
			get { return skip.GetValueOrDefault(); }
			set { skip = value; }
		}
		
		[Alias("Take")]
		[Parameter]
		[ValidateRange(0, Int32.MaxValue)]
		public int First {
			get { return first.GetValueOrDefault(); }
			set { first = value; }
		}
		
		protected override void ProcessRecord()
		{
			ValidateParameters();
			
			IQueryable<IPackage> packages = GetPackages();
			packages = OrderPackages(packages);
			packages = SelectPackageRange(packages);
			WritePackagesToOutputPipeline(packages);
		}
		
		void ValidateParameters()
		{
			if (ParametersRequireProject()) {
				ThrowErrorIfProjectNotOpen();
			}
		}
		
		bool ParametersRequireProject()
		{
			if (ListAvailable.IsPresent || Recent.IsPresent) {
				return false;
			}
			return true;
		}
		
		protected virtual void CmdletThrowTerminatingError(ErrorRecord errorRecord)
		{
			ThrowTerminatingError(errorRecord);
		}
		
		IQueryable<IPackage> GetPackages()
		{
			if (ListAvailable.IsPresent) {
				return GetAvailablePackages();
			} else if (Updates.IsPresent) {
				return GetUpdatedPackages();
			} else if (Recent.IsPresent) {
				return GetRecentPackages();
			}
			return GetInstalledPackages();
		}
		
		IQueryable<IPackage> OrderPackages(IQueryable<IPackage> packages)
		{
			return packages.OrderBy(package => package.Id);
		}
		
		IQueryable<IPackage> SelectPackageRange(IQueryable<IPackage> packages)
		{
			if (skip.HasValue) {
				packages = packages.Skip(skip.Value);
			}
			if (first.HasValue) {
				packages = packages.Take(first.Value);
			}
			return packages;
		}
		
		IQueryable<IPackage> GetAvailablePackages()
		{
			IPackageRepository repository = CreatePackageRepositoryForActivePackageSource();
			IQueryable<IPackage> packages = repository.GetPackages();
			return FilterPackages(packages);
		}
		
		IPackageRepository CreatePackageRepositoryForActivePackageSource()
		{
			PackageSource source = GetActivePackageSource(Source);
			return PackageManagementService.CreatePackageRepository(source);
		}
		
		IQueryable<IPackage> FilterPackages(IQueryable<IPackage> packages)
		{
			if (Filter != null) {
				string[] searchTerms = Filter.Split(' ');
				return packages.Find(searchTerms);
			}
			return packages;
		}
		
		IQueryable<IPackage> GetUpdatedPackages()
		{
			var updatedPackages = new UpdatedPackages(PackageManagementService, DefaultProject);
			updatedPackages.SearchTerms = Filter;
			return updatedPackages.GetUpdatedPackages().AsQueryable();
		}
		
		IQueryable<IPackage> GetInstalledPackages()
		{
			ISharpDevelopProjectManager projectManager = CreateProjectManagerForActiveProject();
			IQueryable<IPackage> packages = projectManager.LocalRepository.GetPackages();
			return FilterPackages(packages);
		}
		
		ISharpDevelopProjectManager CreateProjectManagerForActiveProject()
		{
			IPackageRepository repository = CreatePackageRepositoryForActivePackageSource();
			return CreateProjectManagerForActiveProject(repository);
		}
		
		ISharpDevelopProjectManager CreateProjectManagerForActiveProject(IPackageRepository repository)
		{
			return PackageManagementService.CreateProjectManager(repository, DefaultProject);
		}
		
		IQueryable<IPackage> GetRecentPackages()
		{
			IQueryable<IPackage> packages = PackageManagementService.RecentPackageRepository.GetPackages();
			return FilterPackages(packages);
		}
		
		void WritePackagesToOutputPipeline(IQueryable<IPackage> packages)
		{
			foreach (IPackage package in packages) {
				WriteObject(package);
			}
		}
	}
}
