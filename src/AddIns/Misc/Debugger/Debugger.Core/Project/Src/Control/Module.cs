// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.MetaData;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	public class Module: DebuggerObject, IDisposable
	{
		Process process;
		
		bool   unloaded = false;
		string fullPath;
		
		int orderOfLoading = 0;
		ICorDebugModule corModule;
		ISymUnmanagedReader symReader;
		MetaDataImport metaData;
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
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
		
		public bool SymbolsLoaded { 
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
		
		public bool JMCStatus {
			set {
				uint unused = 0;
				if (corModule.Is<ICorDebugModule2>()) { // Is the debuggee .NET 2.0?
					(corModule.CastTo<ICorDebugModule2>()).SetJMCStatus(value?1:0, 0, ref unused);
				}
			}
		}
		
		/// <summary> Get all non-generic types defined in this module </summary>
		public List<DebugType> GetDefinedTypes()
		{
			return DebugType.GetDefinedTypesInModule(this);
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
		
		internal Module(Process process, ICorDebugModule pModule)
		{
			this.process = process;
			
			corModule = pModule;
			
			metaData = new MetaDataImport(pModule);
			
			fullPath = pModule.Name;
			
			symReader = metaData.GetSymReader(fullPath, null);
			
			JMCStatus = SymbolsLoaded;
			
			FindNonUserCode();
		}
		
		/// <summary>
		/// Finds all classes and methods marked with DebuggerNonUserCode attribute
		/// and it marks them for JMC so that they are not stepped into
		/// </summary>
		void FindNonUserCode()
		{
			if (this.SymbolsLoaded) {
				foreach(CustomAttributeProps ca in metaData.EnumCustomAttributeProps(0, 0)) {
					MemberRefProps constructorMethod = metaData.GetMemberRefProps(ca.Type);
					TypeRefProps attributeType = metaData.GetTypeRefProps(constructorMethod.DeclaringType);
					if (attributeType.Name == "System.Diagnostics.DebuggerStepThroughAttribute" ||
					    attributeType.Name == "System.Diagnostics.DebuggerNonUserCodeAttribute")
					{
						if (ca.Owner >> 24 == 0x02) { // TypeDef
							ICorDebugClass2 corClass = corModule.GetClassFromToken(ca.Owner).CastTo<ICorDebugClass2>();
							corClass.SetJMCStatus(0 /* false */);
							this.Process.TraceMessage("Class {0} marked as non-user code", metaData.GetTypeDefProps(ca.Owner).Name);
						}
						if (ca.Owner >> 24 == 0x06) { // MethodDef
							ICorDebugFunction2 corFunction = corModule.GetFunctionFromToken(ca.Owner).CastTo<ICorDebugFunction2>();
							corFunction.SetJMCStatus(0 /* false */);
							MethodProps methodProps = metaData.GetMethodProps(ca.Owner);
							this.Process.TraceMessage("Function {0}.{1} marked as non-user code", metaData.GetTypeDefProps(methodProps.ClassToken).Name, methodProps.Name);
						}
					}
				}
			}
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
