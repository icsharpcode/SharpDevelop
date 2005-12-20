// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class EmbeddedCLRCorDebug
	{
		
		private Debugger.Interop.CorDebug.EmbeddedCLRCorDebug wrappedObject;
		
		internal Debugger.Interop.CorDebug.EmbeddedCLRCorDebug WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public EmbeddedCLRCorDebug(Debugger.Interop.CorDebug.EmbeddedCLRCorDebug wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static EmbeddedCLRCorDebug Wrap(Debugger.Interop.CorDebug.EmbeddedCLRCorDebug objectToWrap)
		{
			return new EmbeddedCLRCorDebug(objectToWrap);
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
		
		public static bool operator ==(EmbeddedCLRCorDebug o1, EmbeddedCLRCorDebug o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(EmbeddedCLRCorDebug o1, EmbeddedCLRCorDebug o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			EmbeddedCLRCorDebug casted = o as EmbeddedCLRCorDebug;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
	}
}
