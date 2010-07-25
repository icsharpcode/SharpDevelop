// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;

namespace NoGoop.ObjBrowser
{

	// This node represents an node that actually has an 
	// event and is capable of logging
	internal interface IEventLoggingNode : IEventLoggingMenuNode
	{
		// Get the event info corresponding to the event to be logged
		// Returns null if this is not an event node
		EventInfo LogEventInfo { get; }

		// Get the object to be logged
		Object LogEventObject { get; }

		String LogObjectName { get; } 
	}

}
