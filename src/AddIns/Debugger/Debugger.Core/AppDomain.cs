// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using Debugger.Interop.CorDebug;
using Debugger.MetaData;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace Debugger
{
	public class AppDomain: DebuggerObject
	{
		Process process;
		
		ICorDebugAppDomain corAppDomain;
		
		internal Dictionary<ICorDebugType, DebugType> DebugTypeCache = new Dictionary<ICorDebugType, DebugType>();
		
		ICompilation compilation;
		
		internal void InvalidateCompilation()
		{
			compilation = null;
		}
		
		public ICompilation Compilation {
			get {
				if (compilation == null) {
					List<IUnresolvedAssembly> assemblies = new List<IUnresolvedAssembly>();
					foreach (var module in process.Modules) {
						if (module.AppDomain == this) {
							assemblies.Add(module.UnresolvedAssembly);
						}
					}
					if (assemblies.Count == 0)
						compilation = new SimpleCompilation(MinimalCorlib.Instance);
					else
						compilation = new SimpleCompilation(assemblies[0], assemblies.Skip(1));
				}
				return compilation;
			}
		}
		
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
					if (m.Name == "mscorlib.dll" &&
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
