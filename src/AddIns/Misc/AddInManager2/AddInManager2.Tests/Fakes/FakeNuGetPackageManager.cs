// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.AddInManager2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
    public class FakeNuGetPackageManager : INuGetPackageManager
    {
        public NuGet.IPackageManager Packages
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PackageOutputDirectory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool PackageContainsAddIn(NuGet.IPackage package)
        {
            throw new NotImplementedException();
        }

        public NuGet.IPackageOperationResolver CreateInstallPackageOperationResolver(bool allowPrereleaseVersions)
        {
            throw new NotImplementedException();
        }

        public void ExecuteOperation(NuGet.PackageOperation operation)
        {
            throw new NotImplementedException();
        }
    }
}
