// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
					compilation = TypeSystemExtensions.CreateCompilation(this, assemblies);
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
		
		internal IType ObjectType {
			get { return this.Compilation.FindType(KnownTypeCode.Object); }
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
