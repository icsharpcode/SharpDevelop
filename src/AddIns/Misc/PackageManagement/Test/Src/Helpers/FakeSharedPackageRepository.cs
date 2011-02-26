// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakeSharedPackageRepository : FakePackageRepository, ISharedPackageRepository
	{
		public string PathPassedToRegisterRepository;
		public IPackagePathResolver PackagePathResolverPassedToConstructor;
		public IFileSystem FileSystemPassedToConstructor;
		
		public FakeSharedPackageRepository()
		{
		}
		
		public FakeSharedPackageRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem)
		{
			this.PackagePathResolverPassedToConstructor = pathResolver;
			this.FileSystemPassedToConstructor = fileSystem;
		}
		
		public bool IsReferenced(string packageId, Version version)
		{
			throw new NotImplementedException();
		}
		
		public void RegisterRepository(string path)
		{
			PathPassedToRegisterRepository = path;
		}
		
		public void UnregisterRepository(string path)
		{
			throw new NotImplementedException();
		}
	}
}
