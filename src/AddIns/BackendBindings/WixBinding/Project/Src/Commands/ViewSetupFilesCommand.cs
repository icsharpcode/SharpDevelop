// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Opens the package files editor.
	/// </summary>
	public class ViewSetupFilesCommand : AbstractMenuCommand
	{
		IPackageFilesViewFactory factory;
		IWorkbench workbench;
		
		public ViewSetupFilesCommand()
			: this(new PackageFilesViewFactory(), WorkbenchSingleton.Workbench)
		{
		}
		
		public ViewSetupFilesCommand(IPackageFilesViewFactory factory, IWorkbench workbench)
		{
			this.factory = factory;
			this.workbench = workbench;
		}
		
		public override void Run()
		{
			WixProject project = ProjectService.CurrentProject as WixProject;
			if (project != null) {
				Run(project);
			}
		}
		
		public void Run(WixProject project)
		{
			PackageFilesView openView = GetOpenPackageFilesView(project);
			if (openView != null) {
				openView.WorkbenchWindow.SelectWindow();
			} else {
				OpenNewPackageFilesView(project);
			}
		}
		
		void OpenNewPackageFilesView(WixProject project)
		{
			PackageFilesView view = factory.Create(project, workbench);
			workbench.ShowView(view);
			view.ShowFiles();
		}
		
		PackageFilesView GetOpenPackageFilesView(WixProject project)
		{
			foreach (IViewContent view in workbench.ViewContentCollection) {
				PackageFilesView packageFilesView = view as PackageFilesView;
				if ((packageFilesView != null) && (packageFilesView.IsForProject(project))) {
					return packageFilesView;
				}
			}
			return null;
		}		
	}
}
