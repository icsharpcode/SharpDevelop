// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class ExceptionThrowingRegisteredPackageRepositories : FakeRegisteredPackageRepositories
	{
		public Exception ExeptionToThrowWhenActiveRepositoryAccessed { get; set; }
		
		public override IPackageRepository ActiveRepository {
			get {
				if (ExeptionToThrowWhenActiveRepositoryAccessed != null) {
					throw ExeptionToThrowWhenActiveRepositoryAccessed;
				}
				return base.ActiveRepository;
			}
		}
	}
}
