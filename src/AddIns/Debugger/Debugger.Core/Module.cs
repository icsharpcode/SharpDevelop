// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Interop;
using Debugger.Interop.CorDebug;
using Debugger.Interop.CorSym;
using Debugger.Interop.MetaData;
using Debugger.MetaData;

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
		
		internal Dictionary<string, DebugType> LoadedDebugTypes = new Dictionary<string, DebugType>();
		
		/// <summary>
		/// Occurs when symbols are loaded or unloaded (for memory modules)
		/// </summary>
		public event EventHandler<ModuleEventArgs> SymbolsUpdated;
		
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
		
		/// <summary> Returns all non-generic types defined in the module </summary>
		/// <remarks> Generic types can not be returned, because we do not know how to instanciate them </remarks>
		public List<DebugType> GetDefinedTypes()
		{
			List<DebugType> types = new List<DebugType>();
			foreach(TypeDefProps typeDef in this.MetaData.EnumTypeDefProps()) {
				if (this.MetaData.EnumGenericParams(typeDef.Token).Length == 0) {
					types.Add(DebugType.CreateFromTypeDefOrRef(this, null, typeDef.Token, null));
				}
			}
			return types;
		}
		
		/// <summary> Get names of all generic and non-generic types defined in this module </summary>
		public List<string> GetNamesOfDefinedTypes()
		{
			List<string> names = new List<string>();
			foreach(TypeDefProps typeProps in this.MetaData.EnumTypeDefProps()) {
				names.Add(typeProps.Name);
			}
			return names;
		}
		
		internal Module(AppDomain appDomain, ICorDebugModule corModule)
		{
			this.appDomain = appDomain;
			this.process = appDomain.Process;
			this.corModule = corModule;
			
			metaData = new MetaDataImport(corModule);
			
			if (IsDynamic || IsInMemory) {
				name     = corModule.GetName();
			} else {
				fullPath = corModule.GetName();
				name     = System.IO.Path.GetFileName(FullPath);
			}
			
			LoadSymbolsFromDisk(process.Options.SymbolsSearchPaths);
			ResetJustMyCodeStatus();
		}
		
		public void UnloadSymbols()
		{
			if (symReader != null) {
				((ISymUnmanagedDispose)symReader).Destroy();
				symReader = null;
			}
		}
		
		/// <summary>
		/// Load symblos for on-disk module
		/// </summary>
		public void LoadSymbolsFromDisk(string[] searchPath)
		{
			if (!IsDynamic && !IsInMemory) {
				if (symReader == null) {
					symReader = metaData.GetSymReader(fullPath, string.Join("; ", searchPath ?? new string[0]));
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
			SetBreakpoints();
			ResetJustMyCodeStatus();
			if (SymbolsUpdated != null) {
				SymbolsUpdated(this, new ModuleEventArgs(this));
			}
		}
		
		void SetBreakpoints() 
		{
			if (this.HasSymbols) {
				// This is in case that the client modifies the collection as a response to set breakpoint
				// NB: If client adds new breakpoint, it will be set directly as a result of his call, not here (because module is already loaded)
				List<Breakpoint> collection = new List<Breakpoint>();
				collection.AddRange(this.Debugger.Breakpoints);
				
				foreach (Breakpoint b in collection) {
					b.SetBreakpoint(this);
				}
			}
		}
		
		/// <summary> Sets all code as being 'my code'.  The code will be gradually
		/// set to not-user-code as encountered acording to stepping options </summary>
		public void ResetJustMyCodeStatus()
		{
			uint unused = 0;
			if (process.Options.StepOverNoSymbols && !this.HasSymbols) {
				// Optimization - set the code as non-user right away
				this.CorModule2.SetJMCStatus(0, 0, ref unused);
				return;
			}
			try {
				this.CorModule2.SetJMCStatus(1, 0, ref unused);
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
			metaData.Dispose();
			
			unloaded = true;
		}
		
		public override string ToString()
		{
			return string.Format("{0}", this.Name);
		}
	}
	
	[Serializable]
	public class ModuleEventArgs : ProcessEventArgs
	{
		Module module;
		
		public Module Module {
			get {
				return module;
			}
		}
		
		public ModuleEventArgs(Module module): base(module.Process)
		{
			this.module = module;
		}
	}
}
