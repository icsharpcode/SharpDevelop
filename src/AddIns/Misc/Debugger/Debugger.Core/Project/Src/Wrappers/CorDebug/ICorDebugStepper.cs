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
		public static unsafe void StepRange(this ICorDebugStepper corStepper, bool bStepIn, int[] ranges)
		{
			fixed (int* pRanges = ranges)
				corStepper.StepRange(bStepIn?1:0, (IntPtr)pRanges, (uint)ranges.Length / 2);
		}
	}
}

#pragma warning restore 1591
