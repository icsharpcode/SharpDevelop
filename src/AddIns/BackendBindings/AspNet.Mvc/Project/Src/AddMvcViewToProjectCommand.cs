// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class AddMvcViewToProjectCommand : AbstractCommand
	{
		public override void Run()
		{
			IAddMvcItemToProjectView view = CreateView();
			view.DataContext = CreateDataContext();
			view.ShowDialog();
		}
		
		protected virtual IAddMvcItemToProjectView CreateView()
		{
			return new AddMvcViewToProjectView() {
				Owner = WorkbenchSingleton.MainWindow
			};
		}
		
		protected virtual object CreateDataContext()
		{
			SelectedMvcViewFolder selectedFolder = GetSelectedFolder();
			return new AddMvcViewToProjectViewModel(selectedFolder);
		}
		
		SelectedMvcViewFolder GetSelectedFolder()
		{
			var directoryNode = Owner as DirectoryNode;
			return new SelectedMvcViewFolder(directoryNode);
		}
	}
}
