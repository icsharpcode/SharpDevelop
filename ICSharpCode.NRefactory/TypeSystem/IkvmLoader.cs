//
// IkvmLoader.cs
//
// Author:
//       Daniel Grunwald <daniel@danielgrunwald.de>
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using IKVM.Reflection;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Allows loading an IProjectContent from an already compiled assembly.
	/// </summary>
	/// <remarks>Instance methods are not thread-safe; you need to create multiple instances of CecilLoader
	/// if you want to load multiple project contents in parallel.</remarks>
	public class IkvmLoader 
	{
		/// <summary>
		/// Version number of the ikvm loader.
		/// Should be incremented when fixing bugs in the ikvm loader so that project contents cached on disk
		/// (which might be incorrect due to the bug) are re-created.
		/// </summary>
		const int ikvmLoaderVersion = 1;

		#region Options
		/// <summary>
		/// Specifies whether to include internal members. The default is false.
		/// </summary>
		public bool IncludeInternalMembers { get; set; }

		/// <summary>
		/// Gets/Sets the documentation provider that is used to retrieve the XML documentation for all members.
		/// </summary>
		public IDocumentationProvider DocumentationProvider { get; set; }

		InterningProvider interningProvider;

		/// <summary>
		/// Gets/Sets the interning provider.
		/// </summary>
		public InterningProvider InterningProvider {
			get { return interningProvider; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				interningProvider = value;
			}
		}

		/// <summary>
		/// Gets/Sets the cancellation token used by the cecil loader.
		/// </summary>
		public CancellationToken CancellationToken { get; set; }

		/// <summary>
		/// This delegate gets executed whenever an entity was loaded.
		/// </summary>
		/// <remarks>
		/// This callback may be to build a dictionary that maps between
		/// entities and cecil objects.
		/// Warning: if delay-loading is used and the type system is accessed by multiple threads,
		/// the callback may be invoked concurrently on multiple threads.
		/// </remarks>
		public Action<IUnresolvedEntity, MemberInfo> OnEntityLoaded { get; set; }
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="ICSharpCode.NRefactory.TypeSystem.IkvmLoader"/> class.
		/// </summary>
		public IkvmLoader()
		{
			// Enable interning by default.
			this.InterningProvider = new SimpleInterningProvider();
		}

		#region Load Assembly From Disk

		public IUnresolvedAssembly LoadAssemblyFile(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");

			using (var universe = new Universe (UniverseOptions.DisablePseudoCustomAttributeRetrieval)) {
				return LoadAssembly (universe.LoadFile (fileName));
			}
		}

		#endregion

		IkvmUnresolvedAssembly currentAssembly;
		Assembly currentAssemblyDefinition;

		/// <summary>
		/// Loads the assembly definition into a project content.
		/// </summary>
		/// <returns>Unresolved type system representing the assembly</returns>

		[CLSCompliant(false)]
		public IUnresolvedAssembly LoadAssembly(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException ("assembly");

			// Read assembly and module attributes
			IList<IUnresolvedAttribute> assemblyAttributes = new List<IUnresolvedAttribute>();
			IList<IUnresolvedAttribute> moduleAttributes = new List<IUnresolvedAttribute>();
			AddAttributes(assembly, assemblyAttributes);
			AddAttributes(assembly.ManifestModule, moduleAttributes);

			assemblyAttributes = interningProvider.InternList(assemblyAttributes);
			moduleAttributes = interningProvider.InternList(moduleAttributes);

			currentAssemblyDefinition = assembly;
			currentAssembly = new IkvmUnresolvedAssembly (assembly.FullName, DocumentationProvider);
			currentAssembly.Location = assembly.Location;
			currentAssembly.AssemblyAttributes.AddRange(assemblyAttributes);
			currentAssembly.ModuleAttributes.AddRange(moduleAttributes);
			// Register type forwarders:
			foreach (var type in assembly.ManifestModule.__GetExportedTypes ()) {
				if (type.Assembly != assembly) {
					int typeParameterCount;
					string ns = type.Namespace;
					string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(type.Name, out typeParameterCount);
					ns = interningProvider.Intern(ns);
					name = interningProvider.Intern(name);
					var typeRef = new GetClassTypeReference(GetAssemblyReference(type.Assembly), ns, name, typeParameterCount);
					typeRef = interningProvider.Intern(typeRef);
					var key = new TopLevelTypeName(ns, name, typeParameterCount);
					currentAssembly.AddTypeForwarder(key, typeRef);
				}
			}

			// Create and register all types:
			var ikvmTypeDefs = new List<IKVM.Reflection.Type>();
			var typeDefs = new List<DefaultUnresolvedTypeDefinition>();

			foreach (var td in assembly.GetTypes ()) {
				if (td.DeclaringType != null)
					continue;
				CancellationToken.ThrowIfCancellationRequested();

				if (IncludeInternalMembers || td.IsPublic) {
					string name = td.Name;
					if (name.Length == 0)
						continue;

					var t = CreateTopLevelTypeDefinition(td);
					ikvmTypeDefs.Add(td);
					typeDefs.Add(t);
					currentAssembly.AddTypeDefinition(t);
					// The registration will happen after the members are initialized
				}
			}

			// Initialize the type's members:
			for (int i = 0; i < typeDefs.Count; i++) {
				InitTypeDefinition(ikvmTypeDefs[i], typeDefs[i]);
			}

			// Freezing the assembly here is important:
			// otherwise it will be frozen when a compilation is first created
			// from it. But freezing has the effect of changing some collection instances
			// (to ReadOnlyCollection). This hidden mutation was causing a crash
			// when the FastSerializer was saving the assembly at the same time as
			// the first compilation was created from it.
			// By freezing the assembly now, we ensure it is usable on multiple
			// threads without issues.
			currentAssembly.Freeze();

			var result = currentAssembly;
			currentAssembly = null;
			return result;
		}

		#region IUnresolvedAssembly implementation
		[Serializable, FastSerializerVersion(ikvmLoaderVersion)]
		sealed class IkvmUnresolvedAssembly : DefaultUnresolvedAssembly, IDocumentationProvider
		{
			readonly IDocumentationProvider documentationProvider;

			public IkvmUnresolvedAssembly(string fullAssemblyName, IDocumentationProvider documentationProvider)
				: base(fullAssemblyName)
			{
				this.documentationProvider = documentationProvider;
			}

			DocumentationComment IDocumentationProvider.GetDocumentation(IEntity entity)
			{
				if (documentationProvider != null)
					return documentationProvider.GetDocumentation (entity);
				return null;
			}
		}
		#endregion

		#region Read Type Reference
		/// <summary>
		/// Reads a type reference.
		/// </summary>
		/// <param name="type">The Cecil type reference that should be converted into
		/// a type system type reference.</param>
		/// <param name="typeAttributes">Attributes associated with the Cecil type reference.
		/// This is used to support the 'dynamic' type.</param>
		[CLSCompliant(false)]
		public ITypeReference ReadTypeReference(IKVM.Reflection.Type type, IEnumerable<CustomAttributeData> typeAttributes = null)
		{
			int typeIndex = 0;
			return CreateTypeReference(type, typeAttributes, ref typeIndex);
		}

		ITypeReference CreateTypeReference(IKVM.Reflection.Type type, IEnumerable<CustomAttributeData> typeAttributes, ref int typeIndex)
		{
			// TODO:
			//			while (type is OptionalModifierType || type is RequiredModifierType) {
			//				type = ((TypeSpecification)type).ElementType;
			//			}

			if (type == null) {
				return SpecialType.UnknownType;
			}

			if (type.IsByRef) {
				typeIndex++;
				return interningProvider.Intern (
					new ByReferenceTypeReference (
					CreateTypeReference (
					type.GetElementType (),
					typeAttributes, ref typeIndex)));
			}
			if (type.IsPointer) {
				typeIndex++;
				return interningProvider.Intern (
					new PointerTypeReference (
					CreateTypeReference (
					type.GetElementType (),
					typeAttributes, ref typeIndex)));
			}
			if (type.IsArray) {
				typeIndex++;
				return interningProvider.Intern (
					new ArrayTypeReference (
						CreateTypeReference (
						type.GetElementType (),
						typeAttributes, ref typeIndex),
						type.GetArrayRank ()));
			}
			if (type.IsConstructedGenericType) {
				ITypeReference baseType = CreateTypeReference (type.GetGenericTypeDefinition (), typeAttributes, ref typeIndex);
				var args = type.GetGenericArguments ();
				var para = new ITypeReference[args.Length];
				for (int i = 0; i < para.Length; ++i) {
					typeIndex++;
					para [i] = CreateTypeReference (args [i], typeAttributes, ref typeIndex);
				}
				return interningProvider.Intern (new ParameterizedTypeReference (baseType, para));
			}
			if (type.IsGenericParameter) {
				return TypeParameterReference.Create (type.DeclaringMethod != null ? SymbolKind.Method : SymbolKind.TypeDefinition, type.GenericParameterPosition);
			}
			if (type.IsNested) {
				ITypeReference typeRef = CreateTypeReference (type.DeclaringType, typeAttributes, ref typeIndex);
				int partTypeParameterCount;
				string namepart = ReflectionHelper.SplitTypeParameterCountFromReflectionName (type.Name, out partTypeParameterCount);
				namepart = interningProvider.Intern (namepart);
				return interningProvider.Intern (new NestedTypeReference (typeRef, namepart, partTypeParameterCount));
			}

			string ns = interningProvider.Intern (type.Namespace ?? string.Empty);
			string name = type.Name;
			if (name == null)
				throw new InvalidOperationException ("type.Name returned null. Type: " + type);

			if (name == "Object" && ns == "System" && HasDynamicAttribute (typeAttributes, typeIndex)) {
				return SpecialType.Dynamic;
			}
			int typeParameterCount;
			name = ReflectionHelper.SplitTypeParameterCountFromReflectionName (name, out typeParameterCount);
			name = interningProvider.Intern (name);
			if (currentAssembly != null) {
				IUnresolvedTypeDefinition c = currentAssembly.GetTypeDefinition (ns, name, typeParameterCount);
				if (c != null)
					return c;
			}
			return interningProvider.Intern (new GetClassTypeReference (GetAssemblyReference (type.Assembly), ns, name, typeParameterCount));
		}

		IAssemblyReference GetAssemblyReference(Assembly scope)
		{
			if (scope == null || scope == currentAssemblyDefinition)
				return DefaultAssemblyReference.CurrentAssembly;
			return interningProvider.Intern (new DefaultAssemblyReference (scope.FullName));
		}

		static bool HasDynamicAttribute(IEnumerable<CustomAttributeData> attributeProvider, int typeIndex)
		{
			if (attributeProvider == null)
				return false;
			foreach (var a in attributeProvider) {
				var type = a.AttributeType;
				if (type.Name == "DynamicAttribute" && type.Namespace == "System.Runtime.CompilerServices") {
					if (a.ConstructorArguments.Count == 1) {
						var values = a.ConstructorArguments[0].Value as CustomAttributeTypedArgument[];
						if (values != null && typeIndex < values.Length && values[typeIndex].Value is bool) {
							return (bool)values [typeIndex].Value;
						}
					}
					return true;
				}
			}
			return false;
		}
		#endregion

		#region Read Attributes
		#region Assembly Attributes
		static readonly ITypeReference assemblyVersionAttributeTypeRef = typeof(System.Reflection.AssemblyVersionAttribute).ToTypeReference();

		void AddAttributes(Assembly assembly, IList<IUnresolvedAttribute> outputList)
		{
			AddCustomAttributes(assembly.CustomAttributes, outputList);
			AddSecurityAttributes(CustomAttributeData.__GetDeclarativeSecurity (assembly), outputList);

			// AssemblyVersionAttribute
			if (assembly.GetName ().Version != null) {
				var assemblyVersion = new DefaultUnresolvedAttribute(assemblyVersionAttributeTypeRef, new[] { KnownTypeReference.String });
				assemblyVersion.PositionalArguments.Add(CreateSimpleConstantValue(KnownTypeReference.String, assembly.GetName ().Version.ToString()));
				outputList.Add(interningProvider.Intern(assemblyVersion));
			}
		}

		IConstantValue CreateSimpleConstantValue(ITypeReference type, object value)
		{
			return interningProvider.Intern(new SimpleConstantValue(type, interningProvider.InternValue(value)));
		}
		#endregion

		#region Module Attributes
		void AddAttributes(Module module, IList<IUnresolvedAttribute> outputList)
		{
			AddCustomAttributes(module.CustomAttributes, outputList);
		}
		#endregion

		#region Parameter Attributes
		static readonly IUnresolvedAttribute inAttribute = new DefaultUnresolvedAttribute(typeof(InAttribute).ToTypeReference());
		static readonly IUnresolvedAttribute outAttribute = new DefaultUnresolvedAttribute(typeof(OutAttribute).ToTypeReference());

		void AddAttributes(ParameterInfo parameter, DefaultUnresolvedParameter targetParameter)
		{
			if (!targetParameter.IsOut) {
				if (parameter.IsIn)
					targetParameter.Attributes.Add(inAttribute);
				if (parameter.IsOut)
					targetParameter.Attributes.Add(outAttribute);
			}
			AddCustomAttributes(parameter.CustomAttributes, targetParameter.Attributes);

			FieldMarshal marshalInfo;
			if (parameter.__TryGetFieldMarshal (out marshalInfo)) {
				targetParameter.Attributes.Add(ConvertMarshalInfo(marshalInfo));
			}
		}
		#endregion

		#region Method Attributes
		static readonly ITypeReference dllImportAttributeTypeRef = typeof(DllImportAttribute).ToTypeReference();
		static readonly SimpleConstantValue trueValue = new SimpleConstantValue(KnownTypeReference.Boolean, true);
		static readonly SimpleConstantValue falseValue = new SimpleConstantValue(KnownTypeReference.Boolean, false);
		static readonly ITypeReference callingConventionTypeRef = typeof(CallingConvention).ToTypeReference();
		static readonly IUnresolvedAttribute preserveSigAttribute = new DefaultUnresolvedAttribute(typeof(PreserveSigAttribute).ToTypeReference());
		static readonly ITypeReference methodImplAttributeTypeRef = typeof(MethodImplAttribute).ToTypeReference();
		static readonly ITypeReference methodImplOptionsTypeRef = typeof(MethodImplOptions).ToTypeReference();

		static bool HasAnyAttributes(MethodInfo methodDefinition)
		{
			if (methodDefinition.Attributes.HasFlag (MethodAttributes.PinvokeImpl))
				return true;

			if (methodDefinition.MethodImplementationFlags.HasFlag (MethodImplAttributes.CodeTypeMask))
				return true;
			if (methodDefinition.ReturnParameter.Attributes.HasFlag (ParameterAttributes.HasFieldMarshal))
				return true;
			return methodDefinition.CustomAttributes.Any ();
		}

		static bool HasAnyAttributes(ConstructorInfo methodDefinition)
		{
			if (methodDefinition.Attributes.HasFlag (MethodAttributes.PinvokeImpl))
				return true;

			if (methodDefinition.MethodImplementationFlags.HasFlag (MethodImplAttributes.CodeTypeMask))
				return true;
			return methodDefinition.CustomAttributes.Any ();
		}

		void AddAttributes(MethodInfo methodDefinition, IList<IUnresolvedAttribute> attributes, ICollection<IUnresolvedAttribute> returnTypeAttributes)
		{
			var implAttributes = methodDefinition.MethodImplementationFlags;

			#region DllImportAttribute
			if (methodDefinition.Attributes.HasFlag (MethodAttributes.PinvokeImpl)) {

				ImplMapFlags flags;
				string importName;
				string importScope;
				if (methodDefinition.__TryGetImplMap(out flags, out importName, out importScope)) {
					var dllImport = new DefaultUnresolvedAttribute(dllImportAttributeTypeRef, new[] { KnownTypeReference.String });
					dllImport.PositionalArguments.Add(CreateSimpleConstantValue(KnownTypeReference.String, importScope));

					
					if (flags.HasFlag (ImplMapFlags.BestFitOff))
						dllImport.AddNamedFieldArgument("BestFitMapping", falseValue);
					if (flags.HasFlag (ImplMapFlags.BestFitOn))
						dllImport.AddNamedFieldArgument("BestFitMapping", trueValue);

					CallingConvention callingConvention;
					switch (flags & ImplMapFlags.CallConvMask) {
						case (ImplMapFlags)0:
							Debug.WriteLine ("P/Invoke calling convention not set on:" + methodDefinition.Name);
							callingConvention = CallingConvention.StdCall;
							break;
						case ImplMapFlags.CallConvCdecl:
							callingConvention = CallingConvention.Cdecl;
							break;
						case ImplMapFlags.CallConvFastcall:
							callingConvention = CallingConvention.FastCall;
							break;
						case ImplMapFlags.CallConvStdcall:
							callingConvention = CallingConvention.StdCall;
							break;
						case ImplMapFlags.CallConvThiscall:
							callingConvention = CallingConvention.ThisCall;
							break;
						case ImplMapFlags.CallConvWinapi:
							callingConvention = CallingConvention.Winapi;
							break;
						default:
							throw new NotSupportedException("unknown calling convention");
					}
					if (!flags.HasFlag (ImplMapFlags.CallConvWinapi))
						dllImport.AddNamedFieldArgument("CallingConvention", CreateSimpleConstantValue(callingConventionTypeRef, (int)callingConvention));

					CharSet charSet = CharSet.None;
					switch (flags & ImplMapFlags.CharSetMask) {
						case ImplMapFlags.CharSetAnsi:
							charSet = CharSet.Ansi;
							break;
						case ImplMapFlags.CharSetAuto:
							charSet = CharSet.Auto;
							break;
						case ImplMapFlags.CharSetUnicode:
							charSet = CharSet.Unicode;
							break;
					}
					if (charSet != CharSet.None)
						dllImport.AddNamedFieldArgument("CharSet", CreateSimpleConstantValue(charSetTypeRef, (int)charSet));

					if (!string.IsNullOrEmpty(importName) && importName != methodDefinition.Name)
						dllImport.AddNamedFieldArgument("EntryPoint", CreateSimpleConstantValue(KnownTypeReference.String, importName));

					if (flags.HasFlag (ImplMapFlags.NoMangle))
						dllImport.AddNamedFieldArgument("ExactSpelling", trueValue);

					if ((implAttributes & MethodImplAttributes.PreserveSig) == MethodImplAttributes.PreserveSig)
						implAttributes &= ~MethodImplAttributes.PreserveSig;
					else
						dllImport.AddNamedFieldArgument("PreserveSig", falseValue);

					if (flags.HasFlag (ImplMapFlags.SupportsLastError))
						dllImport.AddNamedFieldArgument("SetLastError", trueValue);

					if (flags.HasFlag (ImplMapFlags.CharMapErrorOff))
						dllImport.AddNamedFieldArgument("ThrowOnUnmappableChar", falseValue);
					if (flags.HasFlag (ImplMapFlags.CharMapErrorOn))
						dllImport.AddNamedFieldArgument("ThrowOnUnmappableChar", trueValue);

					attributes.Add(interningProvider.Intern(dllImport));
				}
			}
			#endregion

			#region PreserveSigAttribute
			if (implAttributes == MethodImplAttributes.PreserveSig) {
				attributes.Add(preserveSigAttribute);
				implAttributes = (MethodImplAttributes)0;
			}
			#endregion

			#region MethodImplAttribute
			if (implAttributes != MethodImplAttributes.IL) {
				var methodImpl = new DefaultUnresolvedAttribute(methodImplAttributeTypeRef, new[] { methodImplOptionsTypeRef });
				methodImpl.PositionalArguments.Add(CreateSimpleConstantValue(methodImplOptionsTypeRef, (int)implAttributes));
				attributes.Add(interningProvider.Intern(methodImpl));
			}
			#endregion

			var customAttributes = methodDefinition.CustomAttributes;
			AddCustomAttributes (customAttributes, attributes);

			if (methodDefinition.Attributes.HasFlag (MethodAttributes.HasSecurity)) {
				AddSecurityAttributes(CustomAttributeData.__GetDeclarativeSecurity (methodDefinition), attributes);
			}

			FieldMarshal marshalInfo;
			if (methodDefinition.ReturnParameter.__TryGetFieldMarshal (out marshalInfo)) {
				returnTypeAttributes.Add(ConvertMarshalInfo(marshalInfo));
			}
// TODO: Not needed in ikvm - maybe a work around for a cecil bug ?
//			AddCustomAttributes(methodDefinition.ReturnType.CustomAttributes, returnTypeAttributes);
		}

		void AddAttributes(ConstructorInfo methodDefinition, IList<IUnresolvedAttribute> attributes, IList<IUnresolvedAttribute> returnTypeAttributes)
		{
			var implAttributes = methodDefinition.MethodImplementationFlags;

			#region PreserveSigAttribute
			if (implAttributes == MethodImplAttributes.PreserveSig) {
				attributes.Add(preserveSigAttribute);
				implAttributes = 0;
			}
			#endregion

			#region MethodImplAttribute
			if (implAttributes != MethodImplAttributes.IL) {
				var methodImpl = new DefaultUnresolvedAttribute(methodImplAttributeTypeRef, new[] { methodImplOptionsTypeRef });
				methodImpl.PositionalArguments.Add(CreateSimpleConstantValue(methodImplOptionsTypeRef, (int)implAttributes));
				attributes.Add(interningProvider.Intern(methodImpl));
			}
			#endregion

			AddCustomAttributes(methodDefinition.CustomAttributes, attributes);

			if (methodDefinition.Attributes.HasFlag (MethodAttributes.HasSecurity)) {
				AddSecurityAttributes(CustomAttributeData.__GetDeclarativeSecurity (methodDefinition), attributes);
			}
		}
		#endregion

		#region Type Attributes
		static readonly DefaultUnresolvedAttribute serializableAttribute = new DefaultUnresolvedAttribute(typeof(SerializableAttribute).ToTypeReference());
		static readonly DefaultUnresolvedAttribute comImportAttribute = new DefaultUnresolvedAttribute(typeof(ComImportAttribute).ToTypeReference());
		static readonly ITypeReference structLayoutAttributeTypeRef = typeof(StructLayoutAttribute).ToTypeReference();
		static readonly ITypeReference layoutKindTypeRef = typeof(LayoutKind).ToTypeReference();
		static readonly ITypeReference charSetTypeRef = typeof(CharSet).ToTypeReference();

		void AddAttributes(IKVM.Reflection.Type typeDefinition, IUnresolvedTypeDefinition targetEntity)
		{
			// SerializableAttribute
			if (typeDefinition.IsSerializable)
				targetEntity.Attributes.Add(serializableAttribute);

			// ComImportAttribute
			if (typeDefinition.IsImport)
				targetEntity.Attributes.Add(comImportAttribute);

			#region StructLayoutAttribute
			LayoutKind layoutKind = LayoutKind.Auto;
			switch (typeDefinition.Attributes & TypeAttributes.LayoutMask) {
				case TypeAttributes.SequentialLayout:
				layoutKind = LayoutKind.Sequential;
				break;
				case TypeAttributes.ExplicitLayout:
				layoutKind = LayoutKind.Explicit;
				break;
			}
			CharSet charSet = CharSet.None;
			switch (typeDefinition.Attributes & TypeAttributes.StringFormatMask) {
				case TypeAttributes.AnsiClass:
				charSet = CharSet.Ansi;
				break;
				case TypeAttributes.AutoClass:
				charSet = CharSet.Auto;
				break;
				case TypeAttributes.UnicodeClass:
				charSet = CharSet.Unicode;
				break;
			}


			int packingSize;
			int typeSize;
			if (typeDefinition.__GetLayout (out packingSize, out typeSize)) {
				LayoutKind defaultLayoutKind = (typeDefinition.IsValueType && !typeDefinition.IsEnum) ? LayoutKind.Sequential: LayoutKind.Auto;
				if (layoutKind != defaultLayoutKind || charSet != CharSet.Ansi || packingSize > 0 || typeSize > 0) {
					var structLayout = new DefaultUnresolvedAttribute(structLayoutAttributeTypeRef, new[] { layoutKindTypeRef });
					structLayout.PositionalArguments.Add(CreateSimpleConstantValue(layoutKindTypeRef, (int)layoutKind));
					if (charSet != CharSet.Ansi) {
						structLayout.AddNamedFieldArgument("CharSet", CreateSimpleConstantValue(charSetTypeRef, (int)charSet));
					}
					if (packingSize > 0) {
						structLayout.AddNamedFieldArgument("Pack", CreateSimpleConstantValue(KnownTypeReference.Int32, packingSize));
					}
					if (typeSize > 0) {
						structLayout.AddNamedFieldArgument("Size", CreateSimpleConstantValue(KnownTypeReference.Int32, typeSize));
					}
					targetEntity.Attributes.Add(interningProvider.Intern(structLayout));
				}
			}
			#endregion

			AddCustomAttributes(typeDefinition.CustomAttributes, targetEntity.Attributes);

			if (typeDefinition.Attributes.HasFlag (TypeAttributes.HasSecurity)) {
				AddSecurityAttributes(CustomAttributeData.__GetDeclarativeSecurity(typeDefinition), targetEntity.Attributes);
			}
		}
		#endregion

		#region Field Attributes
		static readonly ITypeReference fieldOffsetAttributeTypeRef = typeof(FieldOffsetAttribute).ToTypeReference();
		static readonly IUnresolvedAttribute nonSerializedAttribute = new DefaultUnresolvedAttribute(typeof(NonSerializedAttribute).ToTypeReference());

		void AddAttributes(FieldInfo fieldDefinition, IUnresolvedEntity targetEntity)
		{
			// FieldOffsetAttribute
			int fOffset;
			if (fieldDefinition.__TryGetFieldOffset(out fOffset)) {
				var fieldOffset = new DefaultUnresolvedAttribute(fieldOffsetAttributeTypeRef, new[] { KnownTypeReference.Int32 });
				fieldOffset.PositionalArguments.Add(CreateSimpleConstantValue(KnownTypeReference.Int32, fOffset));
				targetEntity.Attributes.Add(interningProvider.Intern(fieldOffset));
			}

			// NonSerializedAttribute
			if (fieldDefinition.IsNotSerialized) {
				targetEntity.Attributes.Add(nonSerializedAttribute);
			}
			FieldMarshal marshal;
			if (fieldDefinition.__TryGetFieldMarshal (out marshal))
				targetEntity.Attributes.Add(ConvertMarshalInfo(marshal));

			AddCustomAttributes(fieldDefinition.CustomAttributes, targetEntity.Attributes);
		}
		#endregion

		#region Event Attributes
		void AddAttributes(EventInfo eventDefinition, IUnresolvedEntity targetEntity)
		{
			AddCustomAttributes(eventDefinition.CustomAttributes, targetEntity.Attributes);
		}
		#endregion

		#region Property Attributes
		void AddAttributes(PropertyInfo propertyDefinition, IUnresolvedEntity targetEntity)
		{
			AddCustomAttributes(propertyDefinition.CustomAttributes, targetEntity.Attributes);
		}
		#endregion

		#region MarshalAsAttribute (ConvertMarshalInfo)
		static readonly ITypeReference marshalAsAttributeTypeRef = typeof(MarshalAsAttribute).ToTypeReference();
		static readonly ITypeReference unmanagedTypeTypeRef = typeof(UnmanagedType).ToTypeReference();


		IUnresolvedAttribute ConvertMarshalInfo(FieldMarshal marshalInfo)
		{
			DefaultUnresolvedAttribute attr = new DefaultUnresolvedAttribute(marshalAsAttributeTypeRef, new[] { unmanagedTypeTypeRef });
			attr.PositionalArguments.Add(CreateSimpleConstantValue(unmanagedTypeTypeRef, (int)marshalInfo.UnmanagedType));


			if (marshalInfo.UnmanagedType ==UnmanagedType.ByValArray) {
				attr.AddNamedFieldArgument("SizeConst", CreateSimpleConstantValue(KnownTypeReference.Int32, (int)marshalInfo.SizeConst));
				if (marshalInfo.ArraySubType.HasValue)
					attr.AddNamedFieldArgument("ArraySubType", CreateSimpleConstantValue(unmanagedTypeTypeRef, (int)marshalInfo.ArraySubType.Value));
			}

			if (marshalInfo.UnmanagedType ==UnmanagedType.SafeArray) {
				attr.AddNamedFieldArgument("SafeArraySubType", CreateSimpleConstantValue(typeof(VarEnum).ToTypeReference(), (int)marshalInfo.SafeArraySubType));
			}

			if (marshalInfo.UnmanagedType == UnmanagedType.LPArray) {
				if (marshalInfo.ArraySubType != null)
					attr.AddNamedFieldArgument("ArraySubType", CreateSimpleConstantValue(unmanagedTypeTypeRef, (int)marshalInfo.ArraySubType));
				if (marshalInfo.SizeConst >= 0)
					attr.AddNamedFieldArgument("SizeConst", CreateSimpleConstantValue(KnownTypeReference.Int32, (int)marshalInfo.SizeConst));
				if (marshalInfo.SizeParamIndex >= 0)
					attr.AddNamedFieldArgument("SizeParamIndex", CreateSimpleConstantValue(KnownTypeReference.Int16, (short)marshalInfo.SizeParamIndex));
			}

			if (marshalInfo.UnmanagedType == UnmanagedType.CustomMarshaler) {
				attr.AddNamedFieldArgument("MarshalType", CreateSimpleConstantValue(KnownTypeReference.String, marshalInfo.MarshalTypeRef.FullName));
				if (!string.IsNullOrEmpty(marshalInfo.MarshalCookie))
					attr.AddNamedFieldArgument("MarshalCookie", CreateSimpleConstantValue(KnownTypeReference.String, marshalInfo.MarshalCookie));
			}

			if (marshalInfo.UnmanagedType == UnmanagedType.ByValTStr) {
				attr.AddNamedFieldArgument("SizeConst", CreateSimpleConstantValue(KnownTypeReference.Int32, (int)marshalInfo.SizeConst));
			}

			return InterningProvider.Intern(attr);
		}
		#endregion

		#region Custom Attributes (ReadAttribute)
		void AddCustomAttributes(IEnumerable<CustomAttributeData> attributes, ICollection<IUnresolvedAttribute> targetCollection)
		{
			foreach (var cecilAttribute in attributes) {
				var type = cecilAttribute.AttributeType;
				if (type.Namespace == "System.Runtime.CompilerServices") {
					if (type.Name == "DynamicAttribute" || type.Name == "ExtensionAttribute" || type.Name == "DecimalConstantAttribute")
						continue;
				} else if (type.Name == "ParamArrayAttribute" && type.Namespace == "System") {
					continue;
				}
				targetCollection.Add(ReadAttribute(cecilAttribute));
			}
		}

		[CLSCompliant(false)]
		public IUnresolvedAttribute ReadAttribute(CustomAttributeData attribute)
		{
			if (attribute == null)
				throw new ArgumentNullException("attribute");
			var ctor = attribute.Constructor;
			ITypeReference attributeType = ReadTypeReference(attribute.AttributeType);
			IList<ITypeReference> ctorParameterTypes = EmptyList<ITypeReference>.Instance;
			var parameters = ctor.GetParameters ();
			if (parameters.Length > 0) {
				ctorParameterTypes = new ITypeReference[parameters.Length];
				for (int i = 0; i < ctorParameterTypes.Count; i++) {
					ctorParameterTypes[i] = ReadTypeReference(parameters[i].ParameterType);
				}
				ctorParameterTypes = interningProvider.InternList(ctorParameterTypes);
			}
			return interningProvider.Intern(new CecilUnresolvedAttribute(attributeType, ctorParameterTypes, attribute.__GetBlob ()));
		}
		#endregion

		#region CecilUnresolvedAttribute
		static int GetBlobHashCode(byte[] blob)
		{
			unchecked {
				int hash = 0;
				foreach (byte b in blob) {
					hash *= 257;
					hash += b;
				}
				return hash;
			}
		}

		static bool BlobEquals(byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
				return false;
			for (int i = 0; i < a.Length; i++) {
				if (a[i] != b[i])
					return false;
			}
			return true;
		}

		[Serializable, FastSerializerVersion(ikvmLoaderVersion)]
		sealed class CecilUnresolvedAttribute : IUnresolvedAttribute, ISupportsInterning
		{
			internal readonly ITypeReference attributeType;
			internal readonly IList<ITypeReference> ctorParameterTypes;
			internal readonly byte[] blob;

			public CecilUnresolvedAttribute(ITypeReference attributeType, IList<ITypeReference> ctorParameterTypes, byte[] blob)
			{
				Debug.Assert(attributeType != null);
				Debug.Assert(ctorParameterTypes != null);
				Debug.Assert(blob != null);
				this.attributeType = attributeType;
				this.ctorParameterTypes = ctorParameterTypes;
				this.blob = blob;
			}

			DomRegion IUnresolvedAttribute.Region {
				get { return DomRegion.Empty; }
			}

			IAttribute IUnresolvedAttribute.CreateResolvedAttribute(ITypeResolveContext context)
			{
				if (context.CurrentAssembly == null)
					throw new InvalidOperationException("Cannot resolve CecilUnresolvedAttribute without a parent assembly");
				return new CecilResolvedAttribute(context, this);
			}

			int ISupportsInterning.GetHashCodeForInterning()
			{
				return attributeType.GetHashCode() ^ ctorParameterTypes.GetHashCode() ^ GetBlobHashCode(blob);
			}

			bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
			{
				var o = other as CecilUnresolvedAttribute;
				return o != null && attributeType == o.attributeType && ctorParameterTypes == o.ctorParameterTypes
					&& BlobEquals(blob, o.blob);
			}
		}
		#endregion

		#region CecilResolvedAttribute
		sealed class CecilResolvedAttribute : IAttribute
		{
			readonly ITypeResolveContext context;
			readonly byte[] blob;
			readonly IList<ITypeReference> ctorParameterTypes;
			readonly IType attributeType;

			IMethod constructor;
			volatile bool constructorResolved;

			IList<ResolveResult> positionalArguments;
			IList<KeyValuePair<IMember, ResolveResult>> namedArguments;

			public CecilResolvedAttribute(ITypeResolveContext context, CecilUnresolvedAttribute unresolved)
			{
				this.context = context;
				this.blob = unresolved.blob;
				this.ctorParameterTypes = unresolved.ctorParameterTypes;
				this.attributeType = unresolved.attributeType.Resolve(context);
			}

			public CecilResolvedAttribute(ITypeResolveContext context, IType attributeType)
			{
				this.context = context;
				this.attributeType = attributeType;
				this.ctorParameterTypes = EmptyList<ITypeReference>.Instance;
			}

			public DomRegion Region {
				get { return DomRegion.Empty; }
			}

			public IType AttributeType {
				get { return attributeType; }
			}

			public IMethod Constructor {
				get {
					if (!constructorResolved) {
						constructor = ResolveConstructor();
						constructorResolved = true;
					}
					return constructor;
				}
			}

			IMethod ResolveConstructor()
			{
				var parameterTypes = ctorParameterTypes.Resolve(context);
				foreach (var ctor in attributeType.GetConstructors(m => m.Parameters.Count == parameterTypes.Count)) {
					bool ok = true;
					for (int i = 0; i < parameterTypes.Count; i++) {
						if (!ctor.Parameters[i].Type.Equals(parameterTypes[i])) {
							ok = false;
							break;
						}
					}
					if (ok)
						return ctor;
				}
				return null;
			}

			public IList<ResolveResult> PositionalArguments {
				get {
					var result = LazyInit.VolatileRead(ref positionalArguments);
					if (result != null) {
						return result;
					}
					DecodeBlob();
					return positionalArguments;
				}
			}

			public IList<KeyValuePair<IMember, ResolveResult>> NamedArguments {
				get {
					var result = LazyInit.VolatileRead(ref namedArguments);
					if (result != null) {
						return result;
					}
					DecodeBlob();
					return namedArguments;
				}
			}

			public override string ToString()
			{
				return "[" + attributeType + "(...)]";
			}

			void DecodeBlob()
			{
				var newPositionalArguments = new List<ResolveResult>();
				var newNamedArguments = new List<KeyValuePair<IMember, ResolveResult>>();
				DecodeBlob(newPositionalArguments, newNamedArguments);
				Interlocked.CompareExchange(ref positionalArguments, newPositionalArguments, null);
				Interlocked.CompareExchange(ref namedArguments, newNamedArguments, null);
			}

			void DecodeBlob(ICollection<ResolveResult> newPositionalArguments, ICollection<KeyValuePair<IMember, ResolveResult>> newNamedArguments)
			{
				if (blob == null)
					return;
				var reader = new BlobReader(blob, context.CurrentAssembly);
				if (reader.ReadUInt16() != 0x0001) {
					Debug.WriteLine("Unknown blob prolog");
					return;
				}
				foreach (var ctorParameter in ctorParameterTypes.Resolve(context)) {
					ResolveResult arg = reader.ReadFixedArg(ctorParameter);
					newPositionalArguments.Add(arg);
					if (arg.IsError) {
						// After a decoding error, we must stop decoding the blob because
						// we might have read too few bytes due to the error.
						// Just fill up the remaining arguments with ErrorResolveResult:
						while (newPositionalArguments.Count < ctorParameterTypes.Count)
							newPositionalArguments.Add(ErrorResolveResult.UnknownError);
						return;
					}
				}
				ushort numNamed = reader.ReadUInt16();
				for (int i = 0; i < numNamed; i++) {
					var namedArg = reader.ReadNamedArg(attributeType);
					if (namedArg.Key != null)
						newNamedArguments.Add(namedArg);
				}
			}
		}
		#endregion

		#region class BlobReader
		class BlobReader
		{
			byte[] buffer;
			int position;
			readonly IAssembly currentResolvedAssembly;

			public BlobReader(byte[] buffer, IAssembly currentResolvedAssembly)
			{
				if (buffer == null)
					throw new ArgumentNullException("buffer");
				this.buffer = buffer;
				this.currentResolvedAssembly = currentResolvedAssembly;
			}

			public byte ReadByte()
			{
				return buffer[position++];
			}

			public sbyte ReadSByte()
			{
				unchecked {
					return(sbyte) ReadByte();
				}
			}

			public byte[] ReadBytes(int length)
			{
				var bytes = new byte[length];
				Buffer.BlockCopy(buffer, position, bytes, 0, length);
				position += length;
				return bytes;
			}

			public ushort ReadUInt16()
			{
				unchecked {
					ushort value =(ushort)(buffer[position]
					                       |(buffer[position + 1] << 8));
					position += 2;
					return value;
				}
			}

			public short ReadInt16()
			{
				unchecked {
					return(short) ReadUInt16();
				}
			}

			public uint ReadUInt32()
			{
				unchecked {
					uint value =(uint)(buffer[position]
					                   |(buffer[position + 1] << 8)
					                   |(buffer[position + 2] << 16)
					                   |(buffer[position + 3] << 24));
					position += 4;
					return value;
				}
			}

			public int ReadInt32()
			{
				unchecked {
					return(int) ReadUInt32();
				}
			}

			public ulong ReadUInt64()
			{
				unchecked {
					uint low = ReadUInt32();
					uint high = ReadUInt32();

					return(((ulong) high) << 32) | low;
				}
			}

			public long ReadInt64()
			{
				unchecked {
					return(long) ReadUInt64();
				}
			}

			public uint ReadCompressedUInt32()
			{
				unchecked {
					byte first = ReadByte();
					if((first & 0x80) == 0)
						return first;

					if((first & 0x40) == 0)
						return((uint)(first & ~0x80) << 8)
							| ReadByte();

					return((uint)(first & ~0xc0) << 24)
						|(uint) ReadByte() << 16
							|(uint) ReadByte() << 8
							| ReadByte();
				}
			}

			public float ReadSingle()
			{
				unchecked {
					if(!BitConverter.IsLittleEndian) {
						var bytes = ReadBytes(4);
						Array.Reverse(bytes);
						return BitConverter.ToSingle(bytes, 0);
					}

					float value = BitConverter.ToSingle(buffer, position);
					position += 4;
					return value;
				}
			}

			public double ReadDouble()
			{
				unchecked {
					if(!BitConverter.IsLittleEndian) {
						var bytes = ReadBytes(8);
						Array.Reverse(bytes);
						return BitConverter.ToDouble(bytes, 0);
					}

					double value = BitConverter.ToDouble(buffer, position);
					position += 8;
					return value;
				}
			}

			public ResolveResult ReadFixedArg(IType argType)
			{
				if (argType.Kind == TypeKind.Array) {
					if (((ArrayType)argType).Dimensions != 1) {
						// Only single-dimensional arrays are supported
						return ErrorResolveResult.UnknownError;
					}
					IType elementType = ((ArrayType)argType).ElementType;
					uint numElem = ReadUInt32();
					if (numElem == 0xffffffff) {
						// null reference
						return new ConstantResolveResult(argType, null);
					} else {
						ResolveResult[] elements = new ResolveResult[numElem];
						for (int i = 0; i < elements.Length; i++) {
							elements[i] = ReadElem(elementType);
							// Stop decoding when encountering an error:
							if (elements[i].IsError)
								return ErrorResolveResult.UnknownError;
						}
						IType int32 = currentResolvedAssembly.Compilation.FindType(KnownTypeCode.Int32);
						ResolveResult[] sizeArgs = { new ConstantResolveResult(int32, elements.Length) };
						return new ArrayCreateResolveResult(argType, sizeArgs, elements);
					}
				} else {
					return ReadElem(argType);
				}
			}

			public ResolveResult ReadElem(IType elementType)
			{
				ITypeDefinition underlyingType;
				if (elementType.Kind == TypeKind.Enum) {
					underlyingType = elementType.GetDefinition ().EnumUnderlyingType.GetDefinition ();
				} else {
					underlyingType = elementType.GetDefinition ();
				}
				if (underlyingType == null)
					return ErrorResolveResult.UnknownError;
				KnownTypeCode typeCode = underlyingType.KnownTypeCode;
				if (typeCode == KnownTypeCode.Object) {
					// boxed value type
					IType boxedTyped = ReadCustomAttributeFieldOrPropType ();
					ResolveResult elem = ReadElem (boxedTyped);
					if (elem.IsCompileTimeConstant && elem.ConstantValue == null)
						return new ConstantResolveResult (elementType, null);
					return new ConversionResolveResult (elementType, elem, Conversion.BoxingConversion);
				}
				if (typeCode == KnownTypeCode.Type)
					return new TypeOfResolveResult (underlyingType, ReadType ());
				return new ConstantResolveResult (elementType, ReadElemValue (typeCode));
			}

			object ReadElemValue(KnownTypeCode typeCode)
			{
				switch (typeCode) {
					case KnownTypeCode.Boolean:
					return ReadByte() != 0;
					case KnownTypeCode.Char:
					return (char)ReadUInt16();
					case KnownTypeCode.SByte:
					return ReadSByte();
					case KnownTypeCode.Byte:
					return ReadByte();
					case KnownTypeCode.Int16:
					return ReadInt16();
					case KnownTypeCode.UInt16:
					return ReadUInt16();
					case KnownTypeCode.Int32:
					return ReadInt32();
					case KnownTypeCode.UInt32:
					return ReadUInt32();
					case KnownTypeCode.Int64:
					return ReadInt64();
					case KnownTypeCode.UInt64:
					return ReadUInt64();
					case KnownTypeCode.Single:
					return ReadSingle();
					case KnownTypeCode.Double:
					return ReadDouble();
					case KnownTypeCode.String:
					return ReadSerString();
					default:
					throw new NotSupportedException();
				}
			}

			public string ReadSerString ()
			{
				if (buffer [position] == 0xff) {
					position++;
					return null;
				}

				int length = (int) ReadCompressedUInt32();
				if (length == 0)
					return string.Empty;

				string @string = System.Text.Encoding.UTF8.GetString(
					buffer, position,
					buffer [position + length - 1] == 0 ? length - 1 : length);

				position += length;
				return @string;
			}

			public KeyValuePair<IMember, ResolveResult> ReadNamedArg(IType attributeType)
			{
				SymbolKind memberType;
				var b = ReadByte();
				switch (b) {
					case 0x53:
					memberType = SymbolKind.Field;
					break;
					case 0x54:
					memberType = SymbolKind.Property;
					break;
					default:
					throw new NotSupportedException(string.Format("Custom member type 0x{0:x} is not supported.", b));
				}
				IType type = ReadCustomAttributeFieldOrPropType();
				string name = ReadSerString();
				ResolveResult val = ReadFixedArg(type);
				IMember member = null;
				// Use last matching member, as GetMembers() returns members from base types first.
				foreach (IMember m in attributeType.GetMembers(m => m.SymbolKind == memberType && m.Name == name)) {
					if (m.ReturnType.Equals(type))
						member = m;
				}
				return new KeyValuePair<IMember, ResolveResult>(member, val);
			}

			IType ReadCustomAttributeFieldOrPropType()
			{
				ICompilation compilation = currentResolvedAssembly.Compilation;
				var b = ReadByte();
				switch (b) {
					case 0x02:
					return compilation.FindType(KnownTypeCode.Boolean);
					case 0x03:
					return compilation.FindType(KnownTypeCode.Char);
					case 0x04:
					return compilation.FindType(KnownTypeCode.SByte);
					case 0x05:
					return compilation.FindType(KnownTypeCode.Byte);
					case 0x06:
					return compilation.FindType(KnownTypeCode.Int16);
					case 0x07:
					return compilation.FindType(KnownTypeCode.UInt16);
					case 0x08:
					return compilation.FindType(KnownTypeCode.Int32);
					case 0x09:
					return compilation.FindType(KnownTypeCode.UInt32);
					case 0x0a:
					return compilation.FindType(KnownTypeCode.Int64);
					case 0x0b:
					return compilation.FindType(KnownTypeCode.UInt64);
					case 0x0c:
					return compilation.FindType(KnownTypeCode.Single);
					case 0x0d:
					return compilation.FindType(KnownTypeCode.Double);
					case 0x0e:
					return compilation.FindType(KnownTypeCode.String);
					case 0x1d:
					return new ArrayType(compilation, ReadCustomAttributeFieldOrPropType());
					case 0x50:
					return compilation.FindType(KnownTypeCode.Type);
					case 0x51: // boxed value type
					return compilation.FindType(KnownTypeCode.Object);
					case 0x55: // enum
					return ReadType();
					default:
					throw new NotSupportedException(string.Format("Custom attribute type 0x{0:x} is not supported.", b));
				}
			}

			IType ReadType()
			{
				string typeName = ReadSerString ();
				ITypeReference typeReference = ReflectionHelper.ParseReflectionName (typeName);
				IType typeInCurrentAssembly = typeReference.Resolve (new SimpleTypeResolveContext (currentResolvedAssembly));
				if (typeInCurrentAssembly.Kind != TypeKind.Unknown)
					return typeInCurrentAssembly;

				// look for the type in mscorlib
				ITypeDefinition systemObject = currentResolvedAssembly.Compilation.FindType (KnownTypeCode.Object).GetDefinition ();
				if (systemObject != null)
					return typeReference.Resolve (new SimpleTypeResolveContext (systemObject.ParentAssembly));
				// couldn't find corlib - return the unknown IType for the current assembly
				return typeInCurrentAssembly;
			}
		}
		#endregion

		#region Security Attributes
		static readonly ITypeReference securityActionTypeReference = typeof(System.Security.Permissions.SecurityAction).ToTypeReference();
		static readonly ITypeReference permissionSetAttributeTypeReference = typeof(System.Security.Permissions.PermissionSetAttribute).ToTypeReference();

//		/// <summary>
//		/// Reads a security declaration.
//		/// </summary>
//		[CLSCompliant(false)]
//		public IList<IUnresolvedAttribute> ReadSecurityDeclaration(SecurityDeclaration secDecl)
//		{
//			if (secDecl == null)
//				throw new ArgumentNullException("secDecl");
//			var result = new List<IUnresolvedAttribute>();
//			AddSecurityAttributes(secDecl, result);
//			return result;
//		}
//
//		void AddSecurityAttributes(Mono.Collections.Generic.Collection<SecurityDeclaration> securityDeclarations, IList<IUnresolvedAttribute> targetCollection)
//		{
//			foreach (var secDecl in securityDeclarations) {
//				AddSecurityAttributes(secDecl, targetCollection);
//			}
//		}

		void AddSecurityAttributes(IEnumerable<CustomAttributeData> securityAttributes, IList<IUnresolvedAttribute> targetCollection)
		{
			AddCustomAttributes (securityAttributes, targetCollection);
		}

		[Serializable, FastSerializerVersion(ikvmLoaderVersion)]
		sealed class UnresolvedSecurityDeclaration : ISupportsInterning
		{
			readonly IConstantValue securityAction;
			readonly byte[] blob;

			public UnresolvedSecurityDeclaration(IConstantValue securityAction, byte[] blob)
			{
				Debug.Assert(securityAction != null);
				Debug.Assert(blob != null);
				this.securityAction = securityAction;
				this.blob = blob;
			}

			public IList<IAttribute> Resolve(IAssembly currentAssembly)
			{
				// TODO: make this a per-assembly cache
//				CacheManager cache = currentAssembly.Compilation.CacheManager;
//				IList<IAttribute> result = (IList<IAttribute>)cache.GetShared(this);
//				if (result != null)
//					return result;

				var context = new SimpleTypeResolveContext(currentAssembly);
				var reader = new BlobReader(blob, currentAssembly);
				if (reader.ReadByte() != '.') {
					// should not use UnresolvedSecurityDeclaration for XML secdecls
					throw new InvalidOperationException();
				}
				var securityActionRR = securityAction.Resolve(context);
				uint attributeCount = reader.ReadCompressedUInt32();
				var attributes = new IAttribute[attributeCount];
				try {
					ReadSecurityBlob(reader, attributes, context, securityActionRR);
				} catch (NotSupportedException ex) {
					// ignore invalid blobs
					Debug.WriteLine(ex.ToString());
				}
				for (int i = 0; i < attributes.Length; i++) {
					if (attributes[i] == null)
						attributes[i] = new CecilResolvedAttribute(context, SpecialType.UnknownType);
				}
				return attributes;
//				return (IList<IAttribute>)cache.GetOrAddShared(this, attributes);
			}

			void ReadSecurityBlob(BlobReader reader, IAttribute[] attributes, ITypeResolveContext context, ResolveResult securityActionRR)
			{
				for (int i = 0; i < attributes.Length; i++) {
					string attributeTypeName = reader.ReadSerString();
					ITypeReference attributeTypeRef = ReflectionHelper.ParseReflectionName(attributeTypeName);
					IType attributeType = attributeTypeRef.Resolve(context);

					reader.ReadCompressedUInt32(); // ??
					// The specification seems to be incorrect here, so I'm using the logic from Cecil instead.
					uint numNamed = reader.ReadCompressedUInt32();

					var namedArgs = new List<KeyValuePair<IMember, ResolveResult>>((int)numNamed);
					for (uint j = 0; j < numNamed; j++) {
						var namedArg = reader.ReadNamedArg(attributeType);
						if (namedArg.Key != null)
							namedArgs.Add(namedArg);

					}
					attributes[i] = new DefaultAttribute(
						attributeType,
						positionalArguments: new ResolveResult[] { securityActionRR },
					namedArguments: namedArgs);
				}
			}

			int ISupportsInterning.GetHashCodeForInterning()
			{
				return securityAction.GetHashCode() ^ GetBlobHashCode(blob);
			}

			bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
			{
				var o = other as UnresolvedSecurityDeclaration;
				return o != null && securityAction == o.securityAction && BlobEquals(blob, o.blob);
			}
		}

		[Serializable, FastSerializerVersion(ikvmLoaderVersion)]
		sealed class UnresolvedSecurityAttribute : IUnresolvedAttribute, ISupportsInterning
		{
			readonly UnresolvedSecurityDeclaration secDecl;
			readonly int index;

			public UnresolvedSecurityAttribute(UnresolvedSecurityDeclaration secDecl, int index)
			{
				Debug.Assert(secDecl != null);
				this.secDecl = secDecl;
				this.index = index;
			}

			DomRegion IUnresolvedAttribute.Region {
				get { return DomRegion.Empty; }
			}

			IAttribute IUnresolvedAttribute.CreateResolvedAttribute(ITypeResolveContext context)
			{
				return secDecl.Resolve(context.CurrentAssembly)[index];
			}

			int ISupportsInterning.GetHashCodeForInterning()
			{
				return index ^ secDecl.GetHashCode();
			}

			bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
			{
				var attr = other as UnresolvedSecurityAttribute;
				return attr != null && index == attr.index && secDecl == attr.secDecl;
			}
		}
		#endregion
		#endregion

		#region Read Type Definition
		DefaultUnresolvedTypeDefinition CreateTopLevelTypeDefinition(IKVM.Reflection.Type typeDefinition)
		{
			var td = new DefaultUnresolvedTypeDefinition(typeDefinition.Namespace ?? "", ReflectionHelper.SplitTypeParameterCountFromReflectionName (typeDefinition.Name));
			if (typeDefinition.IsGenericTypeDefinition)
				InitTypeParameters(typeDefinition, td.TypeParameters);
			return td;
		}

		static void InitTypeParameters(IKVM.Reflection.Type typeDefinition, ICollection<IUnresolvedTypeParameter> typeParameters)
		{
			// Type parameters are initialized within the constructor so that the class can be put into the type storage
			// before the rest of the initialization runs - this allows it to be available for early binding as soon as possible.
			var genericArguments = typeDefinition.GetGenericArguments ();
			for (int i = 0; i < genericArguments.Length; i++) {
				if (genericArguments[i].GenericParameterPosition != i)
					throw new InvalidOperationException("g.GenericParameterPosition != i");
				typeParameters.Add(new DefaultUnresolvedTypeParameter(
					SymbolKind.TypeDefinition, i, genericArguments [i].Name));
			}
		}

		void InitTypeParameterConstraints(IKVM.Reflection.Type typeDefinition, IList<IUnresolvedTypeParameter> typeParameters)
		{
			var args = typeDefinition.GetGenericArguments ();
			for (int i = 0; i < typeParameters.Count; i++) {
				var tp = (DefaultUnresolvedTypeParameter)typeParameters[i];
				AddConstraints(tp, args[i]);
				tp.ApplyInterningProvider(interningProvider);
			}
		}

		void InitTypeDefinition(IKVM.Reflection.Type typeDefinition, DefaultUnresolvedTypeDefinition td)
		{
			td.Kind = GetTypeKind(typeDefinition);
			InitTypeModifiers(typeDefinition, td);
			InitTypeParameterConstraints(typeDefinition, td.TypeParameters);

			// nested types can be initialized only after generic parameters were created
			InitNestedTypes(typeDefinition, td, td.NestedTypes);
			AddAttributes(typeDefinition, td);
			td.HasExtensionMethods = HasExtensionAttribute(typeDefinition);

			InitBaseTypes(typeDefinition, td.BaseTypes);

			td.AddDefaultConstructorIfRequired = (td.Kind == TypeKind.Struct || td.Kind == TypeKind.Enum);
			InitMembers(typeDefinition, td, td.Members);
			td.ApplyInterningProvider(interningProvider);
			td.Freeze();
			RegisterCecilObject(td, typeDefinition);
		}

		void InitBaseTypes(IKVM.Reflection.Type typeDefinition, ICollection<ITypeReference> baseTypes)
		{
			// set base classes
			if (typeDefinition.IsEnum) {
				foreach (var enumField in typeDefinition.GetFields (bindingFlags)) {
					if (!enumField.IsStatic) {
						baseTypes.Add(ReadTypeReference(enumField.FieldType));
						break;
					}
				}
			} else {
				if (typeDefinition.BaseType != null) {
					baseTypes.Add(ReadTypeReference(typeDefinition.BaseType));
				}
				foreach (var iface in typeDefinition.GetInterfaces ()) {
					baseTypes.Add(ReadTypeReference(iface));
				}
			}
		}

		void InitNestedTypes(IKVM.Reflection.Type typeDefinition, IUnresolvedTypeDefinition declaringTypeDefinition, ICollection<IUnresolvedTypeDefinition> nestedTypes)
		{
			foreach (var nestedTypeDef in typeDefinition.GetNestedTypes (bindingFlags)) {
				if (IncludeInternalMembers
					|| nestedTypeDef.IsNestedPublic
				    || nestedTypeDef.IsNestedFamily
				    || nestedTypeDef.IsNestedFamORAssem)
				{
					string name = nestedTypeDef.Name;
					int pos = name.LastIndexOf('/');
					if (pos > 0)
						name = name.Substring(pos + 1);
					name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(name);
					var nestedType = new DefaultUnresolvedTypeDefinition(declaringTypeDefinition, name);
					InitTypeParameters(nestedTypeDef, nestedType.TypeParameters);
					nestedTypes.Add(nestedType);
					InitTypeDefinition(nestedTypeDef, nestedType);
				}
			}
		}

		static TypeKind GetTypeKind(IKVM.Reflection.Type typeDefinition)
		{
			// set classtype
			if (typeDefinition.IsInterface)
				return TypeKind.Interface;
			if (typeDefinition.IsEnum)
				return TypeKind.Enum;
			if (typeDefinition.IsValueType)
				return TypeKind.Struct;
			if (IsDelegate (typeDefinition))
				return TypeKind.Delegate;
			if (IsModule (typeDefinition))
				return TypeKind.Module;
			return TypeKind.Class;
		}

		static void InitTypeModifiers(IKVM.Reflection.Type typeDefinition, AbstractUnresolvedEntity td)
		{
			td.IsSealed = typeDefinition.IsSealed;
			td.IsAbstract = typeDefinition.IsAbstract;
			switch (typeDefinition.Attributes & TypeAttributes.VisibilityMask) {
				case TypeAttributes.NotPublic:
				case TypeAttributes.NestedAssembly:
					td.Accessibility = Accessibility.Internal;
					break;
				case TypeAttributes.Public:
				case TypeAttributes.NestedPublic:
					td.Accessibility = Accessibility.Public;
					break;
				case TypeAttributes.NestedPrivate:
					td.Accessibility = Accessibility.Private;
					break;
				case TypeAttributes.NestedFamily:
					td.Accessibility = Accessibility.Protected;
					break;
				case TypeAttributes.NestedFamANDAssem:
					td.Accessibility = Accessibility.ProtectedAndInternal;
					break;
				case TypeAttributes.NestedFamORAssem:
					td.Accessibility = Accessibility.ProtectedOrInternal;
					break;
			}
		}

		static bool IsDelegate(IKVM.Reflection.Type type)
		{
			if (type.BaseType != null && type.BaseType.Namespace == "System") {
				if (type.BaseType.Name == "MulticastDelegate")
					return true;
				if (type.BaseType.Name == "Delegate" && type.Name != "MulticastDelegate")
					return true;
			}
			return false;
		}

		static bool IsModule(IKVM.Reflection.Type type)
		{
			foreach (var att in type.CustomAttributes) {
				if (att.AttributeType.FullName == "Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute"
				    || att.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGlobalScopeAttribute")
				{
					return true;
				}
			}
			return false;
		}

		static readonly BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;

		void InitMembers(IKVM.Reflection.Type typeDefinition, IUnresolvedTypeDefinition td, IList<IUnresolvedMember> members)
		{
			foreach (var method in typeDefinition.GetMethods (bindingFlags)) {
				if (IsVisible(method.Attributes) && !IsAccessor(method)) {
					SymbolKind type = SymbolKind.Method;
					if (method.IsSpecialName) {
						if (method.Name.StartsWith("op_", StringComparison.Ordinal))
							type = SymbolKind.Operator;
					}
					members.Add(ReadMethod(method, td, type));
				}
			}

			foreach (var method in typeDefinition.GetConstructors (bindingFlags)) {
				if (IsVisible(method.Attributes)) {
					SymbolKind type = SymbolKind.Constructor;
					members.Add(ReadConstructor(method, td, type));
				}
			}


			foreach (var field in typeDefinition.GetFields (bindingFlags)) {
				if (IsVisible(field.Attributes) && !field.IsSpecialName) {
					members.Add(ReadField(field, td));
				}
			}

			string defaultMemberName = null;
			var defaultMemberAttribute = typeDefinition.CustomAttributes.FirstOrDefault(
				a => a.AttributeType.FullName == typeof(System.Reflection.DefaultMemberAttribute).FullName);
			if (defaultMemberAttribute != null && defaultMemberAttribute.ConstructorArguments.Count == 1) {
				defaultMemberName = defaultMemberAttribute.ConstructorArguments[0].Value as string;
			}

			foreach (var property in typeDefinition.GetProperties (bindingFlags)) {
				bool getterVisible = property.GetMethod != null && IsVisible(property.GetMethod.Attributes);
				bool setterVisible = property.SetMethod != null && IsVisible(property.SetMethod.Attributes);
				if (getterVisible || setterVisible) {
					SymbolKind type = SymbolKind.Property;
					if (property.GetIndexParameters () != null) {
						// Try to detect indexer:
						if (property.Name == defaultMemberName) {
							type = SymbolKind.Indexer; // normal indexer
						}
						// TODO: HasOverrides ?
						else if (property.Name.EndsWith(".Item", StringComparison.Ordinal) /*&& (property.GetMethod ?? property.SetMethod).HasOverrides*/) {
							// explicit interface implementation of indexer
							type = SymbolKind.Indexer;
							// We can't really tell parameterized properties and indexers apart in this case without
							// resolving the interface, so we rely on the "Item" naming convention instead.
						}
					}
					members.Add(ReadProperty(property, td, type));
				}
			}

			foreach (var ev in typeDefinition.GetEvents (bindingFlags)) {
				if (ev.AddMethod != null && IsVisible(ev.AddMethod.Attributes)) {
					members.Add(ReadEvent(ev, td));
				}
			}
		}

		static bool IsAccessor(MethodInfo methodInfo)
		{
			if (!methodInfo.IsSpecialName)
				return false;

			var name = methodInfo.Name;
			return 
				name.StartsWith("get_", StringComparison.Ordinal) ||
				name.StartsWith("set_", StringComparison.Ordinal) ||
				name.StartsWith("add_", StringComparison.Ordinal) ||
				name.StartsWith("remove_", StringComparison.Ordinal) ||
				name.StartsWith("raise_", StringComparison.Ordinal);
		}
		#endregion

		#region Read Method
		[CLSCompliant(false)]
		public IUnresolvedMethod ReadMethod(MethodInfo method, IUnresolvedTypeDefinition parentType, SymbolKind methodType = SymbolKind.Method)
		{
			return ReadMethod(method, parentType, methodType, null);
		}

		IUnresolvedMethod ReadMethod(MethodInfo method, IUnresolvedTypeDefinition parentType, SymbolKind methodType, IUnresolvedMember accessorOwner)
		{
			if (method == null)
				return null;
			var m = new DefaultUnresolvedMethod(parentType, method.Name);
			m.SymbolKind = methodType;
			m.AccessorOwner = accessorOwner;
			m.HasBody = method.GetMethodBody () != null;
			var genericArguments = method.GetGenericArguments ();
			if (genericArguments != null) {
				for (int i = 0; i < genericArguments.Length; i++) {
					if (genericArguments[i].GenericParameterPosition != i)
						throw new InvalidOperationException("g.Position != i");
					m.TypeParameters.Add(new DefaultUnresolvedTypeParameter(
						SymbolKind.Method, i, genericArguments[i].Name));
				}
				for (int i = 0; i < genericArguments.Length; i++) {
					var tp = (DefaultUnresolvedTypeParameter)m.TypeParameters[i];
					AddConstraints(tp, genericArguments[i]);
					tp.ApplyInterningProvider(interningProvider);
				}
			}

			m.ReturnType = ReadTypeReference(method.ReturnType, typeAttributes: method.ReturnParameter.CustomAttributes);

			if (HasAnyAttributes(method))
				AddAttributes(method, m.Attributes, m.ReturnTypeAttributes);
			TranslateModifiers(method, m);

			foreach (var p in method.GetParameters ()) {
				m.Parameters.Add(ReadParameter(p));
			}

			// mark as extension method if the attribute is set
			if (method.IsStatic && HasExtensionAttribute(method)) {
				m.IsExtensionMethod = true;
			}

			int lastDot = method.Name.LastIndexOf('.');
			if (lastDot >= 0 /*&& method.HasOverrides*/) {
				// To be consistent with the parser-initialized type system, shorten the method name:
				m.Name = method.Name.Substring(lastDot + 1);
				m.IsExplicitInterfaceImplementation = true;
				
				foreach (var or in method.__GetMethodImpls ()) {
					m.ExplicitInterfaceImplementations.Add(new DefaultMemberReference(
						accessorOwner != null ? SymbolKind.Accessor : SymbolKind.Method,
						ReadTypeReference(or.DeclaringType),
						or.Name, or.GetGenericArguments ().Length, m.Parameters.Select(p => p.Type).ToList()));
				}
			}

			FinishReadMember(m, method);
			return m;
		}

		static bool HasExtensionAttribute(MemberInfo provider)
		{
			foreach (var attr in provider.CustomAttributes) {
				if (attr.AttributeType.Name == "ExtensionAttribute" && attr.AttributeType.Namespace == "System.Runtime.CompilerServices")
					return true;
			}
			return false;
		}

		bool IsVisible(MethodAttributes att)
		{
			att &= MethodAttributes.MemberAccessMask;
			return IncludeInternalMembers
				|| att == MethodAttributes.Public
				|| att == MethodAttributes.Family
				|| att == MethodAttributes.FamORAssem;
		}

		static Accessibility GetAccessibility(MethodAttributes attr)
		{
			switch (attr & MethodAttributes.MemberAccessMask) {
				case MethodAttributes.Public:
				return Accessibility.Public;
				case MethodAttributes.FamANDAssem:
				return Accessibility.ProtectedAndInternal;
				case MethodAttributes.Assembly:
				return Accessibility.Internal;
				case MethodAttributes.Family:
				return Accessibility.Protected;
				case MethodAttributes.FamORAssem:
				return Accessibility.ProtectedOrInternal;
				default:
				return Accessibility.Private;
			}
		}

		void TranslateModifiers(MethodBase method, AbstractUnresolvedMember m)
		{
			if (m.DeclaringTypeDefinition.Kind == TypeKind.Interface) {
				// interface members don't have modifiers, but we want to handle them as "public abstract"
				m.Accessibility = Accessibility.Public;
				m.IsAbstract = true;
			} else {
				m.Accessibility = GetAccessibility(method.Attributes);
				if (method.IsAbstract) {
					m.IsAbstract = true;
					m.IsOverride = !method.Attributes.HasFlag (MethodAttributes.NewSlot);
				} else if (method.IsFinal) {
					if (!method.Attributes.HasFlag (MethodAttributes.NewSlot)) {
						m.IsSealed = true;
						m.IsOverride = true;
					}
				} else if (method.IsVirtual) {
					if (method.Attributes.HasFlag (MethodAttributes.NewSlot))
						m.IsVirtual = true;
					else
						m.IsOverride = true;
				}
				m.IsStatic = method.IsStatic;
			}
		}
		#endregion

		#region Read Constructor
		[CLSCompliant(false)]
		public IUnresolvedMethod ReadConstructor(ConstructorInfo method, IUnresolvedTypeDefinition parentType, SymbolKind methodType = SymbolKind.Method)
		{
			return ReadConstructor(method, parentType, methodType, null);
		}

		IUnresolvedMethod ReadConstructor(ConstructorInfo method, IUnresolvedTypeDefinition parentType, SymbolKind methodType, IUnresolvedMember accessorOwner)
		{
			if (method == null)
				return null;
			var m = new DefaultUnresolvedMethod(parentType, method.Name);
			m.SymbolKind = methodType;
			m.AccessorOwner = accessorOwner;
			m.HasBody = method.GetMethodBody () != null;
			var genericArguments = method.GetGenericArguments ();
			if (genericArguments != null) {
				for (int i = 0; i < genericArguments.Length; i++) {
					if (genericArguments[i].GenericParameterPosition != i)
						throw new InvalidOperationException("g.Position != i");
					m.TypeParameters.Add(new DefaultUnresolvedTypeParameter(
						SymbolKind.Method, i, genericArguments[i].Name));
				}
				for (int i = 0; i < genericArguments.Length; i++) {
					var tp = (DefaultUnresolvedTypeParameter)m.TypeParameters[i];
					AddConstraints(tp, genericArguments[i]);
					tp.ApplyInterningProvider(interningProvider);
				}
			}
			m.ReturnType = KnownTypeReference.Void;


			if (HasAnyAttributes(method))
				AddAttributes(method, m.Attributes, m.ReturnTypeAttributes);
			TranslateModifiers(method, m);

			foreach (var p in method.GetParameters ()) {
				m.Parameters.Add(ReadParameter(p));
			}

			FinishReadMember(m, method);
			return m;
		}

		#endregion

		#region Read Parameter
		[CLSCompliant(false)]
		public IUnresolvedParameter ReadParameter(ParameterInfo parameter)
		{
			if (parameter == null)
				throw new ArgumentNullException("parameter");
			var type = ReadTypeReference(parameter.ParameterType, typeAttributes: parameter.CustomAttributes);
			var p = new DefaultUnresolvedParameter(type, interningProvider.Intern(parameter.Name ?? "index"));

			if (parameter.ParameterType.IsByRef) {
				if (!parameter.IsIn && parameter.IsOut)
					p.IsOut = true;
				else
					p.IsRef = true;
			}
			AddAttributes(parameter, p);

			if (parameter.IsOptional) {
				p.DefaultValue = CreateSimpleConstantValue(type, parameter.RawDefaultValue);
			}

			if (parameter.ParameterType.IsArray) {
				foreach (var att in parameter.CustomAttributes) {
					if (att.AttributeType.FullName == typeof(ParamArrayAttribute).FullName) {
						p.IsParams = true;
						break;
					}
				}
			}

			return interningProvider.Intern(p);
		}
		#endregion

		#region Read Field
		bool IsVisible(FieldAttributes att)
		{
			att &= FieldAttributes.FieldAccessMask;
			return IncludeInternalMembers
				|| att == FieldAttributes.Public
					|| att == FieldAttributes.Family
					|| att == FieldAttributes.FamORAssem;
		}

		decimal? TryDecodeDecimalConstantAttribute(CustomAttributeData attribute)
		{
			if (attribute.ConstructorArguments.Count != 5)
				return null;

			var reader = new BlobReader(attribute.__GetBlob(), null);
			if (reader.ReadUInt16() != 0x0001) {
				Debug.WriteLine("Unknown blob prolog");
				return null;
			}

			// DecimalConstantAttribute has the arguments (byte scale, byte sign, uint hi, uint mid, uint low) or (byte scale, byte sign, int hi, int mid, int low)
			// Both of these invoke the Decimal constructor (int lo, int mid, int hi, bool isNegative, byte scale) with explicit argument conversions if required.
			var ctorArgs = new object[attribute.ConstructorArguments.Count];
			for (int i = 0; i < ctorArgs.Length; i++) {
				switch (attribute.ConstructorArguments[i].ArgumentType.FullName) {
					case "System.Byte":
					ctorArgs[i] = reader.ReadByte();
					break;
					case "System.Int32":
					ctorArgs[i] = reader.ReadInt32();
					break;
					case "System.UInt32":
					ctorArgs[i] = unchecked((int)reader.ReadUInt32());
					break;
					default:
					return null;
				}
			}

			if (!ctorArgs.Select(a => a.GetType()).SequenceEqual(new[] { typeof(byte), typeof(byte), typeof(int), typeof(int), typeof(int) }))
				return null;

			return new decimal((int)ctorArgs[4], (int)ctorArgs[3], (int)ctorArgs[2], (byte)ctorArgs[1] != 0, (byte)ctorArgs[0]);
		}

		[CLSCompliant(false)]
		public IUnresolvedField ReadField(FieldInfo field, IUnresolvedTypeDefinition parentType)
		{
			if (field == null)
				throw new ArgumentNullException("field");
			if (parentType == null)
				throw new ArgumentNullException("parentType");
			var f = new DefaultUnresolvedField(parentType, field.Name);
			f.Accessibility = GetAccessibility(field.Attributes);
			f.IsReadOnly = field.IsInitOnly;
			f.IsStatic = field.IsStatic;

			f.ReturnType = ReadTypeReference(field.FieldType, typeAttributes: field.CustomAttributes);

			if (field.Attributes.HasFlag (FieldAttributes.HasDefault)) {
				f.ConstantValue = CreateSimpleConstantValue(f.ReturnType, field.GetRawConstantValue ());
			}
			else {
				var decConstant = field.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.DecimalConstantAttribute");
				if (decConstant != null) {
					var constValue = TryDecodeDecimalConstantAttribute(decConstant);
					if (constValue != null)
						f.ConstantValue = CreateSimpleConstantValue(f.ReturnType, constValue);
				}
			}
			AddAttributes(field, f);

			if (field.GetRequiredCustomModifiers ().Any (mt => mt.FullName == typeof(IsVolatile).FullName)) {
				f.IsVolatile = true;
			}

			FinishReadMember(f, field);
			return f;
		}

		static Accessibility GetAccessibility(FieldAttributes attr)
		{
			switch (attr & FieldAttributes.FieldAccessMask) {
				case FieldAttributes.Public:
					return Accessibility.Public;
				case FieldAttributes.FamANDAssem:
					return Accessibility.ProtectedAndInternal;
				case FieldAttributes.Assembly:
					return Accessibility.Internal;
				case FieldAttributes.Family:
					return Accessibility.Protected;
				case FieldAttributes.FamORAssem:
					return Accessibility.ProtectedOrInternal;
				default:
					return Accessibility.Private;
			}
		}
		#endregion

		#region Type Parameter Constraints
		void AddConstraints(DefaultUnresolvedTypeParameter tp, IKVM.Reflection.Type g)
		{
			if (g.GenericParameterAttributes.HasFlag (GenericParameterAttributes.Contravariant)) {
				tp.Variance = VarianceModifier.Contravariant;
			} else if (g.GenericParameterAttributes.HasFlag (GenericParameterAttributes.Covariant)) {
				tp.Variance = VarianceModifier.Covariant;
			}

			tp.HasReferenceTypeConstraint = g.GenericParameterAttributes.HasFlag (GenericParameterAttributes.ReferenceTypeConstraint);
			tp.HasValueTypeConstraint = g.GenericParameterAttributes.HasFlag (GenericParameterAttributes.NotNullableValueTypeConstraint);
			tp.HasDefaultConstructorConstraint = g.GenericParameterAttributes.HasFlag (GenericParameterAttributes.DefaultConstructorConstraint);

			foreach (var constraint in g.GetGenericParameterConstraints ()) {
				tp.Constraints.Add(ReadTypeReference(constraint));
			}
		}
		#endregion

		#region Read Property

		Accessibility MergePropertyAccessibility (Accessibility left, Accessibility right)
		{
			if (left == Accessibility.Public || right == Accessibility.Public)
				return Accessibility.Public;

			if (left == Accessibility.ProtectedOrInternal || right == Accessibility.ProtectedOrInternal)
				return Accessibility.ProtectedOrInternal;

			if (left == Accessibility.Protected && right == Accessibility.Internal || 
			    left == Accessibility.Internal && right == Accessibility.Protected)
				return Accessibility.ProtectedOrInternal;

			if (left == Accessibility.Protected || right == Accessibility.Protected)
				return Accessibility.Protected;

			if (left == Accessibility.Internal || right == Accessibility.Internal)
				return Accessibility.Internal;

			if (left == Accessibility.ProtectedAndInternal || right == Accessibility.ProtectedAndInternal)
				return Accessibility.ProtectedAndInternal;

			return left;
		}

		[CLSCompliant(false)]
		public IUnresolvedProperty ReadProperty(PropertyInfo property, IUnresolvedTypeDefinition parentType, SymbolKind propertyType = SymbolKind.Property)
		{
			if (property == null)
				throw new ArgumentNullException("property");
			if (parentType == null)
				throw new ArgumentNullException("parentType");

			var p = new DefaultUnresolvedProperty(parentType, property.Name);
			p.SymbolKind = propertyType;
			TranslateModifiers(property.GetMethod ?? property.SetMethod, p);
			if (property.GetMethod != null && property.SetMethod != null)
				p.Accessibility = MergePropertyAccessibility (GetAccessibility (property.GetMethod.Attributes), GetAccessibility (property.SetMethod.Attributes));
			p.ReturnType = ReadTypeReference(property.PropertyType, typeAttributes: property.CustomAttributes);

			p.Getter = ReadMethod(property.GetMethod, parentType, SymbolKind.Accessor, p);
			p.Setter = ReadMethod(property.SetMethod, parentType, SymbolKind.Accessor, p);

			foreach (var par in property.GetIndexParameters ()) {
				p.Parameters.Add(ReadParameter(par));
			}

			AddAttributes(property, p);

			var accessor = p.Getter ?? p.Setter;
			if (accessor != null && accessor.IsExplicitInterfaceImplementation) {
				p.Name = property.Name.Substring(property.Name.LastIndexOf('.') + 1);
				p.IsExplicitInterfaceImplementation = true;
				foreach (var mr in accessor.ExplicitInterfaceImplementations) {
					p.ExplicitInterfaceImplementations.Add(new AccessorOwnerMemberReference(mr));
				}
			}

			FinishReadMember(p, property);
			return p;
		}
		#endregion

		#region Read Event
		[CLSCompliant(false)]
		public IUnresolvedEvent ReadEvent(EventInfo ev, IUnresolvedTypeDefinition parentType)
		{
			if (ev == null)
				throw new ArgumentNullException("ev");
			if (parentType == null)
				throw new ArgumentNullException("parentType");

			DefaultUnresolvedEvent e = new DefaultUnresolvedEvent(parentType, ev.Name);
			TranslateModifiers(ev.AddMethod, e);
			e.ReturnType = ReadTypeReference(ev.EventHandlerType, typeAttributes: ev.CustomAttributes);

			e.AddAccessor    = ReadMethod(ev.AddMethod,    parentType, SymbolKind.Accessor, e);
			e.RemoveAccessor = ReadMethod(ev.RemoveMethod, parentType, SymbolKind.Accessor, e);
			e.InvokeAccessor = ReadMethod(ev.RaiseMethod, parentType, SymbolKind.Accessor, e);

			AddAttributes(ev, e);

			var accessor = e.AddAccessor ?? e.RemoveAccessor ?? e.InvokeAccessor;
			if (accessor != null && accessor.IsExplicitInterfaceImplementation) {
				e.Name = ev.Name.Substring(ev.Name.LastIndexOf('.') + 1);
				e.IsExplicitInterfaceImplementation = true;
				foreach (var mr in accessor.ExplicitInterfaceImplementations) {
					e.ExplicitInterfaceImplementations.Add(new AccessorOwnerMemberReference(mr));
				}
			}

			FinishReadMember(e, ev);

			return e;
		}
		#endregion

		#region FinishReadMember / Interning
		void FinishReadMember(AbstractUnresolvedMember member, MemberInfo ikvmDefinition)
		{
			member.ApplyInterningProvider(interningProvider);
			member.Freeze();
			RegisterCecilObject(member, ikvmDefinition);
		}
		#endregion

		#region Type system translation table
		void RegisterCecilObject(IUnresolvedEntity typeSystemObject, MemberInfo cecilObject)
		{
			if (OnEntityLoaded != null)
				OnEntityLoaded(typeSystemObject, cecilObject);
		}
		#endregion
	}
}
