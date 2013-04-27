// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class FileConflictResolver : ServiceWithWorkbenchOwner, IFileConflictResolver
	{
		public FileConflictResolution ResolveFileConflict(string message)
		{
			var viewModel = new FileConflictViewModel(message);
			FileConflictView view = CreateFileConflictView(viewModel);
			view.ShowDialog();
			return viewModel.GetResolution();
		}
		
		FileConflictView CreateFileConflictView(FileConflictViewModel viewModel)
		{
			var view = new FileConflictView();
			view.ViewModel = viewModel;
			view.Owner = Owner;
			return view;
		}
	}
}
