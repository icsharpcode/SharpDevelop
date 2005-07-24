// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Xml;

using Microsoft.Win32;

using ICSharpCode.Core;

using ICSharpCode.SharpDevelop.Gui;


namespace ICSharpCode.FiletypeRegisterer {
	
	public class RegisterFiletypesCommand : AbstractCommand
	{
		readonly public static string uiFiletypesProperty       = "SharpDevelop.Filetypes";
		
		// used in .addin file
		readonly public static string uiRegisterStartupProperty = "SharpDevelop.FiletypesRegisterStartup";
		
		const int SHCNE_ASSOCCHANGED = 0x08000000;
		const int SHCNF_IDLIST		 = 0x0;
		
		public static string DefaultExtensions = "sln|csproj|vbproj";
		
		public static string[,] GetFileTypes()
		{
			try {
				
				XmlDocument doc = new XmlDocument();
				
				doc.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "filetypes\\Filetypes.xml"));
				
				XmlNodeList nodes = doc.DocumentElement.ChildNodes;
				string[,] ret = new string[doc.DocumentElement.ChildNodes.Count, 3];
				
				for(int i = 0; i < nodes.Count; ++i) {
					XmlElement el = (XmlElement)nodes.Item(i);
					ret[i, 0] = el.InnerText;
					ret[i, 1] = el.Attributes["ext"].InnerText;
					ret[i, 2] = el.Attributes["icon"].InnerText;
				}
				return ret;
			} catch (Exception ex) {
				MessageService.ShowError(ex);
				return new string[0, 0];
			}
		}
		
		public override void Run()
		{
			// register Combine and Project by default
			RegisterFiletypes(PropertyService.Get(uiFiletypesProperty, DefaultExtensions));
			
			RegisterUnknownFiletypes();
		}
		
		public static void RegisterFiletypes(string types)
		{
			string[] singleTypes = types.Split('|');
			string mainExe  = Assembly.GetEntryAssembly().Location;
			string[,] FileTypes = GetFileTypes();
			
			string resPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "filetypes") + Path.DirectorySeparatorChar;
			foreach(string type in singleTypes) {
				for(int i = 0; i < FileTypes.GetLength(0); ++i) {
					if(FileTypes[i, 1] == type) {
						RegisterFiletype(type, FileTypes[i, 0], '"' + Path.GetFullPath(mainExe) + '"' + " \"%1\"", Path.GetFullPath(resPath + FileTypes[i, 2]));
					}
				}
			}
		}
		
		public static void RegisterUnknownFiletypes()
		{
			string mainExe  = Assembly.GetEntryAssembly().Location;
			string[,] FileTypes = GetFileTypes();
			string resPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "filetypes") + Path.DirectorySeparatorChar;
			for(int i = 0; i < FileTypes.GetLength(0); ++i) {
				if (!IsRegisteredFileType(FileTypes[i, 1])) {
					RegisterFiletype(FileTypes[i, 1], FileTypes[i, 0], '"' + Path.GetFullPath(mainExe) + '"' + " \"%1\"", Path.GetFullPath(resPath + FileTypes[i, 2]));
				}
			}
		}
		
		public static bool IsRegisteredFileType(string extension)
		{
			RegistryKey key = Registry.ClassesRoot.OpenSubKey("." + extension);
			if (key == null)
				return false;
			key.Close();
			return true;
		}
		
		public static void RegisterFiletype(string extension, string description, string command, string icon)
		{
			try {
				RegistryKey extKey, clsKey, openKey;
				extKey = Registry.ClassesRoot.CreateSubKey("." + extension);
				
				// save previous association
				string prev = (string)extKey.GetValue("", "");
				if (prev != "" && prev != ("SD." + extension + "file")) {
					extKey.SetValue("PreSD", extKey.GetValue(""));
				}
				extKey.SetValue("", "SD." + extension + "file");
				extKey.Close();
				
				clsKey = Registry.ClassesRoot.CreateSubKey("SD." + extension + "file");
				
				
				clsKey.SetValue("", StringParser.Parse(description));
				clsKey.CreateSubKey("DefaultIcon").SetValue("", '"' + icon + '"');
				openKey = clsKey.CreateSubKey("shell\\open\\command");
				openKey.SetValue("", command);
				openKey.Close();
				clsKey.Close();
				
				SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
			} catch {}
			
		}
		
		public static void UnRegisterFiletype(string extension)
		{
			try {
				Registry.ClassesRoot.DeleteSubKeyTree("SD." + extension + "file");
				
				RegistryKey extKey;
				extKey = Registry.ClassesRoot.OpenSubKey("." + extension, true);
				
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
				
				
				SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
			} catch {}
		}
		
		[System.Runtime.InteropServices.DllImport("shell32.dll")]
		static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);
	}
}
