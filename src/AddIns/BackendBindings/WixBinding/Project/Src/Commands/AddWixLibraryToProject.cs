// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
