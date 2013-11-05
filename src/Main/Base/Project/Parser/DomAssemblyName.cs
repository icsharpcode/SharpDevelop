// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Similar to System.Reflection.AssemblyName, but does not raise an exception
	/// on invalid assembly names. (See SD-1307)
	/// </summary>
	[Serializable]
	public sealed class DomAssemblyName : IEquatable<DomAssemblyName>
	{
		readonly string fullAssemblyName;
		readonly string shortName, culture, publicKeyToken;
		readonly Version version;
		
		public DomAssemblyName(string fullAssemblyName)
		{
			this.fullAssemblyName = fullAssemblyName;
			string[] components = fullAssemblyName.Split(',');
			shortName = components[0];
			for (int i = 1; i < components.Length; i++) {
				string val = components[i].Trim();
				int pos = val.IndexOf('=');
				if (pos > 0) {
					switch (val.Substring(0, pos)) {
						case "Version":
							string versionText = val.Substring(pos + 1);
							Version.TryParse(versionText, out version);
							break;
						case "Culture":
							culture = val.Substring(pos + 1);
							break;
						case "PublicKeyToken":
							publicKeyToken = val.Substring(pos + 1);
							break;
					}
				}
			}
		}
		
		public string FullName {
			get { return fullAssemblyName; }
		}
		
		public string ShortName {
			get { return shortName; }
		}
		
		public Version Version {
			get { return version; }
		}
		
		public string Culture {
			get { return culture; }
		}
		
		public string PublicKeyToken {
			get { return publicKeyToken; }
		}
		
		public override string ToString()
		{
			return fullAssemblyName;
		}
		
		public override int GetHashCode()
		{
			return fullAssemblyName.GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as DomAssemblyName);
		}
		
		public bool Equals(DomAssemblyName other)
		{
			return other != null && fullAssemblyName == other.fullAssemblyName;
		}
	}
}
