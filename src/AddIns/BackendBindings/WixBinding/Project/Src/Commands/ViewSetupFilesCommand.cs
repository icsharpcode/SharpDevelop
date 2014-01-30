// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

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
			: this(new PackageFilesViewFactory(), SD.Workbench)
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
