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
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	public class Module: RemotingObjectBase, IDisposable
	{
		NDebugger debugger;
		
		bool   unloaded = false;
		string fullPath;
		string fullPathPDB;
		
		int orderOfLoading = 0;
		ICorDebugModule corModule;
		SymReader symReader;
		MetaData metaData;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		internal MetaData MetaData {
			get {
				return metaData;
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
			
			metaData = new MetaData(pModule);
			
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
			
			string tempPath = String.Empty;
			string pdbFilename = String.Empty;
			string oldPdbPath = String.Empty;
			string newPdbPath = String.Empty;
			try {
				tempPath = Path.Combine(Path.GetTempPath(), Path.Combine("DebeggerPdb", new Random().Next().ToString()));
				pdbFilename = Path.GetFileNameWithoutExtension(FullPath) + ".pdb";
				oldPdbPath = Path.Combine(Path.GetDirectoryName(FullPath), pdbFilename);
				newPdbPath = Path.Combine(tempPath, pdbFilename);
				if (File.Exists(oldPdbPath)) {
					Directory.CreateDirectory(tempPath);
					File.Move(oldPdbPath, newPdbPath);
				}
				fullPathPDB = newPdbPath;
			} catch {}
			
			symReader = metaData.GetSymReader(fullPath, tempPath);
			
			try {
				if (File.Exists(newPdbPath) && !File.Exists(oldPdbPath)) {
					File.Copy(newPdbPath, oldPdbPath);
				}
			} catch {}
			
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
			
			metaData.Dispose();
			
			unloaded = true;
		}
	}
}
