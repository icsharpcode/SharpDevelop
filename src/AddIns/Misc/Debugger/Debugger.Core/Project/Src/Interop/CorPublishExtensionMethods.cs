// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.Interop.CorPublish
{
	public static partial class CorPublishExtensionMethods
	{
		static void ProcessOutParameter(object parameter)
		{
			TrackedComObjects.ProcessOutParameter(parameter);
		}
	}
}
