// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Debugger.Interop.CorDebug
{
	public static partial class CorDebugExtensionMethods
	{
		// TODO: Remove
		public static T CastTo<T>(this object obj)
		{
			return (T)obj;
		}
		
		// TODO: Remove
		public static bool Is<T>(this object obj)
		{
			return obj is T;
		}
		
		// TODO: Remove
		public static T As<T>(this object obj) where T:class
		{
			return obj as T;
		}
		
		// ICorDebugArrayValue
		
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
		
		// ICorDebugClass2
		
		public static ICorDebugType GetParameterizedType(this ICorDebugClass2 corClass, uint elementType, ICorDebugType[] ppTypeArgs)
		{
			return corClass.GetParameterizedType(elementType, (uint)ppTypeArgs.Length, ppTypeArgs);
		}
		
		// ICorDebugCode
		
		public static unsafe byte[] GetCode(this ICorDebugCode corCode)
		{
			if (corCode.IsIL() == 0) return null;
			byte[] code = new byte[corCode.GetSize()];
			fixed(void* pCode = code)
				corCode.GetCode(0, (uint)code.Length, (uint)code.Length, new IntPtr(pCode));
			return code;
		}
		
		// ICorDebugFrameEnum
		
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
		
		// ICorDebugGenericValue
		
		public static unsafe Byte[] GetRawValue(this ICorDebugGenericValue corGenVal)
		{
			// TODO: Unset fixing insead
			Byte[] retValue = new Byte[(int)corGenVal.GetSize()];
			IntPtr pValue = Marshal.AllocHGlobal(retValue.Length);
			corGenVal.GetValue(pValue);
			Marshal.Copy(pValue, retValue, 0, retValue.Length);
			Marshal.FreeHGlobal(pValue);
			return retValue;
		}
		
		public static unsafe void SetRawValue(this ICorDebugGenericValue corGenVal, byte[] value)
		{
			if (corGenVal.GetSize() != value.Length) throw new ArgumentException("Incorrect length");
			IntPtr pValue = Marshal.AllocHGlobal(value.Length);
			Marshal.Copy(value, 0, pValue, value.Length);
			corGenVal.SetValue(pValue);
			Marshal.FreeHGlobal(pValue);
		}
		
		public static unsafe object GetValue(this ICorDebugGenericValue corGenVal,Type type)
		{
			object retValue;
			IntPtr pValue = Marshal.AllocHGlobal((int)corGenVal.GetSize());
			corGenVal.GetValue(pValue);
			switch(type.FullName) {
				case "System.Boolean": retValue = *((System.Boolean*)pValue); break;
				case "System.Char":    retValue = *((System.Char*)   pValue); break;
				case "System.SByte":   retValue = *((System.SByte*)  pValue); break;
				case "System.Byte":    retValue = *((System.Byte*)   pValue); break;
				case "System.Int16":   retValue = *((System.Int16*)  pValue); break;
				case "System.UInt16":  retValue = *((System.UInt16*) pValue); break;
				case "System.Int32":   retValue = *((System.Int32*)  pValue); break;
				case "System.UInt32":  retValue = *((System.UInt32*) pValue); break;
				case "System.Int64":   retValue = *((System.Int64*)  pValue); break;
				case "System.UInt64":  retValue = *((System.UInt64*) pValue); break;
				case "System.Single":  retValue = *((System.Single*) pValue); break;
				case "System.Double":  retValue = *((System.Double*) pValue); break;
				case "System.IntPtr":  retValue = *((System.IntPtr*) pValue); break;
				case "System.UIntPtr": retValue = *((System.UIntPtr*)pValue); break;
				default: throw new NotSupportedException();
			}
			Marshal.FreeHGlobal(pValue);
			return retValue;
		}
		
		public static unsafe void SetValue(this ICorDebugGenericValue corGenVal, Type type, object value)
		{
			IntPtr pValue = Marshal.AllocHGlobal((int)corGenVal.GetSize());
			switch(type.FullName) {
				case "System.Boolean": *((System.Boolean*)pValue) = (System.Boolean)value; break;
				case "System.Char":    *((System.Char*)   pValue) = (System.Char)   value; break;
				case "System.SByte":   *((System.SByte*)  pValue) = (System.SByte)  value; break;
				case "System.Byte":    *((System.Byte*)   pValue) = (System.Byte)   value; break;
				case "System.Int16":   *((System.Int16*)  pValue) = (System.Int16)  value; break;
				case "System.UInt16":  *((System.UInt16*) pValue) = (System.UInt16) value; break;
				case "System.Int32":   *((System.Int32*)  pValue) = (System.Int32)  value; break;
				case "System.UInt32":  *((System.UInt32*) pValue) = (System.UInt32) value; break;
				case "System.Int64":   *((System.Int64*)  pValue) = (System.Int64)  value; break;
				case "System.UInt64":  *((System.UInt64*) pValue) = (System.UInt64) value; break;
				case "System.Single":  *((System.Single*) pValue) = (System.Single) value; break;
				case "System.Double":  *((System.Double*) pValue) = (System.Double) value; break;
				case "System.IntPtr":  *((System.IntPtr*) pValue) = (System.IntPtr) value; break;
				case "System.UIntPtr": *((System.UIntPtr*)pValue) = (System.UIntPtr)value; break;
				default: throw new NotSupportedException();
			}
			corGenVal.SetValue(pValue);
			Marshal.FreeHGlobal(pValue);
		}
		
		// ICorDebugChainEnum
		
		public static IEnumerable<ICorDebugChain> GetEnumerator(this ICorDebugChainEnum corChainEnum)
		{
			corChainEnum.Reset();
			while (true) {
				ICorDebugChain corChain = corChainEnum.Next();
				if (corChain != null) {
					yield return corChain;
				} else {
					break;
				}
			}
		}
			
		public static ICorDebugChain Next(this ICorDebugChainEnum corChainEnum)
		{
			ICorDebugChain[] corChains = new ICorDebugChain[1];
			uint chainsFetched = corChainEnum.Next(1, corChains);
			if (chainsFetched == 0) {
				return null;
			} else {
				return corChains[0];
			}
		}
		
		// ICorDebugModule
		
		public static string GetName(this ICorDebugModule corModule)
		{
			return Util.GetString(corModule.GetName);
		}
		
		// ICorDebugProcess
		
		public static bool HasQueuedCallbacks(this ICorDebugProcess corProcess)
		{
			int pbQueued;
			corProcess.HasQueuedCallbacks(null, out pbQueued);
			return pbQueued != 0;
		}
		
		// ICorDebugStepper
		
		public static unsafe void StepRange(this ICorDebugStepper corStepper, bool bStepIn, int[] ranges)
		{
			fixed (int* pRanges = ranges)
				corStepper.StepRange(bStepIn?1:0, (IntPtr)pRanges, (uint)ranges.Length / 2);
		}
		
		// ICorDebugStringValue
		
		public static string GetString(this ICorDebugStringValue corString)
		{
			return Util.GetString(corString.GetString, 64, false);
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
