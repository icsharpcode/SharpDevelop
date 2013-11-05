// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Basic interface providing instances to all services of AddInManager AddIn.
	/// </summary>
	public interface IAddInManagerServices
	{
		IAddInManagerEvents Events
		{
			get;
		}

		IPackageRepositories Repositories
		{
			get;
		}

		IAddInSetup Setup
		{
			get;
		}

		INuGetPackageManager NuGet
		{
			get;
		}
		
		IAddInManagerSettings Settings
		{
			get;
		}
		ISDAddInManagement SDAddInManagement
		{
			get;
		}
	}
}
