// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.TypeSystem;

namespace Debugger.Interop.CorDebug
{
	public static partial class CorDebugExtensionMethods
	{
		const int EnumerateBufferSize = 16;
		
		static void ProcessOutParameter(object parameter)
		{
			TrackedComObjects.ProcessOutParameter(parameter);
		}
		
		// ICorDebugArrayValue
		
		public static unsafe uint[] GetDimensions(this ICorDebugArrayValue corArray)
		{
			uint[] dimensions = new uint[corArray.GetRank()];
			fixed(uint* pDimensions = dimensions)
				corArray.GetDimensions((uint)dimensions.Length, new IntPtr(pDimensions));
			return dimensions;
		}
		
		public static unsafe uint[] GetBaseIndicies(this ICorDebugArrayValue corArray)
		{
			uint[] baseIndicies = new uint[corArray.GetRank()];
			fixed(uint* pBaseIndicies = baseIndicies)
				corArray.GetBaseIndicies((uint)baseIndicies.Length, new IntPtr(pBaseIndicies));
			return baseIndicies;
		}
		
		public static unsafe ICorDebugValue GetElement(this ICorDebugArrayValue corArray, uint[] indices)
		{
			fixed(uint* pIndices = indices)
				return corArray.GetElement((uint)indices.Length, new IntPtr(pIndices));
		}
		
		public static unsafe ICorDebugValue GetElement(this ICorDebugArrayValue corArray, int[] indices)
		{
			fixed(int* pIndices = indices)
				return corArray.GetElement((uint)indices.Length, new IntPtr(pIndices));
		}
		
		// ICorDebugClass2
		
		public static ICorDebugType GetParameterizedType(this ICorDebugClass2 corClass, uint elementType, ICorDebugType[] ppTypeArgs)
		{
			return corClass.GetParameterizedType(elementType, (uint)ppTypeArgs.Length, ppTypeArgs);
		}
		
		// ICorDebugCode
		
		public static unsafe byte[] GetCode(this ICorDebugCode corCode)
		{
			byte[] code = new byte[corCode.GetSize()];
			fixed(byte* pCode = code)
				corCode.GetCode(0, (uint)code.Length, (uint)code.Length, new IntPtr(pCode));
			return code;
		}
		
		// ICorDebugEnum
		
		public static IEnumerable<ICorDebugFrame> GetEnumerator(this ICorDebugFrameEnum corEnum)
		{
			corEnum.Reset();
			while (true) {
				ICorDebugFrame[] corFrames = new ICorDebugFrame[EnumerateBufferSize];
				uint fetched = corEnum.Next(EnumerateBufferSize, corFrames);
				if (fetched == 0)
					yield break;
				for(int i = 0; i < fetched; i++)
					yield return corFrames[i];
			}
		}
		
		public static ICorDebugFrame Next(this ICorDebugFrameEnum corEnum)
		{
			ICorDebugFrame[] corFrames = new ICorDebugFrame[] { null };
			uint framesFetched = corEnum.Next(1, corFrames);
			return corFrames[0];
		}
		
		public static IEnumerable<ICorDebugChain> GetEnumerator(this ICorDebugChainEnum corEnum)
		{
			corEnum.Reset();
			while (true) {
				ICorDebugChain[] corChains = new ICorDebugChain[EnumerateBufferSize];
				uint fetched = corEnum.Next(EnumerateBufferSize, corChains);
				if (fetched == 0)
					yield break;
				for(int i = 0; i < fetched; i++)
					yield return corChains[i];
			}
		}
			
		public static ICorDebugChain Next(this ICorDebugChainEnum corChainEnum)
		{
			ICorDebugChain[] corChains = new ICorDebugChain[] { null };
			uint chainsFetched = corChainEnum.Next(1, corChains);
			return corChains[0];
		}
		
		// ICorDebugGenericValue
		
		public static unsafe Byte[] GetRawValue(this ICorDebugGenericValue corGenVal)
		{
			byte[] retValue = new byte[(int)corGenVal.GetSize()];
			fixed(byte* pRetValue = retValue)
				corGenVal.GetValue(new IntPtr(pRetValue));
			return retValue;
		}
		
		public static unsafe void SetRawValue(this ICorDebugGenericValue corGenVal, byte[] value)
		{
			if (corGenVal.GetSize() != value.Length)
				throw new ArgumentException("Incorrect length");
			fixed(byte* pValue = value)
				corGenVal.SetValue(new IntPtr(pValue));
		}
		
		public static unsafe object GetValue(this ICorDebugGenericValue corGenVal, KnownTypeCode type)
		{
			object retValue;
			byte[] value = new byte[(int)corGenVal.GetSize()];
			fixed(byte* pValue = value) {
				corGenVal.GetValue(new IntPtr(pValue));
				switch (type) {
					case KnownTypeCode.Boolean: retValue = *((System.Boolean*)pValue); break;
					case KnownTypeCode.Char:    retValue = *((System.Char*)   pValue); break;
					case KnownTypeCode.SByte:   retValue = *((System.SByte*)  pValue); break;
					case KnownTypeCode.Byte:    retValue = *((System.Byte*)   pValue); break;
					case KnownTypeCode.Int16:   retValue = *((System.Int16*)  pValue); break;
					case KnownTypeCode.UInt16:  retValue = *((System.UInt16*) pValue); break;
					case KnownTypeCode.Int32:   retValue = *((System.Int32*)  pValue); break;
					case KnownTypeCode.UInt32:  retValue = *((System.UInt32*) pValue); break;
					case KnownTypeCode.Int64:   retValue = *((System.Int64*)  pValue); break;
					case KnownTypeCode.UInt64:  retValue = *((System.UInt64*) pValue); break;
					case KnownTypeCode.Single:  retValue = *((System.Single*) pValue); break;
					case KnownTypeCode.Double:  retValue = *((System.Double*) pValue); break;
					case KnownTypeCode.IntPtr:  retValue = *((System.IntPtr*) pValue); break;
					case KnownTypeCode.UIntPtr: retValue = *((System.UIntPtr*)pValue); break;
					default: throw new NotSupportedException();
				}
			}
			return retValue;
		}
		
		public static unsafe void SetValue(this ICorDebugGenericValue corGenVal, KnownTypeCode type, object value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			byte[] val = new byte[(int)corGenVal.GetSize()];
			fixed(byte* pValue = val) {
				switch(type) {
					case KnownTypeCode.Boolean: *((System.Boolean*)pValue) = (System.Boolean)value; break;
					case KnownTypeCode.Char:    *((System.Char*)   pValue) = (System.Char)   value; break;
					case KnownTypeCode.SByte:   *((System.SByte*)  pValue) = (System.SByte)  value; break;
					case KnownTypeCode.Byte:    *((System.Byte*)   pValue) = (System.Byte)   value; break;
					case KnownTypeCode.Int16:   *((System.Int16*)  pValue) = (System.Int16)  value; break;
					case KnownTypeCode.UInt16:  *((System.UInt16*) pValue) = (System.UInt16) value; break;
					case KnownTypeCode.Int32:   *((System.Int32*)  pValue) = (System.Int32)  value; break;
					case KnownTypeCode.UInt32:  *((System.UInt32*) pValue) = (System.UInt32) value; break;
					case KnownTypeCode.Int64:   *((System.Int64*)  pValue) = (System.Int64)  value; break;
					case KnownTypeCode.UInt64:  *((System.UInt64*) pValue) = (System.UInt64) value; break;
					case KnownTypeCode.Single:  *((System.Single*) pValue) = (System.Single) value; break;
					case KnownTypeCode.Double:  *((System.Double*) pValue) = (System.Double) value; break;
					case KnownTypeCode.IntPtr:  *((System.IntPtr*) pValue) = (System.IntPtr) value; break;
					case KnownTypeCode.UIntPtr: *((System.UIntPtr*)pValue) = (System.UIntPtr)value; break;
					default: throw new NotSupportedException();
				}
				corGenVal.SetValue(new IntPtr(pValue));
			}
		}
		
		// ICorDebugModule
		
		public static string GetName(this ICorDebugModule corModule)
		{
			// The 'out' parameter returns the size of the needed buffer as in other functions
			return Util.GetString(corModule.GetName, 256, true);
		}
		
		// ICorDebugProcess
		
		public static bool HasQueuedCallbacks(this ICorDebugProcess corProcess)
		{
			return corProcess.HasQueuedCallbacks(null) != 0;
		}
		
		// ICorDebugStepper
		
		public static unsafe void StepRange(this ICorDebugStepper corStepper, bool bStepIn, int[] ranges)
		{
			fixed(int* pRanges = ranges)
				corStepper.StepRange(bStepIn?1:0, (IntPtr)pRanges, (uint)ranges.Length / 2);
		}
		
		// ICorDebugStringValue
		
		public static string GetString(this ICorDebugStringValue corString)
		{
			uint length = corString.GetLength();
			return Util.GetString(corString.GetString, length, false);
		}
		
		// ICorDebugTypeEnum
		
		public static IEnumerable<ICorDebugType> GetEnumerator(this ICorDebugTypeEnum corTypeEnum)
		{
			corTypeEnum.Reset();
			while (true) {
				ICorDebugType corType = corTypeEnum.Next();
				if (corType != null) {
					yield return corType;
				} else {
					break;
				}
			}
		}
		
		public static ICorDebugType Next(this ICorDebugTypeEnum corTypeEnum)
		{
			ICorDebugType[] corTypes = new ICorDebugType[1];
			uint typesFetched = corTypeEnum.Next(1, corTypes);
			if (typesFetched == 0) {
				return null;
			} else {
				return corTypes[0];
			}
		}
		
		public static List<ICorDebugType> ToList(this ICorDebugTypeEnum corTypeEnum)
		{
			return new List<ICorDebugType>(corTypeEnum.GetEnumerator());
		}
	}
}
