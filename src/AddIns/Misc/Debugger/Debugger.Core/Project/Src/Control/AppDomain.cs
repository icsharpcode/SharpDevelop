// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger.MetaData;
using System;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class AppDomain: DebuggerObject
	{
		Process process;
		
		ICorDebugAppDomain corAppDomain;
		
		[Debugger.Tests.Ignore]
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
		
		DebugType valueType;
		
		internal DebugType ValueType {
			get {
				// TODO: Search only mscorlib module
				if (valueType == null)
					valueType = DebugType.CreateFromType(this, typeof(ValueType));
				return valueType;
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