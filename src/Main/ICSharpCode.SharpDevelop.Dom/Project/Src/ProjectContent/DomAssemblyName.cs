// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Similar to System.Reflection.AssemblyName, but does not raise an exception
	/// on invalid assembly names. (See SD2-1307)
	/// </summary>
	public sealed class DomAssemblyName
	{
		readonly string fullAssemblyName;
		
		public DomAssemblyName(string fullAssemblyName)
		{
			this.fullAssemblyName = fullAssemblyName;
		}
		
		public string FullName {
			get { return fullAssemblyName; }
		}
		
		internal static DomAssemblyName[] Convert(System.Reflection.AssemblyName[] names)
		{
			if (names == null) return null;
			DomAssemblyName[] n = new DomAssemblyName[names.Length];
			for (int i = 0; i < names.Length; i++) {
				n[i] = new DomAssemblyName(names[i].FullName);
			}
			return n;
		}
	}
}
