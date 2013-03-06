using ICSharpCode.AddInManager2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
    public class FakeAddInSetup : IAddInSetup
    {
    	private ISDAddInManagement _sdAddInManagement;
    	private List<ManagedAddIn> _addInsMarkedForInstall;
    	
    	public FakeAddInSetup(ISDAddInManagement sdAddInManagement)
    	{
    		_sdAddInManagement = sdAddInManagement;
    		_addInsMarkedForInstall = new List<ManagedAddIn>();
    	}
    	
    	public List<ManagedAddIn> AddInsMarkedForInstallList
    	{
    		get
    		{
    			return _addInsMarkedForInstall;
    		}
    	}
    	
        public IEnumerable<ManagedAddIn> AddInsWithMarkedForInstallation
        {
            get
			{
				return _sdAddInManagement.AddIns.Select(a => new ManagedAddIn(a) { IsTemporary = false, IsUpdate = false })
					.GroupJoin(
						_addInsMarkedForInstall,
						installedAddIn => installedAddIn.AddIn.Manifest.PrimaryIdentity,
						markedAddIn => markedAddIn.AddIn.Manifest.PrimaryIdentity,
						(installedAddIn, e) => e.ElementAtOrDefault(0) ?? installedAddIn);
			}
        }

        public ICSharpCode.Core.AddIn InstallAddIn(string archiveFileName)
        {
           	if (InstallAddInFromArchiveCallback != null)
           	{
           		return InstallAddInFromArchiveCallback(archiveFileName);
           	}
           	else
           	{
           		return null;
           	}
        }
        
        public Func<string, ICSharpCode.Core.AddIn> InstallAddInFromArchiveCallback
		{
			get;
			set;
		}

        public ICSharpCode.Core.AddIn InstallAddIn(NuGet.IPackage package, string packageDirectory)
        {
            if (InstallAddInFromArchiveCallback != null)
           	{
           		return InstallAddInFromPackageCallback(package, packageDirectory);
           	}
           	else
           	{
           		return null;
           	}
        }
        
        public Func<NuGet.IPackage, string, ICSharpCode.Core.AddIn> InstallAddInFromPackageCallback
		{
			get;
			set;
		}

        public void UninstallAddIn(ICSharpCode.Core.AddIn addIn)
        {
            if (UninstallAddInCallback != null)
           	{
           		UninstallAddInCallback(addIn);
           	}
        }
        
        public Action<ICSharpCode.Core.AddIn> UninstallAddInCallback
		{
			get;
			set;
		}

        public void CancelUpdate(ICSharpCode.Core.AddIn addIn)
        {
            if (CancelUpdateCallback != null)
           	{
           		CancelUpdateCallback(addIn);
           	}
        }
        
        public Action<ICSharpCode.Core.AddIn> CancelUpdateCallback
		{
			get;
			set;
		}

        public void CancelInstallation(ICSharpCode.Core.AddIn addIn)
        {
            if (CancelInstallationCallback != null)
           	{
           		CancelInstallationCallback(addIn);
           	}
        }
        
        public Action<ICSharpCode.Core.AddIn> CancelInstallationCallback
		{
			get;
			set;
		}

        public void CancelUninstallation(ICSharpCode.Core.AddIn addIn)
        {
            if (CancelUninstallationCallback != null)
           	{
           		CancelUninstallationCallback(addIn);
           	}
        }
        
        public Action<ICSharpCode.Core.AddIn> CancelUninstallationCallback
		{
			get;
			set;
		}

        public void SwitchAddInActivation(ICSharpCode.Core.AddIn addIn)
        {
            if (SwitchAddInActivationCallback != null)
           	{
           		SwitchAddInActivationCallback(addIn);
           	}
        }
        
        public Action<ICSharpCode.Core.AddIn> SwitchAddInActivationCallback
		{
			get;
			set;
		}

        public ICSharpCode.Core.AddIn GetInstalledAddInByIdentity(string identity)
        {
            if (!String.IsNullOrEmpty(identity))
			{
				return _sdAddInManagement.AddIns
					.Where(a => (a.Manifest != null) && a.Manifest.Identities.ContainsKey(identity))
					.FirstOrDefault();
			}
			else
			{
				return null;
			}
        }

        public bool IsAddInInstalled(ICSharpCode.Core.AddIn addIn)
        {
            if (addIn == null)
			{
				// Without a valid AddIn instance we can't do anything...
				return false;
			}
			
			string identity = null;
			if (addIn.Manifest != null)
			{
				identity = addIn.Manifest.PrimaryIdentity;
			}
			return (_sdAddInManagement.AddIns
			        .Where(a => (addIn == a) || ((identity != null) && (a.Manifest != null) && a.Manifest.Identities.ContainsKey(identity)))
			        .FirstOrDefault() != null);
        }

        public bool IsAddInPreinstalled(ICSharpCode.Core.AddIn addIn)
        {
            if (addIn != null)
			{
				return
					String.Equals(addIn.Properties["addInManagerHidden"], "preinstalled", StringComparison.OrdinalIgnoreCase)
					&& FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, addIn.FileName);
			}
			else
			{
				return false;
			}
        }

        public NuGet.IPackage GetNuGetPackageForAddIn(ICSharpCode.Core.AddIn addIn, bool getLatest)
        {
            if (GetNuGetPackageForAddInCallback != null)
           	{
           		return GetNuGetPackageForAddInCallback(addIn, getLatest);
           	}
           	else
           	{
           		return null;
           	}
        }
        
        public Func<ICSharpCode.Core.AddIn, bool, NuGet.IPackage> GetNuGetPackageForAddInCallback
		{
			get;
			set;
		}

        public ICSharpCode.Core.AddIn GetAddInForNuGetPackage(NuGet.IPackage package)
        {
            return GetAddInForNuGetPackage(package, false);
        }
        
        public ICSharpCode.Core.AddIn GetAddInForNuGetPackage(NuGet.IPackage package, bool withAddInsMarkedForInstallation)
        {
            if (GetAddInForNuGetPackageCallback != null)
           	{
           		return GetAddInForNuGetPackageCallback(package, withAddInsMarkedForInstallation);
           	}
           	else
           	{
           		return null;
           	}
        }
        
        public Func<NuGet.IPackage, bool, ICSharpCode.Core.AddIn> GetAddInForNuGetPackageCallback
		{
			get;
			set;
		}

        public IEnumerable<ManagedAddIn> GetDependentAddIns(ICSharpCode.Core.AddIn addIn)
        {
            if ((addIn != null) && (addIn.Manifest != null) && (addIn.Manifest.PrimaryIdentity != null))
			{
				// Get all AddIns which are dependent from given AddIn
				var dependentAddIns = AddInsWithMarkedForInstallation.Where(
					a => (a.AddIn.Manifest != null) && a.AddIn.Manifest.Dependencies.Any(
						reference => reference.Name == addIn.Manifest.PrimaryIdentity));
				return dependentAddIns;
			}
			
			return null;
        }

        public void RemoveUnreferencedNuGetPackages()
        {
            // No-op
        }
    	
		public int CompareAddInToPackageVersion(ICSharpCode.Core.AddIn addIn, NuGet.IPackage nuGetPackage)
		{
			if ((addIn == null) || (nuGetPackage == null))
			{
				// Consider this as equal (?)
				return 0;
			}
			
			// Look if AddIn has a NuGet version tag in manifest
			Version addInVersion = addIn.Version;
			if (addIn.Properties.Contains(ManagedAddIn.NuGetPackageVersionManifestAttribute))
			{
				try
				{
					addInVersion = new Version(addIn.Properties[ManagedAddIn.NuGetPackageVersionManifestAttribute]);
				}
				catch (Exception)
				{
					addInVersion = addIn.Version;
				}
			}
			
			if (addInVersion == null)
			{
				if (nuGetPackage == null)
				{
					// Both versions are null -> equal
					return 0;
				}
				else
				{
					// Non-null version is greater
					return -1;
				}
			}
			
			return addInVersion.CompareTo(nuGetPackage.Version.Version);
		}
    }
}
