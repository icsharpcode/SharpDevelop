// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpAssembly.PE;

namespace ICSharpCode.SharpAssembly.Metadata
{
	
	public class AssemblyMetadata
	{
		const uint MAGIC_SIGN = 0x424A5342;
		ushort majorVersion;
		ushort minorVersion;
		uint   reserved;
		uint   length;
		string versionString;
		ushort flags;
		ushort numerOfStreams;
		
		StreamHeader[] streamHeaders;
		
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
		public uint Reserved {
			get {
				return reserved;
			}
			set {
				reserved = value;
			}
		}
		public uint Length {
			get {
				return length;
			}
			set {
				length = value;
			}
		}
		public string VersionString {
			get {
				return versionString;
			}
			set {
				versionString = value;
			}
		}
		public ushort Flags {
			get {
				return flags;
			}
			set {
				flags = value;
			}
		}
		public ushort NumerOfStreams {
			get {
				return numerOfStreams;
			}
			set {
				numerOfStreams = value;
			}
		}
		public StreamHeader[] StreamHeaders {
			get {
				return streamHeaders;
			}
			set {
				streamHeaders = value;
			}
		}
		
		
		public void LoadFrom(BinaryReader binaryReader)
		{
			uint signature = binaryReader.ReadUInt32();
			if (signature != MAGIC_SIGN) {
				Console.WriteLine("WARNING signature != MAGIC_SIGN ");
			}
			
			majorVersion = binaryReader.ReadUInt16();
			minorVersion = binaryReader.ReadUInt16();
			reserved = binaryReader.ReadUInt32();
			length = binaryReader.ReadUInt32();
			byte[] versionStringBytes = new byte[length];
			binaryReader.Read(versionStringBytes, 0, (int)length);
			versionString = System.Text.Encoding.UTF8.GetString(versionStringBytes);
			flags = binaryReader.ReadUInt16();
			numerOfStreams = binaryReader.ReadUInt16();
			streamHeaders = new StreamHeader[numerOfStreams];
			for (int i = 0; i < numerOfStreams; ++i) {
				streamHeaders[i] = new StreamHeader();
				streamHeaders[i].LoadFrom(binaryReader);
			}
			
		}
	}
}
