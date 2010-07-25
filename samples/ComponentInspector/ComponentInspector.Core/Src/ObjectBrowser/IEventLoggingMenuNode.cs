// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.ObjBrowser
{

    // This node changes the context menu
	internal interface IEventLoggingMenuNode : IBrowserNode 
	{
        bool EventLogging { get; }

        void EventLoggingClick(object sender, EventArgs e);
	}

}
