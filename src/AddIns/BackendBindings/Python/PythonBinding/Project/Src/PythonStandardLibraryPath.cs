// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
