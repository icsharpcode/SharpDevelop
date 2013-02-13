// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager2.Model
{
	public enum AddInInstallationSource
	{
		Offline,
		NuGetRepository
	}
	
	/// <summary>
	/// Extension of AddIn class used in AddInManager internally.
	/// </summary>
	public class ManagedAddIn
	{
		public const string NuGetPackageIDManifestAttribute = "__nuGetPackageID";
		public const string NuGetPackageVersionManifestAttribute = "__nuGetPackageVersion";
		
		private AddIn _addIn;
		
		public ManagedAddIn(AddIn addIn)
		{
			_addIn = addIn;
			InstallationSource = AddInInstallationSource.Offline;
		}
		
		public bool IsTemporary
		{
			get;
			set;
		}
		
		public bool IsUpdate
		{
			get;
			set;
		}
		
		public Version OldVersion
		{
			get;
			set;
		}
		
		public AddInInstallationSource InstallationSource
		{
			get;
			set;
		}
		
		public AddIn AddIn
		{
			get
			{
				return _addIn;
			}
		}
		
		public string LinkedNuGetPackageID
		{
			get
			{
				if (_addIn != null)
				{
					if (_addIn.Properties.Contains(NuGetPackageIDManifestAttribute))
					{
						return _addIn.Properties[NuGetPackageIDManifestAttribute];
					}
				}
				
				return null;
			}
		}
		
		public string LinkedNuGetPackageVersion
		{
			get
			{
				if (_addIn != null)
				{
					if (_addIn.Properties.Contains(NuGetPackageVersionManifestAttribute))
					{
						return _addIn.Properties[NuGetPackageVersionManifestAttribute];
					}
				}
				
				return null;
			}
		}
	}
}
