// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata.Rows
{
	
	public class MethodBody
	{
		public const byte CorILMethod_Fat        = 0x3;
		public const byte CorILMethod_TinyFormat = 0x2;
		public const byte CorILMethod_MoreSects  = 0x8;
		public const byte CorILMethod_InitLocals = 0x10;
		
		uint   flags          = 0;
		uint   headerSize     = 0;
		ushort maxStack       = 8;
		uint   codeSize       = 0;
		uint   localVarSigTok = 0;
		
		byte[] methodData;
		
		public uint Flags {
			get {
				return flags;
			}
			set {
				flags = value;
			}
		}
		public uint HeaderSize {
			get {
				return headerSize;
			}
			set {
				headerSize = value;
			}
		}
		public ushort MaxStack {
			get {
				return maxStack;
			}
			set {
				maxStack = value;
			}
		}
		public uint CodeSize {
			get {
				return codeSize;
			}
			set {
				codeSize = value;
			}
		}
		public uint LocalVarSigTok {
			get {
				return localVarSigTok;
			}
			set {
				localVarSigTok = value;
			}
		}
		public byte[] MethodData {
			get {
				return methodData;
			}
			set {
				methodData = value;
			}
		}
		
		
		public void Load(BinaryReader reader)
		{
			byte flagByte = reader.ReadByte();
			Console.Write("flagByte : " + flagByte.ToString("X"));
			
			switch (flagByte & 0x03) {
				case CorILMethod_Fat:
					byte nextByte       = reader.ReadByte();
					Console.WriteLine("  nextByte : " + nextByte.ToString("X"));
				
					flags        = (uint)(flagByte & ((nextByte & 0x0F) << 8));
					headerSize   = (uint)(nextByte >> 4);
					maxStack     = reader.ReadUInt16();
					codeSize     = reader.ReadUInt32();
					localVarSigTok = reader.ReadUInt32();
					// TODO : CorILMethod_MoreSects
					break;
				case CorILMethod_TinyFormat:
					flags      = (uint)flagByte & 0x03;
					codeSize   = (uint)flagByte >> 2;
					break;
				default:
					throw new System.NotSupportedException("not supported method body flag " + flagByte);
			}
			methodData = new byte[codeSize];
			reader.Read(methodData, 0, (int)codeSize);
		}
	}
}
