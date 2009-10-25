// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger.Interop.CorDebug;
using Debugger.MetaData;
using System.Collections.Generic;

namespace Debugger
{
	public class AppDomain: DebuggerObject
	{
		Process process;
		
		ICorDebugAppDomain corAppDomain;
		
		internal Dictionary<ICorDebugType, DebugType> DebugTypeCache = new Dictionary<ICorDebugType, DebugType>();
		
		public Process Process {
			get { return process; }
		}
		
		public uint ID {
			get {
				return corAppDomain.GetID();
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
		
		internal DebugType ObjectType {
			get { return DebugType.CreateFromType(this.Mscorlib, typeof(object)); }
		}
		
		internal ICorDebugAppDomain CorAppDomain {
			get { return corAppDomain; }
		}
		
		internal ICorDebugAppDomain2 CorAppDomain2 {
			get { return (ICorDebugAppDomain2)corAppDomain; }
		}
		
		public AppDomain(Process process, ICorDebugAppDomain corAppDomain)
		{
			this.process = process;
			this.corAppDomain = corAppDomain;
		}
	}
}