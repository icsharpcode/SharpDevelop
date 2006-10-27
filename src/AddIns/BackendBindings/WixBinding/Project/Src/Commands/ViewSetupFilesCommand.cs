// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		public override void Run()
		{
			WixProject project = ProjectService.CurrentProject as WixProject;
			if (project != null) {
				PackageFilesView.Show(project, WorkbenchSingleton.Workbench);
			}
		}
	}
}
