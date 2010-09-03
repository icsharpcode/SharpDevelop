// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	public class AddWixLibraryToProject : AddWixItemToProjectBaseCommand
	{
		public override string FileFilter {
			get { return "${res:ICSharpCode.WixBinding.AddWixLibraryToProject.WixLibraryFileFilterName} (*.wixlib)|*.wixlib|${res:SharpDevelop.FileFilter.AllFiles}|*.*"; }
		}
		
		public override Type FolderNodeType {
			get { return typeof(WixLibraryFolderNode); }
		}
		
		protected override void AddFiles(WixProject project, string[] fileNames)
		{
			project.AddWixLibraries(fileNames);
		}		
	}
}
