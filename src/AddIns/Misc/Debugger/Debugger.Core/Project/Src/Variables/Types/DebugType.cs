// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	/// <summary>
	/// Represents a type in a debugee. That is, a class, array, value type or a primitive type.
	/// This class mimics the <see cref="System.Type"/> class.
	/// </summary>
	/// <remarks> If two types are identical, the references to DebugType will also be identical </remarks>
	public partial class DebugType: DebuggerObject
	{
		Process process;
		ICorDebugType corType;
		CorElementType corElementType;
		string fullName;
		
		// Class/ValueType specific
		ICorDebugClass corClass;
		Module module;
		TypeDefProps classProps;
		
		// Class/ValueType/Array/Ref/Ptr specific
		List<DebugType>    typeArguments = new List<DebugType>();
		
		// Members of the type; empty lists if not applicable
		List<FieldInfo>    fields = new List<FieldInfo>();
		List<MethodInfo>   methods = new List<MethodInfo>();
		List<PropertyInfo> properties = new List<PropertyInfo>();
		List<MemberInfo>   members = new List<MemberInfo>(); // All combined
		
		// Stores all DebugType instances. FullName is the key
		static Dictionary<string, List<DebugType>> loadedTypes = new Dictionary<string, List<DebugType>>();
		
		void AssertClassOrValueType()
		{
			if(!IsClass && !IsValueType) {
				throw new DebuggerException("The type is not a class or value type.");
			}
		}
		
		/// <summary> Gets the process in which the type was loaded </summary>
		[Debugger.Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		internal ICorDebugType CorType {
			get {
				return corType;
			}
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
		public uint MetadataToken {
			get {
				AssertClassOrValueType();
				return classProps.Token;
			}
		}
		
		/// <summary> Returns a string describing the type including the namespace
		/// and generic arguments but excluding the assembly name. </summary>
		public string FullName { 
			get {
				return fullName;
			} 
		}
		
		/// <summary> Returns the number of dimensions of an array </summary>
		/// <remarks> Throws <see cref="System.ArgumentException"/> if type is not array </remarks>
		public int GetArrayRank()
		{
			if (IsArray) {
				return (int)corType.Rank;
			} else {
				throw new ArgumentException("Type is not array");
			}
		}
		
		/// <summary> Returns true if the type has an element type. 
		/// (ie array, reference or pointer) </summary>
		public bool HasElementType {
			get {
				return IsArray;
			}
		}
		
		/// <summary> Returns an element type for array, reference or pointer. 
		/// Retuns null otherwise. (Secificaly, returns null for generic types) </summary>
		public DebugType GetElementType()
		{
			if (HasElementType) {
				return typeArguments[0];
			} else {
				return null;
			}
		}
		
		/// <summary> Gets a value indicating whether the type is an array </summary>
		public bool IsArray {
			get {
				return this.corElementType == CorElementType.ARRAY ||
				       this.corElementType == CorElementType.SZARRAY;
			}
		}
		
		/// <summary> Gets a value indicating whether the immediate type is generic.  
		/// Arrays, references and pointers are never generic types. </summary>
		public bool IsGenericType {
			get {
				return (IsClass || IsValueType) &&
				       typeArguments.Count > 0;
			}
		}
		
		/// <summary> Returns generics arguments for a type or an emtpy 
		/// array for non-generic types. </summary>
		public DebugType[] GetGenericArguments()
		{
			if (IsGenericType) {
				return typeArguments.ToArray();
			} else {
				return new DebugType[] {};
			}
		}
		
		/// <summary> Gets a value indicating whether the type is a class </summary>
		public bool IsClass {
			get { 
				return this.corElementType == CorElementType.CLASS ||
				       this.corElementType == CorElementType.OBJECT;
			}
		}
		
		/// <summary> Gets a value indicating whether the type is a value type (that is, a structre in C#) </summary>
		public bool IsValueType {
			get {
				return this.corElementType == CorElementType.VALUETYPE;
			}
		}
		
		/// <summary> Gets a value indicating whether the type is a primitive type </summary>
		public bool IsPrimitive {
			get {
				return this.corElementType == CorElementType.BOOLEAN ||
				       this.corElementType == CorElementType.CHAR ||
				       this.corElementType == CorElementType.I1 ||
				       this.corElementType == CorElementType.U1 ||
				       this.corElementType == CorElementType.I2 ||
				       this.corElementType == CorElementType.U2 ||
				       this.corElementType == CorElementType.I4 ||
				       this.corElementType == CorElementType.U4 ||
				       this.corElementType == CorElementType.I8 ||
				       this.corElementType == CorElementType.U8 ||
				       this.corElementType == CorElementType.R4 ||
				       this.corElementType == CorElementType.R8 ||
				       this.corElementType == CorElementType.I ||
				       this.corElementType == CorElementType.U ||
				       this.corElementType == CorElementType.STRING;
			}
		}
		
		/// <summary> Gets a value indicating whether the type is an integer type </summary>
		public bool IsInteger {
			get {
				return this.corElementType == CorElementType.I1 ||
				       this.corElementType == CorElementType.U1 ||
				       this.corElementType == CorElementType.I2 ||
				       this.corElementType == CorElementType.U2 ||
				       this.corElementType == CorElementType.I4 ||
				       this.corElementType == CorElementType.U4 ||
				       this.corElementType == CorElementType.I8 ||
				       this.corElementType == CorElementType.U8 ||
				       this.corElementType == CorElementType.I ||
				       this.corElementType == CorElementType.U;
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
				ICorDebugType baseType = corType.Base;
				if (baseType != null) {
					return Create(process, baseType);
				} else {
					return null;
				}
			}
		}
		
		DebugType(Process process, ICorDebugType corType)
		{
			if (corType == null) throw new ArgumentNullException("corType");
			
			this.process = process;
			this.corType = corType;
			this.corElementType = (CorElementType)corType.Type;
			
			if (this.IsClass || this.IsValueType) {
				this.corClass = corType.Class;
				this.module = process.GetModule(corClass.Module);
				this.classProps = module.MetaData.GetTypeDefProps(corClass.Token);
			}
			
			if (this.IsClass || this.IsValueType || this.IsArray) {
				foreach(ICorDebugType t in corType.EnumerateTypeParameters().Enumerator) {
					typeArguments.Add(DebugType.Create(process, t));
				}
			}
			
			this.fullName = GetFullName();
		}
		
		static internal DebugType Create(Process process, ICorDebugClass corClass, List<ICorDebugType> typeArguments)
		{
			MetaData metaData = process.GetModule(corClass.Module).MetaData;
			
			bool isValueType = false;
			uint superClassToken = metaData.GetTypeDefProps(corClass.Token).SuperClassToken;
			if ((superClassToken & 0xFF000000) == 0x02000000) { // TypeDef
				if (metaData.GetTypeDefProps(superClassToken).Name == "System.ValueType") {
					isValueType = true;
				}
			}
			if ((superClassToken & 0xFF000000) == 0x01000000) { // TypeRef
				if (metaData.GetTypeRefProps(superClassToken).Name == "System.ValueType") {
					isValueType = true;
				}
			}
			
			int getArgsCount = metaData.GetGenericParamCount(corClass.Token);
			
			ICorDebugType corType = corClass.CastTo<ICorDebugClass2>().GetParameterizedType(
				isValueType ? (uint)CorElementType.VALUETYPE : (uint)CorElementType.CLASS,
				typeArguments.GetRange(0, getArgsCount).ToArray()
			);
			
			return Create(process, corType);
		}
		
		/// <summary> Obtains instance of DebugType. Same types will return identical instance. </summary>
		static internal DebugType Create(Process process, ICorDebugType corType)
		{
			DateTime startTime = Util.HighPrecisionTimer.Now;
			
			DebugType type = new DebugType(process, corType);
			
			// Get types with matching names from cache
			List<DebugType> typesWithMatchingName;
			if (!loadedTypes.TryGetValue(type.FullName, out typesWithMatchingName)) {
				// No types with such name - create a new list
				typesWithMatchingName = new List<DebugType>(1);
				loadedTypes.Add(type.FullName, typesWithMatchingName);
			}
			
			// Try to find the type
			foreach(DebugType loadedType in typesWithMatchingName) {
				if (loadedType.Equals(type)) {
					TimeSpan totalTime = Util.HighPrecisionTimer.Now - startTime;
					//process.TraceMessage("Type " + type.FullName + " was loaded already (" + totalTime.TotalMilliseconds + " ms)");
					return loadedType; // Type was loaded before
				}
			}
			
			// The type is not in the cache, finish loading it and add it to the cache
			if (type.IsClass || type.IsValueType) {
				type.LoadMemberInfo();
			}
			typesWithMatchingName.Add(type);
			type.Process.Expired += delegate { typesWithMatchingName.Remove(type); };
			
			TimeSpan totalTime2 = Util.HighPrecisionTimer.Now - startTime;
			process.TraceMessage("Loaded type " + type.FullName + " (" + totalTime2.TotalMilliseconds + " ms)");
			
			return type;
		}
		
		string GetFullName()
		{
			if (IsArray) {
				return GetElementType().FullName + "[" + new String(',', GetArrayRank() - 1) + "]";
			} else if (IsClass || IsValueType) {
				if (IsGenericType) {
					List<string> argNames = new List<string>();
					foreach(DebugType arg in GetGenericArguments()) {
						argNames.Add(arg.FullName);
					}
					string className = classProps.Name;
					// Remove generic parameter count at the end
					// '`' might be missing in nested generic classes
					int index = className.LastIndexOf('`');
					if (index != -1) {
						className = className.Substring(0, index);
					}
					return className + "<" + String.Join(",", argNames.ToArray()) + ">";
				} else {
					return classProps.Name;
				}
			} else if (IsPrimitive) {
				return this.ManagedType.ToString();
			} else {
				throw new DebuggerException("Unknown type");
			}
		}
		
		void LoadMemberInfo()
		{
			// Load fields
			foreach(FieldProps field in module.MetaData.EnumFields(this.MetadataToken)) {
				if (field.IsStatic && field.IsLiteral) continue; // Skip static literals TODO: Why?
				fields.Add(new FieldInfo(this, field));
			};
			
			// Load methods
			foreach(MethodProps m in module.MetaData.EnumMethods(this.MetadataToken)) {
				methods.Add(new MethodInfo(this, m));
			}
			
			// Load properties
			// TODO: Handle indexers ("get_Item") in other code
			// Collect data
			Dictionary<string, MethodInfo> accessors = new Dictionary<string, MethodInfo>();
			Dictionary<string, object> propertyNames = new Dictionary<string, object>();
			foreach(MethodInfo method in methods) {
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
				properties.Add(new PropertyInfo(this, getter, setter));
			}
			
			// Make a combined collection
			foreach(FieldInfo field in fields) {
				members.Add(field);
			}
			foreach(MemberInfo method in methods) {
				members.Add(method);
			}
			foreach(PropertyInfo property in properties) {
				members.Add(property);
			}
		}
		
		/// <summary> Determines whether the current type is sublass of 
		/// the the given type. That is, it derives from the given type. </summary>
		/// <remarks> Returns false if the given type is same as the current type </remarks>
		public bool IsSubclassOf(DebugType superType)
		{
			DebugType type = this.BaseType;
			while (type != null) {
				if (type.Equals(superType)) return true;
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
		
		/// <summary> Return all public fields.</summary>
		public IList<FieldInfo> GetFields()
		{
			return GetFields(BindingFlags.Public);
		}
		
		/// <summary> Return all fields satisfing binding flags.</summary>
		public IList<FieldInfo> GetFields(BindingFlags bindingFlags)
		{
			return FilterMemberInfo(fields, bindingFlags);
		}
		
		/// <summary> Return all public methods.</summary>
		public IList<MethodInfo> GetMethods()
		{
			return GetMethods(BindingFlags.Public);
		}
		
		/// <summary> Return all methods satisfing binding flags.</summary>
		public IList<MethodInfo> GetMethods(BindingFlags bindingFlags)
		{
			return FilterMemberInfo(methods, bindingFlags);
		}
		
		/// <summary> Return all public properties.</summary>
		public IList<PropertyInfo> GetProperties()
		{
			return GetProperties(BindingFlags.Public);
		}
		
		/// <summary> Return all properties satisfing binding flags.</summary>
		public IList<PropertyInfo> GetProperties(BindingFlags bindingFlags)
		{
			return FilterMemberInfo(properties, bindingFlags);
		}
		
		/// <summary> Return all members that have spefied name and satisfy binding flags </summary>
		public IList<MemberInfo> GetMember(string name, BindingFlags bindingFlags)
		{
			return FilterMemberInfo(FilterMemberInfo(members, name), bindingFlags);
		}
		
		/// <summary> Return all public members.</summary>
		public IList<MemberInfo> GetMembers()
		{
			return GetMembers(BindingFlags.Public);
		}
		
		/// <summary> Return all members satisfing binding flags.</summary>
		public IList<MemberInfo> GetMembers(BindingFlags bindingFlags)
		{
			return FilterMemberInfo(members, bindingFlags);
		}
		
		/// <summary> Return whether the type has any members stisfing the given flags </summary>
		public bool HasMembers(BindingFlags bindingFlags)
		{
			return (GetMembers(bindingFlags).Count > 0);
		}
		
		/// <summary> Compares two types </summary>
		public override bool Equals(object obj)
		{
			DebugType other = obj as DebugType;
			if (other != null && this.Process == other.Process) {
				if (this.IsArray) {
					return other.IsArray &&
					       other.GetArrayRank() == this.GetArrayRank() &&
					       other.GetElementType().Equals(this.GetElementType());
				}
				if (this.IsPrimitive) {
					return other.IsPrimitive &&
					       other.corElementType == this.corElementType;
				}
				if (this.IsClass || this.IsValueType) {
					return (other.IsClass || other.IsValueType) &&
					       other.Module == this.Module &&
					       other.MetadataToken == this.MetadataToken;
				}
				throw new DebuggerException("Unknown type");
			} else {
				return false;
			}
		}
		
		/// <summary> Get hash code of the object </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			return string.Format("{0}", this.fullName);
		}
	}
}
