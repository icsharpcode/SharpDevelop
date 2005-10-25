// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class AssemblyOS : AbstractRow
	{
		public static readonly int TABLE_ID = 0x22;
		
		
		uint osPlatformID;
		uint osMajorVersion;
		uint osMinorVersion;
		
		public uint OSPlatformID {
			get {
				return osPlatformID;
			}
			set {
				osPlatformID = value;
			}
		}
		public uint OSMajorVersion {
			get {
				return osMajorVersion;
			}
			set {
				osMajorVersion = value;
			}
		}
		public uint OSMinorVersion {
			get {
				return osMinorVersion;
			}
			set {
				osMinorVersion = value;
			}
		}
		
		
		public override void LoadRow()
		{
			osPlatformID   = binaryReader.ReadUInt32();
			osMajorVersion = binaryReader.ReadUInt32();
			osMinorVersion = binaryReader.ReadUInt32();
		}
	}
}
