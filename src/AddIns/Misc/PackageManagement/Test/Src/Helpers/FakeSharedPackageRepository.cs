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
		public string PathPassedToConstructor;
		public string PathPassedToRegisterRepository;
		
		public FakeSharedPackageRepository(string path)
		{
			PathPassedToConstructor = path;
		}
		
		public FakeSharedPackageRepository()
		{
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
