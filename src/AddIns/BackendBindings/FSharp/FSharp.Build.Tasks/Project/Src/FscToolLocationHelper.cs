// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Robert Pickering" email="robert@strangelights.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace FSharp.Build.Tasks
{
	public sealed class FscToolLocationHelper
	{
		public static readonly string ToolName = "fsc.exe";
		
		FscToolLocationHelper()
		{
		}
				
		public static string GetPathToTool()
		{
			string path = null;
			if (Array.Exists<string>(ConfigurationManager.AppSettings.AllKeys, delegate(string k) { return k == "alt_fs_bin_path"; })) {
				path = Path.Combine(ConfigurationManager.AppSettings["alt_fs_bin_path"], ToolName);
				if (!File.Exists(path)) {
					throw new Exception("you are trying to use the app setting alt_fs_bin_path, but fsc.exe is not localed in the given directory");
				}
			} else {
				string[] paths = Environment.GetEnvironmentVariable("path").Split(';');
				path = Array.Find(paths, delegate(string x) {
					bool res = false;
					try {
						res = File.Exists(Path.Combine(x, "fsc.exe"));
					} catch {
						res = false;
					}
					return res;
				});
				if (path != null) {
					path = Path.Combine(path, ToolName);
				} else {
					string[] dirs = Directory.GetDirectories(Environment.GetEnvironmentVariable("ProgramFiles"), "FSharp*");
					List<FileInfo> files = new List<FileInfo>();
					foreach (string dir in dirs) {
						FileInfo file = new FileInfo(Path.Combine(Path.Combine(dir, "bin"), ToolName));
						if (file.Exists) {
							files.Add(file);
						}
					}
					if (files.Count > 0) {
						files.Sort(delegate(FileInfo x, FileInfo y) { return DateTime.Compare(x.CreationTime, y.CreationTime); });
						path = files[0].FullName;
					}
				}
			}
			return path;			
		}
	}
}
