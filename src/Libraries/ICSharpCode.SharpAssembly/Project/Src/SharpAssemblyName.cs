// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpAssembly.Metadata;
using ICSharpCode.SharpAssembly.Metadata.Rows;

namespace ICSharpCode.SharpAssembly.Assembly
{
	/// <summary>
	/// imitates Reflection.AssemblyName, but has less functionality
	/// </summary>
	public class SharpAssemblyName : object
	{
		public string Name;
		public uint   Flags;
		public string Culture;
		public Version Version;
		public byte[] PublicKey;
		public int    RefId;
		
		public string FullName {
			get {
				string cult = (Culture == "" ? "neutral" : Culture);
				
				return Name + ", Version=" + Version.ToString() + ", Culture=" + cult;
						// + ", PublicKeyToken=" + PublicKey.ToString();
			}
		}
		
		public SharpAssemblyName()
		{
		}
	}
}
