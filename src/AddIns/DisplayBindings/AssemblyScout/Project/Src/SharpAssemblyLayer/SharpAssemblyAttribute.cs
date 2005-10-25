// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpAssembly.Metadata.Rows;
using ICSharpCode.SharpAssembly.Metadata;
using ICSharpCode.SharpAssembly.PE;
using SA = ICSharpCode.SharpAssembly.Assembly;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	[Serializable]
	public class SharpAssemblyAttribute : DefaultAttribute
	{
		SharpAssemblyClass attributeType;
		
		public SharpAssemblyClass AttributeType {
			get {
				return attributeType;
			}
		}
		
		public SharpAssemblyAttribute(SA.SharpAssembly assembly, SA.SharpCustomAttribute attribute) : base(null)
		{	
			uint sigOffset = 0;
			
			if (attribute.IsMemberRef) {
				MemberRef[] memberTable = assembly.Tables.MemberRef;
				TypeRef[] typeRefTable  = assembly.Tables.TypeRef;
				
				sigOffset = memberTable[attribute.MemberIndex].Signature;
				uint trIndex = memberTable[attribute.MemberIndex].Class;
				
				int table = assembly.Reader.GetCodedIndexTable(CodedIndex.MemberRefParent, ref trIndex);
				if (table != 1) {
					Console.WriteLine("SharpAssemblyAttribute: unsupported MemberRefParent coded index");
					return; // unsupported
				}
				
				attributeType = SharpAssemblyClass.FromTypeRef(assembly, trIndex);
				
			} else {
				TypeDef[] typeDefTable = assembly.Tables.TypeDef;
				
				sigOffset = assembly.Tables.Method[attribute.MemberIndex].Signature;
				uint tdIndex = 0;
				
				for (uint i = 1; i <= typeDefTable.GetUpperBound(0); ++i) {
					if (typeDefTable[i].MethodList <= attribute.MemberIndex && i == typeDefTable.GetUpperBound(0)) {	
						tdIndex = i;
						break;
					}
					if (typeDefTable[i].MethodList <= attribute.MemberIndex && typeDefTable[i+1].MethodList > attribute.MemberIndex) {
						tdIndex = i;
						break;
					}
				}
				
				attributeType = SharpAssemblyClass.FromTypeDef(assembly, tdIndex);
			}
			if (attributeType != null) Name = attributeType.FullyQualifiedName;
			
			if (attribute.ValueIndex == 0) return;
			
			try {
			
				// Get return types of positional arguments by inspecting the method signature
				int size = assembly.Reader.LoadBlob(ref sigOffset);
				sigOffset += 1;  // skip calling convention
				int numReturnTypes = assembly.Reader.LoadBlob(ref sigOffset);
						
				SharpAssemblyReturnType dummy = SharpAssemblyReturnType.Create(assembly, ref sigOffset);
				
				SharpAssemblyReturnType[] returnTypes = new SharpAssemblyReturnType[numReturnTypes];
				for (int i = 0; i < returnTypes.Length; ++i) {
					returnTypes[i] = SharpAssemblyReturnType.Create(assembly, ref sigOffset);
				}
				
				// Get the return type values and the named arguments
				byte[] argBlob = assembly.Reader.GetBlobFromHeap(attribute.ValueIndex);
				Stream str = new MemoryStream(argBlob);
				BinaryReader binaryReader = new BinaryReader(str);
				
				ushort prolog = binaryReader.ReadUInt16();
				if (prolog != 1) {
					Console.WriteLine("SharpAssemblyAttribute: Wrong prolog in argument list");
					return;
				}
				
				// read positional arguments
				for (int i = 0; i < returnTypes.Length; ++i) {
					string rettypename = returnTypes[i].Name;
					
					SharpAssemblyClass underlyingClass = returnTypes[i].UnderlyingClass;
					
					// enum -> determine base integer size and try to display the user-friendly name of the value					
					if (underlyingClass != null && underlyingClass.IsSubclassOf("System.Enum")) {
						//underlyingClass.LoadMembers();
						foreach (IField field in underlyingClass.Fields) {
							if (field.Name.EndsWith("value__")) {
								rettypename = field.ReturnType.Name;
								break;
							}
						}
						
						object argValue = GetFixedArg(argBlob, binaryReader, rettypename);
						
						foreach (IField field in underlyingClass.Fields) {
							if (field is SharpAssemblyField) {
								try {
									if (((field as SharpAssemblyField).InitialValue as IComparable).CompareTo(argValue) == 0) {
										// TODO PositionArguments
										//PositionalArguments.Add(underlyingClass.Name + "." + field.Name);
										goto namefound;
									}
								} catch {}
							}
						}
						// if the value cannot be found
						// TODO PositionArguments
						//PositionalArguments.Add(argValue.ToString());

					namefound: ;
						
					} else {
						// TODO PositionArguments
						//PositionalArguments.Add(GetFixedArg(argBlob, binaryReader, rettypename).ToString());
					}
				}
				
				ushort numnamed = binaryReader.ReadUInt16();
				
				for (int i = 0; i < numnamed; ++i) {
					byte field_or_prop = binaryReader.ReadByte();
					byte type = binaryReader.ReadByte();
					
					string typename = "";
					if (type == 0x50) {
						typename = "Type";
					} else {
						DataType dt = (DataType)type;
						typename = dt.ToString();
					}
					
					string argname = GetSerString(argBlob, binaryReader.BaseStream);
					
					// TODO NamedArgs
					//NamedArguments.Add(argname, GetFixedArg(argBlob, binaryReader, typename).ToString());
				}
				
				binaryReader.Close();
			} catch (Exception ex) {
				LoggingService.Error("SharpAssemblyAttribute: Error loading arguments. " + ex.ToString());
			}
		}
		
		object GetFixedArg(byte[] argBlob, BinaryReader binaryReader, string name)
		{
			switch (name) {
				case "Boolean":
					return binaryReader.ReadBoolean();
				case "Char":
					return binaryReader.ReadChar();
				case "SByte":
					return binaryReader.ReadSByte();
				case "Byte":
					return binaryReader.ReadByte();
				case "Int16":
					return binaryReader.ReadInt16();
				case "UInt16":
					return binaryReader.ReadUInt16();
				case "Int32":
					return binaryReader.ReadInt32();
				case "UInt32":
					return binaryReader.ReadUInt32();
				case "Int64":
					return binaryReader.ReadInt64();
				case "UInt64":
					return binaryReader.ReadUInt64();
				case "Single":
					return binaryReader.ReadSingle();
				case "Double":
					return binaryReader.ReadDouble();
				case "String":
				case "Type":
					return '"' + GetSerString(argBlob, binaryReader.BaseStream) + '"';
				default:
					return "";
			}
		}
		
		string GetSerString(byte[] blob, Stream stream)
		{
			uint pos2 = (uint)stream.Position;
			int size = SA.AssemblyReader.GetCompressedInt(blob, ref pos2);
			
			string str;
			try {
				str = System.Text.Encoding.UTF8.GetString(blob, (int)pos2, size);
			} catch {
				str = "<error with string>";
			}
			
			stream.Position = pos2 + size;
			
			return str;
		}
		
		public override string ToString()
		{	
			string ret = Name + " (";
			
			foreach (AttributeArgument arg in PositionalArguments) {
				ret += arg.Type.FullyQualifiedName + ", ";
			}
			
			// TODO NamedArguments
			
			foreach (KeyValuePair<string, AttributeArgument> item in NamedArguments) {
				try {
					ret += item.Key + " = " + item.Value.Type.FullyQualifiedName + ", ";
				} catch {
					LoggingService.Error("Error in NamedArguments.");
				}
			}

			
			// delete last bracket
			if (ret.EndsWith(", ")) ret = ret.Substring(0, ret.Length - 2);
			
			return ret + ")";
		}
		
		public static List<IAttribute> GetAssemblyAttributes(SA.SharpAssembly assembly)
		{
			List<IAttribute> attributes = new List<IAttribute>();
			
			foreach (ArrayList al in assembly.Attributes.Assembly.Values) {
				foreach (SA.SharpCustomAttribute attr in al) {
					attributes.Add(new SharpAssemblyAttribute(assembly, attr));
				}
			}
			
			return attributes;
		}
	}
}

