// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Holds reference to currently used NuGet package repositories to install AddIns from.
	/// </summary>
	public class PackageRepositories : IPackageRepositories
	{
		private IPackageRepository _currentRepository;
		private IPackageRepository _activeRepository;
		private PackageSource _activeSource;
		private List<PackageSource> _registeredPackageSources;

		public PackageRepositories()
		{
			_registeredPackageSources = new List<PackageSource>();
			LoadPackageSources();
			UpdateCurrentRepository();
			UpdateActiveRepository();
		}

		public IPackageRepository Registered
		{
			get
			{
				return _currentRepository;
			}
		}
		
		public IPackageRepository Active
		{
			get
			{
				return _activeRepository;
			}
		}
		
		public PackageSource ActiveSource
		{
			get
			{
				return _activeSource;
			}
			set
			{
				_activeSource = value;
				UpdateActiveRepository();
			}
		}
		
		public IEnumerable<PackageSource> RegisteredPackageSources
		{
			get
			{
				return _registeredPackageSources;
			}
			set
			{
				_registeredPackageSources.Clear();
				_registeredPackageSources.AddRange(value);
				SavePackageSources();
			}
		}
		
		private void LoadPackageSources()
		{
			_registeredPackageSources.Clear();
			var savedRepositories = SD.PropertyService.Get<string[]>("AddInManager2.PackageRepositories", null);
			if ((savedRepositories != null) && (savedRepositories.Length > 0))
			{
				foreach (string repositoryEntry in savedRepositories)
				{
					string[] splittedEntry = repositoryEntry.Split(new char[] { '=' }, 2);
					if ((splittedEntry != null) && (splittedEntry.Length == 2))
					{
						// Create PackageSource from this entry
						PackageSource savedPackageSource = new PackageSource(splittedEntry[1], splittedEntry[0]);
						_registeredPackageSources.Add(savedPackageSource);
					}
				}
			}
			else
			{
				// If we don't have any repositories, so add the default one
				PackageSource defaultPackageSource =
					new PackageSource("https://nuget.org/api/v2", ResourceService.GetString("AddInManager2.DefaultRepository"));
				_registeredPackageSources.Add(defaultPackageSource);
				SavePackageSources();
			}
		}
		
		private void SavePackageSources()
		{
			var savedRepositories = _registeredPackageSources.Select(ps => ps.Name + "=" + ps.Source);
			PropertyService.Set<string[]>("AddInManager2.PackageRepositories", savedRepositories.ToArray());
			UpdateCurrentRepository();
		}
		
		private void UpdateCurrentRepository()
		{
			var repositories =
				_registeredPackageSources.Select(packageSource => PackageRepositoryFactory.Default.CreateRepository(packageSource.Source));
			if (repositories.Any())
			{
				_currentRepository = new AggregateRepository(repositories);
			}
		}
		
		private void UpdateActiveRepository()
		{
			if ((_activeSource == null) && (_registeredPackageSources != null))
			{
				_activeSource = _registeredPackageSources[0];
			}
			
			if (_activeSource != null)
			{
				_activeRepository = PackageRepositoryFactory.Default.CreateRepository(_activeSource.Source);
			}
			else
			{
				// If no active repository is set, get packages from all repositories
				_activeRepository = _currentRepository;
			}
		}
	}
}
