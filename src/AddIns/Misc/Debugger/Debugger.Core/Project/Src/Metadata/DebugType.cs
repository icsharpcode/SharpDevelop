// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory.Ast;
using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;
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
	public partial class DebugType: DebuggerObject
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
		
		/// <summary>
		/// Gets the module in which the class or value type is defined.
		/// <para> Only applicable to class or value type! </para>
		/// </summary>
		public Module Module {
			get {
				AssertClassOrValueType();
				return module;
			}
		}
		
		/// <summary>
		/// Gets the metadata token of the class or value type.
		/// <para> Only applicable to class or value type! </para>
		/// </summary>
		[Debugger.Tests.Ignore]
		public uint Token {
			get {
				AssertClassOrValueType();
				return classProps.Token;
			}
		}
		
		/// <summary> Gets the name of the type excluding the namespace </summary>
		public string Name {
			get { return name; }
		}
		
		/// <summary> Returns a string describing the type including the namespace
		/// and generic arguments but excluding the assembly name. </summary>
		public string FullName { 
			get { return fullName; } 
		}
		
		/// <summary> Returns the number of dimensions of an array </summary>
		/// <remarks> Throws <see cref="System.ArgumentException"/> if type is not array </remarks>
		public int GetArrayRank()
		{
			if (!IsArray) throw new ArgumentException("Type is not array");
			
			return (int)corType.Rank;
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
		
		/// <summary> Gets a value indicating whether the type is an array </summary>
		[Tests.Ignore]
		public bool IsArray {
			get {
				return this.Kind == DebugTypeKind.Array;
			}
		}
		
		/// <summary> Gets a value indicating whether the type is a class </summary>
		[Tests.Ignore]
		public bool IsClass {
			get {
				return this.Kind == DebugTypeKind.Class;
			}
		}
		
		/// <summary> Returns true if this type represents interface </summary>
		[Tests.Ignore]
		public bool IsInterface {
			get {
				return this.Kind == DebugTypeKind.Class && classProps.IsInterface;
			}
		}
		
		/// <summary> Gets a value indicating whether the type is a value type (that is, a structre in C#).
		/// Return false, if the type is a primitive type. </summary>
		[Tests.Ignore]
		public bool IsValueType {
			get {
				return this.Kind == DebugTypeKind.ValueType;
			}
		}
		
		/// <summary> Gets a value indicating whether the type is a primitive type </summary>
		/// <remarks> Primitive types are: boolean, char, string and all numeric types </remarks>
		[Tests.Ignore]
		public bool IsPrimitive {
			get {
				return this.PrimitiveType != null;
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
		
		/// <summary> Gets a value indicating whether the type is an managed or unmanaged pointer </summary>
		[Tests.Ignore]
		public bool IsPointer {
			get {
				return this.Kind == DebugTypeKind.Pointer;
			}
		}
		
		/// <summary> Gets a value indicating whether the type is the void type </summary>
		[Tests.Ignore]
		public bool IsVoid {
			get {
				return this.Kind == DebugTypeKind.Void;
			}
		}
		
		/// <summary>
		/// Gets the type from which this type inherits. 
		/// <para>
		/// Returns null if the current type is <see cref="System.Object"/>.
		/// </para>
		/// </summary>
		public DebugType BaseType {
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
			foreach(Module module in appDomain.Process.Modules) {
				if (module.AppDomain == appDomain) {
					uint token;
					try {
						token = module.MetaData.FindTypeDefPropsByName(GetQualifiedName(typeName, genericArguments), enclosingType == null ? 0 : enclosingType.Token).Token;
					} catch {
						continue;
					}
					return CreateFromTypeDefOrRef(module, null, token, genericArguments);
				}
			}
			throw new DebuggerException("Can not find type " + typeName);
		}
		
		/// <summary> Converts type name to the form suitable for COM API. </summary>
		static string GetQualifiedName(string typeName, params DebugType[] genericArguments)
		{
			if (genericArguments != null && genericArguments.Length > 0 && !typeName.Contains("`")) {
				return typeName + "`" + genericArguments.Length;
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
				type = CreatePointer(type);
			}
			for(int i = typeRef.RankSpecifier.Length - 1; i >= 0; i--) {
				type = CreateArray(type, typeRef.RankSpecifier[i] + 1);
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
				return CreateArray(elementType, arraySig.Shape.Rank);
			}
			
			if (sigType is SZARRAY) {
				SZARRAY arraySig = (SZARRAY)sigType;
				DebugType elementType = CreateFromSignature(module, arraySig.Type, declaringType);
				return CreateSzArray(elementType);
			}
			
			if (sigType is PTR) {
				PTR ptrSig = (PTR)sigType;
				DebugType elementType;
				if (ptrSig.Void) {
					elementType = CreateFromType(module.AppDomain, typeof(void));
				} else {
					elementType = CreateFromSignature(module, ptrSig.PtrType, declaringType);
				}
				return CreatePointer(elementType);
			}
			
			if (sigType is FNPTR) {
				// TODO: FNPTR
			}
			
			throw new NotImplementedException(sigType.ElementType.ToString());
		}
		
		public static DebugType CreateArray(DebugType elementType, int rank)
		{
			ICorDebugType res = elementType.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.ARRAY, (uint)rank, elementType.CorType);
			return CreateFromCorType(elementType.AppDomain, res);
		}
		
		public static DebugType CreateSzArray(DebugType elementType)
		{
			ICorDebugType res = elementType.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.SZARRAY, 1, elementType.CorType);
			return CreateFromCorType(elementType.AppDomain, res);
		}
		
		public static DebugType CreatePointer(DebugType elementType)
		{
			ICorDebugType res = elementType.AppDomain.CorAppDomain.CastTo<ICorDebugAppDomain2>().GetArrayOrPointerType((uint)CorElementType.PTR, 0, elementType.CorType);
			return CreateFromCorType(elementType.AppDomain, res);
		}
		
		public static DebugType CreateReference(DebugType elementType)
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
				members.Add(new FieldInfo(this, field));
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
		
		/// <summary> Determines whether the current type is sublass of 
		/// the the given type. That is, it derives from the given type. </summary>
		/// <remarks> Returns false if the given type is same as the current type </remarks>
		public bool IsSubclassOf(DebugType superType)
		{
			DebugType type = this;
			while (type != null) {
				if (type.Equals(superType)) return true;
				if (superType.IsInterface) {
					// Does this 'type' implement the interface?
					foreach(DebugType inter in type.Interfaces) {
						if (inter == superType) return true;
					}
				}
				type = type.BaseType;
			}
			return false;
		}
		
		/// <summary> Determines whether the given object is instance of the
		/// current type or can be implicitly cast to it </summary>
		public bool IsInstanceOfType(Value objectInstance)
		{
			return objectInstance.Type.Equals(this) ||
			       objectInstance.Type.IsSubclassOf(this);
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
				return this.IsCompilerGenerated && this.Name.Contains("DisplayClass");
			}
		}
		
		public override string ToString()
		{
			return string.Format("{0}", this.FullName);
		}
	}
}
