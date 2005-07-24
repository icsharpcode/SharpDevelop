// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
		bool isGlobal = false;
		
		static Dictionary<string, string> types   = new Dictionary<string, string>();
		static Dictionary<string, string> vbtypes = new Dictionary<string, string>();
		
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
			vbtypes.Add("sbyte",   "System.SByte");
			vbtypes.Add("date",	   "System.DateTime");
			vbtypes.Add("char",    "System.Char");
			vbtypes.Add("decimal", "System.Decimal");
			vbtypes.Add("double",  "System.Double");
			vbtypes.Add("single",  "System.Single");
			vbtypes.Add("integer", "System.Int32");
			vbtypes.Add("long",    "System.Int64");
			vbtypes.Add("uinteger","System.UInt32");
			vbtypes.Add("ulong",   "System.UInt64");
			vbtypes.Add("object",  "System.Object");
			vbtypes.Add("short",   "System.Int16");
			vbtypes.Add("ushort",  "System.UInt16");
			vbtypes.Add("string",  "System.String");
		}
		
		public static IEnumerable<KeyValuePair<string, string>> GetPrimitiveTypesCSharp()
		{
			return types;
		}
		
		public static IEnumerable<KeyValuePair<string, string>> GetPrimitiveTypesVB()
		{
			return vbtypes;
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
		
		/// <summary>
		/// Gets/Sets if the type reference had a "global::" prefix.
		/// </summary>
		public bool IsGlobal {
			get {
				return isGlobal;
			}
			set {
				isGlobal = value;
			}
		}
		
		public static TypeReference CheckNull(TypeReference typeReference)
		{
			return typeReference == null ? NullTypeReference.Instance : typeReference;
		}
		
		string GetSystemType(string type)
		{
			if (types.ContainsKey(type)) {
				return types[type];
			}
			string lowerType = type.ToLower();
			if (vbtypes.ContainsKey(lowerType)) {
				return vbtypes[lowerType];
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
		
		public TypeReference(string type, List<TypeReference> genericTypes) : this(type)
		{
			if (genericTypes != null) {
				this.genericTypes = genericTypes;
			}
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
