// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
		private IPackageRepository _aggregatedRepository;
		private List<PackageSource> _registeredPackageSources;
		private IEnumerable<IPackageRepository> _registeredPackageRepositories;
		
		private IAddInManagerEvents _events;
		private IAddInManagerSettings _settings;
		
		public const string DefaultRepositoryName = "SharpDevelop AddIn Repository";
		public const string DefaultRepositorySource = "https://www.myget.org/F/sharpdevelop/";

		public PackageRepositories(IAddInManagerEvents events, IAddInManagerSettings settings)
		{
			_events = events;
			_settings = settings;
			
			_registeredPackageSources = new List<PackageSource>();
			
			LoadPackageSources();
			UpdateCurrentRepository();
		}

		public IPackageRepository AllRegistered
		{
			get
			{
				return _aggregatedRepository;
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
				if (value != null)
				{
					_registeredPackageSources.AddRange(value);
				}
				SavePackageSources();
				
				// Send around the update
				_events.OnPackageSourcesChanged(new EventArgs());
			}
		}
		
		public IEnumerable<IPackageRepository> RegisteredPackageRepositories
		{
			get
			{
				return _registeredPackageRepositories;
			}
		}
		
		public IPackageRepository GetRepositoryFromSource(PackageSource packageSource)
		{
			IPackageRepository resultRepository = null;
			if (packageSource != null)
			{
				resultRepository = PackageRepositoryFactory.Default.CreateRepository(packageSource.Source);
			}
			else
			{
				// If no active repository is set, get packages from all repositories
				resultRepository = _aggregatedRepository;
			}
			
			return resultRepository;
		}
		
		private void LoadPackageSources()
		{
			_registeredPackageSources.Clear();
			var savedRepositories = _settings.PackageRepositories;
			if ((savedRepositories != null) && (savedRepositories.Length > 0))
			{
				foreach (string repositoryEntry in savedRepositories)
				{
					string[] splittedEntry = repositoryEntry.Split(new char[] { '=' }, 2);
					if ((splittedEntry != null) && (splittedEntry.Length == 2))
					{
						if (!String.IsNullOrEmpty(splittedEntry[0]) && !String.IsNullOrEmpty(splittedEntry[1]))
						{
							// Create PackageSource from this entry
							try
							{
								PackageSource savedPackageSource = new PackageSource(splittedEntry[1], splittedEntry[0]);
								_registeredPackageSources.Add(savedPackageSource);
							}
							catch (Exception)
							{
								SD.Log.WarnFormatted("[AddInManager2] URL '{0}' can't be used as valid package source.", splittedEntry[1]);
							}
						}
					}
				}
			}
			
			AddDefaultRepository();
			
			// Send around the update
			_events.OnPackageSourcesChanged(new EventArgs());
		}
		
		private void SavePackageSources()
		{
			AddDefaultRepository();
			var savedRepositories = _registeredPackageSources.Select(ps => ps.Name + "=" + ps.Source);
			_settings.PackageRepositories = savedRepositories.ToArray();
			UpdateCurrentRepository();
		}
		
		private void AddDefaultRepository()
		{
			var defaultPackageSource = (
				from packageSource in _registeredPackageSources
				where packageSource.Source == DefaultRepositorySource
				select packageSource).SingleOrDefault();
			if (defaultPackageSource == null)
			{
				string defaultRepositoryName =
					SD.ResourceService.GetString("AddInManager2.DefaultRepository") ?? DefaultRepositoryName;
				
				// Default repository is not configured, add it
				defaultPackageSource =
					new PackageSource(DefaultRepositorySource, defaultRepositoryName);
				_registeredPackageSources.Insert(0, defaultPackageSource);
				SavePackageSources();
			}
		}
		
		private void UpdateCurrentRepository()
		{
			_registeredPackageRepositories =
				_registeredPackageSources.Select(packageSource => PackageRepositoryFactory.Default.CreateRepository(packageSource.Source));
			if (_registeredPackageRepositories.Any())
			{
				_aggregatedRepository = new AggregateRepository(_registeredPackageRepositories);
			}
		}
	}
}
