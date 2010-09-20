// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Win32;

namespace ICSharpCode.PythonBinding
{
	public class PythonStandardLibraryPath
	{
		List<string> directories = new List<string>();
		string path = String.Empty;
		
		public PythonStandardLibraryPath(string path)
		{
			Path = path;
		}
		
		public PythonStandardLibraryPath()
		{
			ReadPathFromRegistry();
		}
		
		void ReadPathFromRegistry()
		{
			try {
				using (RegistryKey registryKey = GetPythonLibraryRegistryKey()) {
					if (registryKey != null) {
						Path = (string)registryKey.GetValue(String.Empty, String.Empty);
					}
				}
			} catch (SecurityException) {
			} catch (UnauthorizedAccessException) {
			} catch (IOException) {
			}
		}
		
		RegistryKey GetPythonLibraryRegistryKey()
		{
			return Registry.LocalMachine.OpenSubKey(@"Software\Python\PythonCore\2.6\PythonPath");
		}
		
		public string[] Directories {
			get { return directories.ToArray(); }
		}
		
		public string Path {
			get { return path; }
			set {
				path = value;
				ReadDirectories();
			}
		}
		
		void ReadDirectories()
		{
			directories.Clear();
			foreach (string item in path.Split(';')) {
				string directory = item.Trim();
				if (!String.IsNullOrEmpty(directory)) {
					directories.Add(directory);
				}
			}
		}
		
		public bool HasPath {
			get { return directories.Count > 0; }
		}
	}
}
