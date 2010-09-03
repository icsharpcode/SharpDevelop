// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/*
using Microsoft.Win32; 
using System;

namespace ICSharpCode.FiletypeRegisterer.Unregister
{

	public class MainClass
	{
		const int SHCNE_ASSOCCHANGED = 0x08000000;
		const int SHCNF_IDLIST		 = 0x0;
		
		public static void UnRegisterFiletype(string extension) {
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

		public static void Main() {
			Console.WriteLine("--------------------------------");
			Console.WriteLine(" Filetype Unregister");
			Console.WriteLine("--------------------------------");
			
			string[] args = Environment.GetCommandLineArgs();
			if (args.Length != 2) {
				Console.WriteLine("\nUsage: Unregister.exe ext");
				Console.WriteLine("      where ext is the extension, e.g. cs");
				return;
			}
			
			System.Console.WriteLine("Unregistering " + args[1] + "...");
			UnRegisterFiletype(args[1]);
			Console.WriteLine("\nSuccessful.");
			return;
		}

	}
}
*/
