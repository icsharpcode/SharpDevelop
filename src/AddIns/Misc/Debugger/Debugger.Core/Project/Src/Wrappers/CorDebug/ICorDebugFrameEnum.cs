// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugFrameEnum
	{
		
		private Debugger.Interop.CorDebug.ICorDebugFrameEnum wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugFrameEnum WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugFrameEnum(Debugger.Interop.CorDebug.ICorDebugFrameEnum wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugFrameEnum Wrap(Debugger.Interop.CorDebug.ICorDebugFrameEnum objectToWrap)
		{
			return new ICorDebugFrameEnum(objectToWrap);
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
		
		public static bool operator ==(ICorDebugFrameEnum o1, ICorDebugFrameEnum o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugFrameEnum o1, ICorDebugFrameEnum o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugFrameEnum casted = o as ICorDebugFrameEnum;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void Skip(uint celt)
		{
			this.WrappedObject.Skip(celt);
		}
		
		public void Reset()
		{
			this.WrappedObject.Reset();
		}
		
		public void Clone(out ICorDebugEnum ppEnum)
		{
			Debugger.Interop.CorDebug.ICorDebugEnum out_ppEnum;
			this.WrappedObject.Clone(out out_ppEnum);
			ppEnum = ICorDebugEnum.Wrap(out_ppEnum);
		}
		
		public void GetCount(out uint pcelt)
		{
			this.WrappedObject.GetCount(out pcelt);
		}
		
		public void Next(uint celt, ICorDebugFrame[] frames, out uint pceltFetched)
		{
			Debugger.Interop.CorDebug.ICorDebugFrame[] array_frames = new Debugger.Interop.CorDebug.ICorDebugFrame[frames.Length];
			for (int i = 0; (i < frames.Length); i = (i + 1))
			{
				if ((frames[i] != null))
				{
					array_frames[i] = frames[i].WrappedObject;
				}
			}
			this.WrappedObject.Next(celt, array_frames, out pceltFetched);
			for (int i = 0; (i < frames.Length); i = (i + 1))
			{
				if ((array_frames[i] != null))
				{
					frames[i] = ICorDebugFrame.Wrap(array_frames[i]);
				} else
				{
					frames[i] = null;
				}
			}
		}
	}
}
