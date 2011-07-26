// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class SelectProjectsService : ISelectProjectsService
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
		
		public bool SelectProjects(IEnumerable<IPackageManagementSelectedProject> projects)
		{
			SelectProjectsView view = CreateSelectProjectsView(projects);
			return view.ShowDialog() ?? false;			
		}
		
		SelectProjectsView CreateSelectProjectsView(IEnumerable<IPackageManagementSelectedProject> projects)
		{
			var viewModel = new SelectProjectsViewModel(projects);
			return CreateSelectProjectsView(viewModel);
		}
		
		SelectProjectsView CreateSelectProjectsView(SelectProjectsViewModel viewModel)
		{
			var view = new SelectProjectsView();
			view.DataContext = viewModel;
			view.Owner = Owner;
			return view;
		}
	}
}
