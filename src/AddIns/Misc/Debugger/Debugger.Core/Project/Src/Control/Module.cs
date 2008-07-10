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
using System.Runtime.InteropServices;

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
		
		public event EventHandler<ModuleEventArgs> SymbolsLoaded;
		
		protected virtual void OnSymbolsLoaded(ModuleEventArgs e)
		{
			if (SymbolsLoaded != null) {
				SymbolsLoaded(this, e);
			}
		}
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		NDebugger Debugger {
			get {
				return this.Process.Debugger;
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
			
			LoadSymbols(process.Debugger.SymbolsSearchPaths);
			SetJustMyCodeStatus();
		}
		
		public void LoadSymbols(string[] searchPath)
		{
			if (symReader == null) {
				symReader = metaData.GetSymReader(fullPath, string.Join("; ", searchPath ?? new string[0]));
				if (symReader != null) {
					OnSymbolsLoaded(new ModuleEventArgs(this));
				}
			}
		}
		
		/// <summary>
		/// Finds all classes and methods marked with DebuggerNonUserCode attribute
		/// and marks them for JMC so that they are not stepped into
		/// </summary>
		public void SetJustMyCodeStatus()
		{
			DateTime start = Util.HighPrecisionTimer.Now;
			uint unused = 0;
			
			if (!this.HasSymbols) {
				corModule.CastTo<ICorDebugModule2>().SetJMCStatus(0, 0, ref unused);
				return;
			}
			
			// By default the code is my code
			corModule.CastTo<ICorDebugModule2>().SetJMCStatus(1, 0, ref unused);
			
			// Apply non-user code attributes
			if (this.Debugger.ObeyDebuggerAttributes) {
				foreach(CustomAttributeProps ca in metaData.EnumCustomAttributeProps(0, 0)) {
					MemberRefProps constructorMethod = metaData.GetMemberRefProps(ca.Type);
					TypeRefProps attributeType = metaData.GetTypeRefProps(constructorMethod.DeclaringType);
					if (attributeType.Name == "System.Diagnostics.DebuggerStepThroughAttribute" ||
					    attributeType.Name == "System.Diagnostics.DebuggerNonUserCodeAttribute" ||
					    attributeType.Name == "System.Diagnostics.DebuggerHiddenAttribute")
					{
						if (ca.Owner >> 24 == 0x02) { // TypeDef
							ICorDebugClass2 corClass = corModule.GetClassFromToken(ca.Owner).CastTo<ICorDebugClass2>();
							corClass.SetJMCStatus(0 /* false */);
							this.Process.TraceMessage("Class {0} marked as non-user code", metaData.GetTypeDefProps(ca.Owner).Name);
						}
						if (ca.Owner >> 24 == 0x06) { // MethodDef
							DisableJustMyCode(ca.Owner);
						}
					}
				}
			}
			
			// Mark generated constructors as non-user code
			foreach(uint typeDef in metaData.EnumTypeDefs()) {
				foreach(uint methodDef in metaData.EnumMethodsWithName(typeDef, ".ctor")) {
					try {
						this.SymReader.GetMethod(methodDef);
					} catch (COMException) { // Symbols not found
						DisableJustMyCode(methodDef);
					}
				}
				foreach(uint methodDef in metaData.EnumMethodsWithName(typeDef, ".cctor")) {
					try {
						this.SymReader.GetMethod(methodDef);
					} catch (COMException) { // Symbols not found
						DisableJustMyCode(methodDef);
					}
				}
			}
			
			// Skip properties
			if (this.Debugger.SkipProperties) {
				foreach(uint typeDef in metaData.EnumTypeDefs()) {
					foreach(PropertyProps prop in metaData.EnumPropertyProps(typeDef)) {
						if ((prop.GetterMethod & 0xFFFFFF) != 0) {
							if (!Debugger.SkipOnlySingleLineProperties || IsSingleLine(prop.GetterMethod)) {
								DisableJustMyCode(prop.GetterMethod);
							}
						}
						if ((prop.SetterMethod & 0xFFFFFF) != 0) {
							if (!Debugger.SkipOnlySingleLineProperties || IsSingleLine(prop.SetterMethod)) {
								DisableJustMyCode(prop.SetterMethod);
							}
						}
					}
				}
			}
			
			DateTime end = Util.HighPrecisionTimer.Now;
			this.Process.TraceMessage("Set Just-My-Code for module \"{0}\" ({1} ms)", this.Filename, (end - start).TotalMilliseconds);
		}
		
		void DisableJustMyCode(uint methodDef)
		{
			MethodProps methodProps = metaData.GetMethodProps(methodDef);
			TypeDefProps typeProps = metaData.GetTypeDefProps(methodProps.ClassToken);
			
			ICorDebugFunction2 corFunction = corModule.GetFunctionFromToken(methodProps.Token).CastTo<ICorDebugFunction2>();
			corFunction.SetJMCStatus(0 /* false */);
			this.Process.TraceMessage("Funciton {0}.{1} marked as non-user code", typeProps.Name, methodProps.Name);
		}
		
		bool IsSingleLine(uint methodDef)
		{
			ISymUnmanagedMethod symMethod = this.SymReader.GetMethod(methodDef);
			List<SequencePoint> seqPoints = new List<SequencePoint>(symMethod.SequencePoints);
			seqPoints.Sort();
			
			// Remove initial "{"
			if (seqPoints.Count > 0 &&
			    seqPoints[0].Line == seqPoints[0].EndLine &&
			    seqPoints[0].EndColumn - seqPoints[0].Column <= 1) {
				seqPoints.RemoveAt(0);
			}
			
			// Remove last "}"
			int listIndex = seqPoints.Count - 1;
			if (seqPoints.Count > 0 &&
			    seqPoints[listIndex].Line == seqPoints[listIndex].EndLine &&
			    seqPoints[listIndex].EndColumn - seqPoints[listIndex].Column <= 1) {
				seqPoints.RemoveAt(listIndex);
			}
			
			// Is single line
			return seqPoints.Count == 0 || seqPoints[0].Line == seqPoints[seqPoints.Count - 1].EndLine;
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
