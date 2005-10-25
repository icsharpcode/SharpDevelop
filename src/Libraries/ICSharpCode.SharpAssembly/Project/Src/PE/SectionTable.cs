// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.PE {
	
	public class SectionTable
	{
		const int  NAME_LEN = 8;
		
		const uint IMAGE_SCN_CNT_CODE               = 0x20;
		const uint IMAGE_SCN_CNT_INITIALIZED_DATA   = 0x40;
		const uint IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x80;
		const uint IMAGE_SCN_MEM_EXECUTE            = 0x20000000;
		const uint IMAGE_SCN_MEM_READ               = 0x40000000;
		const uint IMAGE_SCN_MEM_WRITE              = 0x80000000;
		
		byte[] name                 = new byte[NAME_LEN];
		uint virtualSize            = 0;
		uint virtualAddress         = 0;
		uint sizeOfRawData          = 0;
		uint pointerToRawData       = 0;
		uint pointerToRelocations   = 0;
		uint pointerToLinenumbers   = 0;
		uint numberOfRelocations    = 0;
		uint numberOfLinenumbers    = 0;
		uint characteristics        = 0;
		
		public string Name {
			get {
				return System.Text.Encoding.ASCII.GetString(name);
			}
			set {
				byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(value);
				name = new byte[NAME_LEN];
				for (int i = 0; i < NAME_LEN; ++i) {
					name[i] = 0;
				}
				Array.Copy(nameBytes, 0, name, 0, Math.Min(NAME_LEN, nameBytes.Length));
			}
		}
		
		
		public uint VirtualSize {
			get {
				return virtualSize;
			}
			set {
				virtualSize = value;
			}
		}
		
		public uint VirtualAddress {
			get {
				return virtualAddress;
			}
			set {
				virtualAddress = value;
			}
		}
		
		public uint SizeOfRawData {
			get {
				return sizeOfRawData;
			}
			set {
				sizeOfRawData = value;
			}
		}
		
		public uint PointerToRawData {
			get {
				return pointerToRawData;
			}
			set {
				pointerToRawData = value;
			}
		}
		
		public uint PointerToRelocations {
			get {
				return pointerToRelocations;
			}
			set {
				pointerToRelocations = value;
			}
		}
		
		public uint PointerToLinenumbers {
			get {
				return pointerToLinenumbers;
			}
			set {
				pointerToLinenumbers = value;
			}
		}
		
		public uint NumberOfRelocations {
			get {
				return numberOfRelocations;
			}
			set {
				numberOfRelocations = value;
			}
		}
		
		public uint NumberOfLinenumbers {
			get {
				return numberOfLinenumbers;
			}
			set {
				numberOfLinenumbers = value;
			}
		}
		
		// characteristics
		public bool ContainsExecutableCode {
			get {
				return (characteristics & IMAGE_SCN_CNT_CODE) == IMAGE_SCN_CNT_CODE;
			}
			set {
				if (value) {
					characteristics |= IMAGE_SCN_CNT_CODE;
				} else {
					characteristics &= ~IMAGE_SCN_CNT_CODE;
				}
			}
		}
		
		public bool ContainsInitializedData {
			get {
				return (characteristics & IMAGE_SCN_CNT_INITIALIZED_DATA) == IMAGE_SCN_CNT_INITIALIZED_DATA;
			}
			set {
				if (value) {
					characteristics |= IMAGE_SCN_CNT_INITIALIZED_DATA;
				} else {
					characteristics &= ~IMAGE_SCN_CNT_INITIALIZED_DATA;
				}
			}
		}
		
		public bool ContainsUninitializedData {
			get {
				return (characteristics & IMAGE_SCN_CNT_UNINITIALIZED_DATA) == IMAGE_SCN_CNT_UNINITIALIZED_DATA;
			}
			set {
				if (value) {
					characteristics |= IMAGE_SCN_CNT_UNINITIALIZED_DATA;
				} else {
					characteristics &= ~IMAGE_SCN_CNT_UNINITIALIZED_DATA;
				}
			}
		}
		
		public bool SectionCanBeExecutedAsCode {
			get {
				return (characteristics & IMAGE_SCN_MEM_EXECUTE) == IMAGE_SCN_MEM_EXECUTE;
			}
			set {
				if (value) {
					characteristics |= IMAGE_SCN_MEM_EXECUTE;
				} else {
					characteristics &= ~IMAGE_SCN_MEM_EXECUTE;
				}
			}
		}
		
		public bool SectionCanBeRead {
			get {
				return (characteristics & IMAGE_SCN_MEM_READ) == IMAGE_SCN_MEM_READ;
			}
			set {
				if (value) {
					characteristics |= IMAGE_SCN_MEM_READ;
				} else {
					characteristics &= ~IMAGE_SCN_MEM_READ;
				}
			}
		}
		
		public bool SectionCanWrittenTo {
			get {
				return (characteristics & IMAGE_SCN_MEM_WRITE) == IMAGE_SCN_MEM_WRITE;
			}
			set {
				if (value) {
					characteristics |= IMAGE_SCN_MEM_WRITE;
				} else {
					characteristics &= ~IMAGE_SCN_MEM_WRITE;
				}
			}
		}
		
		public void LoadFrom(BinaryReader binaryReader)
		{
			binaryReader.Read(name, 0, NAME_LEN);
			virtualSize          = binaryReader.ReadUInt32();
			virtualAddress       = binaryReader.ReadUInt32();
			sizeOfRawData        = binaryReader.ReadUInt32();
			pointerToRawData     = binaryReader.ReadUInt32();
			pointerToRelocations = binaryReader.ReadUInt32();
			pointerToLinenumbers = binaryReader.ReadUInt32();
			numberOfRelocations  = binaryReader.ReadUInt16();
			numberOfLinenumbers  = binaryReader.ReadUInt16();
			characteristics      = binaryReader.ReadUInt32();
		}
	}
}
