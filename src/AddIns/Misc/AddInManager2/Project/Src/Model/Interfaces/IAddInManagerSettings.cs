// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Interface for all settings of AddInManager.
	/// </summary>
	public interface IAddInManagerSettings
	{
		string[] PackageRepositories
		{
			get;
			set;
		}
		
		bool ShowPreinstalledAddIns
		{
			get;
			set;
		}
		
		bool ShowPrereleases
		{
			get;
			set;
		}
		
		bool AutoSearchForUpdates
		{
			get;
			set;
		}
	}
}
