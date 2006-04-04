// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	using System.Collections.Generic;
	
	
	public partial class ICorDebugFrameEnum
	{
		public IEnumerable<ICorDebugFrame> Enumerator {
			get {
				while (true) {
					ICorDebugFrame corFrame = Next();
					if (corFrame != null) {
						yield return corFrame;
					} else {
						break;
					}
				}
			}
		}
		
		public ICorDebugFrame Next()
		{
			ICorDebugFrame[] corFrames = new ICorDebugFrame[1];
			uint framesFetched = this.Next(1, corFrames);
			if (framesFetched == 0) {
				return null;
			} else {
				return corFrames[0];
			}
		}
	}
}
