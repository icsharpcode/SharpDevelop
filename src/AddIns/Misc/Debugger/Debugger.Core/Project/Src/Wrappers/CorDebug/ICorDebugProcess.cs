// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Interop.CorDebug
{
	using System;

	public static partial class CorDebugExtensionMethods
	{
		public static bool HasQueuedCallbacks(this ICorDebugProcess corProcess)
		{
			int pbQueued;
			corProcess.HasQueuedCallbacks(null, out pbQueued);
			return pbQueued != 0;
		}
	}
}
