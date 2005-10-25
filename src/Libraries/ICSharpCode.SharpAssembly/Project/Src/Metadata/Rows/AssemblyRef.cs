// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows {
	
	public class AssemblyRef : AbstractRow
	{
		public static readonly int TABLE_ID = 0x23;
		
		ushort major;
		ushort minor;
		ushort build;
		ushort revision;
		uint flags;
		uint publicKeyOrToken; // index into Blob heap
		uint name;    // index into String heap
		uint culture; // index into String heap
		uint hashValue; // index into Blob heap
		
		public ushort Major {
			get {
				return major;
			}
			set {
				major = value;
			}
		}
		public ushort Minor {
			get {
				return minor;
			}
			set {
				minor = value;
			}
		}
		
		public ushort Build {
			get {
				return build;
			}
			set {
				build = value;
			}
		}
		
		public ushort Revision {
			get {
				return revision;
			}
			set {
				revision = value;
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
		public uint PublicKeyOrToken {
			get {
				return publicKeyOrToken;
			}
			set {
				publicKeyOrToken = value;
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
		public uint HashValue {
			get {
				return hashValue;
			}
			set {
				hashValue = value;
			}
		}
		
		public override void LoadRow()
		{
			major			 = binaryReader.ReadUInt16();
			minor			 = binaryReader.ReadUInt16();
			build			 = binaryReader.ReadUInt16();
			revision		 = binaryReader.ReadUInt16();
			flags            = binaryReader.ReadUInt32();
			publicKeyOrToken = LoadBlobIndex();
			name             = LoadStringIndex();
			culture          = LoadStringIndex();
			hashValue        = LoadBlobIndex();
		}
	}
}
