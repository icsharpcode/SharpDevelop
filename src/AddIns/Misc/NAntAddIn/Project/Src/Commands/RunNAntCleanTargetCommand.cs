// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.NAntAddIn;
using System;
using System.Collections;
using System.IO;

namespace ICSharpCode.NAntAddIn.Commands
{
	/// <summary>
	/// Runs the NAnt "clean" target on the project's build file.
	/// </summary>
	public class RunNAntCleanTargetCommand : AbstractRunNAntCommand
	{
		/// <summary>
        /// Runs the <see cref="RunNAntCleanTargetCommand"/>.
        /// </summary>
        public override void Run()
        {   
         	try {
        		string buildFileName = GetProjectBuildFileName();
        			
        		RunPreBuildSteps();
       		        		
        		RunBuild(Path.GetFileName(buildFileName),
        		         Path.GetDirectoryName(buildFileName),
        		         IsActiveConfigurationDebug,
        		         "clean");
        	
        	} catch (NAntAddInException ex) {
        		MessageService.ShowMessage(ex.Message);
        	}       	
        }
	}
}
