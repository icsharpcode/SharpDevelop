// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class Assembly : AbstractRow
	{
		public static readonly int TABLE_ID = 0x20;
		
		uint  hashAlgID;
		ushort majorVersion;
		ushort minorVersion;
		ushort buildNumber;
		ushort revisionNumber;
		uint   flags;
		
		uint publicKey; // index into the BLOB heap
		uint name;      // index into the string heap
		uint culture;   // index into the string heap
		
		public uint HashAlgID {
			get {
				return hashAlgID;
			}
			set {
				hashAlgID = value;
			}
		}
		public ushort MajorVersion {
			get {
				return majorVersion;
			}
			set {
				majorVersion = value;
			}
		}
		public ushort MinorVersion {
			get {
				return minorVersion;
			}
			set {
				minorVersion = value;
			}
		}
		public ushort BuildNumber {
			get {
				return buildNumber;
			}
			set {
				buildNumber = value;
			}
		}
		public ushort RevisionNumber {
			get {
				return revisionNumber;
			}
			set {
				revisionNumber = value;
			}
		}
		public uint Flags {
			get {
				return flags;
			}
			set {
				flags = value;
			}
		}
		public uint PublicKey {
			get {
				return publicKey;
			}
			set {
				publicKey = value;
			}
		}
		public uint Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		public uint Culture {
			get {
				return culture;
			}
			set {
				culture = value;
			}
		}
		
		public override void LoadRow()
		{
			hashAlgID      = binaryReader.ReadUInt32();
			majorVersion   = binaryReader.ReadUInt16();
			minorVersion   = binaryReader.ReadUInt16();
			buildNumber    = binaryReader.ReadUInt16();
			revisionNumber = binaryReader.ReadUInt16();
			flags          = binaryReader.ReadUInt32();
			publicKey      = LoadBlobIndex();
			name           = LoadStringIndex();
			culture        = LoadStringIndex();
		}
	}
}
