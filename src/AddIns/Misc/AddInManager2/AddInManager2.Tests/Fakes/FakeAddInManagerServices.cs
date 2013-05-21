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
		
		public FakePackageRepositories FakeRepositories
		{
			get;
			set;
		}
		
		public FakeAddInSetup FakeSetup 
		{
			get;
			set;
		}
		
		public FakeNuGetPackageManager FakeNuGet
		{
			get;
			set;
		}
		
		public FakeAddInManagerSettings FakeSettings
		{
			get;
			set;
		}
		
		public FakeSDAddInManagement FakeSDAddInManagement
		{
			get;
			set;
		}
		
		public IAddInManagerEvents Events
		{
			get;
			set;
		}
		
		public IPackageRepositories Repositories
		{
			get
			{
				return FakeRepositories;
			}
		}
		
		public IAddInSetup Setup 
		{
			get
			{
				return FakeSetup;
			}
		}
		
		public INuGetPackageManager NuGet
		{
			get
			{
				return FakeNuGet;
			}
		}
		
		public IAddInManagerSettings Settings
		{
			get
			{
				return FakeSettings;
			}
		}
		
		public ISDAddInManagement SDAddInManagement
		{
			get
			{
				return FakeSDAddInManagement;
			}
		}
	}
}
