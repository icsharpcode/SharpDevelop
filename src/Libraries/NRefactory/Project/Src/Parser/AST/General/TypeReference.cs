// TypeReference.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class TypeReference : AbstractNode, INullable
	{
		string type = "";
		string systemType = "";
		int    pointerNestingLevel = 0;
		int[]  rankSpecifier = null;
		List<TypeReference> genericTypes = new List<TypeReference>(1);
		
		static Hashtable types = new Hashtable();
		static Hashtable vbtypes = new Hashtable();
		
		static TypeReference()
		{
			// C# types
			types.Add("bool",    "System.Boolean");
			types.Add("byte",    "System.Byte");
			types.Add("char",    "System.Char");
			types.Add("decimal", "System.Decimal");
			types.Add("double",  "System.Double");
			types.Add("float",   "System.Single");
			types.Add("int",     "System.Int32");
			types.Add("long",    "System.Int64");
			types.Add("object",  "System.Object");
			types.Add("sbyte",   "System.SByte");
			types.Add("short",   "System.Int16");
			types.Add("string",  "System.String");
			types.Add("uint",    "System.UInt32");
			types.Add("ulong",   "System.UInt64");
			types.Add("ushort",  "System.UInt16");
			types.Add("void",    "System.Void");
			
			// VB.NET types
			vbtypes.Add("boolean", "System.Boolean");
			vbtypes.Add("byte",    "System.Byte");
			vbtypes.Add("date",	   "System.DateTime");
			vbtypes.Add("char",    "System.Char");
			vbtypes.Add("decimal", "System.Decimal");
			vbtypes.Add("double",  "System.Double");
			vbtypes.Add("single",  "System.Single");
			vbtypes.Add("integer", "System.Int32");
			vbtypes.Add("long",    "System.Int64");
			vbtypes.Add("object",  "System.Object");
			vbtypes.Add("short",   "System.Int16");
			vbtypes.Add("string",  "System.String");
		}
		
		public static ICollection GetPrimitiveTypes()
		{
			return types.Keys;
		}
		
		public static ICollection GetPrimitiveTypesVB()
		{
			return vbtypes.Keys;
		}
		
		public string Type {
			get {
				return type;
			}
			set {
				Debug.Assert(value != null);
				type = value;
			}
		}
		
		public string SystemType {
			get {
				return systemType;
			}
		}
		
		public int PointerNestingLevel {
			get {
				return pointerNestingLevel;
			}
			set {
				pointerNestingLevel = value;
			}
		}
		
		public int[] RankSpecifier {
			get {
				return rankSpecifier;
			}
			set {
				rankSpecifier = value;
			}
		}
		
		public List<TypeReference> GenericTypes {
			get {
				return genericTypes;
			}
		}
		
		public bool IsArrayType {
			get {
				return rankSpecifier != null && rankSpecifier.Length > 0;
			}
		}
		
		public static NullTypeReference Null {
			get {
				return NullTypeReference.Instance;
			}
		}
		
		public virtual bool IsNull {
			get {
				return false;
			}
		}
		
		public static TypeReference CheckNull(TypeReference typeReference)
		{
			return typeReference == null ? NullTypeReference.Instance : typeReference;
		}
		
		string GetSystemType(string type)
		{
			if (types.Contains(type)) {
				return (string)types[type];
			}
			string lowerType = type.ToLower();
			if (vbtypes.Contains(lowerType)) {
				return (string)vbtypes[lowerType];
			}
			return type;
		}
		
		public TypeReference(string type)
		{
			Debug.Assert(type != null);
			this.systemType = GetSystemType(type);
			this.type = type;
		}
		
		public TypeReference(string type, string systemType)
		{
			this.type       = type;
			this.systemType = systemType;
		}
		
		public TypeReference(string type, int[] rankSpecifier) : this(type, 0, rankSpecifier)
		{
		}
		
		public TypeReference(string type, int pointerNestingLevel, int[] rankSpecifier) : this(type, pointerNestingLevel, rankSpecifier, null)
		{
		}
		
		public TypeReference(string type, int pointerNestingLevel, int[] rankSpecifier, List<TypeReference> genericTypes)
		{
			Debug.Assert(type != null);
			this.type = type;
			this.systemType = GetSystemType(type);
			this.pointerNestingLevel = pointerNestingLevel;
			this.rankSpecifier = rankSpecifier;
			if (genericTypes != null) {
				this.genericTypes = genericTypes;
			}
		}
		
		protected TypeReference()
		{}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[TypeReference: Type={0}, PointerNestingLevel={1}, RankSpecifier={2}]", type, pointerNestingLevel, rankSpecifier);
		}
	}
	
	public class NullTypeReference : TypeReference
	{
		static NullTypeReference nullTypeReference = new NullTypeReference();
		public override bool IsNull {
			get {
				return true;
			}
		}
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return data;
		}
		public static NullTypeReference Instance {
			get {
				return nullTypeReference;
			}
		}
		NullTypeReference() {}
		public override string ToString()
		{
			return String.Format("[NullTypeReference]");
		}
	}
}
