// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;

using ICSharpCode.SharpDevelop.Gui;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class LicenseAcceptanceService : ILicenseAcceptanceService
	{
		Window owner;
		
		public Window Owner {
			get {
				if (owner == null) {
					owner = WorkbenchSingleton.MainWindow;
				}
				return owner;
			}
			set { owner = value; }
		}
		
		public bool AcceptLicenses(IEnumerable<IPackage> packages)
		{
			LicenseAcceptanceView view = CreateLicenseAcceptanceView(packages);
			return view.ShowDialog() ?? false;
		}
		
		LicenseAcceptanceView CreateLicenseAcceptanceView(IEnumerable<IPackage> packages)
		{
			var viewModel = new LicenseAcceptanceViewModel(packages);
			return CreateLicenseAcceptanceView(viewModel);
		}
		
		LicenseAcceptanceView CreateLicenseAcceptanceView(LicenseAcceptanceViewModel viewModel)
		{
			var view = new LicenseAcceptanceView();
			view.DataContext = viewModel;
			view.Owner = Owner;
			return view;
		}
	}
}
