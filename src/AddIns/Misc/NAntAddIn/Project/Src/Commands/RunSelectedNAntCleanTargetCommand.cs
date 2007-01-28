// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.NAntAddIn.Gui;

namespace ICSharpCode.NAntAddIn.Commands
{
	/// <summary>
	/// Runs the NAnt clean target for the file selected in the NAnt Pad view.
	/// </summary>
	public class RunSelectedNAntCleanTargetCommand : AbstractRunNAntCommand
	{
        /// <summary>
        /// Runs the <see cref="RunSelectedNAntCleanTargetCommand"/>.
        /// </summary>		
		public override void Run()
		{
			try {
				NAntBuildFile buildFile = NAntPadContent.Instance.SelectedBuildFile;
				if (buildFile != null) {
    				RunPreBuildSteps();
    				RunBuild(buildFile.FileName,
    		        	buildFile.Directory,
    		        	IsActiveConfigurationDebug,
    		        	"clean",
    		        	GetPadTextBoxArguments());
				}
			} catch (Exception ex) {
        		MessageService.ShowMessage(ex.Message);				
			}
		}
	}
}
