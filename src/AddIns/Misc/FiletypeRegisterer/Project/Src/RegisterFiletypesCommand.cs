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
		readonly public static string uiRegisterStartupProperty = "SharpDevelop.FiletypesRegisterStartup";
		
		const int SHCNE_ASSOCCHANGED = 0x08000000;
		const int SHCNF_IDLIST		 = 0x0;
		
		readonly public static string[,] FileTypes_err = new string[,]
			{{"${res:ICSharpCode.FiletypeRegisterer.SharpDevelopCombineFileAssozisation}", "cmbx", "cmbx.ico"}, 
			 {"${res:ICSharpCode.FiletypeRegisterer.SharpDevelopProjectFileAssozisation}", "prjx", "prjx.ico"},
			 {"${res:ICSharpCode.FiletypeRegisterer.CSharpSourceFileAssozisation}"       , "cs"  , "cs.ico"}, 
			 {"${res:ICSharpCode.FiletypeRegisterer.VBNetSourceFileAssozisation}"        , "vb"  , "vb.ico"}, 
			 {"${res:ICSharpCode.FiletypeRegisterer.JavaSourceFileAssozisation}"         , "java", "java.ico"},
			 {"${res:ICSharpCode.FiletypeRegisterer.XMLFormFileAssozisation}"            , "xfrm", "xfrm.ico"},
			 {"${res:ICSharpCode.FiletypeRegisterer.ResXResourceFilesFileAssozisation}"  , "resx", "resx.ico"},
			 {"${res:ICSharpCode.FiletypeRegisterer.BinaryResourceFilesFileAssozisation}", "resources", "resx.ico"},
			 {"${res:ICSharpCode.FiletypeRegisterer.XmlFileAssozisation}"                , "xml", "xml.ico"}};
		
		public static string[,] GetFileTypes()
		{
			try {
				
				XmlDocument doc = new XmlDocument();
				
				doc.Load(System.IO.Path.Combine(PropertyService.DataDirectory, "resources") +
				         Path.DirectorySeparatorChar + "filetypes" + 
				         Path.DirectorySeparatorChar + "Filetypes.xml");
			
				XmlNodeList nodes = doc.DocumentElement.ChildNodes;
				string[,] ret = new string[doc.DocumentElement.ChildNodes.Count, 3];
				
				for(int i = 0; i < nodes.Count; ++i) {
					XmlElement el = (XmlElement)nodes.Item(i);
					ret[i, 0] = el.InnerText;
					ret[i, 1] = el.Attributes["ext"].InnerText;
					ret[i, 2] = el.Attributes["icon"].InnerText;
				}
				return ret;
			} catch (Exception) {
				return FileTypes_err;
			}
		}
		
		public override void Run()
		{
			
			
			if (PropertyService.Get(uiRegisterStartupProperty, true)) {
				// register Combine and Project by default
				RegisterFiletypes(PropertyService.Get(uiFiletypesProperty, "cmbx|prjx"));
			}
		}
		
		public static void RegisterFiletypes(string types)
		{
			string[] singleTypes = types.Split('|');
			string mainExe  = Assembly.GetEntryAssembly().Location;
			string[,] FileTypes = GetFileTypes();
			
			string resPath = System.IO.Path.Combine(PropertyService.DataDirectory, "resources") + Path.DirectorySeparatorChar + "filetypes" + Path.DirectorySeparatorChar;
			foreach(string type in singleTypes) {
				for(int i = 0; i < FileTypes.GetLength(0); ++i) {
					if(FileTypes[i, 1] == type) {
						RegisterFiletype(type, FileTypes[i, 0], '"' + Path.GetFullPath(mainExe) + '"' + " \"%1\"", Path.GetFullPath(resPath + FileTypes[i, 2]));
					}
				}
			}
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
