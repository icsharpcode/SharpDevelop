// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.NAntAddIn;
using ICSharpCode.NAntAddIn.Gui;
using ICSharpCode.SharpDevelop;
using System;
using System.IO;
using System.Windows.Forms;

namespace ICSharpCode.NAntAddIn.Commands
{
	/// <summary>
	/// Opens up a NAnt build file and goes to the line of the 
	/// target selected.
	/// </summary>
	public class GoToTargetDefinitionCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Runs the <see cref="GoToDefinitionCommand"/>.
		/// </summary>
		public override void Run()
		{
			NAntPadTreeView padTreeView = (NAntPadTreeView)Owner;
			
			NAntBuildFile buildFile = padTreeView.SelectedBuildFile;
			
			if (buildFile != null) {
				NAntBuildTarget target = padTreeView.SelectedTarget;
				
				if (target != null) {
					string fileName = Path.Combine(buildFile.Directory, buildFile.FileName);
					FileService.JumpToFilePosition(fileName, target.Line, target.Column);
				}			
			}
		}
	}
}
