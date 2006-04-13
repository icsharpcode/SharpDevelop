// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public partial class ICorDebugStepper
	{
		public unsafe void StepRange(bool bStepIn, int[] ranges)
		{
			fixed (int* pRanges = ranges) {
				this.StepRange(bStepIn?1:0, (IntPtr)pRanges, (uint)ranges.Length / 2);
			}
		}
	}
}
