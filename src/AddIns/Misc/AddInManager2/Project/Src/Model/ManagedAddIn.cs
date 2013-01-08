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
	}
}
