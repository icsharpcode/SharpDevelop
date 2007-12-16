// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.IO;

namespace Mono.Build.Tasks
{
	public class MonoGlobalAssemblyCache
	{
		MonoGlobalAssemblyCache()
		{
		}
		
		/// <summary>
		/// Gets all the assembly names in the Mono GAC.
		/// </summary>
		/// <param name="folder">The GAC folder</param>
		public static List<MonoAssemblyName> GetAssemblyNames(string folder)
		{
			List<MonoAssemblyName> assemblyNames = new List<MonoAssemblyName>();
			if (folder != null && Directory.Exists(folder)) {
				DirectoryInfo gacDirectoryInfo = new DirectoryInfo(folder);
				foreach (DirectoryInfo assemblyNameDirectoryInfo in gacDirectoryInfo.GetDirectories()) {
					foreach (DirectoryInfo versionDirectoryInfo in assemblyNameDirectoryInfo.GetDirectories()) {
						string assemblyFullName = MonoGacDirectory.GetAssemblyName(assemblyNameDirectoryInfo.Name, versionDirectoryInfo.Name);
						MonoAssemblyName name = CreateMonoAssemblyName(assemblyFullName);
						if (name != null) {
							name.Directory =  versionDirectoryInfo.FullName;
							assemblyNames.Add(name);
						}
					}
				}
			}
			return assemblyNames;			
		}
		
		/// <summary>
		/// Gets all the assembly names in the Mono GAC.
		/// </summary>
		public static List<MonoAssemblyName> GetAssemblyNames()
		{
			return GetAssemblyNames(MonoToolLocationHelper.GetPathToMonoGac());
		}
		
		/// <summary>
		/// Finds the GAC assembly which matches the specified name.
		/// </summary>
		/// <param name="name">
		/// A short name or a fully qualifed assembly name of the form 
		/// (glib-sharp, Version=1.0, Culture=neutral, PublicKeyToken=9449494)
		/// </param>
		public static MonoAssemblyName FindAssemblyName(string name)
		{
			string gacFolder = MonoToolLocationHelper.GetPathToMonoGac();
			if (gacFolder != null) {
				MonoAssemblyName assemblyName = CreateMonoAssemblyName(name);
				if (assemblyName != null) {
					if (assemblyName.IsFullyQualified) {
						MonoGacDirectory directory = MonoGacDirectory.GetAssemblyDirectory(gacFolder, assemblyName);
						if (directory != null) {
							assemblyName.Directory = directory.FullPath;
							return assemblyName;
						}
					} else {
						return FindPartialAssemblyName(assemblyName);
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Tries to match the partially specified assembly name.
		/// </summary>
		static MonoAssemblyName FindPartialAssemblyName(MonoAssemblyName partialName)
		{
			string assemblyDirectoryName = Path.Combine(MonoToolLocationHelper.GetPathToMonoGac(), partialName.Name);
			if (Directory.Exists(assemblyDirectoryName)) {
				MonoAssemblyName matchedName = null;
				DirectoryInfo assemblyNameDirectoryInfo = new DirectoryInfo(assemblyDirectoryName);
				foreach (DirectoryInfo versionDirectoryInfo in assemblyNameDirectoryInfo.GetDirectories()) {
					string assemblyFullName = MonoGacDirectory.GetAssemblyName(assemblyNameDirectoryInfo.Name, versionDirectoryInfo.Name);
					MonoAssemblyName name = CreateMonoAssemblyName(assemblyFullName, versionDirectoryInfo.FullName);
					if (name != null && name.IsFullyQualified) {
						if (matchedName == null) {
							if (partialName.IsMatch(name)) {
								matchedName = name;
							}
						} else if (partialName.IsMatch(name) && name.Version > matchedName.Version) {
							matchedName = name;
						}
					}
				}
				return matchedName;
			}
			
			return null;
		}
		
		static MonoAssemblyName CreateMonoAssemblyName(string name, string directory)
		{
			try {
				return new MonoAssemblyName(name, directory);
			} catch (Exception) { }
				
			return null;
		}
		
		static MonoAssemblyName CreateMonoAssemblyName(string name)
		{
			return CreateMonoAssemblyName(name, String.Empty);
		}
	}
}
