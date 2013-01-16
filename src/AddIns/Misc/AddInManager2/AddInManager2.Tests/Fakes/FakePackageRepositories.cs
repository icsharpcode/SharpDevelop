using ICSharpCode.AddInManager2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
    public class FakePackageRepositories : IPackageRepositories
    {
        public NuGet.IPackageRepository Registered
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public NuGet.IPackageRepository Active
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public NuGet.PackageSource ActiveSource
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<NuGet.PackageSource> RegisteredPackageSources
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
