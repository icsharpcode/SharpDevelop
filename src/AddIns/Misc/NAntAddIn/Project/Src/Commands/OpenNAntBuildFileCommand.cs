// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.NAntAddIn;
using ICSharpCode.NAntAddIn.Gui;
using System;
using System.IO;
using System.Windows.Forms;

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
