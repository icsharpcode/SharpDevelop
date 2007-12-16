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
using System.Reflection;
using System.Text;

namespace Mono.Build.Tasks
{
	public class MonoAssemblyName
	{
		string name = String.Empty;
		Version version;
		string directory = String.Empty;
		CultureInfo cultureInfo;
		byte[] publicKeyToken;
		
		public MonoAssemblyName(string name) : this(name, String.Empty)
		{
		}
		
		public MonoAssemblyName(string name, string directory)
		{
			AssemblyName assemblyName = new AssemblyName(name);
			this.name = assemblyName.Name;
			version = assemblyName.Version;
			cultureInfo = assemblyName.CultureInfo;
			publicKeyToken = assemblyName.GetPublicKeyToken();
			this.directory = directory;
		}
		
		/// <summary>
		/// Returns whether the assembly name, version, culture and public key token
		/// are specified.
		/// </summary>
		public bool IsFullyQualified {
			get {
				return (name != null) && (version != null) && (cultureInfo != null) && (publicKeyToken != null);
			}
		}
		
		/// <summary>
		/// The short name of the assembly.
		/// </summary>
		public string Name {
			get { return name; }
		}
		
		/// <summary>
		/// Returns the full assembly name of the form:
		/// Name, Version, Culture, Public Key Token.
		/// </summary>
		public string FullName {
			get {
				return GetFullName(name, GetVersionAsString(version), GetCultureAsString(cultureInfo), GetPublicKeyTokenAsString(publicKeyToken));
			}
		}
		
		/// <summary>
		/// Returns the full assembly name of the form:
		/// Name, Version, Culture, Public Key Token.
		/// </summary>
		public static string GetFullName(string name, string version, string cultureInfo, string publicKeyToken)
		{
			StringBuilder fullName = new StringBuilder();
			
			// Add name.
			fullName.Append(name);
			
			// Add version.
			if (version != null) {
				fullName.AppendFormat(", Version={0}", version);
			}
			
			// Add culture.
			string culture = "neutral";
			if (cultureInfo != null) {
				culture = cultureInfo;
			}
			fullName.AppendFormat(", Culture={0}", culture);
			
			// Add public key token.
			if (publicKeyToken != null && publicKeyToken.Length > 0) {
				fullName.AppendFormat(", PublicKeyToken={0}", publicKeyToken);
			}
			return fullName.ToString();
		}
		
		public Version Version {
			get { return version; }
		}
		
		public CultureInfo CultureInfo {
			get { return cultureInfo; }
		}
		
		public byte[] GetPublicKeyToken()
		{
			return publicKeyToken;
		}
		
		public static string GetPublicKeyTokenAsString(byte[] publicKeyToken)
		{
			StringBuilder convertedToken = new StringBuilder();
			if (publicKeyToken != null) {
				foreach (byte b in publicKeyToken) {
					convertedToken.Append(b.ToString("x2"));
				}
			}
			return convertedToken.ToString();
		}
		
		/// <summary>
		/// Gets or sets the full path to the assembly excluding the assembly name.
		/// </summary>
		public string Directory {
			get { return directory; }
			set { directory = value; }
		}
		
		/// <summary>
		/// Gets the full filename of the assembly.
		/// </summary>
		public string FileName {
			get {
				return Path.Combine(directory, String.Concat(name, ".dll"));
			}
		}
		
		/// <summary>
		/// Determines whether the assembly names match.
		/// </summary>
		/// <returns>
		/// Handles partially qualified assembly names and will return 
		/// <see langword="true"/> if the match is a partial match.
		/// </returns>
		public bool IsMatch(MonoAssemblyName assemblyName)
		{
			if (name == assemblyName.Name) {
				if (assemblyName.publicKeyToken != null && publicKeyToken != null) {
					if (assemblyName.publicKeyToken.Length == publicKeyToken.Length) {
						for (int i = 0; i < publicKeyToken.Length; ++i) {
							if (publicKeyToken[i] != assemblyName.publicKeyToken[i]) {
								return false;
							}
						}
					}
				}
				if (assemblyName.version != null && version != null) {
					if (assemblyName.version != version) {
						return false;
					}
				}
				if (assemblyName.cultureInfo != null && cultureInfo != null) {
					if (assemblyName.cultureInfo == cultureInfo) {
						return false;
					}
				}
				return true;
			}
			return false;
		}
		
		static string GetVersionAsString(Version version)
		{
			if (version != null) {
				return version.ToString();
			}
			return null;
		}
		
		static string GetCultureAsString(CultureInfo cultureInfo)
		{
			if (cultureInfo != null && cultureInfo.Name.Length > 0) {
				return cultureInfo.Name;
			}
			return null;
		}
	}
}
