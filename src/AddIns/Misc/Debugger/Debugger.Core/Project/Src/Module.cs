// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.MetaData;
using Debugger.Interop.CorDebug;
using Debugger.Interop.CorSym;
using Debugger.Interop.MetaData;

namespace Debugger
{
	public class Module: DebuggerObject, IDisposable
	{
		AppDomain appDomain;
		Process   process;
		
		bool   unloaded = false;
		string fullPath;
		
		int orderOfLoading = 0;
		ICorDebugModule corModule;
		ISymUnmanagedReader symReader;
		MetaDataImport metaData;
		
		public event EventHandler<ModuleEventArgs> SymbolsLoaded;
		
		protected virtual void OnSymbolsLoaded(ModuleEventArgs e)
		{
			if (SymbolsLoaded != null) {
				SymbolsLoaded(this, e);
			}
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
		
		public ISymUnmanagedReader SymReader {
			get {
				return symReader;
			}
		}
		
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
		
		public ICorDebugModule CorModule {
			get {
				return corModule;
			}
		}
		
		public ulong BaseAdress { 
			get {
				return corModule.BaseAddress;
			} 
		}
		
		public bool IsDynamic { 
			get {
				return corModule.IsDynamic == 1;
			} 
		}
		
		public bool IsInMemory { 
			get {
				return corModule.IsInMemory == 1;
			} 
		}
		
		internal uint AppDomainID {
			get {
				return this.CorModule.Assembly.AppDomain.ID;
			}
		}
		
		[Debugger.Tests.Ignore]
		public string FullPath { 
			get {
				return fullPath;
			} 
		}
		
		public string Filename {
			get {
				if (IsDynamic || IsInMemory) return String.Empty;
				return System.IO.Path.GetFileName(FullPath);
			}
		}
		
		[Debugger.Tests.Ignore]
		public string DirectoryName {
			get {
				if (IsDynamic || IsInMemory) return String.Empty;
				return System.IO.Path.GetDirectoryName(FullPath);
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
				if (this.MetaData.GetGenericParamCount(typeDef.Token) == 0) {
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
		
		internal Module(AppDomain appDomain, ICorDebugModule pModule)
		{
			this.appDomain = appDomain;
			this.process = appDomain.Process;
			
			corModule = pModule;
			
			metaData = new MetaDataImport(pModule);
			
			fullPath = pModule.Name;
			
			LoadSymbols(process.Options.SymbolsSearchPaths);
			
			ResetJustMyCodeStatus();
		}
		
		/// <summary> Try to load the debugging symbols (.pdb) from the given path </summary>
		public void LoadSymbols(string[] searchPath)
		{
			if (symReader == null) {
				symReader = metaData.GetSymReader(fullPath, string.Join("; ", searchPath ?? new string[0]));
				if (symReader != null) {
					OnSymbolsLoaded(new ModuleEventArgs(this));
				}
			}
		}
		
		public void UpdateSymbolsFromStream(Debugger.Interop.CorDebug.IStream pSymbolStream)
		{
			if (symReader != null) {
				symReader.As<ISymUnmanagedDispose>().Destroy();
			}
			
			symReader = metaData.GetSymReader(pSymbolStream);
			if (symReader != null) {
				OnSymbolsLoaded(new ModuleEventArgs(this));
			}
		}
		
		/// <summary> Sets all code as being 'my code'.  The code will be gradually
		/// set to not-user-code as encountered acording to stepping options </summary>
		public void ResetJustMyCodeStatus()
		{
			uint unused = 0;
			if (process.Options.StepOverNoSymbols && !this.HasSymbols) {
				// Optimization - set the code as non-user right away
				corModule.CastTo<ICorDebugModule2>().SetJMCStatus(0, 0, ref unused);
				return;
			}
			corModule.CastTo<ICorDebugModule2>().SetJMCStatus(1, 0, ref unused);
		}
		
		public void ApplyChanges(byte[] metadata, byte[] il)
		{
			if (corModule.Is<ICorDebugModule2>()) { // Is the debuggee .NET 2.0?
				(corModule.CastTo<ICorDebugModule2>()).ApplyChanges((uint)metadata.Length, metadata, (uint)il.Length, il);
			}
		}
		
		public void Dispose()
		{
			metaData.Dispose();
			if (symReader != null) {
				symReader.As<ISymUnmanagedDispose>().Destroy();
			}
			
			unloaded = true;
		}
		
		public override string ToString()
		{
			return string.Format("{0}", this.Filename);
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
