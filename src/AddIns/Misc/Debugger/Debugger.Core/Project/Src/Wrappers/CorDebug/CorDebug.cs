// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class CorDebug
	{
		
		private Debugger.Interop.CorDebug.CorDebug wrappedObject;
		
		internal Debugger.Interop.CorDebug.CorDebug WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public CorDebug(Debugger.Interop.CorDebug.CorDebug wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static CorDebug Wrap(Debugger.Interop.CorDebug.CorDebug objectToWrap)
		{
			return new CorDebug(objectToWrap);
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
		
		public static bool operator ==(CorDebug o1, CorDebug o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(CorDebug o1, CorDebug o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			CorDebug casted = o as CorDebug;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
	}
}
