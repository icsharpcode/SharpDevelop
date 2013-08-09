// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.AddInManager2.Model;

namespace ICSharpCode.AddInManager2.ViewModel
{
	/// <summary>
	/// Base class for view models of AddIn list entries.
	/// </summary>
	public abstract class AddInPackageViewModelBase : Model<AddInPackageViewModelBase>, IAddInPackage
	{
		private DelegateCommand addPackageCommand;
		private DelegateCommand updatePackageCommand;
		private DelegateCommand removePackageCommand;
		private DelegateCommand disablePackageCommand;
		private DelegateCommand cancelInstallationCommand;
		private DelegateCommand cancelUpdateCommand;
		private DelegateCommand cancelUninstallationCommand;
		private DelegateCommand optionsCommand;

		protected AddInPackageViewModelBase()
			: base()
		{
			CreateCommands();
		}
		
		protected AddInPackageViewModelBase(IAddInManagerServices services)
			: base(services)
		{
			CreateCommands();
		}

		private void CreateCommands()
		{
			addPackageCommand = new DelegateCommand(
				param => AddPackage(),
				param => !IsAdded && !IsRemoved && !IsInstalled && IsInstallable
			);
			updatePackageCommand = new DelegateCommand(
				param => UpdatePackage(),
				param => !IsAdded && !IsRemoved && IsUpdate && IsInstallable
			);
			removePackageCommand = new DelegateCommand(
				param => RemovePackage(),
				param => !IsAdded && !IsRemoved && IsInstalled && !IsUpdate && IsUninstallable
			);
			disablePackageCommand = new DelegateCommand(
				param => DisablePackage(),
				param => !IsAdded && !IsRemoved && IsInstalled && IsOffline && IsDisablingPossible
			);
			cancelInstallationCommand = new DelegateCommand(
				param => CancelInstallation(),
				param => IsAdded && !IsUpdate
			);
			cancelUpdateCommand = new DelegateCommand(
				param => CancelUpdate(),
				param => IsAdded && IsUpdate
			);
			cancelUninstallationCommand = new DelegateCommand(
				param => CancelUninstallation(),
				param => IsRemoved
			);
			optionsCommand = new DelegateCommand(
				param => ShowOptions(),
				param => HasOptions
			);
		}

		public ICommand AddPackageCommand
		{
			get
			{
				return addPackageCommand;
			}
		}
		
		public ICommand UpdatePackageCommand
		{
			get
			{
				return updatePackageCommand;
			}
		}

		public ICommand RemovePackageCommand
		{
			get
			{
				return removePackageCommand;
			}
		}
		
		public ICommand DisablePackageCommand
		{
			get
			{
				return disablePackageCommand;
			}
		}

		public ICommand CancelInstallationCommand
		{
			get
			{
				return cancelInstallationCommand;
			}
		}
		
		public ICommand CancelUpdateCommand
		{
			get
			{
				return cancelUpdateCommand;
			}
		}
		
		public ICommand CancelUninstallationCommand
		{
			get
			{
				return cancelUninstallationCommand;
			}
		}
		
		public ICommand OptionsCommand
		{
			get
			{
				return optionsCommand;
			}
		}
		
		public abstract string Name
		{
			get;
		}

		public bool HasLicenseUrl
		{
			get
			{
				return LicenseUrl != null;
			}
		}

		public bool HasProjectUrl
		{
			get
			{
				return ProjectUrl != null;
			}
		}

		public bool HasReportAbuseUrl
		{
			get
			{
				return ReportAbuseUrl != null;
			}
		}

		public bool HasNoDependencies
		{
			get
			{
				return !HasDependencies;
			}
		}

		public bool HasLastUpdated
		{
			get
			{
				return LastUpdated.HasValue;
			}
		}
		
		public abstract bool HasDependencyConflicts
		{
			get;
		}
		
		public bool HasVersion
		{
			get
			{
				return !ShowSplittedVersions && (Version != null) && (Version.ToString() != "0.0.0.0");
			}
		}
		
		public bool HasOldVersion
		{
			get
			{
				return ShowSplittedVersions && (OldVersion != null) && (OldVersion.ToString() != "0.0.0.0");
			}
		}
		
		public bool HasNewVersion
		{
			get
			{
				return ShowSplittedVersions && (Version != null) && (Version.ToString() != "0.0.0.0");
			}
		}
		
		public virtual bool ShowSplittedVersions
		{
			get
			{
				return false;
			}
		}
		
		public abstract bool IsOffline
		{
			get;
		}
		
		public virtual bool IsExternallyReferenced
		{
			get
			{
				return false;
			}
		}
		
		public abstract bool IsPreinstalled
		{
			get;
		}
		
		public abstract bool IsEnabled
		{
			get;
		}
		
		public abstract bool IsRemoved
		{
			get;
		}
		
		public virtual bool HasNuGetConnection
		{
			get
			{
				return false;
			}
		}
		
		public bool IsSelected
		{
			get;
			set;
		}

		public virtual void AddPackage()
		{
		}
		
		public virtual void UpdatePackage()
		{
		}

		public virtual void RemovePackage()
		{
		}
		
		public virtual void DisablePackage()
		{
		}
		
		public virtual void CancelInstallation()
		{
		}
		
		public virtual void CancelUpdate()
		{
		}
		
		public virtual void CancelUninstallation()
		{
		}
		
		public virtual void ShowOptions()
		{
			
		}
		
		public void UpdateInstallationState()
		{
			OnPropertyChanged(m => m.IsAdded);
			OnPropertyChanged(m => m.IsUpdate);
			OnPropertyChanged(m => m.IsInstallable);
			OnPropertyChanged(m => m.IsInstalled);
			OnPropertyChanged(m => m.IsEnabled);
			OnPropertyChanged(m => m.IsRemoved);
			OnPropertyChanged(m => m.Summary);
		}

		public bool IsManaged
		{
			get
			{
				//				if (selectedProjects.HasMultipleProjects()) {
				//					return true;
				//				}
				//				return !selectedProjects.HasSingleProjectSelected();
				return false;
			}
		}
		
		public abstract Uri LicenseUrl
		{
			get;
		}

		public abstract Uri ProjectUrl
		{
			get;
		}

		public abstract Uri ReportAbuseUrl
		{
			get;
		}

		public abstract bool IsAdded
		{
			get;
		}
		
		public abstract bool IsUpdate
		{
			get;
		}
		
		public abstract bool IsInstalled
		{
			get;
		}
		
		public abstract bool IsInstallable
		{
			get;
		}
		
		public abstract bool IsUninstallable
		{
			get;
		}
		
		public abstract bool IsDisablingPossible
		{
			get;
		}

		public abstract IEnumerable<AddInDependency> Dependencies
		{
			get;
		}

		public virtual bool HasDependencies
		{
			get
			{
				return (Dependencies != null) && Dependencies.Any();
			}
		}

		public abstract IEnumerable<string> Authors
		{
			get;
		}

		public abstract bool HasDownloadCount
		{
			get;
		}

		public abstract string Id
		{
			get;
		}

		public abstract Uri IconUrl
		{
			get;
		}

		public abstract string Summary
		{
			get;
		}

		public abstract Version Version
		{
			get;
		}
		
		public virtual Version OldVersion
		{
			get
			{
				return null;
			}
		}
		
		public virtual bool HasOptions
		{
			get
			{
				return false;
			}
		}

		public abstract int DownloadCount
		{
			get;
		}

		public abstract string Description
		{
			get;
		}

		public abstract DateTime? LastUpdated
		{
			get;
		}
		
		public virtual string FileName
		{
			get
			{
				return null;
			}
		}
		
		protected string SurroundWithParantheses(string content)
		{
			return "(" + content + ")";
		}
	}
}
