// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Collections;
using System.IO;
using ICSharpCode.SharpAssembly.Metadata.Rows;
using MDRows = ICSharpCode.SharpAssembly.Metadata.Rows;

namespace ICSharpCode.SharpAssembly.Metadata
{
	
	public class MetadataTable
	{
		uint reserved;
		byte majorVersion;
		byte minorVersion;
		byte heapSizes;
		ulong valid;
		ulong sorted;
		uint[] rows;
		
		Hashtable tableIndices  = new Hashtable();
		Hashtable tables        = new Hashtable();
		
		// map TABLE_ID to index in rows
		public Hashtable TableIndices {
			get {
				return tableIndices;
			}
		}
		
		public Hashtable Tables {
			get {
				return tables;
			}
		}
		
		public uint Reserved {
			get {
				return reserved;
			}
			set {
				reserved = value;
			}
		}
		public byte MajorVersion {
			get {
				return majorVersion;
			}
			set {
				majorVersion = value;
			}
		}
		public byte MinorVersion {
			get {
				return minorVersion;
			}
			set {
				minorVersion = value;
			}
		}
		public byte HeapSizes {
			get {
				return heapSizes;
			}
			set {
				heapSizes = value;
			}
		}
		public ulong Valid {
			get {
				return valid;
			}
			set {
				valid = value;
			}
		}
		public ulong Sorted {
			get {
				return sorted;
			}
			set {
				sorted = value;
			}
		}
		public uint[] Rows {
			get {
				return rows;
			}
			set {
				rows = value;
			}
		}
		
		public bool FourByteStringIndices {
			get {
				return (heapSizes & 1) == 1;
			}
		}
		public bool FourByteGUIDIndices {
			get {
				return (heapSizes & 2) == 2;
			}
		}
		public bool FourByteBlobIndices {
			get {
				return (heapSizes & 4) == 4;
			}
		}
		
		public AbstractRow[] LoadTable(BinaryReader binaryReader, Type tableType, uint count)
		{
			// rows start at 1, as the indices in the metadata do
			AbstractRow[] array = (AbstractRow[])Array.CreateInstance(tableType, count+1);
			for (int i = 1; i <= count; ++i) {
				array[i] = (AbstractRow)tableType.Assembly.CreateInstance(tableType.FullName);
				array[i].BinaryReader  = binaryReader;
				array[i].MetadataTable = this;
				array[i].LoadRow();
			}
			return array;
		}
		
		public uint GetRowCount(int tableID)
		{
			object index = tableIndices[tableID];
			if (index is uint) {
				return rows[(uint)index];
			}
			return 0;
		}
		
		public uint GetMultipleRowCount(params int[] tableID)
		{
			uint count = 0;
			foreach (int id in tableID) {
				object index = tableIndices[id];
				if (index != null) {
					count += rows[(uint)index];
				}
			}
			return count;
		}
		
		public uint GetMaxRowCount(params int[] tableID)
		{
			uint maxcount = 0;
			foreach (int id in tableID) {
				object index = tableIndices[id];
				if (index is uint) {
					uint count = rows[(uint)index];
					if (count > maxcount) maxcount = count;
				}
			}
			return maxcount;
		}
		
		static int GetTableID(Type type)
		{
			return (int)type.InvokeMember("TABLE_ID", 
			                              BindingFlags.Static |
			                              BindingFlags.Public |
			                              BindingFlags.Instance |
			                              BindingFlags.GetField, null, null, null);
		}
		
		public void LoadFrom(BinaryReader binaryReader)
		{
			reserved     = binaryReader.ReadUInt32();
			majorVersion = binaryReader.ReadByte();
			minorVersion = binaryReader.ReadByte();
			heapSizes    = binaryReader.ReadByte();
			reserved     = binaryReader.ReadByte();
			valid        = binaryReader.ReadUInt64();
			sorted       = binaryReader.ReadUInt64();
			rows = new uint[CalculateNumberOfRows()];
			for (int i = 0; i < rows.Length; ++i) {
				rows[i] = binaryReader.ReadUInt32();
			}
			
			ArrayList types = new ArrayList();
//			ArrayList allTypes = new ArrayList();
			foreach (Type type in typeof(AbstractRow).Assembly.GetTypes()) {
				if (type.IsSubclassOf(typeof(AbstractRow))) {
//					allTypes.Add(type);
					ulong tableBit = (ulong)1 << GetTableID(type);
					if ((valid & tableBit) == tableBit) {
						types.Add(type);
					} 
				}
			}
//			allTypes.Sort(new TypeComparer());
//			foreach (Type type in allTypes) {
//				Console.WriteLine(GetTableID(type) + " -- " + type.Name);
//			}
			
			types.Sort(new TypeComparer());
			
			for (int i = 0; i < types.Count; ++i) {
				tableIndices[GetTableID((Type)types[i])] = (uint)i;
			}
			
			foreach (Type type in types) {
				int id = GetTableID(type);
				Tables[id] = LoadTable(binaryReader, type, rows[(uint)tableIndices[id]]);
			}
		}
		
		int CalculateNumberOfRows()
		{
			int rows = 0;
			ulong v = valid;
			for (int i = 0; i < 64; ++i) {
				rows += (int)(v & 1);
				v /= 2;
			}
			return rows;
		}
		
		class TypeComparer : IComparer
		{
			public int Compare(object o1, object o2)
			{
				return GetTableID((Type)o1).CompareTo(GetTableID((Type)o2));
			}
		}
		

	}
}
