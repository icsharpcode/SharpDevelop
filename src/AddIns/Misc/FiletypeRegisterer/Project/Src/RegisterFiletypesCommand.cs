// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

using ICSharpCode.Core;
using Microsoft.Win32;

namespace ICSharpCode.FiletypeRegisterer {
	
	public class RegisterFiletypesCommand : AbstractCommand
	{
		readonly public static string uiFiletypesProperty       = "SharpDevelop.Filetypes";
		
		// used in .addin file
		readonly public static string uiRegisterStartupProperty = "SharpDevelop.FiletypesRegisterStartup";
		
		const int SHCNE_ASSOCCHANGED = 0x08000000;
		const int SHCNF_IDLIST		 = 0x0;
		
		public static string GetDefaultExtensions(List<FiletypeAssociation> list)
		{
			StringBuilder b = new StringBuilder();
			foreach (FiletypeAssociation a in list) {
				if (a.IsDefault) {
					if (b.Length > 0)
						b.Append('|');
					b.Append(a.Extension);
				}
			}
			return b.ToString();
		}
		
		public override void Run()
		{
			List<FiletypeAssociation> list = FiletypeAssociationDoozer.GetList();
			
			// register Combine and Project by default
			RegisterFiletypes(list, PropertyService.Get(uiFiletypesProperty, GetDefaultExtensions(list)));
			
			RegisterUnknownFiletypes(list);
		}
		
		public static void RegisterFiletypes(List<FiletypeAssociation> allTypes, string types)
		{
			string[] singleTypes = types.Split('|');
			string mainExe  = Assembly.GetEntryAssembly().Location;
			
			foreach (FiletypeAssociation type in allTypes) {
				if (Array.IndexOf(singleTypes, type.Extension) >= 0) {
					RegisterFiletype(type.Extension,
					                 type.Text,
					                 '"' + Path.GetFullPath(mainExe) + '"' + " \"%1\"",
					                 Path.GetFullPath(type.Icon));
				}
			}
		}
		
		public static void RegisterUnknownFiletypes(List<FiletypeAssociation> allTypes)
		{
			string mainExe  = Assembly.GetEntryAssembly().Location;
			string resPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "filetypes") + Path.DirectorySeparatorChar;
			foreach (FiletypeAssociation type in allTypes) {
				if (!IsRegisteredFileType(type.Extension)) {
					RegisterFiletype(type.Extension,
					                 type.Text,
					                 '"' + Path.GetFullPath(mainExe) + '"' + " \"%1\"",
					                 Path.GetFullPath(type.Icon));
				}
			}
		}
		
		public static bool IsRegisteredFileType(string extension)
		{
			try {
				using (RegistryKey key = Registry.ClassesRoot.OpenSubKey("." + extension)) {
					if (key != null)
						return true;
				}
			} catch (System.Security.SecurityException) {
				// registry access might be denied
			}
			try {
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Classes\\." + extension)) {
					return key != null;
				}
			} catch (System.Security.SecurityException) {
				return false;
			}
		}
		
		public static void RegisterFiletype(string extension, string description, string command, string icon)
		{
			try {
				RegisterFiletype(Registry.ClassesRoot, extension, description, command, icon);
			} catch (UnauthorizedAccessException) {
				try {
					RegisterFiletype(Registry.CurrentUser.CreateSubKey("Software\\Classes"), extension, description, command, icon);
				} catch {}
			}
			try {
				SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
			} catch {}
		}
		
		static void RegisterFiletype(RegistryKey rootKey, string extension, string description, string command, string icon)
		{
			RegistryKey extKey, clsKey, openKey;
			extKey = rootKey.CreateSubKey("." + extension);
			// save previous association
			string prev = (string)extKey.GetValue("", "");
			if (prev != "" && prev != ("SD." + extension + "file")) {
				extKey.SetValue("PreSD", extKey.GetValue(""));
			}
			extKey.SetValue("", "SD." + extension + "file");
			extKey.Close();
			
			clsKey = rootKey.CreateSubKey("SD." + extension + "file");
			
			
			clsKey.SetValue("", StringParser.Parse(description));
			clsKey.CreateSubKey("DefaultIcon").SetValue("", '"' + icon + '"');
			openKey = clsKey.CreateSubKey("shell\\open\\command");
			openKey.SetValue("", command);
			openKey.Close();
			clsKey.Close();
		}
		
		public static void UnRegisterFiletype(string extension)
		{
			UnRegisterFiletype(extension, Registry.ClassesRoot);
			try {
				UnRegisterFiletype(extension, Registry.CurrentUser.CreateSubKey("Software\\Classes"));
			} catch {} // catch CreateSubKey(Software\Classes)-exceptions
			try {
				SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
			} catch {}
		}
		
		static void UnRegisterFiletype(string extension, RegistryKey root)
		{
			try {
				root.DeleteSubKeyTree("SD." + extension + "file");
			} catch {}
			try {
				RegistryKey extKey;
				extKey = root.OpenSubKey("." + extension, true);
				
				// if no association return
				if (extKey == null) return;
				// if other association return too
				if ((string)extKey.GetValue("", "") != ("SD." + extension + "file")) return;
				
				// restore previous association
				string prev = (string)extKey.GetValue("PreSD", "");
				if(prev != "") {
					extKey.SetValue("", prev);
				}
				extKey.Close();
				if (prev != null) {
					root.DeleteSubKeyTree("." + extension);
				}
			} catch {}
		}
		
		[System.Runtime.InteropServices.DllImport("shell32.dll")]
		static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
	}
}
