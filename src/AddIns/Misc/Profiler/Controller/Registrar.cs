// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.Profiler.Controller
{
	/// <summary>
	/// Description of Registrar.
	/// </summary>
	static class Registrar
	{
		/// <summary>
		/// Registers a COM component in the Windows Registry.
		/// </summary>
		/// <param name="guid">the GUID of the Component.</param>
		/// <param name="libraryId">The ID-string of the library.</param>
		/// <param name="classId">The ID-string of the class.</param>
		/// <param name="path">The path the to the DLL file containing the class. If the file does not exist FileNotFoundException is thrown.</param>
		/// <param name="is64Bit">Defines whether to register the component as 32- or 64-bit component.</param>
		/// <returns>true, if the component could be registered correctly, otherwise false.</returns>
		public static bool Register(string guid, string libraryId, string classId, string path, bool is64Bit)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (classId == null)
				throw new ArgumentNullException("classId");
			if (libraryId == null)
				throw new ArgumentNullException("libraryId");
			if (guid == null)
				throw new ArgumentNullException("guid");
			
			if (!File.Exists(path))
				throw new FileNotFoundException("DLL not found!");
			
			return Register(is64Bit, guid, libraryId, classId, path);
		}

		static bool Register(bool is64Bit, string guid, string libraryId, string classId, string path)
		{
			try {
				CreateKeys(is64Bit ? ExtendedRegistry.LocalMachine64 : ExtendedRegistry.LocalMachine32, guid, libraryId, classId, path);
			} catch (UnauthorizedAccessException) {
				try {
					CreateKeys(is64Bit ? ExtendedRegistry.CurrentUser64 : ExtendedRegistry.CurrentUser32, guid, libraryId, classId, path);
				} catch (UnauthorizedAccessException) {
					return false;
				}
			}
			
			return true;
		}

		static void CreateKeys(ExtendedRegistry reg, string guid, string libraryId, string classId, string path)
		{
			reg.SetValue("Software\\Classes\\CLSID\\" + guid, null, classId + " Class");
			reg.SetValue("Software\\Classes\\CLSID\\" + guid + "\\InProcServer32", null, path);
			reg.SetValue("Software\\Classes\\CLSID\\" + guid + "\\ProgId", null, libraryId + "." + classId);
		}
		
		static void DeleteKey(ExtendedRegistry reg, string guid)
		{
			reg.DeleteSubkey("Software\\Classes\\CLSID\\" + guid + "\\ProgId");
			reg.DeleteSubkey("Software\\Classes\\CLSID\\" + guid + "\\InProcServer32");
			reg.DeleteSubkey("Software\\Classes\\CLSID\\" + guid);
		}
		
		/// <summary>
		/// Removes the registration of a COM component from the Windows Registry.
		/// </summary>
		/// <param name="guid">The GUID of the component to remove.</param>
		/// <param name="is64Bit">Defines whether to remove the component from 32- or 64-bit registry.</param>
		/// <returns>true, if the component could be removed from the registry correctly, otherwise false.</returns>
		public static bool Deregister(string guid, bool is64Bit)
		{
			if (guid == null)
				throw new ArgumentNullException("guid");
			
			try {
				DeleteKey(is64Bit ? ExtendedRegistry.LocalMachine64 : ExtendedRegistry.LocalMachine32, guid);
				DeleteKey(is64Bit ? ExtendedRegistry.CurrentUser64 : ExtendedRegistry.CurrentUser32, guid);
			} catch (UnauthorizedAccessException) {
				try {
					DeleteKey(is64Bit ? ExtendedRegistry.CurrentUser64 : ExtendedRegistry.CurrentUser32, guid);
				} catch (UnauthorizedAccessException) {
					return false;
				}
			}
			
			return true;
		}
	}
}
