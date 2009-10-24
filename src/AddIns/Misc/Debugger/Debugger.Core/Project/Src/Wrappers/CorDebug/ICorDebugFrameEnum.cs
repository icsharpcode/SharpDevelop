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
	using System.Collections.Generic;
	
	
	public static partial class CorDebugExtensionMethods
	{
		public static IEnumerable<ICorDebugFrame> GetEnumerator(this ICorDebugFrameEnum corFrameEnum)
		{
			// TODO: As list
			corFrameEnum.Reset();
			while (true) {
				ICorDebugFrame corFrame = corFrameEnum.Next();
				if (corFrame != null) {
					yield return corFrame;
				} else {
					break;
				}
			}
		}
		
		public static ICorDebugFrame Next(this ICorDebugFrameEnum corFrameEnum)
		{
			ICorDebugFrame[] corFrames = new ICorDebugFrame[1];
			uint framesFetched = corFrameEnum.Next(1, corFrames);
			if (framesFetched == 0) {
				return null;
			} else {
				return corFrames[0];
			}
		}
	}
}

#pragma warning restore 1591
