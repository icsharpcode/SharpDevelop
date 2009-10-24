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
		public static unsafe uint[] GetDimensions(this ICorDebugArrayValue corArray)
		{
			uint[] dimensions = new uint[corArray.GetRank()];
			fixed (void* pDimensions = dimensions)
				corArray.GetDimensions((uint)dimensions.Length, new IntPtr(pDimensions));
			return dimensions;
		}
		
		public static unsafe uint[] GetBaseIndicies(this ICorDebugArrayValue corArray)
		{
			uint[] baseIndicies = new uint[corArray.GetRank()];
			fixed (void* pBaseIndicies = baseIndicies)
				corArray.GetBaseIndicies((uint)baseIndicies.Length, new IntPtr(pBaseIndicies));
			return baseIndicies;
		}
		
		public static unsafe ICorDebugValue GetElement(this ICorDebugArrayValue corArray, uint[] indices)
		{
			fixed (void* pIndices = indices)
				return corArray.GetElement((uint)indices.Length, new IntPtr(pIndices));
		}
		
		public static unsafe ICorDebugValue GetElement(this ICorDebugArrayValue corArray, int[] indices)
		{
			fixed (void* pIndices = indices)
				return corArray.GetElement((uint)indices.Length, new IntPtr(pIndices));
		}
	}
}

#pragma warning restore 1591