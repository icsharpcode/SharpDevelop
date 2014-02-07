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
		IRegisteredPackageRepositories registeredPackageRepositories;
		
		public GetPackageCmdlet()
			: this(
				PackageManagementServices.RegisteredPackageRepositories,
				PackageManagementServices.ConsoleHost,
				null)
		{
		}
		
		public GetPackageCmdlet(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementConsoleHost consoleHost,
			ICmdletTerminatingError terminatingError)
			: base(consoleHost, terminatingError)
		{
			this.registeredPackageRepositories = registeredPackageRepositories;
		}
		
		[Alias("Online", "Remote")]
		[Parameter(ParameterSetName = "Available")]
		public SwitchParameter ListAvailable { get; set; }
		
		[Parameter(ParameterSetName = "Updated")]
		public SwitchParameter Updates { get; set; }
		
		[Parameter(ParameterSetName = "Available")]
		[Parameter(ParameterSetName = "Updated")]
		[Alias("Prerelease")]
		public SwitchParameter IncludePrerelease { get; set; }
		
		[Parameter(ParameterSetName = "Available")]
		[Parameter(ParameterSetName = "Updated")]
		public SwitchParameter AllVersions { get; set; }
		
		[Parameter(Position = 0)]
		public string Filter { get; set; }
		
		[Parameter(Position = 1, ParameterSetName = "Project")]
		public string ProjectName { get; set; }
		
		[Parameter(ParameterSetName = "Available")]
		[Parameter(ParameterSetName = "Updated")]
		public string Source { get; set; }
		
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
			IEnumerable<IPackage> packages = GetPackagesForDisplay();
			WritePackagesToOutputPipeline(packages);
		}
		
		IEnumerable<IPackage> GetPackagesForDisplay()
		{
			IQueryable<IPackage> packages = GetPackages();
			packages = OrderPackages(packages);
			IEnumerable<IPackage> distinctPackages = DistinctPackagesById(packages);
			return SelectPackageRange(distinctPackages);
		}
		
		IEnumerable<IPackage> DistinctPackagesById(IQueryable<IPackage> packages)
		{
			if (ListAvailable && !AllVersions) {
				return packages.DistinctLast<IPackage>(PackageEqualityComparer.Id);
			}
			return packages;
		}
		
		void ValidateParameters()
		{
			if (ParametersRequireProject()) {
				ThrowErrorIfProjectNotOpen();
			}
		}
		
		bool ParametersRequireProject()
		{
			if (ListAvailable) {
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
			if (ListAvailable) {
				return GetAvailablePackages();
			} else if (Updates) {
				return GetUpdatedPackages();
			}
			return GetInstalledPackages();
		}
		
		IQueryable<IPackage> OrderPackages(IQueryable<IPackage> packages)
		{
			return packages.OrderBy(package => package.Id);
		}
		
		IEnumerable<IPackage> SelectPackageRange(IEnumerable<IPackage> packages)
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
			PackageSource packageSource = ConsoleHost.GetActivePackageSource(Source);
			return registeredPackageRepositories.CreateRepository(packageSource);
		}
		
		IQueryable<IPackage> FilterPackages(IQueryable<IPackage> packages)
		{
			IQueryable<IPackage> filteredPackages = packages.Find(Filter);
			if (IncludePrerelease || AllVersions) {
				return filteredPackages;
			}
			return filteredPackages.Where(package => package.IsLatestVersion);
		}
		
		IQueryable<IPackage> GetUpdatedPackages()
		{
			IPackageRepository repository = CreatePackageRepositoryForActivePackageSource();
			UpdatedPackages updatedPackages = CreateUpdatedPackages(repository);
			updatedPackages.SearchTerms = Filter;
			return updatedPackages
				.GetUpdatedPackages(IncludePrerelease)
				.AsQueryable();
		}
		
		UpdatedPackages CreateUpdatedPackages(IPackageRepository repository)
		{
			IPackageManagementProject project = GetSelectedProject(repository);
			if (project != null) {
				return new UpdatedPackages(project, repository);
			}
			return new UpdatedPackages(GetSolutionPackages(), repository);
		}
		
		IQueryable<IPackage> GetSolutionPackages()
		{
			return ConsoleHost.Solution.GetPackages();
		}
		
		IPackageManagementProject GetSelectedProject(IPackageRepository repository)
		{
			if (HasSelectedProjectName()) {
				return ConsoleHost.GetProject(repository, ProjectName);
			}
			return null;
		}
		
		bool HasSelectedProjectName()
		{
			return ProjectName != null;
		}
		
		IQueryable<IPackage> GetInstalledPackages()
		{
			IQueryable<IPackage> packages = GetPackagesFromSelectedProjectOrSolution();
			return FilterPackages(packages);
		}
		
		IQueryable<IPackage> GetPackagesFromSelectedProjectOrSolution()
		{
			IPackageManagementProject project = GetSelectedProject();
			if (project != null) {
				return project.GetPackages();
			}
			return GetSolutionPackages();
		}
		
		IPackageManagementProject GetSelectedProject()
		{
			if (HasSelectedProjectName()) {
				return ConsoleHost.GetProject(Source, ProjectName);
			}
			return null;
		}
		
		void WritePackagesToOutputPipeline(IEnumerable<IPackage> packages)
		{
			foreach (IPackage package in packages) {
				WriteObject(package);
			}
		}
	}
}
