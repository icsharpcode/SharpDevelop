// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.NAntAddIn;
using System;
using System.IO;

namespace ICSharpCode.NAntAddIn.Commands
{
	/// <summary>
    /// Represents the command that runs NAnt on the project's build file.
    /// </summary>
	public class RunNAntCommand : AbstractRunNAntCommand
	{		
        /// <summary>
        /// Runs the <see cref="RunNAntCommand"/>.
        /// </summary>
        public override void Run()
        {   
        	try {
        		string buildFileName = GetProjectBuildFileName();
        			
        		RunPreBuildSteps();
        		
        		RunBuild(Path.GetFileName(buildFileName),
        		         Path.GetDirectoryName(buildFileName),
        		         IsActiveConfigurationDebug);
        	
        	} catch (NAntAddInException ex) {
        		MessageService.ShowMessage(ex.Message);
        	}
        }
	}
}
