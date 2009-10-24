// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger.MetaData;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class AppDomain: DebuggerObject
	{
		Process process;
		
		ICorDebugAppDomain corAppDomain;
		
		public Process Process {
			get { return process; }
		}
		
		public uint ID {
			get {
				return corAppDomain.ID;
			}
		}
		
		Module mscorlib;
		
		public Module Mscorlib {
			get {
				if (mscorlib != null) return mscorlib;
				foreach(Module m in Process.Modules) {
					if (m.Filename == "mscorlib.dll" &&
					    m.AppDomain == this) {
						mscorlib = m;
						return mscorlib;
					}
				}
				throw new DebuggerException("Mscorlib not loaded");
			}
		}
		
		DebugType objectType;
		
		public DebugType ObjectType {
			get {
				if (objectType == null)
					objectType = DebugType.CreateFromType(this.Mscorlib, typeof(object));
				return objectType;
			}
		}
		
		DebugType valueType;
		
		public DebugType ValueType {
			get {
				if (valueType == null)
					valueType = DebugType.CreateFromType(this.Mscorlib, typeof(ValueType));
				return valueType;
			}
		}
		
		DebugType voidType;
		
		public DebugType VoidType {
			get {
				if (voidType == null)
					voidType = DebugType.CreateFromType(this.Mscorlib, typeof(void));
				return voidType;
			}
		}
		
		internal ICorDebugAppDomain CorAppDomain {
			get {
				return corAppDomain;
			}
		}
		
		public AppDomain(Process process, ICorDebugAppDomain corAppDomain)
		{
			this.process = process;
			this.corAppDomain = corAppDomain;
		}
	}
}