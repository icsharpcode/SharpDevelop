// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;

namespace PackageManagement.Tests.Helpers
{
	public class TestablePackageFromRepository : PackageFromRepository
	{
		public FakePackage FakePackagePassedToConstructor;
		public FakePackageRepository FakePackageRepositoryPassedToConstructor;

		public TestablePackageFromRepository()
			: this(new FakePackage("Test"), new FakePackageRepository())
		{
		}
		
		public TestablePackageFromRepository(FakePackage package, FakePackageRepository repository)
			: base(package, repository)
		{
			this.FakePackagePassedToConstructor = package;
			this.FakePackageRepositoryPassedToConstructor = repository;
		}
		
		public DateTimeOffset? DateTimeOffsetToReturnFromGetDataServicePackageLastUpdated;
		
		protected override DateTimeOffset? GetDataServicePackageLastUpdated()
		{
			return DateTimeOffsetToReturnFromGetDataServicePackageLastUpdated;
		}
	}
}
