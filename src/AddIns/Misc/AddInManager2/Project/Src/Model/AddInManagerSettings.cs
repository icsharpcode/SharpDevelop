// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Handler for managing all persisted settings of AddInManager AddIn.
	/// </summary>
	public class AddInManagerSettings : IAddInManagerSettings
	{
		public string[] PackageRepositories
		{
			get
			{
				return SD.PropertyService.Get<string[]>("AddInManager2.PackageRepositories", null);
			}
			set
			{
				SD.PropertyService.Set<string[]>("AddInManager2.PackageRepositories", value ?? new string[0]);
			}
		}
		
		public bool ShowPreinstalledAddIns
		{
			get
			{
				return SD.PropertyService.Get<bool>("AddInManager2.ShowPreinstalledAddIns", false);
			}
			set
			{
				SD.PropertyService.Set<bool>("AddInManager2.ShowPreinstalledAddIns", value);
			}
		}
		
		public bool ShowPrereleases
		{
			get
			{
				return SD.PropertyService.Get<bool>("AddInManager2.ShowPrereleases", false);
			}
			set
			{
				SD.PropertyService.Set<bool>("AddInManager2.ShowPrereleases", value);
			}
		}
		
		public bool AutoSearchForUpdates
		{
			get
			{
				return SD.PropertyService.Get<bool>("AddInManager2.AutoSearchForUpdates", true);
			}
			set
			{
				SD.PropertyService.Set<bool>("AddInManager2.AutoSearchForUpdates", value);
			}
		}
	}
}
