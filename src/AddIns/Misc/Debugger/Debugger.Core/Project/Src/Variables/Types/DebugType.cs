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
	/// <para> This class mimics the <see cref="System.Type"/> class. </para>
	/// </summary>
	public partial class DebugType: RemotingObjectBase
	{
		Process process;
		ICorDebugType corType;
		CorElementType corElementType;
		
		// Class/ValueType specific data
		ICorDebugClass corClass;
		Module module;
		TypeDefProps classProps;
		
		// Cache
		List<FieldInfo> fields;
		List<MethodInfo> methods;
		List<PropertyInfo> properties;
		
		void AssertClassOrValueType()
		{
			if(!IsClass && !IsValueType) {
				throw new DebuggerException("The type is not a class or value type.");
			}
		}
		
		/// <summary> Gets the process in which the type was loaded </summary>
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
		
		/// <summary> Returns a string describing the type </summary>
		public string Name { 
			get {
				// TODO: Improve
				if(IsClass || IsValueType) {
					return classProps.Name;
				} else {
					System.Type managedType = this.ManagedType;
					if (managedType != null) {
						return managedType.ToString();
					} else {
						return "<unknown>";
					}
				}
			} 
		}
		
		/// <summary> Gets a value indicating whether the type is an array </summary>
		public bool IsArray {
			get {
				return this.corElementType == CorElementType.ARRAY ||
				       this.corElementType == CorElementType.SZARRAY;
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
					return new DebugType(process, baseType);
				} else {
					return null;
				}
			}
		}
		
		internal DebugType(Process process, ICorDebugType corType)
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
			
			process.TraceMessage("Created type " + this.Name);
		}
		
		/// <summary>
		/// Obtains instance of DebugType using process cache
		/// </summary>
		static internal DebugType Create(Process process, ICorDebugType corType)
		{
			return process.GetDebugType(corType);
		}
		
		/// <summary> Determines whether the current type is sublass of 
		/// the the given type. That is, it derives from the given type. </summary>
		/// <remarks> Returns false if the given type is same as the current type </remarks>
		public bool IsSubclassOf(DebugType superType)
		{
			DebugType type = this.BaseType;
			while (type != null) {
				if (this.Equals(type)) return true;
				type = type.BaseType;
			}
			return false;
		}
		
		/// <summary> Determines whether the given object is instance of the
		/// current type or can be implicitly cast to it </summary>
		public bool IsInstanceOfType(Value objectInstance)
		{
			return this.Equals(objectInstance.Type) ||
			       this.IsSubclassOf(objectInstance.Type);
		}
		
		List<FieldInfo> GetAllFields()
		{
			AssertClassOrValueType();
			
			// Build cache
			if (fields == null) {
				process.TraceMessage("Loading fields for type " + this.Name);
				fields = new List<FieldInfo>();
				foreach(FieldProps field in module.MetaData.EnumFields(this.MetadataToken)) {
					// TODO: Why?
					if (field.IsStatic && field.IsLiteral) continue; // Skip static literals
					fields.Add(new FieldInfo(this, field));
				};
			}
			return fields;
		}
		
		List<MethodInfo> GetAllMethods()
		{
			AssertClassOrValueType();
			
			// Build cache
			if (methods == null) {
				process.TraceMessage("Loading methods for type " + this.Name);
				methods = new List<MethodInfo>();
				foreach(MethodProps m in module.MetaData.EnumMethods(this.MetadataToken)) {
					methods.Add(new MethodInfo(this, m));
				}
			}
			return methods;
		}
		
		// TODO: Handle indexers ("get_Item") in other code
		List<PropertyInfo> GetAllProperties()
		{
			AssertClassOrValueType();
			
			// Build cache
			if (properties == null) {
				process.TraceMessage("Loading properties for type " + this.Name);
				properties = new List<PropertyInfo>();
				// Collect data
				Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
				Dictionary<string, object> names = new Dictionary<string, object>();
				foreach(MethodInfo method in GetAllMethods()) {
					if (method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))) {
						methods.Add(method.Name, method);
						names.Add(method.Name.Remove(0,4), null);
					}
				}
				// Pair up getters and setters
				foreach(KeyValuePair<string, object> kvp in names) {
					MethodInfo getter = null;
					MethodInfo setter = null;
					methods.TryGetValue("get_" + kvp.Key, out getter);
					methods.TryGetValue("set_" + kvp.Key, out setter);
					properties.Add(new PropertyInfo(this, getter, setter));
				}
			}
			return properties;
		}
		
		/// <summary> Return all public fields.</summary>
		public IList<FieldInfo> GetFields()
		{
			return GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		}
		
		/// <summary> Return all fields satisfing binding flags.</summary>
		public IList<FieldInfo> GetFields(BindingFlags bindingFlags)
		{
			if (IsClass || IsValueType) {
				return FilterMemberInfo(GetAllFields(), bindingFlags);
			} else {
				return new List<FieldInfo>();
			}
		}
		
		/// <summary> Return all public methods.</summary>
		public IList<MethodInfo> GetMethods()
		{
			return GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		}
		
		/// <summary> Return all methods satisfing binding flags.</summary>
		public IList<MethodInfo> GetMethods(BindingFlags bindingFlags)
		{
			if (IsClass || IsValueType) {
				return FilterMemberInfo(GetAllMethods(), bindingFlags);
			} else {
				return new List<MethodInfo>();
			}
		}
		
		/// <summary> Return all public properties.</summary>
		public IList<PropertyInfo> GetProperties()
		{
			return GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		}
		
		/// <summary> Return all properties satisfing binding flags.</summary>
		public IList<PropertyInfo> GetProperties(BindingFlags bindingFlags)
		{
			if (IsClass || IsValueType) {
				return FilterMemberInfo(GetAllProperties(), bindingFlags);
			} else {
				return new List<PropertyInfo>();
			}
		}
		
		/// <summary> Compares two types </summary>
		public override bool Equals(object obj)
		{
			return obj is DebugType &&
			       ((DebugType)obj).CorType == this.CorType;
		}
		
		/// <summary> Get hash code of the object </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
