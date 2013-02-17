using ICSharpCode.AddInManager2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
    public class FakePackageRepositories : IPackageRepositories
    {
        public NuGet.IPackageRepository AllRegistered
        {
            get;
            set;
        }

        public IEnumerable<NuGet.PackageSource> RegisteredPackageSources
        {
            get;
            set;
        }
    	
		public IEnumerable<NuGet.IPackageRepository> RegisteredPackageRepositories
		{
			get;
			set;
		}
    	
		public NuGet.IPackageRepository GetRepositoryFromSource(NuGet.PackageSource packageSource)
		{
			if (GetRepositoryFromSourceCallback != null)
			{
				return GetRepositoryFromSourceCallback(packageSource);
			}
			else
			{
				return null;
			}
		}
		
		public Func<NuGet.PackageSource, NuGet.IPackageRepository> GetRepositoryFromSourceCallback
		{
			get;
			set;
		}
    }
}
