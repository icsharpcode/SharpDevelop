// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.NAntAddIn.Gui;
using System;
using System.IO;
using System.Windows.Forms;

namespace ICSharpCode.NAntAddIn.Commands
{
	/// <summary>
	/// Runs the NAnt target selected in the NAnt Pad view.
	/// </summary>
	public class RunSelectedNAntTargetCommand : AbstractRunNAntCommand
	{
        /// <summary>
        /// Runs the <see cref="RunSelectedNAntTargetCommand"/>.
        /// </summary>		
		public override void Run()
		{
			try {
				NAntBuildFile buildFile = NAntPadContent.Instance.SelectedBuildFile;
				
				if (buildFile != null) {
					NAntBuildTarget target = NAntPadContent.Instance.SelectedTarget;
					
					string targetName = String.Empty;
					if (target != null) {
						targetName = target.Name;
					}
					
    				RunPreBuildSteps();
    				
    				RunBuild(buildFile.FileName,
    		        	buildFile.Directory,
    		        	IsActiveConfigurationDebug,
    		        	targetName,
    		        	GetPadTextBoxArguments());
				}
				
			} catch (Exception ex) {
        		MessageService.ShowMessage(ex.Message);				
			}
		}
	}
}
