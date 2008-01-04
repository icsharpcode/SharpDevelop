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

	public partial class ICorDebugArrayValue
	{
		public unsafe uint[] Dimensions {
			get {
				uint[] dimensions = new uint[this.Rank];
				fixed (void* pDimensions = dimensions) {
					this.GetDimensions((uint)dimensions.Length, new IntPtr(pDimensions));
				}
				return dimensions;
			}
		}
		
		public unsafe uint[] BaseIndicies {
			get {
				uint[] baseIndicies = new uint[this.Rank];
				fixed (void* pBaseIndicies = baseIndicies) {
					this.GetBaseIndicies((uint)baseIndicies.Length, new IntPtr(pBaseIndicies));
				}
				return baseIndicies;
			}
		}
		
		public unsafe ICorDebugValue GetElement(uint[] indices)
		{
			fixed (void* pIndices = indices) {
				return this.GetElement((uint)indices.Length, new IntPtr(pIndices));
			}
		}
		
		public unsafe ICorDebugValue GetElement(int[] indices)
		{
			fixed (void* pIndices = indices) {
				return this.GetElement((uint)indices.Length, new IntPtr(pIndices));
			}
		}
	}
}

#pragma warning restore 1591