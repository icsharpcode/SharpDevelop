// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.AddInManager2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
    public class FakeNuGetPackageManager : INuGetPackageManager
    {
    	private FakeNuGetCorePackageManager _corePackageManager;
    	private FakePackageOperationResolver _packageOperationResolver;
    	
    	public FakeNuGetPackageManager()
    	{
    		_corePackageManager = new FakeNuGetCorePackageManager();
    		_packageOperationResolver = new FakePackageOperationResolver();
    	}
    	
        public NuGet.IPackageManager Packages
        {
            get
            {
                return _corePackageManager;
            }
        }
        
        public FakeNuGetCorePackageManager FakeCorePackageManager
        {
            get
            {
                return _corePackageManager;
            }
        }

        public string PackageOutputDirectory
        {
            get;
            set;
        }
        
        public bool FakePackageContainsAddIn
        {
        	get;
        	set;
        }

        public bool PackageContainsAddIn(NuGet.IPackage package)
        {
            return FakePackageContainsAddIn;
        }

        public NuGet.IPackageOperationResolver CreateInstallPackageOperationResolver(bool allowPrereleaseVersions)
        {
            return _packageOperationResolver;
        }

        public void ExecuteOperation(NuGet.PackageOperation operation)
        {
            if (ExecuteOperationCallback != null)
            {
            	ExecuteOperationCallback(operation);
            }
        }
        
       	public Action<PackageOperation> ExecuteOperationCallback
       	{
       		get;
       		set;
       	}
       	
       	public string LocalPackageDirectory
       	{
       		get;
       		set;
       	}
    	
		public string GetLocalPackageDirectory(IPackage package)
		{
			return LocalPackageDirectory;
		}
    }
}
