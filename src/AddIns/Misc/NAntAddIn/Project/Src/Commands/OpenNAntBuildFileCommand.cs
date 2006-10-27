// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NAntAddIn.Gui;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.NAntAddIn.Commands
{
	/// <summary>
	/// Opens the build file selected in the NAnt pad view.
	/// </summary>
	public class OpenNAntBuildFileCommand : AbstractMenuCommand
	{
        /// <summary>
        /// Runs the <see cref="OpenNAntBuildFile"/>.
        /// </summary>		
		public override void Run()
		{
			NAntPadTreeView padTreeView = (NAntPadTreeView)Owner;
			
			NAntBuildFile buildFile = padTreeView.SelectedBuildFile;
			
			if (buildFile != null) {
				string fileName = Path.Combine(buildFile.Directory, buildFile.FileName);
				FileService.OpenFile(fileName);
			}
		}
	}
}
