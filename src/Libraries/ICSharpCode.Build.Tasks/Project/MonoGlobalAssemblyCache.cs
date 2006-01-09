// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace ICSharpCode.Build.Tasks
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
