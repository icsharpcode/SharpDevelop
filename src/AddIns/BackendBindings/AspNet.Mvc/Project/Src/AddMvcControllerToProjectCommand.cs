// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class AddMvcControllerToProjectCommand : AbstractCommand
	{
		public override void Run()
		{
			IAddMvcItemToProjectView view = CreateView();
			view.DataContext = CreateDataContext();
			view.ShowDialog();
		}
		
		protected virtual IAddMvcItemToProjectView CreateView()
		{
			return new AddMvcControllerToProjectView() {
				Owner = WorkbenchSingleton.MainWindow
			};
		}
		
		protected virtual object CreateDataContext()
		{
			SelectedMvcControllerFolder selectedFolder = GetSelectedFolder();
			return new AddMvcControllerToProjectViewModel(selectedFolder);
		}
		
		SelectedMvcControllerFolder GetSelectedFolder()
		{
			var directoryNode = Owner as DirectoryNode;
			return new SelectedMvcControllerFolder(directoryNode);
		}
	}
}
