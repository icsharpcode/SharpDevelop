using ICSharpCode.AddInManager2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
    public class FakeAddInSetup : IAddInSetup
    {
        public IEnumerable<ManagedAddIn> AddInsWithMarkedForInstallation
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICSharpCode.Core.AddIn InstallAddIn(string archiveFileName)
        {
            throw new NotImplementedException();
        }

        public ICSharpCode.Core.AddIn InstallAddIn(NuGet.IPackage package, string packageDirectory)
        {
            throw new NotImplementedException();
        }

        public void UninstallAddIn(ICSharpCode.Core.AddIn addIn)
        {
            throw new NotImplementedException();
        }

        public void CancelUpdate(ICSharpCode.Core.AddIn addIn)
        {
            throw new NotImplementedException();
        }

        public void CancelInstallation(ICSharpCode.Core.AddIn addIn)
        {
            throw new NotImplementedException();
        }

        public void CancelUninstallation(ICSharpCode.Core.AddIn addIn)
        {
            throw new NotImplementedException();
        }

        public void SwitchAddInActivation(ICSharpCode.Core.AddIn addIn)
        {
            throw new NotImplementedException();
        }

        public ICSharpCode.Core.AddIn GetInstalledAddInByIdentity(string identity)
        {
            throw new NotImplementedException();
        }

        public bool IsAddInInstalled(ICSharpCode.Core.AddIn addIn)
        {
            throw new NotImplementedException();
        }

        public bool IsAddInPreinstalled(ICSharpCode.Core.AddIn addin)
        {
            throw new NotImplementedException();
        }

        public NuGet.IPackage GetNuGetPackageForAddIn(ICSharpCode.Core.AddIn addIn, bool getLatest)
        {
            throw new NotImplementedException();
        }

        public ICSharpCode.Core.AddIn GetAddInForNuGetPackage(NuGet.IPackage package)
        {
            throw new NotImplementedException();
        }

        public ICSharpCode.Core.AddIn GetAddInForNuGetPackage(NuGet.IPackage package, bool withAddInsMarkedForInstallation)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ManagedAddIn> GetDependentAddIns(ICSharpCode.Core.AddIn addIn)
        {
            throw new NotImplementedException();
        }

        public void RemoveUnreferencedNuGetPackages()
        {
            throw new NotImplementedException();
        }
    	
		public int CompareAddInToPackageVersion(ICSharpCode.Core.AddIn addIn, NuGet.IPackage nuGetPackage)
		{
			throw new NotImplementedException();
		}
    }
}
