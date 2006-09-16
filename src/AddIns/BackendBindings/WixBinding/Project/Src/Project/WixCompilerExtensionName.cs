// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;

namespace ICSharpCode.WixBinding
{
	public class WixCompilerExtensionName
	{
		string assemblyName;
		string className;
		string displayName = String.Empty;

		public WixCompilerExtensionName(string qualifiedName)
		{
			int index = qualifiedName.IndexOf(',');
			if (index >= 0) {
				className = qualifiedName.Substring(0, index).Trim();
				assemblyName = qualifiedName.Substring(index + 1).Trim();
			} else {
				className = qualifiedName;
				assemblyName = String.Empty;
			}
		}
		
		public WixCompilerExtensionName(string assemblyName, string className)
		{
			this.assemblyName = assemblyName;
			this.className = className;
		}
		
		public string AssemblyName {
			get {
				return assemblyName;
			}
		}
		
		/// <summary>
		/// Gets the qualified name for the extension "ClassName, AssemblyName".
		/// </summary>
		public string QualifiedName {
			get {
				if (assemblyName.Length > 0) {
					return String.Concat(className, ", ", assemblyName);
				}
				return className;
			}
		}
		
		public string ClassName {
			get {
				return className;
			}
		}
		
		public string DisplayName {
			get {
				return displayName;
			}
			set {
				displayName = value;
			}
		}
		
		public override bool Equals(object obj)
		{
			WixCompilerExtensionName name = (WixCompilerExtensionName)obj;
			return name.assemblyName == assemblyName && name.className == className;
		}
		
		public override int GetHashCode()
		{
			return assemblyName.GetHashCode() ^ className.GetHashCode();
		}
		
		/// <summary>
		/// Creates a new WixCompilerExtensionName from a string of the form
		/// "AssemblyName, ClassName|DisplayName".
		/// </summary>
		public static WixCompilerExtensionName CreateFromString(string s)
		{
			s = StringParser.Parse(s);
			int index = s.IndexOf("|");
			if (index >= 0) {
				string qualifiedName = s.Substring(0, index);
				string displayName = s.Substring(index + 1);
				WixCompilerExtensionName name = new WixCompilerExtensionName(qualifiedName);
				name.DisplayName = displayName;
				return name;
			} 
			return new WixCompilerExtensionName(s);
		}
	}
}
