// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	
	
	public class ICorDebugAssembly
	{
		
		private Debugger.Interop.CorDebug.ICorDebugAssembly wrappedObject;
		
		internal Debugger.Interop.CorDebug.ICorDebugAssembly WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}
		
		public ICorDebugAssembly(Debugger.Interop.CorDebug.ICorDebugAssembly wrappedObject)
		{
			this.wrappedObject = wrappedObject;
		}
		
		public static ICorDebugAssembly Wrap(Debugger.Interop.CorDebug.ICorDebugAssembly objectToWrap)
		{
			return new ICorDebugAssembly(objectToWrap);
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
		
		public static bool operator ==(ICorDebugAssembly o1, ICorDebugAssembly o2)
		{
			return ((object)o1 == null && (object)o2 == null) ||
			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);
		}
		
		public static bool operator !=(ICorDebugAssembly o1, ICorDebugAssembly o2)
		{
			return !(o1 == o2);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override bool Equals(object o)
		{
			ICorDebugAssembly casted = o as ICorDebugAssembly;
			return (casted != null) && (casted.WrappedObject == wrappedObject);
		}
		
		
		public void GetProcess(out ICorDebugProcess ppProcess)
		{
			Debugger.Interop.CorDebug.ICorDebugProcess out_ppProcess;
			this.WrappedObject.GetProcess(out out_ppProcess);
			ppProcess = ICorDebugProcess.Wrap(out_ppProcess);
		}
		
		public void GetAppDomain(out ICorDebugAppDomain ppAppDomain)
		{
			Debugger.Interop.CorDebug.ICorDebugAppDomain out_ppAppDomain;
			this.WrappedObject.GetAppDomain(out out_ppAppDomain);
			ppAppDomain = ICorDebugAppDomain.Wrap(out_ppAppDomain);
		}
		
		public void EnumerateModules(out ICorDebugModuleEnum ppModules)
		{
			Debugger.Interop.CorDebug.ICorDebugModuleEnum out_ppModules;
			this.WrappedObject.EnumerateModules(out out_ppModules);
			ppModules = ICorDebugModuleEnum.Wrap(out_ppModules);
		}
		
		public void GetCodeBase(uint cchName, out uint pcchName, System.IntPtr szName)
		{
			this.WrappedObject.GetCodeBase(cchName, out pcchName, szName);
		}
		
		public void GetName(uint cchName, out uint pcchName, System.IntPtr szName)
		{
			this.WrappedObject.GetName(cchName, out pcchName, szName);
		}
	}
}
