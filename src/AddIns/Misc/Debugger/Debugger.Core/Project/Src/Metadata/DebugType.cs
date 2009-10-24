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
using System.Text;

namespace Debugger.MetaData
{
	/// <summary>
	/// Represents a type in a debugee. That is, a class, array, value type or a primitive type.
	/// This class mimics the <see cref="System.Type"/> class.
	/// </summary>
	/// <remarks>
	/// If two types are identical, the references to DebugType will also be identical 
	/// Type will be loaded once per each appdomain.
	/// </remarks>
	[Debugger.Tests.IgnoreOnException]
	public class DebugType: System.Type, IDebugMemberInfo
	{
		public const BindingFlags BindingFlagsAll = BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
		public const BindingFlags BindingFlagsAllDeclared = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
		
		Module module;
		ICorDebugType corType;
		CorElementType corElementType;
		Type primitiveType;
		TypeDefProps classProps;
		string ns;
		string name;
		string fullName;
		DebugType elementType;
		List<DebugType> genericArguments = new List<DebugType>();
		List<DebugType> interfaces = new List<DebugType>();
		
		// Members of the type; empty if not applicable
		Dictionary<string, List<MemberInfo>> membersByName = new Dictionary<string, List<MemberInfo>>();
		Dictionary<int, MemberInfo> membersByToken = new Dictionary<int, MemberInfo>();
		
		// Stores all DebugType instances. FullName is the key
		static Dictionary<ICorDebugType, DebugType> loadedTypes = new Dictionary<ICorDebugType, DebugType>();
		
		internal ICorDebugType CorType {
			get { return corType; }
		}
		
		/// <inheritdoc/>
		public override Type DeclaringType {
			get { throw new NotSupportedException(); }
		}
		
		/// <summary> The AppDomain in which this type is loaded </summary>
		public AppDomain AppDomain {
			get { return module.AppDomain; }
		}
		
		/// <summary> The Process in which this type is loaded </summary>
		public Process Process {
			get { return module.Process; }
		}
		
		/// <summary> The Module in which this type is loaded </summary>
		public Debugger.Module DebugModule {
			get { return module; }
		}
		
		/// <inheritdoc/>
		public override int MetadataToken {
			get { return (int)classProps.Token; }
		}
		
		/// <inheritdoc/>
		public override System.Reflection.Module Module {
			get { throw new NotSupportedException(); }
		}
		
		/// <inheritdoc/>
		public override string Name {
			get { return name; }
		}
		
		/// <inheritdoc/>
		public override Type ReflectedType {
			get { throw new NotSupportedException(); }
		}
		
		/// <inheritdoc/>
		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return IsDefined(this, inherit, attributeType);
		}
		
		public static bool IsDefined(IDebugMemberInfo member, bool inherit, params Type[] attributeTypes)
		{
			// TODO: Support inherit
			MetaDataImport metaData = member.DebugModule.MetaData;
			uint token = (uint)member.MetadataToken;
			foreach(CustomAttributeProps ca in metaData.EnumCustomAttributeProps(token, 0)) {
				CorTokenType tkType = (CorTokenType)(ca.Type & 0xFF000000);
				string attributeName;
				if (tkType == CorTokenType.MemberRef) {
					MemberRefProps constructorMethod = metaData.GetMemberRefProps(ca.Type);
					attributeName = metaData.GetTypeRefProps(constructorMethod.DeclaringType).Name;
				} else if (tkType == CorTokenType.MethodDef) {
					MethodProps constructorMethod = metaData.GetMethodProps(ca.Type);
					attributeName = metaData.GetTypeDefProps(constructorMethod.ClassToken).Name;
				} else {
					throw new DebuggerException("Not expected: " + tkType);
				}
				foreach(Type attributeType in attributeTypes) {
					if (attributeName == attributeType.FullName)
						return true;
				}
			}
			return false;
		}
		
		/// <inheritdoc/>
		public override Assembly Assembly {
			get { throw new NotSupportedException(); }
		}
		
		/// <inheritdoc/>
		public override string AssemblyQualifiedName {
			get { throw new NotSupportedException(); }
		}
		
		/// <inheritdoc/>
		public override Type BaseType {
			get {
				// corType.Base *sometimes* does not work for object and can cause "Value does not fall within the expected range." exception
				if (this.FullName == typeof(object).FullName) {
					return null;
				}
				// corType.Base does not work for arrays
				if (this.IsArray) {
					return DebugType.CreateFromType(this.AppDomain, typeof(Array));
				}
				// corType.Base does not work for primitive types
//				if (this.IsPrimitive) {
//					return DebugType.CreateFromType(this.AppDomain, typeof(ValueType));
//				}
				if (this.IsPointer || corElementType == CorElementType.VOID) {
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
		
		/// <inheritdoc/>
		public override string FullName {
			get { return fullName; }
		}
		
		/// <inheritdoc/>
		public override Guid GUID {
			get { throw new NotSupportedException(); }
		}
		
		//		public virtual GenericParameterAttributes GenericParameterAttributes { get; }
		//		public virtual int GenericParameterPosition { get; }
		//		public virtual bool IsGenericParameter { get; }
		//		public virtual bool IsGenericTypeDefinition { get; }
		
		/// <inheritdoc/>
		public override bool IsGenericType {
			get {
				return this.GetGenericArguments().Length > 0;
			}
		}
		
		/// <inheritdoc/>
		public override string Namespace {
			get { return ns; }
		}
		
		//		public virtual StructLayoutAttribute StructLayoutAttribute { get; }
		
		/// <inheritdoc/>
		public override RuntimeTypeHandle TypeHandle {
			get { throw new NotSupportedException(); }
		}
		
		/// <inheritdoc/>
		public override Type UnderlyingSystemType {
			get { return this; }
		}
		
		/// <inheritdoc/>
		public override int GetArrayRank()
		{
			if (!IsArray) throw new ArgumentException("Type is not array");
			
			return (int)corType.Rank;
		}
		
		/// <inheritdoc/>
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return (TypeAttributes)classProps.Flags;
		}
		
		/// <inheritdoc/>
		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			// TODO
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		//		public virtual MemberInfo[] GetDefaultMembers();
		
		/// <inheritdoc/>
		public override Type GetElementType()
		{
			return elementType;
		}
		
		const BindingFlags supportedFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy;
		
		/// <summary> Return member with the given token</summary>
		public MemberInfo GetMember(uint token)
		{
			return membersByToken[(int)token];
		}
		
		public T GetMember<T>(string name, BindingFlags bindingFlags, Predicate<T> filter) where T:MemberInfo
		{
			T[] res = GetMembers<T>(name, bindingFlags, filter);
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
			foreach(List<MemberInfo> memberInfos in membersByName.Values) {
				foreach(MemberInfo memberInfo in memberInfos) {
					// Filter by type
					if (!(memberInfo is T)) continue; // Reject item
					
					// Filter by name
					if (name != null) {
						if (memberInfo.Name != name) continue; // Reject item
					}
					
					// Filter by access
					if (((IDebugMemberInfo)memberInfo).IsPublic) {
						if ((bindingFlags & BindingFlags.Public) == 0) continue; // Reject item
					} else {
						if ((bindingFlags & BindingFlags.NonPublic) == 0) continue; // Reject item
					}
					
					// Filter by static / instance
					if (((IDebugMemberInfo)memberInfo).IsStatic) {
						if ((bindingFlags & BindingFlags.Static) == 0) continue; // Reject item
					} else {
						if ((bindingFlags & BindingFlags.Instance) == 0) continue; // Reject item
					}
					
					// Filter using predicate
					if (filter != null && !filter((T)memberInfo)) continue; // Reject item
					
					results.Add((T)memberInfo);
				}
			}
			
			// Query supertype
			if ((bindingFlags & BindingFlags.DeclaredOnly) == 0 && this.BaseType != null) {
				if ((bindingFlags & BindingFlags.FlattenHierarchy) == 0) {
					// Do not include static types
					bindingFlags = bindingFlags & ~BindingFlags.Static;
				}
				T[] superResults = ((DebugType)this.BaseType).GetMembers<T>(name, bindingFlags, filter);
				results.AddRange(superResults);
			}
			
			return results.ToArray();
		}
		
		/// <inheritdoc/>
		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		//		public virtual EventInfo[] GetEvents();
		
		/// <inheritdoc/>
		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			return GetMember<FieldInfo>(name, bindingAttr, null);
		}
		
		/// <inheritdoc/>
		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			return GetMembers<FieldInfo>(null, bindingAttr, null);
		}
		
		/// <inheritdoc/>
		public override Type[] GetGenericArguments()
		{
			return genericArguments.ToArray();
		}
		
		internal ICorDebugType[] GenericArgumentsAsCorDebugType {
			get {
				List<ICorDebugType> types = new List<ICorDebugType>();
				foreach(DebugType arg in GetGenericArguments()) {
					types.Add(arg.CorType);
				}
				return types.ToArray();
			}
		}
		
		//		public virtual Type[] GetGenericParameterConstraints();
		//		public virtual Type GetGenericTypeDefinition();
		
		/// <inheritdoc/>
		public override Type GetInterface(string name, bool ignoreCase)
		{
			foreach(DebugType inter in this.GetInterfaces()) {
				if (string.Equals(inter.FullName, name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) {
					return inter;
				}
			}
			if (BaseType != null) {
				return BaseType.GetInterface(fullName);
			} else {
				return null;
			}
		}
		
		//		public virtual InterfaceMapping GetInterfaceMap(Type interfaceType);
		
		/// <inheritdoc/>
		public override Type[] GetInterfaces()
		{
			return this.interfaces.ToArray();
		}
		
		/// <inheritdoc/>
		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			return GetMembers<MemberInfo>(name, bindingAttr, delegate(MemberInfo info) { return (info.MemberType & type) != 0; });
		}
		
		/// <inheritdoc/>
		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			return GetMembers<MemberInfo>(null, bindingAttr, null);
		}
		
		/// <summary> Return method overload with given parameter names </summary>
		/// <returns> Null if not found </returns>
		public MethodInfo GetMethod(string name, string[] paramNames)
		{
			foreach(DebugMethodInfo candidate in GetMembers<DebugMethodInfo>(name, BindingFlagsAll, null)) {
				if (candidate.ParameterCount == paramNames.Length) {
					bool match = true;
					for(int i = 0; i < paramNames.Length; i++) {
						if (paramNames[i] != candidate.GetParameters()[i].Name)
							match = false;
					}
					if (match)
						return candidate;
				}
			}
			return null;
		}
		
		/// <inheritdoc/>
		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] paramTypes, ParameterModifier[] modifiers)
		{
			// TODO: Finish
			foreach(DebugMethodInfo candidate in GetMethods(name, bindingAttr)) {
				if (paramTypes == null)
					return candidate;
				if (candidate.ParameterCount == paramTypes.Length) {
					bool match = true;
					for(int i = 0; i < paramTypes.Length; i++) {
						if (paramTypes[i] != candidate.GetParameters()[i].ParameterType)
							match = false;
					}
					if (match)
						return candidate;
				}
			}
			return null;
		}
		
		public MethodInfo[] GetMethods(string name, BindingFlags bindingAttr)
		{
			return GetMembers<MethodInfo>(name, bindingAttr, null);
		}
		
		/// <inheritdoc/>
		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			return GetMembers<MethodInfo>(null, bindingAttr, null);
		}
		
		/// <inheritdoc/>
		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
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
		
		/// <inheritdoc/>
		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			return GetMembers<PropertyInfo>(null, bindingAttr, null);
		}
		
		/// <inheritdoc/>
		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			// TODO: Finsih
			return GetMember<PropertyInfo>(name, bindingAttr, null);
		}
		
		/// <inheritdoc/>
		protected override bool HasElementTypeImpl()
		{
			return elementType != null;
		}
		
		/// <inheritdoc/>
		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		protected override bool IsArrayImpl()
		{
			return corElementType == CorElementType.ARRAY ||
			       corElementType == CorElementType.SZARRAY;
		}
		
		/// <inheritdoc/>
		protected override bool IsByRefImpl()
		{
			return corElementType == CorElementType.BYREF;
		}
		
		/// <inheritdoc/>
		protected override bool IsPointerImpl()
		{
			return corElementType == CorElementType.PTR;
		}
		
//		public bool IsClass {
//			get {
//				return !this.IsInterface && !this.IsSubclassOf(valueType);
//			}
//		}
//		
//		public bool IsInterface {
//			get {
//				return ((this.GetAttributeFlagsImpl() & TypeAttributes.Interface) != 0);
//			}
//		}
		
		/// <inheritdoc/>
		protected override bool IsValueTypeImpl()
		{
			// ValueType and Enum are exceptions and are threated as classes
			return this.FullName != typeof(ValueType).FullName &&
			       this.FullName != typeof(Enum).FullName &&
			       this.IsSubclassOf(this.AppDomain.ValueType);
		}
		
		/// <inheritdoc/>
		public override bool IsSubclassOf(Type superType)
		{
			if (!(superType is DebugType)) {
				superType = CreateFromType(this.AppDomain, superType);
			}
			return base.IsSubclassOf(superType);
		}
		
		/// <inheritdoc/>
		protected override bool IsCOMObjectImpl()
		{
			throw new NotSupportedException();
		}
		
		/// <inheritdoc/>
		public override bool IsInstanceOfType(object o)
		{
			if (o == null) return false;
			if (!(o is Value)) return false;
			return this.IsAssignableFrom(((Value)o).Type);
		}
		
		/// <inheritdoc/>
		public override bool IsAssignableFrom(Type c)
		{
			// TODO: Finsih
			if (this == c) return true;
			return c.IsSubclassOf(this);
		}
		
		//		protected virtual bool IsContextfulImpl();
		//		protected virtual bool IsMarshalByRefImpl();
		
		/// <summary> Returns simple managed type coresponding to the primitive type. </summary>
		[Debugger.Tests.Ignore]
		public System.Type PrimitiveType {
			get { return primitiveType; }
		}
		
		/// <inheritdoc/>
		protected override bool IsPrimitiveImpl()
		{
			return this.PrimitiveType != null;
		}
		
		/// <summary> Gets a value indicating whether the type is an integer type </summary>
		public bool IsInteger {
			get {
				switch (this.FullName) {
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
		
		public bool IsCompilerGenerated {
			get {
				return IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
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
					return GetInterface(typeof(System.Collections.IEnumerator).FullName) != null;
				}
				return false;
			}
		}
		
		bool IDebugMemberInfo.IsStatic {
			get { return false; }
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
					typeName = GetQualifiedName(typeName, genericArguments.Length - enclosingType.GetGenericArguments().Length);
				else
					typeName = GetQualifiedName(typeName, genericArguments.Length);
			}
			foreach(Module module in appDomain.Process.Modules) {
				if (module.AppDomain == appDomain) {
					uint token;
					try {
						token = module.MetaData.FindTypeDefPropsByName(typeName, enclosingType == null ? 0 : (uint)enclosingType.MetadataToken).Token;
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
				type = (DebugType)type.MakePointerType();
			}
			for(int i = typeRef.RankSpecifier.Length - 1; i >= 0; i--) {
				type = (DebugType)type.MakeArrayType(typeRef.RankSpecifier[i] + 1);
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
				return (DebugType)declaringType.GetGenericArguments()[((VAR)sigType).Index];
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
				return (DebugType)elementType.MakeArrayType(arraySig.Shape.Rank);
			}
			
			if (sigType is SZARRAY) {
				SZARRAY arraySig = (SZARRAY)sigType;
				DebugType elementType = CreateFromSignature(module, arraySig.Type, declaringType);
				return (DebugType)elementType.MakeArrayType();
			}
			
			if (sigType is PTR) {
				PTR ptrSig = (PTR)sigType;
				DebugType elementType;
				if (ptrSig.Void) {
					elementType = CreateFromType(module.AppDomain, typeof(void));
				} else {
					elementType = CreateFromSignature(module, ptrSig.PtrType, declaringType);
				}
				return (DebugType)elementType.MakePointerType();
			}
			
			if (sigType is FNPTR) {
				// TODO: FNPTR
			}
			
			throw new NotImplementedException(sigType.ElementType.ToString());
		}
		
		// public virtual Type MakeGenericType(params Type[] typeArguments);
		
		/// <inheritdoc/>
		public override Type MakeArrayType(int rank)
		{
			ICorDebugType res = this.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.ARRAY, (uint)rank, this.CorType);
			return CreateFromCorType(this.AppDomain, res);
		}
		
		/// <inheritdoc/>
		public override Type MakeArrayType()
		{
			ICorDebugType res = this.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.SZARRAY, 1, this.CorType);
			return CreateFromCorType(this.AppDomain, res);
		}
		
		/// <inheritdoc/>
		public override Type MakePointerType()
		{
			ICorDebugType res = this.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.PTR, 0, this.CorType);
			return CreateFromCorType(this.AppDomain, res);
		}
		
		/// <inheritdoc/>
		public override Type MakeByRefType()
		{
			ICorDebugType res = this.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.BYREF, 0, this.CorType);
			return CreateFromCorType(this.AppDomain, res);
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
			// Convert primitive type to class
			CorElementType corElemType = (CorElementType)(corType.Type);
			Type primitiveType = CorElementTypeToManagedType(corElemType);
			if (primitiveType != null) {
				// TODO: Look only in mscorlib
				corType = CreateFromType(appDomain, primitiveType).CorType;
			}
			if (corElemType == CorElementType.VOID) {
				// TODO: Look only in mscorlib
				corType = CreateFromType(appDomain, typeof(void)).CorType;
			}
			
			if (loadedTypes.ContainsKey(corType)) return loadedTypes[corType];
			
			return new DebugType(appDomain, corType);
		}
		
		DebugType(AppDomain appDomain, ICorDebugType corType)
		{
			if (corType == null) throw new ArgumentNullException("corType");
			DateTime startTime = Util.HighPrecisionTimer.Now;
			
			this.corType = corType;
			this.corElementType = (CorElementType)corType.Type;
			
			// Loading might access the type again
			loadedTypes[corType] = this;
			appDomain.Process.Exited += delegate { loadedTypes.Remove(corType); };
			
			if (corElementType == CorElementType.ARRAY ||
			    corElementType == CorElementType.SZARRAY ||
			    corElementType == CorElementType.PTR ||
			    corElementType == CorElementType.BYREF)
			{
				this.elementType = CreateFromCorType(appDomain, corType.FirstTypeParameter);
				this.module = appDomain.Mscorlib;
				this.classProps = new TypeDefProps();
				// Get names
				string suffix = string.Empty;
				if (corElementType == CorElementType.SZARRAY) suffix = "[]";
				if (corElementType == CorElementType.ARRAY)   suffix = "[" + new String(',', GetArrayRank() - 1) + "]";
				if (corElementType == CorElementType.PTR)     suffix = "*";
				if (corElementType == CorElementType.BYREF)   suffix = "&";
				this.ns = this.GetElementType().Namespace;
				this.name = this.GetElementType().Name + suffix;
				this.fullName = this.GetElementType().FullName + suffix;
			}
			
			if (corElementType == CorElementType.OBJECT ||
			    corElementType == CorElementType.CLASS ||
			    corElementType == CorElementType.VALUETYPE)
			{
				// We need to know generic arguments before getting name
				foreach(ICorDebugType t in corType.EnumerateTypeParameters().Enumerator) {
					genericArguments.Add(DebugType.CreateFromCorType(appDomain, t));
				}
				// We need class props for getting name
				this.module = appDomain.Process.Modules[corType.Class.Module];
				this.classProps = module.MetaData.GetTypeDefProps(corType.Class.Token);
				// Get names
				int index = classProps.Name.LastIndexOf('.');
				if (index == -1) {
					this.ns = string.Empty;
					this.name = classProps.Name;
				} else {
					this.ns = classProps.Name.Substring(0, index);
					this.name = classProps.Name.Substring(index + 1);
				}
				this.fullName = GetFullClassName();
				this.primitiveType = NameToManagedType(this.FullName);
				LoadMembers();
			}
			
			if (module == null)
				throw new DebuggerException("Unexpected: " + corElementType);
			
			TimeSpan totalTime2 = Util.HighPrecisionTimer.Now - startTime;
			if (appDomain.Process.Options.Verbose) {
				string prefix = this.IsInterface ? "interface" : "type";
				appDomain.Process.TraceMessage("Loaded {0} {1} ({2} ms)", prefix, this.FullName, totalTime2.TotalMilliseconds);
				foreach(DebugType inter in GetInterfaces()) {
					appDomain.Process.TraceMessage(" - Implements {0}", inter.FullName);
				}
			}
		}
		
		static Type CorElementTypeToManagedType(CorElementType corElementType)
		{
			switch(corElementType) {
				case CorElementType.BOOLEAN: return typeof(System.Boolean);
				case CorElementType.CHAR:    return typeof(System.Char);
				case CorElementType.I1:      return typeof(System.SByte);
				case CorElementType.U1:      return typeof(System.Byte);
				case CorElementType.I2:      return typeof(System.Int16);
				case CorElementType.U2:      return typeof(System.UInt16);
				case CorElementType.I4:      return typeof(System.Int32);
				case CorElementType.U4:      return typeof(System.UInt32);
				case CorElementType.I8:      return typeof(System.Int64);
				case CorElementType.U8:      return typeof(System.UInt64);
				case CorElementType.R4:      return typeof(System.Single);
				case CorElementType.R8:      return typeof(System.Double);
				case CorElementType.I:       return typeof(System.IntPtr);
				case CorElementType.U:       return typeof(System.UIntPtr);
				case CorElementType.STRING:  return typeof(System.String);
				default: return null;
			}
		}
		
		static Type NameToManagedType(string fullname)
		{
			switch (fullname) {
				case "System.Boolean": return typeof(System.Boolean);
				case "System.Char":    return typeof(System.Char);
				case "System.SByte":   return typeof(System.SByte);
				case "System.Byte":    return typeof(System.Byte);
				case "System.Int16":   return typeof(System.Int16);
				case "System.UInt16":  return typeof(System.UInt16);
				case "System.Int32":   return typeof(System.Int32);
				case "System.UInt32":  return typeof(System.UInt32);
				case "System.Int64":   return typeof(System.Int64);
				case "System.UInt64":  return typeof(System.UInt64);
				case "System.Single":  return typeof(System.Single);
				case "System.Double":  return typeof(System.Double);
				case "System.String":  return typeof(System.String);
				default: return null;
			}
		}
		
		string GetFullClassName()
		{
			StringBuilder name = new StringBuilder();
			
			if (classProps.IsNested) {
				uint enclosingTk = module.MetaData.GetNestedClassProps((uint)this.MetadataToken).EnclosingClass;
				DebugType enclosingType = DebugType.CreateFromTypeDefOrRef(this.DebugModule, null, enclosingTk, genericArguments.ToArray());
				name.Append(enclosingType.FullName);
				name.Append(".");
			}
			
			// '`' might be missing in nested generic classes
			name.Append(classProps.Name);
			
			if (this.GetGenericArguments().Length > 0) {
				name.Append("[[");
				bool first = true;
				foreach(DebugType arg in this.GetGenericArguments()) {
					if (!first)
						name.Append(", ");
					first = false;
					name.Append(arg.FullName);
				}
				name.Append("]]");
			}
			
			return name.ToString();
		}
		
		void LoadMembers()
		{
			// Load interfaces
			foreach(InterfaceImplProps implProps in module.MetaData.EnumInterfaceImplProps((uint)this.MetadataToken)) {
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
			foreach(FieldProps field in module.MetaData.EnumFieldProps((uint)this.MetadataToken)) {
				if (field.IsStatic && field.IsLiteral) continue; // Skip static literals TODO: Why?
				AddMember(new DebugFieldInfo(this, field));
			};
			
			// Load methods
			foreach(MethodProps m in module.MetaData.EnumMethodProps((uint)this.MetadataToken)) {
				AddMember(new DebugMethodInfo(this, m));
			}
			
			// Load properties
			// TODO: Handle properties properly
			// TODO: Handle indexers ("get_Item") in other code
			// Collect data
			Dictionary<string, MethodInfo> accessors = new Dictionary<string, MethodInfo>();
			Dictionary<string, object> propertyNames = new Dictionary<string, object>();
			foreach(MethodInfo method in this.GetMethods(BindingFlagsAllDeclared)) {
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
				AddMember(new DebugPropertyInfo(this, getter, setter));
			}
		}
		
		void AddMember(MemberInfo member)
		{
			if (!membersByName.ContainsKey(member.Name))
				membersByName.Add(member.Name, new List<MemberInfo>(1));
			membersByName[member.Name].Add(member);
			membersByToken[member.MetadataToken] = member;
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			return this.FullName;
		}
	}
}
