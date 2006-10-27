// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NAntAddIn.Gui;

namespace ICSharpCode.NAntAddIn.Commands
{
	/// <summary>
    /// Refreshes the NAnt pad.
    /// </summary>
	public class RefreshNAntPadCommand : AbstractRunNAntCommand
	{		
        public override void Run()
        {   
        	NAntPadContent.Instance.Refresh();
        }
	}
}
