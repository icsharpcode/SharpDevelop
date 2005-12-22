// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;
using Debugger.Interop.MetaData;

namespace Debugger
{
	public class Module: RemotingObjectBase, IDisposable
	{
		NDebugger debugger;
		
		bool   unloaded = false;
		string fullPath;
		string fullPathPDB;
		
		int orderOfLoading = 0;
		readonly ICorDebugModule corModule;
		SymReader symReader;
		IMetaDataImport metaDataInterface;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		internal MetaData MetaData {
			get {
				return new MetaData(metaDataInterface);
			}
		}
		
		public bool Unloaded {
			get {
				return unloaded;
			}
		}
		
		public SymReader SymReader {
			get {
				return symReader;
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
		
		internal Module(NDebugger debugger, ICorDebugModule pModule)
		{
			this.debugger = debugger;
			
			corModule = pModule;
			
			Guid metaDataInterfaceGuid = new Guid("{ 0x7dac8207, 0xd3ae, 0x4c75, { 0x9b, 0x67, 0x92, 0x80, 0x1a, 0x49, 0x7d, 0x44 } }");
			object pMetaDataInterface = pModule.GetMetaDataInterface(ref metaDataInterfaceGuid);
			
			metaDataInterface = (IMetaDataImport) pMetaDataInterface;
			
			uint pStringLenght = 0; // Terminating character included in pStringLenght
			IntPtr pString = IntPtr.Zero;
			pModule.GetName(pStringLenght,
							out pStringLenght, // real string lenght
			                pString);
			// Allocate string buffer
			pString = Marshal.AllocHGlobal((int)pStringLenght * 2);
			pModule.GetName(pStringLenght,
							out pStringLenght, // real string lenght
			                pString);
			fullPath = Marshal.PtrToStringUni(pString);
			Marshal.FreeHGlobal(pString);
			
			
			SymBinder symBinder = new SymBinder();
			IntPtr ptr = IntPtr.Zero;
			try {
				ptr = Marshal.GetIUnknownForObject(metaDataInterface);
				symReader = (SymReader)symBinder.GetReader(ptr, fullPath, string.Empty);
			} catch (System.Exception) {
				symReader = null;
			} finally {
				if (ptr != IntPtr.Zero) {
					Marshal.Release(ptr);
				}
			}
			
			if (symReader != null) {
				string tempPath = Path.Combine(Path.GetTempPath(), Path.Combine("DebeggerPdb", new Random().Next().ToString()));
				string pdbFilename = Path.GetFileNameWithoutExtension(FullPath) + ".pdb";
				string oldPdbPath = Path.Combine(Path.GetDirectoryName(FullPath), pdbFilename);
				string newPdbPath = Path.Combine(tempPath, pdbFilename);
				
				Directory.CreateDirectory(tempPath);
				File.Copy(oldPdbPath, newPdbPath);
				symReader.UpdateSymbolStore(newPdbPath, IntPtr.Zero);
				
				fullPathPDB = newPdbPath;
			}
			
			JMCStatus = SymbolsLoaded;
		}
		
		public void ApplyChanges(byte[] metadata, byte[] il)
		{
			if (corModule.Is<ICorDebugModule2>()) { // Is the debuggee .NET 2.0?
				(corModule.CastTo<ICorDebugModule2>()).ApplyChanges((uint)metadata.Length, metadata, (uint)il.Length, il);
			}
		}
		
		public void Dispose()
		{
			if (symReader != null) {
				try {
					System.Reflection.MethodInfo m = symReader.GetType().GetMethod("{dtor}");
					m.Invoke(symReader, null);
				} catch {
					Console.WriteLine("symReader release failed. ({dtor})");
				} finally {
					symReader = null;
				}
				try {
					File.Delete(fullPathPDB);
				} catch {
					Console.WriteLine("Could not delete pdb temp file");
				}
			}
			
			try {
				Marshal.FinalReleaseComObject(metaDataInterface);
			} catch {
				Console.WriteLine("metaDataInterface release failed. (FinalReleaseComObject)");
			} finally {
				metaDataInterface = null;
			}
			
			unloaded = true;
		}
	}
}
