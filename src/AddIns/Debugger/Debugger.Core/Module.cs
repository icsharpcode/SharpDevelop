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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Linq;
using Debugger.Interop;
using Debugger.Interop.CorDebug;
using Debugger.Interop.CorSym;
using Debugger.Interop.MetaData;
using Debugger.MetaData;
using ICSharpCode.NRefactory.TypeSystem;

namespace Debugger
{
	public class Module: DebuggerObject, IDisposable
	{
		AppDomain appDomain;
		Process   process;
		
		bool   unloaded = false;
		string name;
		string fullPath = string.Empty;
		
		int orderOfLoading = 0;
		ICorDebugModule corModule;
		ISymUnmanagedReader symReader;
		MetaDataImport metaData;
		
		Task<IUnresolvedAssembly> unresolvedAssembly;
		
		public IUnresolvedAssembly UnresolvedAssembly {
			get { return unresolvedAssembly.Result; }
		}
		
		public IAssembly Assembly {
			get { return this.UnresolvedAssembly.Resolve(appDomain.Compilation.TypeResolveContext); }
		}
		
		public AppDomain AppDomain {
			get { return appDomain; }
		}
		
		public Process Process {
			get { return process; }
		}
		
		NDebugger Debugger {
			get { return this.AppDomain.Process.Debugger; }
		}
		
		[Debugger.Tests.Ignore]
		public MetaDataImport MetaData {
			get {
				return metaData;
			}
		}
		
		public bool Unloaded {
			get {
				return unloaded;
			}
		}
		
		[Debugger.Tests.Ignore]
		public uint GetEntryPoint()
		{
			try {
				if (symReader != null)
					return symReader.GetUserEntryPoint();
				var info = TypeSystemExtensions.GetInfo(Assembly);
				var cecilModule = info.CecilModule;
				if (cecilModule == null)
					return 0;
				var ep = cecilModule.EntryPoint;
				if (ep != null)
					return ep.MetadataToken.ToUInt32();
				return 0;
			} catch {
				return 0;
			}
		}
		
		[Debugger.Tests.Ignore]
		public ISymUnmanagedReader SymReader {
			get {
				return symReader;
			}
		}
		
		[Debugger.Tests.Ignore]
		public ISymUnmanagedDocument[] SymDocuments {
			get {
				ISymUnmanagedDocument[] docs;
				uint maxCount = 2;
				uint fetched;
				do {
					maxCount *= 8;
					docs = new ISymUnmanagedDocument[maxCount];
					symReader.GetDocuments(maxCount, out fetched, docs);
				} while (fetched == maxCount);
				Array.Resize(ref docs, (int)fetched);
				return docs;
			}
		}
		
		[Debugger.Tests.Ignore]
		public ICorDebugModule CorModule {
			get { return corModule; }
		}
		
		[Debugger.Tests.Ignore]
		public ICorDebugModule2 CorModule2 {
			get { return (ICorDebugModule2)corModule; }
		}
		
		[Debugger.Tests.Ignore]
		public ulong BaseAdress {
			get {
				return this.CorModule.GetBaseAddress();
			}
		}
		
		public bool IsDynamic {
			get {
				return this.CorModule.IsDynamic() == 1;
			}
		}
		
		public bool IsInMemory {
			get {
				return this.CorModule.IsInMemory() == 1;
			}
		}
		
		internal uint AppDomainID {
			get {
				return this.CorModule.GetAssembly().GetAppDomain().GetID();
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		[Debugger.Tests.Ignore]
		public string FullPath {
			get {
				return fullPath;
			}
		}
		
		public bool HasSymbols {
			get {
				return symReader != null;
			}
		}
		
		public int OrderOfLoading {
			get {
				return orderOfLoading;
			}
			set {
				orderOfLoading = value;
			}
		}
		
		[Debugger.Tests.Ignore]
		public CorDebugJITCompilerFlags JITCompilerFlags
		{
			get
			{
				uint retval = ((ICorDebugModule2)corModule).GetJITCompilerFlags();
				return (CorDebugJITCompilerFlags)retval;
			}
			set
			{
				// ICorDebugModule2.SetJITCompilerFlags can return successful HRESULTS other than S_OK.
				// Since we have asked the COMInterop layer to preservesig, we need to marshal any failing HRESULTS.
				((ICorDebugModule2)corModule).SetJITCompilerFlags((uint)value);
			}
		}
		
		internal Module(AppDomain appDomain, ICorDebugModule corModule)
		{
			this.appDomain = appDomain;
			this.process = appDomain.Process;
			this.corModule = corModule;
			
			unresolvedAssembly = TypeSystemExtensions.LoadModuleAsync(this, corModule);
			metaData = new MetaDataImport(corModule);
			
			if (IsDynamic || IsInMemory) {
				name     = corModule.GetName();
			} else {
				fullPath = corModule.GetName();
				name     = System.IO.Path.GetFileName(FullPath);
			}
			
			SetJITCompilerFlags();
			
			LoadSymbolsFromDisk(process.Options.SymbolsSearchPaths);
			ResetJustMyCode();
			LoadSymbolsDynamic();
		}
		
		public void UnloadSymbols()
		{
			if (symReader != null) {
				// The interface is not always supported, I did not manage to reproduce it, but the
				// last callbacks in the user's log were UnloadClass and UnloadModule so I guess
				// it has something to do with dynamic modules.
				if (symReader is ISymUnmanagedDispose) {
					((ISymUnmanagedDispose)symReader).Destroy();
				}
				symReader = null;
			}
		}
		
		/// <summary>
		/// Load symblos for on-disk module
		/// </summary>
		public void LoadSymbolsFromDisk(IEnumerable<string> symbolsSearchPaths)
		{
			if (!IsDynamic && !IsInMemory) {
				if (symReader == null) {
					symReader = metaData.GetSymReader(fullPath, string.Join("; ", symbolsSearchPaths ?? new string[0]));
					if (symReader != null) {
						process.TraceMessage("Loaded symbols from disk for " + this.Name);
						OnSymbolsUpdated();
					}
				}
			}
		}
		
		/// <summary>
		/// Load symbols for in-memory module
		/// </summary>
		public void LoadSymbolsFromMemory(IStream pSymbolStream)
		{
			if (this.IsInMemory) {
				UnloadSymbols();
				
				symReader = metaData.GetSymReader(pSymbolStream);
				if (symReader != null) {
					process.TraceMessage("Loaded symbols from memory for " + this.Name);
				} else {
					process.TraceMessage("Failed to load symbols from memory");
				}
				
				OnSymbolsUpdated();
			}
		}
		
		/// <summary>
		/// Load symbols for dynamic module
		/// (as of .NET 4.0)
		/// </summary>
		public void LoadSymbolsDynamic()
		{
			if (this.CorModule is ICorDebugModule3 && this.IsDynamic) {
				Guid guid = new Guid(0, 0, 0, 0xc0, 0, 0, 0, 0, 0, 0, 70);
				try {
					symReader = (ISymUnmanagedReader)((ICorDebugModule3)this.CorModule).CreateReaderForInMemorySymbols(guid);
				} catch (COMException e) {
					// 0x80131C3B The application did not supply symbols when it loaded or created this module, or they are not yet available.
					if ((uint)e.ErrorCode == 0x80131C3B) {
						process.TraceMessage("Failed to load dynamic symbols for " + this.Name);
						return;
					}
					throw;
				}
				TrackedComObjects.Track(symReader);
				process.TraceMessage("Loaded dynamic symbols for " + this.Name);
				OnSymbolsUpdated();
			}
		}
		
		void OnSymbolsUpdated()
		{
			foreach (Breakpoint b in this.Debugger.Breakpoints) {
				b.SetBreakpoint(this);
			}
			ResetJustMyCode();
		}
		
		void SetJITCompilerFlags()
		{
			// ad CORDEBUG_JIT_ENABLE_ENC:
			// see issue #553 on GitHub: "ExecutionEngineException or AccessViolationException (CLR 2.0)
			// when debugging a program that uses System.Data.OleDb"
			// System.Data.dll (partially) is a C++/CLI assembly and EnC is not supported there.
			// trying to set CORDEBUG_JIT_ENABLE_ENC succeeds at first, but leads to strange exceptions afterwards.
			// CORDEBUG_JIT_ENABLE_ENC should only be set for managed-only modules with User Code.
			CorDebugJITCompilerFlags flags;
			if (this.Process.Options.SuppressJITOptimization) {
				flags = CorDebugJITCompilerFlags.CORDEBUG_JIT_DISABLE_OPTIMIZATION;
			} else {
				flags = CorDebugJITCompilerFlags.CORDEBUG_JIT_DEFAULT;
			}

			try
			{
				this.JITCompilerFlags = flags;
			}
			catch (COMException ex)
			{
				Console.WriteLine(string.Format("Failed to set flags with hr=0x{0:x}", ex.ErrorCode));
			}
			
			CorDebugJITCompilerFlags actual = this.JITCompilerFlags;
			if (flags != actual)
				this.Process.TraceMessage("Couldn't set JIT flags to {0}. Actual flags: {1}", flags, actual);
			else
				this.Process.TraceMessage("JIT flags: {0}", actual);
		}
		
		/// <summary>
		/// Sets all code as being 'my code'.  The code will be gradually switch
		/// to not-user-code as encountered according to stepping options.
		/// </summary>
		public void ResetJustMyCode()
		{
			uint unused = 0;
			if (this.Process.Debugger.SymbolSources.All(s => s is PdbSymbolSource) && this.SymReader == null) {
				// Optimization - set the code as non-user right away
				this.CorModule2.SetJMCStatus(0, 0, ref unused);
				return;
			}
			try {
				// Reqires the process to be synchronized!
				this.CorModule2.SetJMCStatus(process.Options.EnableJustMyCode ? 1 : 0, 0, ref unused);
			} catch (COMException e) {
				// Cannot use JMC on this code (likely wrong JIT settings).
				if ((uint)e.ErrorCode == 0x80131323) {
					process.TraceMessage("Cannot use JMC on this code.  Release build?");
					return;
				}
				throw;
			}
		}
		
		public void ApplyChanges(byte[] metadata, byte[] il)
		{
			this.CorModule2.ApplyChanges((uint)metadata.Length, metadata, (uint)il.Length, il);
		}
		
		public void Dispose()
		{
			UnloadSymbols();
			unloaded = true;
		}
		
		public override string ToString()
		{
			return string.Format("{0}", this.Name);
		}
	}
}
