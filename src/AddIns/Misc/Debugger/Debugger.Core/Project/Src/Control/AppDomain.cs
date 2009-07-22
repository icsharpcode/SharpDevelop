// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
		
		public Module Mscorlib {
			get {
				foreach(Module m in Process.Modules) {
					if (m.FullPath == "mscorlib.dll" &&
					    m.AppDomain == this) {
						return m;
					}
				}
				throw new DebuggerException("Mscorlib not loaded");
			}
		}
		
		internal ICorDebugAppDomain CorDebugAppDomain {
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