// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugCode
	{
		
		private Debugger.Interop.CorDebug.ICorDebugCode wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugCode WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugCode(Debugger.Interop.CorDebug.ICorDebugCode wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugCode Wrap(Debugger.Interop.CorDebug.ICorDebugCode objectToWrap)
		{
			return new ICorDebugCode(objectToWrap);
		}
		
		public bool Is<T>() where T: class
		{
			try {
				CastTo<T>();
				return true;
			} catch {
				return false;
			}
		}
		
		public T As<T>() where T: class
		{
			try {
				return CastTo<T>();
			} catch {
				return null;
			}
		}
		
		public T CastTo<T>() where T: class
		{
			return (T)Activator.CreateInstance(typeof(T), this.WrappedObject);
		}
		
		public static bool operator ==(ICorDebugCode o1, ICorDebugCode o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugCode o1, ICorDebugCode o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugCode casted = o as ICorDebugCode;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void IsIL(out int pbIL)
		{
			this.WrappedObject.IsIL(out pbIL);
		}
		
		public void GetFunction(out ICorDebugFunction ppFunction)
		{
			Debugger.Interop.CorDebug.ICorDebugFunction out_ppFunction;
			this.WrappedObject.GetFunction(out out_ppFunction);
			ppFunction = ICorDebugFunction.Wrap(out_ppFunction);
		}
		
		public void GetAddress(out ulong pStart)
		{
			this.WrappedObject.GetAddress(out pStart);
		}
		
		public void GetSize(out uint pcBytes)
		{
			this.WrappedObject.GetSize(out pcBytes);
		}
		
		public void CreateBreakpoint(uint offset, out ICorDebugFunctionBreakpoint ppBreakpoint)
		{
			Debugger.Interop.CorDebug.ICorDebugFunctionBreakpoint out_ppBreakpoint;
			this.WrappedObject.CreateBreakpoint(offset, out out_ppBreakpoint);
			ppBreakpoint = ICorDebugFunctionBreakpoint.Wrap(out_ppBreakpoint);
		}
		
		public void GetCode(uint startOffset, uint endOffset, uint cBufferAlloc, System.IntPtr buffer, out uint pcBufferSize)
		{
			this.WrappedObject.GetCode(startOffset, endOffset, cBufferAlloc, buffer, out pcBufferSize);
		}
		
		public void GetVersionNumber(out uint nVersion)
		{
			this.WrappedObject.GetVersionNumber(out nVersion);
		}
		
		public void GetILToNativeMapping(uint cMap, out uint pcMap, System.IntPtr map)
		{
			this.WrappedObject.GetILToNativeMapping(cMap, out pcMap, map);
		}
		
		public void GetEnCRemapSequencePoints(uint cMap, out uint pcMap, System.IntPtr offsets)
		{
			this.WrappedObject.GetEnCRemapSequencePoints(cMap, out pcMap, offsets);
		}
	}
}
