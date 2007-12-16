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
using System.Globalization;
using System.IO;
using System.Text;

namespace Mono.Build.Tasks
{
	public class MonoGacDirectory
	{
		string gacBaseDirectory = String.Empty;
		MonoAssemblyName assemblyName;
		
		public MonoGacDirectory()
		{
		}
		
		public MonoGacDirectory(string gacBaseDirectory, MonoAssemblyName assemblyName)
		{
			this.gacBaseDirectory = gacBaseDirectory;
			this.assemblyName = assemblyName;
		}
		
		public MonoGacDirectory(string gacBaseDirectory, string assemblyDirectoryName, string versionDirectoryName)
		{
			string assemblyDirectory = Path.Combine(gacBaseDirectory, Path.Combine(assemblyDirectoryName, versionDirectoryName));
			assemblyName = new MonoAssemblyName(GetAssemblyName(assemblyDirectoryName, versionDirectoryName), assemblyDirectory);
		}
		
		/// <summary>
		/// The assembly name associated with this directory.
		/// </summary>
		public MonoAssemblyName AssemblyName {
			get { return assemblyName; }
		}
		
		/// <summary>
		/// Gets the full path to the directory containing the assembly.
		/// </summary>
		public string FullPath {
			get {
				return Path.Combine(gacBaseDirectory, Path.Combine(assemblyName.Name, GetSubFolderName(assemblyName.Version, assemblyName.CultureInfo, assemblyName.GetPublicKeyToken())));
			}
		}
		
		/// <summary>
		/// Gets the Mono Gac directory based on the assembly name.
		/// </summary>
		/// <remarks>
		/// If the assembly name is not fully qualified then this method returns
		/// null.
		/// </remarks>
		public static MonoGacDirectory GetAssemblyDirectory(string gacBaseDirectory, MonoAssemblyName assemblyName)
		{
			if (assemblyName.IsFullyQualified) {
				return new MonoGacDirectory(gacBaseDirectory, assemblyName);
			}
			return null;
		}
		
		public static string GetAssemblyName(string assemblyDirectoryName, string versionDirectoryName)
		{	
			string version = null;
			string culture = null;
			string publicKeyToken = null;
			
			if (versionDirectoryName != null && versionDirectoryName.Length > 0) {
				string[] items = versionDirectoryName.Split('_');
				if (items.Length == 3) {
					if (items[0].Length > 0) {
						version = items[0];
					} 
					if (items[1].Length > 0) {
						culture = items[1];
					}
					if (items[2].Length > 0) {
						publicKeyToken = items[2];
					}
				}
			}
			return MonoAssemblyName.GetFullName(assemblyDirectoryName, version, culture, publicKeyToken);
		}
		
		static string GetSubFolderName(Version version, CultureInfo culture, byte[] publicKeyToken)
		{
			StringBuilder folderName = new StringBuilder();
			
			// Add version.
			if (version != null) {
				folderName.Append(version.ToString());
			}
			folderName.Append("_");
			
			// Add culture.
			if (culture != null) {
				folderName.Append(culture.Name);
			}
			folderName.Append("_");
			
			// Add public key token.
			if (publicKeyToken != null) {
				foreach (byte b in publicKeyToken) {
					folderName.Append(b.ToString("x2"));
				}
			}
			
			return folderName.ToString();
		}
	}
}
