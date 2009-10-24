// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;
using ICSharpCode.NRefactory.Ast;
using Mono.Cecil.Signatures;

namespace Debugger.MetaData
{
	public enum DebugTypeKind { Array, Class, ValueType, Primitive, Pointer, Void };
	
	/// <summary>
	/// Represents a type in a debugee. That is, a class, array, value type or a primitive type.
	/// This class mimics the <see cref="System.Type"/> class.
	/// </summary>
	/// <remarks>
	/// If two types are identical, the references to DebugType will also be identical 
	/// Type will be loaded once per each appdomain.
	/// </remarks>
	public partial class DebugType: System.Type
	{
		AppDomain appDomain;
		Process   process;
		ICorDebugType corType;
		CorElementType corElementType;
		string name;
		string fullName;
		
		// Class/ValueType specific
		Module module;
		TypeDefProps classProps;
		
		// Class/ValueType/Array/Ref/Ptr specific
		List<DebugType>    typeArguments = new List<DebugType>();
		List<DebugType>    interfaces = new List<DebugType>();
		
		// Members of the type; empty list if not applicable
		List<MemberInfo>   members = new List<MemberInfo>();
		
		// Stores all DebugType instances. FullName is the key
		static Dictionary<ICorDebugType, DebugType> loadedTypes = new Dictionary<ICorDebugType, DebugType>();
		
		public override Type DeclaringType {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override int MetadataToken {
			get {
				AssertClassOrValueType();
				return classProps.Token;
			}
		}
		
		//		public virtual Module Module { get; }
		
		public override string Name {
			get {
				return name;
			}
		}
		
		public override Type ReflectedType {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}
		
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}
		
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}
		
		
		public override Assembly Assembly {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override string AssemblyQualifiedName {
			get {
				throw new NotSupportedException();
			}
		}
		
		public override Type BaseType {
			get {
				// corType.Base *sometimes* does not work for object and can cause "Value does not fall within the expected range." exception
				if (this.FullName == "System.Object") {
					return null;
				}
				// corType.Base does not work for arrays
				if (this.IsArray) {
					return DebugType.CreateFromType(this.AppDomain, typeof(System.Array));
				}
				// corType.Base does not work for primitive types
				if (this.IsPrimitive) {
					return DebugType.CreateFromType(this.AppDomain, typeof(object));
				}
				if (this.IsPointer || this.IsVoid) {
					return null;
				}
				ICorDebugType baseType = corType.Base;
				if (baseType != null) {
					return CreateFromCorType(this.AppDomain, baseType);
				} else {
					return null;
				}
			}
		}
		
		//		public virtual bool ContainsGenericParameters { get; }
		//		public virtual MethodBase DeclaringMethod { get; }
		
		public override string FullName {
			get {
				return fullName;
			}
		}
		
		//		public virtual GenericParameterAttributes GenericParameterAttributes { get; }
		//		public virtual int GenericParameterPosition { get; }
		
		public override Guid GUID {
			get {
				throw new NotSupportedException();
			}
		}
		
		//		public virtual bool IsGenericParameter { get; }
		//		public virtual bool IsGenericType { get; }
		//		public virtual bool IsGenericTypeDefinition { get; }
		//		internal virtual bool IsSzArray { get; }
		//		public override MemberTypes MemberType { get; }
		
		public override Module Module {
			get {
				AssertClassOrValueType();
				return module;
			}
		}
		
		public override string Namespace {
			get {
				throw new NotSupportedException();
			}
		}
		
		//		public override Type ReflectedType { get; }
		//		public virtual StructLayoutAttribute StructLayoutAttribute { get; }
		//		public virtual RuntimeTypeHandle TypeHandle { get; }
		
		public override Type UnderlyingSystemType {
			get {
				throw new NotSupportedException();
			}
		}
		
		//		public virtual Type[] FindInterfaces(TypeFilter filter, object filterCriteria);
		//		public virtual MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria);
		
		public override int GetArrayRank()
		{
			if (!IsArray) throw new ArgumentException("Type is not array");
			
			return (int)corType.Rank;
		}
		
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return (TypeAttributes)classProps.Flags;
		}
		
		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotSupportedException();
		}
		
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		//		internal virtual string GetDefaultMemberName();
		//		public virtual MemberInfo[] GetDefaultMembers();
		
		public override Type GetElementType()
		{
			return this.GenericArguments[0];
		}
		
		const BindingFlags supportedFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy;
		
		public T GetMember<T>(string name, BindingFlags bindingFlags, Predicate<T> filter) where T:MemberInfo
		{
			T[] res = GetMembersImpl<T>(name, bindingFlags, filter);
			if (res.Length > 0) {
				return res[0];
			} else {
				return null;
			}
		}
		
		public T[] GetMembers<T>(string name, BindingFlags bindingFlags, Predicate<T> filter) where T:MemberInfo
		{
			BindingFlags unsupported = bindingFlags & ~supportedFlags;
			if (unsupported != 0)
				throw new NotSupportedException("BindingFlags: " + unsupported);
			
			if ((bindingFlags & (BindingFlags.Public | BindingFlags.NonPublic)) == 0)
				throw new ArgumentException("Public or NonPublic flag must be included", "bindingFlags");
			
			if ((bindingFlags & (BindingFlags.Instance | BindingFlags.Static)) == 0)
				throw new ArgumentException("Instance or Static flag must be included", "bindingFlags");
			
			List<T> results = new List<T>();
			foreach(MemberInfo memberInfo in members) {
				// Filter by type
				if (!(memberInfo is T)) continue; // Reject item
				
				// Filter by name
				if (name != null) {
					if (memberInfo.Name != name) continue; // Reject item
				}
				
				// Filter by access
				bool memberIsPublic;
				if (memberInfo is FieldInfo) {
					memberIsPublic = ((FieldInfo)memberInfo).IsPublic;
				} else if (memberInfo is DebugPropertyInfo) {
					memberIsPublic = ((DebugPropertyInfo)memberInfo).IsPublic;
				} else if (memberInfo is MethodInfo) {
					memberIsPublic = ((MethodInfo)memberInfo).IsPublic;
				} else {
					throw new DebuggerException("Unexpected type: " + memberInfo.GetType());
				}
				if (memberIsPublic) {
					if ((bindingFlags & BindingFlags.Public) == 0) continue; // Reject item
				} else {
					if ((bindingFlags & BindingFlags.NonPublic) == 0) continue; // Reject item
				}
				
				// Filter by static / instance
				bool memberIsStatic;
				if (memberInfo is FieldInfo) {
					memberIsStatic = ((FieldInfo)memberInfo).IsStatic;
				} else if (memberInfo is DebugPropertyInfo) {
					memberIsStatic = ((DebugPropertyInfo)memberInfo).IsStatic;
				} else if (memberInfo is MethodInfo) {
					memberIsStatic = ((MethodInfo)memberInfo).IsStatic;
				} else {
					throw new DebuggerException("Unexpected type: " + memberInfo.GetType());
				}
				if (memberIsStatic) {
					if ((bindingFlags & BindingFlags.Static) == 0) continue; // Reject item
				} else {
					if ((bindingFlags & BindingFlags.Instance) == 0) continue; // Reject item
				}
				
				// Filter using predicate
				if (filter != null && !filter((T)memberInfo)) continue; // Reject item
				
				results.Add((T)memberInfo);
			}
			
			// Query supertype
			if ((bindingFlags & BindingFlags.DeclaredOnly) == 0 && this.BaseType != null) {
				if ((bindingFlags & BindingFlags.FlattenHierarchy) == 0) {
					// Do not include static types
					bindingFlags = bindingFlags & ~BindingFlags.Static;
				}
				List<T> superResults = this.BaseType.QueryMembers<T>(bindingFlags, name, token);
				results.AddRange(superResults);
			}
			
			return results.ToArray();
		}
		
		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		//		public virtual EventInfo[] GetEvents();
		
		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			return GetMember<FieldInfo>(name, bindingAttr, null);
		}
		
		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			return GetMembers<FieldInfo>(null, bindingAttr, null);
		}
		
		//		public virtual Type[] GetGenericArguments();
		//		public virtual Type[] GetGenericParameterConstraints();
		//		public virtual Type GetGenericTypeDefinition();
		
		public override Type GetInterface(string name, bool ignoreCase)
		{
			throw new NotSupportedException();
		}
		
		//		public virtual InterfaceMapping GetInterfaceMap(Type interfaceType);
		
		public override Type[] GetInterfaces()
		{
			return this.interfaces.ToArray();
		}
		
		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			return GetMembers<MemberInfo>(name, bindingAttr, delegate(MemberInfo info) { return (info.MemberType & type) != 0; });
		}
		
		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return GetMembers<MemberInfo>(null, bindingAttr, null);
		}
		
		/// <summary> Return first method with the given token</summary>
		public MethodInfo GetMethod(uint token)
		{
			return QueryMember<MethodInfo>(token);
		}
		
		/// <summary> Return method overload with given parameter names </summary>
		/// <returns> Null if not found </returns>
		public MethodInfo GetMethod(string name, string[] paramNames)
		{
			foreach(MethodInfo candidate in GetMethod(name)) {
				if (candidate.ParameterCount == paramNames.Length) {
					bool match = true;
					for(int i = 0; i < paramNames.Length; i++) {
						if (paramNames[i] != candidate.ParameterNames[i])
							match = false;
					}
					if (match)
						return candidate;
				}
			}
			return null;
		}
		
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] paramTypes, ParameterModifier[] modifiers)
		{
			// TODO: Finish
			foreach(MethodInfo candidate in GetMembers<MethodInfo>(name, bindingAttr, null))) {
				if (paramTypes == null)
					return candidate;
				if (candidate.ParameterCount == paramTypes.Length) {
					bool match = true;
					for(int i = 0; i < paramTypes.Length; i++) {
						if (paramTypes[i] != candidate.ParameterTypes[i])
							match = false;
					}
					if (match)
						return candidate;
				}
			}
			return null;
		}
		
		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			return GetMembers<MethodInfo>(null, bindingAttr, null);
		}
		
		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		public MemberInfo[] GetFieldsAndNonIndexedProperties(BindingFlags bindingAttr)
		{
			return GetMembers<MemberInfo>(null, bindingAttr, delegate (MemberInfo info) {
			                              	if (info is FieldInfo)
			                              		return true;
			                              	if (info is PropertyInfo) {
			                              		return ((PropertyInfo)info).GetGetMethod(true) != null &&
			                              		       ((PropertyInfo)info).GetGetMethod(true).GetParameters().Length == 0;
			                              	}
			                              	return false;
			                              });
		}
		
		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			return GetMembers<PropertyInfo>(null, bindingAttr, null);
		}
		
		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			// TODO: Finsih
			return GetMember<PropertyInfo>(name, bindingAttr, null);
		}
		
		//		internal virtual Type GetRootElementType();
		//		internal virtual TypeCode GetTypeCodeInternal();
		//		internal virtual RuntimeTypeHandle GetTypeHandleInternal();
		
		protected override bool HasElementTypeImpl()
		{
			throw new NotSupportedException();
		}
		
		//		internal virtual bool HasProxyAttributeImpl();
		
		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}
		
		protected override bool IsArrayImpl()
		{
			return this.Kind == DebugTypeKind.Array;
		}
		
		//		public virtual bool IsAssignableFrom(Type c);
		
		protected override bool IsByRefImpl()
		{
			throw new NotSupportedException();
		}
		
		protected override bool IsCOMObjectImpl()
		{
			throw new NotSupportedException();
		}
		
		//		protected virtual bool IsContextfulImpl();
		//		public virtual bool IsInstanceOfType(object o);
		//		protected virtual bool IsMarshalByRefImpl();
		
		protected override bool IsPointerImpl()
		{
			return this.Kind == DebugTypeKind.Pointer;
		}
		
		protected override bool IsPrimitiveImpl()
		{
			return this.PrimitiveType != null;
		}
		
		public override bool IsSubclassOf(Type superType)
		{
			return base.IsSubclassOf(superType);
		}
		
		protected virtual bool IsValueTypeImpl()
		{
			return this.Kind == DebugTypeKind.ValueType;
		}
		
		void AssertClassOrValueType()
		{
			if(!IsClass && !IsValueType) {
				throw new DebuggerException("The type is not a class or value type.");
			}
		}
		
		/// <summary> Gets the appdomain in which the type was loaded </summary>
		[Debugger.Tests.Ignore]
		public AppDomain AppDomain {
			get { return appDomain; }
		}
		
		[Debugger.Tests.Ignore]
		public Process Process {
			get { return process; }
		}
		
		internal ICorDebugType CorType {
			get { return corType; }
		}
		
		/// <summary> Gets a list of all interfaces that this type implements </summary>
		public List<DebugType> Interfaces {
			get { return interfaces; }
		}
		
		/// <summary> Return an interface with the given name </summary>
		/// <returns> Null if not found </returns>
		public DebugType GetInterface(string fullName)
		{
			foreach(DebugType inter in this.Interfaces) {
				if (inter.FullName == fullName) {
					return inter;
				}
			}
			if (BaseType != null) {
				return BaseType.GetInterface(fullName);
			} else {
				return null;
			}
		}
		
		/// <summary> Get an element type for array or pointer. </summary>
		public DebugType ElementType {
			get {
				if (this.IsArray || this.IsPointer) {
					return typeArguments[0];
				} else {
					return null;
				}
			}
		}
		
		/// <summary> Gets generics arguments for a type or an empty List for non-generic types. </summary>
		public List<DebugType> GenericArguments {
			get {
				if (this.IsArray || this.IsPointer) {
					return new List<DebugType>();
				} else {
					return typeArguments;
				}
			}
		}
		
		internal ICorDebugType[] GenericArgumentsAsCorDebugType {
			get {
				List<ICorDebugType> types = new List<ICorDebugType>();
				foreach(DebugType arg in this.GenericArguments) {
					types.Add(arg.CorType);
				}
				return types.ToArray();
			}
		}
		
		/// <summary> Returns what kind of type this is. (eg. value type) </summary>
		public DebugTypeKind Kind {
			get {
				switch (this.corElementType) {
					case CorElementType.BOOLEAN:
					case CorElementType.CHAR:
					case CorElementType.I1:
					case CorElementType.U1:
					case CorElementType.I2:
					case CorElementType.U2:
					case CorElementType.I4:
					case CorElementType.U4:
					case CorElementType.I8:
					case CorElementType.U8:
					case CorElementType.R4:
					case CorElementType.R8:
					case CorElementType.I:
					case CorElementType.U:
					case CorElementType.STRING:    return DebugTypeKind.Primitive;
					case CorElementType.ARRAY:
					case CorElementType.SZARRAY:   return DebugTypeKind.Array;
					case CorElementType.CLASS:
					case CorElementType.OBJECT:    return DebugTypeKind.Class;
					case CorElementType.VALUETYPE: return DebugTypeKind.ValueType;
					case CorElementType.PTR:
					case CorElementType.BYREF:     return DebugTypeKind.Pointer;
					case CorElementType.VOID:      return DebugTypeKind.Void;
					default: throw new DebuggerException("Unknown kind of type");
				}
			}
		}
		
		/// <summary> Gets a value indicating whether the type is an integer type </summary>
		[Tests.Ignore]
		public bool IsInteger {
			get {
				if (this.PrimitiveType == null) {
					return false;
				}
				switch (this.PrimitiveType.FullName) {
					case "System.SByte":
					case "System.Byte":
					case "System.Int16":
					case "System.UInt16":
					case "System.Int32":
					case "System.UInt32":
					case "System.Int64":
					case "System.UInt64": return true;
					default: return false;
				}
			}
		}
		
		/// <summary> Gets a value indicating whether the type is an string </summary>
		[Tests.Ignore]
		public bool IsString {
			get {
				return this.corElementType == CorElementType.STRING;
			}
		}
		
		/// <summary> Gets a value indicating whether the type is the void type </summary>
		[Tests.Ignore]
		public bool IsVoid {
			get {
				return this.Kind == DebugTypeKind.Void;
			}
		}
		
		public static DebugType CreateFromTypeDefOrRef(Module module, bool? valueType, uint token, DebugType[] genericArguments)
		{
			CorTokenType tkType = (CorTokenType)(token & 0xFF000000);
			if (tkType == CorTokenType.TypeDef) {
				ICorDebugClass corClass = module.CorModule.GetClassFromToken(token);
				return CreateFromCorClass(module.AppDomain, valueType, corClass, genericArguments);
			} else if (tkType == CorTokenType.TypeRef) {
				TypeRefProps refProps = module.MetaData.GetTypeRefProps(token);
				string fullName = refProps.Name;
				CorTokenType scopeType = (CorTokenType)(refProps.ResolutionScope & 0xFF000000);
				DebugType enclosingType = null;
				if (scopeType == CorTokenType.TypeDef || scopeType == CorTokenType.TypeRef) {
					// Resolve the enclosing TypeRef in this scope
					enclosingType = CreateFromTypeDefOrRef(module, null, refProps.ResolutionScope, genericArguments);
				}
				return CreateFromName(module.AppDomain, fullName, enclosingType, genericArguments);
			} else {
				throw new DebuggerException("TypeDef or TypeRef expected.  Seen " + tkType);
			}
		}
		
		public static DebugType CreateFromType(AppDomain appDomain, System.Type type, params DebugType[] genericArguments)
		{
			return CreateFromName(appDomain, type.FullName, genericArguments);
		}
		
		public static DebugType CreateFromName(AppDomain appDomain, string typeName, params DebugType[] genericArguments)
		{
			return CreateFromName(appDomain, typeName, null, genericArguments);
		}
		
		public static DebugType CreateFromName(AppDomain appDomain, string typeName, DebugType enclosingType, params DebugType[] genericArguments)
		{
			if (genericArguments != null) {
				if (enclosingType != null)
					typeName = GetQualifiedName(typeName, genericArguments.Length - enclosingType.GenericArguments.Count);
				else
					typeName = GetQualifiedName(typeName, genericArguments.Length);
			}
			foreach(Module module in appDomain.Process.Modules) {
				if (module.AppDomain == appDomain) {
					uint token;
					try {
						token = module.MetaData.FindTypeDefPropsByName(typeName, enclosingType == null ? 0 : enclosingType.Token).Token;
					} catch {
						continue;
					}
					return CreateFromTypeDefOrRef(module, null, token, genericArguments);
				}
			}
			throw new DebuggerException("Can not find type " + typeName);
		}
		
		/// <summary> Converts type name to the form suitable for COM API. </summary>
		static string GetQualifiedName(string typeName, int genericArgumentsCount)
		{
			if (genericArgumentsCount > 0 && !typeName.Contains("`")) {
				return typeName + "`" + genericArgumentsCount;
			} else 	{
				return typeName;
			}
		}
		
		public static DebugType CreateFromTypeSpec(Module module, uint token, DebugType declaringType)
		{
			CorTokenType tokenType = (CorTokenType)(token & 0xFF000000);
			if (tokenType != CorTokenType.TypeSpec) {
				throw new DebuggerException("TypeSpec expected.  Seen " + tokenType);
			}
			
			byte[] typeSpecBlob = module.MetaData.GetTypeSpecFromToken(token).GetData();
			return CreateFromSignature(module, typeSpecBlob, declaringType);
		}
		
		public static DebugType CreateFromTypeReference(AppDomain appDomain, TypeReference typeRef)
		{
			List<DebugType> genArgs = new List<DebugType>();
			foreach(TypeReference argRef in typeRef.GenericTypes) {
				genArgs.Add(CreateFromTypeReference(appDomain, argRef));
			}
			DebugType type = CreateFromName(appDomain, typeRef.Type, genArgs.ToArray());
			for(int i = 0; i < typeRef.PointerNestingLevel; i++) {
				type = MakePointerType(type);
			}
			for(int i = typeRef.RankSpecifier.Length - 1; i >= 0; i--) {
				type = MakeArrayType(type, typeRef.RankSpecifier[i] + 1);
			}
			return type;
		}
		
		public static DebugType CreateFromSignature(Module module, byte[] signature, DebugType declaringType)
		{
			SignatureReader sigReader = new SignatureReader(signature);
			int start;
			SigType sigType = sigReader.ReadType(signature, 0, out start);
			return CreateFromSignature(module, sigType, declaringType);
		}
		
		internal static DebugType CreateFromSignature(Module module, SigType sigType, DebugType declaringType)
		{
			System.Type sysType = CorElementTypeToManagedType((CorElementType)(uint)sigType.ElementType);
			if (sysType != null) {
				return CreateFromType(module.AppDomain, sysType);
			}
			
			if (sigType.ElementType == Mono.Cecil.Metadata.ElementType.Object) {
				return CreateFromType(module.AppDomain, typeof(object));
			}
			
			if (sigType is CLASS) {
				return CreateFromTypeDefOrRef(module, false, ((CLASS)sigType).Type.ToUInt(), null);
			}
			
			if (sigType is VALUETYPE) {
				return CreateFromTypeDefOrRef(module, true, ((VALUETYPE)sigType).Type.ToUInt(), null);
			}
			
			// Numbered generic reference
			if (sigType is VAR) {
				if (declaringType == null) throw new DebuggerException("declaringType is needed");
				return declaringType.GenericArguments[((VAR)sigType).Index];
			}
			
			// Numbered generic reference
			if (sigType is MVAR) {
				return CreateFromType(module.AppDomain, typeof(object));
			}
			
			if (sigType is GENERICINST) {
				GENERICINST genInst = (GENERICINST)sigType;
				
				List<DebugType> genArgs = new List<DebugType>(genInst.Signature.Arity);
				foreach(GenericArg genArgSig in genInst.Signature.Types) {
					genArgs.Add(CreateFromSignature(module, genArgSig.Type, declaringType));
				}
				
				return CreateFromTypeDefOrRef(module, genInst.ValueType, genInst.Type.ToUInt(), genArgs.ToArray());
			}
			
			if (sigType is ARRAY) {
				ARRAY arraySig = (ARRAY)sigType;
				DebugType elementType = CreateFromSignature(module, arraySig.Type, declaringType);
				return MakeArrayType(elementType, arraySig.Shape.Rank);
			}
			
			if (sigType is SZARRAY) {
				SZARRAY arraySig = (SZARRAY)sigType;
				DebugType elementType = CreateFromSignature(module, arraySig.Type, declaringType);
				return MakeArrayType(elementType);
			}
			
			if (sigType is PTR) {
				PTR ptrSig = (PTR)sigType;
				DebugType elementType;
				if (ptrSig.Void) {
					elementType = CreateFromType(module.AppDomain, typeof(void));
				} else {
					elementType = CreateFromSignature(module, ptrSig.PtrType, declaringType);
				}
				return MakePointerType(elementType);
			}
			
			if (sigType is FNPTR) {
				// TODO: FNPTR
			}
			
			throw new NotImplementedException(sigType.ElementType.ToString());
		}
		
		// public virtual Type MakeGenericType(params Type[] typeArguments);
		
		public override Type MakeArrayType(int rank)
		{
			ICorDebugType res = elementType.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.ARRAY, (uint)rank, elementType.CorType);
			return CreateFromCorType(elementType.AppDomain, res);
		}
		
		public override Type MakeArrayType()
		{
			ICorDebugType res = elementType.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.SZARRAY, 1, elementType.CorType);
			return CreateFromCorType(elementType.AppDomain, res);
		}
		
		public override Type MakePointerType()
		{
			ICorDebugType res = elementType.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.PTR, 0, elementType.CorType);
			return CreateFromCorType(elementType.AppDomain, res);
		}
		
		public override Type MakeByRefType()
		{
			ICorDebugType res = elementType.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.BYREF, 0, elementType.CorType);
			return CreateFromCorType(elementType.AppDomain, res);
		}
		
		public static DebugType CreateFromCorClass(AppDomain appDomain, bool? valueType, ICorDebugClass corClass, DebugType[] genericArguments)
		{
			MetaDataImport metaData = appDomain.Process.Modules[corClass.Module].MetaData;
			
			if (valueType == null) {
				uint superClassToken = metaData.GetTypeDefProps(corClass.Token).SuperClassToken;
				CorTokenType tkType = (CorTokenType)(superClassToken & 0xFF000000);
				if (tkType == CorTokenType.TypeDef) {
					valueType = metaData.GetTypeDefProps(superClassToken).Name == typeof(ValueType).FullName;
				}
				if (tkType == CorTokenType.TypeRef) {
					valueType = metaData.GetTypeRefProps(superClassToken).Name == typeof(ValueType).FullName;
				}
				if (tkType == CorTokenType.TypeSpec) {
					valueType = false; // TODO: Resolve properly
				}
			}
			
			genericArguments = genericArguments ?? new DebugType[] {};
			if (genericArguments.Length < metaData.GetGenericParamCount(corClass.Token)) {
				throw new DebuggerException("Not enough generic arguments");
			}
			
			List<ICorDebugType> corGenArgs = new List<ICorDebugType>(genericArguments.Length);
			foreach(DebugType genAgr in genericArguments) {
				corGenArgs.Add(genAgr.CorType);
			}
			
			ICorDebugType corType = corClass.CastTo<ICorDebugClass2>().GetParameterizedType((uint)(valueType.Value ? CorElementType.VALUETYPE : CorElementType.CLASS), corGenArgs.ToArray());
			
			return CreateFromCorType(appDomain, corType);
		}
		
		/// <summary> Obtains instance of DebugType. Same types will return identical instance. </summary>
		public static DebugType CreateFromCorType(AppDomain appDomain, ICorDebugType corType)
		{
			if (loadedTypes.ContainsKey(corType)) return loadedTypes[corType];
			
			return new DebugType(appDomain, corType);
		}
		
		DebugType(AppDomain appDomain, ICorDebugType corType)
		{
			if (corType == null) throw new ArgumentNullException("corType");
			DateTime startTime = Util.HighPrecisionTimer.Now;
			
			this.appDomain = appDomain;
			this.process = appDomain.Process;
			this.corType = corType;
			this.corElementType = (CorElementType)corType.Type;
			
			// Loading might access the type again
			loadedTypes[corType] = this;
			
			// We need to know generic arguments before getting name
			if (this.IsClass || this.IsValueType || this.IsArray || this.IsPointer) {
				foreach(ICorDebugType t in corType.EnumerateTypeParameters().Enumerator) {
					typeArguments.Add(DebugType.CreateFromCorType(appDomain, t));
				}
			}
			
			// We need class props for getting name
			if (this.IsClass || this.IsValueType) {
				this.module = process.Modules[corType.Class.Module];
				this.classProps = module.MetaData.GetTypeDefProps(corType.Class.Token);
			}
			
			this.fullName = GetName(true);
			this.name = GetName(false);
			
			if (this.IsClass || this.IsValueType) {
				LoadMemberInfo();
			}
			
			this.Process.Exited += delegate { loadedTypes.Remove(corType); };
			
			TimeSpan totalTime2 = Util.HighPrecisionTimer.Now - startTime;
			if (appDomain.Process.Options.Verbose) {
				string prefix = this.IsInterface ? "interface" : "type";
				appDomain.Process.TraceMessage("Loaded {0} {1} ({2} ms)", prefix, this.FullName, totalTime2.TotalMilliseconds);
				foreach(DebugType inter in this.Interfaces) {
					appDomain.Process.TraceMessage(" - Implements {0}", inter.FullName);
				}
			}
		}
		
		string GetName(bool includeNamespace)
		{
			if (IsArray) {
				return Trim(this.ElementType.FullName, includeNamespace) + "[" + new String(',', GetArrayRank() - 1) + "]";
			} else if (IsClass || IsValueType) {
				List<string> argNames = new List<string>();
				foreach(DebugType arg in this.GenericArguments) {
					argNames.Add(includeNamespace ? arg.FullName : arg.Name);
				}
				string className = Trim(classProps.Name, includeNamespace);
				// Remove generic parameter count at the end
				// '`' might be missing in nested generic classes
				int index = className.LastIndexOf('`');
				if (index != -1) {
					className = className.Substring(0, index);
				}
				if (argNames.Count > 0) {
					className += "<" + String.Join(",", argNames.ToArray()) + ">";
				}
				/*
				if (classProps.IsNested) {
					uint enclosingTk = module.MetaData.GetNestedClassProps(this.Token).EnclosingClass;
					DebugType enclosingType = DebugType.CreateFromTypeDefOrRef(this.Module, null, enclosingTk, this.GenericArguments.ToArray());
					className = enclosingType.FullName + "." + className;
				}
				*/
				return className;
			} else if (IsPrimitive) {
				return Trim(this.PrimitiveType.ToString(), includeNamespace);
			} else if (IsPointer) {
				return Trim(this.ElementType.FullName, includeNamespace) + (this.corElementType == CorElementType.BYREF ? "&" : "*");
			} else if (IsVoid) {
				return includeNamespace ? "System.Void" : "Void";
			} else {
				throw new DebuggerException("Unknown type: " + this.corElementType.ToString());
			}
		}
		
		string Trim(string name, bool includeNamespace)
		{
			if (includeNamespace) {
				return name;
			}
			int index = name.LastIndexOf('.');
			if (index == -1) {
				return name;
			} else {
				return name.Substring(index + 1);
			}
		}
		
		void LoadMemberInfo()
		{
			// Load interfaces
			foreach(InterfaceImplProps implProps in module.MetaData.EnumInterfaceImplProps(this.Token)) {
				CorTokenType tkType = (CorTokenType)(implProps.Interface & 0xFF000000);
				if (tkType == CorTokenType.TypeDef || tkType == CorTokenType.TypeRef) {
					this.interfaces.Add(DebugType.CreateFromTypeDefOrRef(module, false, implProps.Interface, null));
				} else if (tkType == CorTokenType.TypeSpec) {
					this.interfaces.Add(DebugType.CreateFromTypeSpec(module, implProps.Interface, this));
				} else {
					throw new DebuggerException("Uknown token type for interface: " + tkType);
				}
			}
			
			// Load fields
			foreach(FieldProps field in module.MetaData.EnumFieldProps(this.Token)) {
				if (field.IsStatic && field.IsLiteral) continue; // Skip static literals TODO: Why?
				members.Add(new DebugFieldInfo(this, field));
			};
			
			// Load methods
			foreach(MethodProps m in module.MetaData.EnumMethodProps(this.Token)) {
				members.Add(new MethodInfo(this, m));
			}
			
			// Load properties
			// TODO: Handle indexers ("get_Item") in other code
			// Collect data
			Dictionary<string, MethodInfo> accessors = new Dictionary<string, MethodInfo>();
			Dictionary<string, object> propertyNames = new Dictionary<string, object>();
			foreach(MethodInfo method in this.GetMethods(BindingFlags.AllInThisType)) {
				if (method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))) {
					// There can be many get_Items
					// TODO: This returns only last, return all
					accessors[method.Name] = method;
					propertyNames[method.Name.Remove(0,4)] = null;
				}
			}
			// Pair up getters and setters
			foreach(KeyValuePair<string, object> kvp in propertyNames) {
				MethodInfo getter = null;
				MethodInfo setter = null;
				accessors.TryGetValue("get_" + kvp.Key, out getter);
				accessors.TryGetValue("set_" + kvp.Key, out setter);
				members.Add(new PropertyInfo(this, getter, setter));
			}
		}
		
		/// <summary> Return whether the type has any members stisfing the given flags </summary>
		public bool HasMembers(BindingFlags bindingFlags)
		{
			return (GetMembers(bindingFlags).Count > 0);
		}
		
		public bool IsCompilerGenerated {
			get {
				if (this.IsClass || this.IsValueType) {
					return MethodInfo.HasAnyAttribute(this.Module.MetaData, this.Token, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute));
				} else {
					return false;
				}
			}
		}
		
		public bool IsDisplayClass {
			get {
				return this.Name.StartsWith("<>") && this.Name.Contains("__DisplayClass");
			}
		}
		
		public bool IsYieldEnumerator {
			get {
				if (this.IsCompilerGenerated) {
					foreach(DebugType intf in this.Interfaces) {
						if (intf.FullName == typeof(System.Collections.IEnumerator).FullName)
							return true;
					}
				}
				return false;
			}
		}
		
		public override string ToString()
		{
			return string.Format("{0}", this.FullName);
		}
	}
}
