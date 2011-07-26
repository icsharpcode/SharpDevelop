// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageOperation : PackageOperation
	{
		public FakePackageOperation()
			: this(new FakePackage("MyPackage"), PackageAction.Install)
		{
		}
		
		public FakePackageOperation(FakePackage package, PackageAction action)
			: base(package, action)
		{
			this.FakePackage = package;
		}
		
		public FakePackage FakePackage { get; set; }
	}
}
