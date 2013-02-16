// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AddInManager2.Model;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	public class FakeAddInManagerServices : IAddInManagerServices
	{
		public FakeAddInManagerServices()
		{
		}
		
		public IAddInManagerEvents Events
		{
			get;
			set
		}
		
		public IPackageRepositories Repositories
		{
			get;
			set
		}
		
		public IAddInSetup Setup 
		{
			get;
			set
		}
		
		public INuGetPackageManager NuGet
		{
			get;
			set
		}
		
		public IAddInManagerSettings Settings
		{
			get;
			set
		}
		
		public ISDAddInManagement SDAddInManagement
		{
			get;
			set
		}
	}
}
