// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AddInManager2.Model;

namespace ICSharpCode.AddInManager2.ViewModel
{
	/// <summary>
	/// View model for AddInManager options in OptionDialog.
	/// </summary>
	public class PackageOptionsViewModel : Model<PackageOptionsViewModel>
	{
		private bool _autoSearchForUpdates;
				
		public bool AutoSearchForUpdates
		{
			get
			{
				return _autoSearchForUpdates;
			}
			set
			{
				_autoSearchForUpdates = value;
				OnPropertyChanged(m => m.AutoSearchForUpdates);
			}
		}
		
		public void Load()
		{
			AutoSearchForUpdates = AddInManagerServices.Settings.AutoSearchForUpdates;
		}
		
		public void Save()
		{
			AddInManagerServices.Settings.AutoSearchForUpdates = AutoSearchForUpdates;
		}
	}
}
