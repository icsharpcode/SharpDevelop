// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

namespace Debugger.Wrappers.CorDebug
{
	using System;
	using System.Collections.Generic;
	
	
	public partial class ICorDebugFrameEnum
	{
		public IEnumerable<ICorDebugFrame> Enumerator {
			get {
				Reset();
				uint index = this.Count - 1;
				while (true) {
					ICorDebugFrame corFrame = Next();
					if (corFrame != null) {
						corFrame.Index = index--;
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

#pragma warning restore 1591
