// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AddInManager2.Model;

namespace ICSharpCode.AddInManager2
{
	/// <summary>
	/// Container for public services of AddInManager AddIn.
	/// </summary>
	public class AddInManager
	{
		private static readonly AddInManagerEvents _events;
		private static readonly PackageRepositories _repositories;
		private static readonly AddInSetup _setup;
		private static readonly NuGetPackageManager _nuGet;
		
		static AddInManager()
		{
			_events = new AddInManagerEvents();
			_repositories = new PackageRepositories();
			_nuGet = new NuGetPackageManager(_repositories, _events);
			_setup = new AddInSetup(_events, _nuGet);
		}
		
		public static IAddInManagerEvents Events
		{
			get
			{
				return _events;
			}
		}
		
		public static IPackageRepositories Repositories
		{
			get
			{
				return _repositories;
			}
		}
		
		public static IAddInSetup Setup
		{
			get
			{
				return _setup;
			}
		}
		
		public static INuGetPackageManager NuGet
		{
			get
			{
				return _nuGet;
			}
		}
	}
}
