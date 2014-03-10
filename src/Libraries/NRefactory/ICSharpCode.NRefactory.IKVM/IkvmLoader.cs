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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.Documentation;
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
	public sealed class IkvmLoader : AssemblyLoader
	{
		/// <summary>
		/// Version number of the ikvm loader.
		/// Should be incremented when fixing bugs in the ikvm loader so that project contents cached on disk
		/// (which might be incorrect due to the bug) are re-created.
		/// </summary>
		const int ikvmLoaderVersion = 1;

		#region Options
		// Most options are defined in the AssemblyLoader base class
		
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
			interningProvider = new NonFrozenInterningProvider ();
		}

		#region Load Assembly From Disk

		public override IUnresolvedAssembly LoadAssemblyFile(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");

			using (var universe = new Universe (UniverseOptions.DisablePseudoCustomAttributeRetrieval | UniverseOptions.SupressReferenceTypeIdentityConversion | UniverseOptions.ResolveMissingMembers)) {
				universe.AssemblyResolve += delegate(object sender, IKVM.Reflection.ResolveEventArgs args) {
					return universe.CreateMissingAssembly(args.Name);
				};

				return LoadAssembly (universe.LoadFile (fileName));
			}
		}

		public IUnresolvedAssembly LoadAssemblyFile(string fileName, Stream stream)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");

			using (var universe = new Universe (UniverseOptions.DisablePseudoCustomAttributeRetrieval | UniverseOptions.SupressReferenceTypeIdentityConversion)) {
				universe.AssemblyResolve += delegate(object sender, IKVM.Reflection.ResolveEventArgs args) {
					return universe.CreateMissingAssembly(args.Name);
				};
				using (RawModule module = universe.OpenRawModule (stream, fileName)) {
					return LoadAssembly (universe.LoadAssembly (module));
				}

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

			currentAssemblyDefinition = assembly;
			currentAssembly = new IkvmUnresolvedAssembly (assembly.FullName, DocumentationProvider);

			// Read assembly and module attributes
			IList<IUnresolvedAttribute> assemblyAttributes = new List<IUnresolvedAttribute>();
			IList<IUnresolvedAttribute> moduleAttributes = new List<IUnresolvedAttribute>();
			AddAttributes(assembly, assemblyAttributes);
			AddAttributes(assembly.ManifestModule, moduleAttributes);

			assemblyAttributes = interningProvider.InternList(assemblyAttributes);
			moduleAttributes = interningProvider.InternList(moduleAttributes);

			currentAssembly.Location = assembly.Location;
			currentAssembly.AssemblyAttributes.AddRange(assemblyAttributes);
			currentAssembly.ModuleAttributes.AddRange(moduleAttributes);
			// Register type forwarders:
			foreach (var type in assembly.ManifestModule.__GetExportedTypes ()) {
				if (type.Assembly != assembly) {
					int typeParameterCount;
					string ns = type.Namespace ?? "";
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
						CreateTypeReference (type.GetElementType (),typeAttributes, ref typeIndex))
					);
			}
			if (type.IsPointer) {
				typeIndex++;
				return interningProvider.Intern (
					new PointerTypeReference (
						CreateTypeReference (type.GetElementType (), typeAttributes, ref typeIndex))
					);
			}
			if (type.IsArray) {
				typeIndex++;
				return interningProvider.Intern (
					new ArrayTypeReference (
						CreateTypeReference (type.GetElementType (), typeAttributes, ref typeIndex),
						type.GetArrayRank ()
					)
				);
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
				var c = currentAssembly.GetTypeDefinition (ns, name, typeParameterCount);
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
				var declaringType = a.Constructor.DeclaringType;
				if (declaringType.__Name == "DynamicAttribute" && declaringType.__Namespace == "System.Runtime.CompilerServices") {
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
				assemblyVersion.PositionalArguments.Add(CreateSimpleConstantValue(assembly.GetName ().Version.ToString()));
				outputList.Add(interningProvider.Intern(assemblyVersion));
			}
		}

		IConstantValue CreateSimpleConstantValue(string value)
		{
			return interningProvider.Intern(new StringConstantValue(value));
		}

		IConstantValue CreateSimpleConstantValue(int value)
		{
			return interningProvider.Intern(new IntConstantValue(value));
		}

		IConstantValue CreateSimpleConstantValue(short value)
		{
			return interningProvider.Intern(new ShortConstantValue(value));
		}		

		IConstantValue CreateSimpleConstantValue<T>(ITypeReference type, T value) where T : struct
		{
			return interningProvider.Intern(new StructConstantValue<T>(type, value));
		}

		IConstantValue CreateSimpleConstantValue<T>(ITypeReference type, T? value) where T : struct
		{
			return interningProvider.Intern(new SimpleConstantValue(type, value));
		}

		IConstantValue CreateSimpleConstantValue(ITypeReference type, object value)
		{
			if (ReferenceEquals(value, Missing.Value))
				return CreateSimpleConstantValue(type, null);
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

		void AddAttributes(ParameterInfo parameter, DefaultUnresolvedParameter targetParameter, IEnumerable<CustomAttributeData> customAttributes)
		{
			if (!targetParameter.IsOut) {
				if (parameter.IsIn)
					targetParameter.Attributes.Add(inAttribute);
				if (parameter.IsOut)
					targetParameter.Attributes.Add(outAttribute);
			}
			AddCustomAttributes(customAttributes, targetParameter.Attributes);

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
			if ((methodDefinition.Attributes & MethodAttributes.PinvokeImpl) != 0)
				return true;

			if ((methodDefinition.MethodImplementationFlags & MethodImplAttributes.CodeTypeMask) != 0)
				return true;
			if ((methodDefinition.ReturnParameter.Attributes & ParameterAttributes.HasFieldMarshal) != 0)
				return true;
			return methodDefinition.CustomAttributes.Any () || methodDefinition.ReturnParameter.CustomAttributes.Any ();
		}

		static bool HasAnyAttributes(ConstructorInfo methodDefinition)
		{
			if ((methodDefinition.Attributes & MethodAttributes.PinvokeImpl) != 0)
				return true;

			if ((methodDefinition.MethodImplementationFlags & MethodImplAttributes.CodeTypeMask) != 0)
				return true;
			return methodDefinition.CustomAttributes.Any ();
		}

		void AddAttributes(MethodInfo methodDefinition, IList<IUnresolvedAttribute> attributes, ICollection<IUnresolvedAttribute> returnTypeAttributes)
		{
			var implAttributes = methodDefinition.MethodImplementationFlags;

			#region DllImportAttribute
			if ((methodDefinition.Attributes & MethodAttributes.PinvokeImpl) != 0) {

				ImplMapFlags flags;
				string importName;
				string importScope;
				if (methodDefinition.__TryGetImplMap(out flags, out importName, out importScope)) {
					var dllImport = new DefaultUnresolvedAttribute(dllImportAttributeTypeRef, new[] { KnownTypeReference.String });
					dllImport.PositionalArguments.Add(CreateSimpleConstantValue(importScope));

					
					if ((flags & ImplMapFlags.BestFitOff) != 0)
						dllImport.AddNamedFieldArgument("BestFitMapping", falseValue);
					if ((flags & ImplMapFlags.BestFitOn) != 0)
						dllImport.AddNamedFieldArgument("BestFitMapping", trueValue);

					CallingConvention callingConvention;
					switch (flags & ImplMapFlags.CallConvMask) {
						case (ImplMapFlags)0:
							Debug.WriteLine ("P/Invoke calling convention not set on:" + methodDefinition.Name);
							callingConvention = 0;
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
					if (callingConvention != CallingConvention.Winapi)
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
						dllImport.AddNamedFieldArgument("EntryPoint", CreateSimpleConstantValue(importName));

					if ((flags & ImplMapFlags.NoMangle) != 0)
						dllImport.AddNamedFieldArgument("ExactSpelling", trueValue);

					if ((implAttributes & MethodImplAttributes.PreserveSig) == MethodImplAttributes.PreserveSig)
						implAttributes &= ~MethodImplAttributes.PreserveSig;
					else
						dllImport.AddNamedFieldArgument("PreserveSig", falseValue);

					if ((flags & ImplMapFlags.SupportsLastError) != 0)
						dllImport.AddNamedFieldArgument("SetLastError", trueValue);

					if ((flags & ImplMapFlags.CharMapErrorOff) != 0)
						dllImport.AddNamedFieldArgument("ThrowOnUnmappableChar", falseValue);
					if ((flags & ImplMapFlags.CharMapErrorOn) != 0)
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

			if ((methodDefinition.Attributes & MethodAttributes.HasSecurity) != 0) {
				AddSecurityAttributes(CustomAttributeData.__GetDeclarativeSecurity (methodDefinition), attributes);
			}

			FieldMarshal marshalInfo;
			if (methodDefinition.ReturnParameter.__TryGetFieldMarshal (out marshalInfo)) {
				returnTypeAttributes.Add(ConvertMarshalInfo(marshalInfo));
			}

			AddCustomAttributes(methodDefinition.ReturnParameter.CustomAttributes, returnTypeAttributes);
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

			if ((methodDefinition.Attributes & MethodAttributes.HasSecurity) != 0) {
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
						structLayout.AddNamedFieldArgument("Pack", CreateSimpleConstantValue(packingSize));
					}
					if (typeSize > 0) {
						structLayout.AddNamedFieldArgument("Size", CreateSimpleConstantValue(typeSize));
					}
					targetEntity.Attributes.Add(interningProvider.Intern(structLayout));
				}
			}
			#endregion

			AddCustomAttributes(typeDefinition.CustomAttributes, targetEntity.Attributes);

			if ((typeDefinition.Attributes & TypeAttributes.HasSecurity) != 0) {
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
				fieldOffset.PositionalArguments.Add(CreateSimpleConstantValue(fOffset));
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
				attr.AddNamedFieldArgument("SizeConst", CreateSimpleConstantValue((int)marshalInfo.SizeConst));
				if (marshalInfo.ArraySubType.HasValue)
					attr.AddNamedFieldArgument("ArraySubType", CreateSimpleConstantValue(unmanagedTypeTypeRef, (int)marshalInfo.ArraySubType.Value));
			}

			if (marshalInfo.UnmanagedType == UnmanagedType.SafeArray && marshalInfo.SafeArraySubType.HasValue) {
				attr.AddNamedFieldArgument("SafeArraySubType", CreateSimpleConstantValue(typeof(VarEnum).ToTypeReference(), (int)marshalInfo.SafeArraySubType));
			}

			if (marshalInfo.UnmanagedType == UnmanagedType.LPArray) {
				if (marshalInfo.ArraySubType != null)
					attr.AddNamedFieldArgument("ArraySubType", CreateSimpleConstantValue(unmanagedTypeTypeRef, (int)marshalInfo.ArraySubType));
				if (marshalInfo.SizeConst >= 0)
					attr.AddNamedFieldArgument("SizeConst", CreateSimpleConstantValue((int)marshalInfo.SizeConst));
				if (marshalInfo.SizeParamIndex >= 0)
					attr.AddNamedFieldArgument("SizeParamIndex", CreateSimpleConstantValue((short)marshalInfo.SizeParamIndex));
			}

			if (marshalInfo.UnmanagedType == UnmanagedType.CustomMarshaler) {
				attr.AddNamedFieldArgument("MarshalType", CreateSimpleConstantValue(marshalInfo.MarshalType));
				if (!string.IsNullOrEmpty(marshalInfo.MarshalCookie))
					attr.AddNamedFieldArgument("MarshalCookie", CreateSimpleConstantValue(marshalInfo.MarshalCookie));
			}

			if (marshalInfo.UnmanagedType == UnmanagedType.ByValTStr) {
				attr.AddNamedFieldArgument("SizeConst", CreateSimpleConstantValue((int)marshalInfo.SizeConst));
			}

			return InterningProvider.Intern(attr);
		}
		#endregion

		#region Custom Attributes (ReadAttribute)
		void AddCustomAttributes(IEnumerable<CustomAttributeData> attributes, ICollection<IUnresolvedAttribute> targetCollection)
		{
			foreach (var cecilAttribute in attributes) {
				var type = cecilAttribute.Constructor.DeclaringType;
				if (type.__Namespace == "System.Runtime.CompilerServices") {
					if (type.__Name == "DynamicAttribute" || type.__Name == "ExtensionAttribute" || type.__Name == "DecimalConstantAttribute")
						continue;
				} else if (type.__Name == "ParamArrayAttribute" && type.__Namespace == "System") {
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
			byte[] blob;
			try {
				blob = attribute.__GetBlob ();
			} catch (IKVM.Reflection.MissingMemberException) {
				blob = new byte[0];
			} catch (Exception e) {
				blob = new byte[0];
				Console.Error.WriteLine ("IKVM error while getting blob:" + e);
			}
			return interningProvider.Intern(new UnresolvedAttributeBlob(attributeType, ctorParameterTypes, blob));
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
			RegisterCecilObject(td, typeDefinition);
		}

		void InitBaseTypes(IKVM.Reflection.Type typeDefinition, ICollection<ITypeReference> baseTypes)
		{
			// set base classes
			if (typeDefinition.IsEnum) {
				foreach (var enumField in typeDefinition.__GetDeclaredFields ()) {
					if (!enumField.IsStatic) {
						baseTypes.Add(ReadTypeReference(enumField.FieldType));
						break;
					}
				}
			} else {
				if (typeDefinition.BaseType != null) {
					baseTypes.Add(ReadTypeReference(typeDefinition.BaseType));
				}
				foreach (var iface in typeDefinition.__GetDeclaredInterfaces ()) {
					baseTypes.Add(ReadTypeReference(iface));
				}
			}
		}

		void InitNestedTypes(IKVM.Reflection.Type typeDefinition, IUnresolvedTypeDefinition declaringTypeDefinition, ICollection<IUnresolvedTypeDefinition> nestedTypes)
		{
			foreach (var nestedTypeDef in typeDefinition.__GetDeclaredTypes()) {
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
				var dt = att.Constructor.DeclaringType;
				if (dt.__Name == "StandardModuleAttribute" && dt.__Namespace == "Microsoft.VisualBasic.CompilerServices" 
				    || dt.__Name == "CompilerGlobalScopeAttribute" && dt.__Namespace == "System.Runtime.CompilerServices")
				{
					return true;
				}
			}
			return false;
		}

		void InitMembers(IKVM.Reflection.Type typeDefinition, IUnresolvedTypeDefinition td, IList<IUnresolvedMember> members)
		{
			foreach (var method in typeDefinition.__GetDeclaredMethods ()) {
				if (method.IsConstructor) {
					if (IsVisible(method.Attributes)) {
						SymbolKind type = SymbolKind.Constructor;
						members.Add(ReadConstructor(method, td, type));
					}
					continue;
				}
				if (IsVisible(method.Attributes) && !IsAccessor(method)) {
					SymbolKind type = SymbolKind.Method;
					if (method.IsSpecialName) {
						if (method.Name.StartsWith("op_", StringComparison.Ordinal))
							type = SymbolKind.Operator;
					}
					members.Add(ReadMethod(method, td, type));
				}
			}

			foreach (var field in typeDefinition.__GetDeclaredFields ()) {
				if (IsVisible(field.Attributes) && !field.IsSpecialName) {
					members.Add(ReadField(field, td));
				}
			}

			string defaultMemberName = null;
			var defaultMemberAttribute = typeDefinition.CustomAttributes.FirstOrDefault(
				a => {
					var dt = a.Constructor.DeclaringType;
					return dt.__Name == "DefaultMemberAttribute" && dt.Namespace == "System.Reflection";
				});
			if (defaultMemberAttribute != null && defaultMemberAttribute.ConstructorArguments.Count == 1) {
				defaultMemberName = defaultMemberAttribute.ConstructorArguments[0].Value as string;
			}

			foreach (var property in typeDefinition.__GetDeclaredProperties ()) {
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

			foreach (var ev in typeDefinition.__GetDeclaredEvents ()) {
				if (ev.AddMethod != null && IsVisible(ev.AddMethod.Attributes)) {
					members.Add(ReadEvent(ev, td));
				}
			}
		}

		static bool IsAccessor(MethodBase methodInfo)
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
		public IUnresolvedMethod ReadMethod(MethodBase method, IUnresolvedTypeDefinition parentType, SymbolKind methodType = SymbolKind.Method)
		{
			return ReadMethod((MethodInfo)method, parentType, methodType, null);
		}

		const MethodAttributes nonBodyAttr = MethodAttributes.Abstract | MethodAttributes.PinvokeImpl;
		const MethodImplAttributes nonBodyImplAttr = MethodImplAttributes.InternalCall | MethodImplAttributes.Native | MethodImplAttributes.Unmanaged;

		IUnresolvedMethod ReadMethod(MethodInfo method, IUnresolvedTypeDefinition parentType, SymbolKind methodType, IUnresolvedMember accessorOwner)
		{
			if (method == null)
				return null;
			var m = new DefaultUnresolvedMethod(parentType, method.Name);
			m.SymbolKind = methodType;
			m.AccessorOwner = accessorOwner;
			m.HasBody = (method.Attributes & nonBodyAttr) == 0 && (method.GetMethodImplementationFlags () & nonBodyImplAttr) == 0;
			if (method.IsGenericMethodDefinition) {
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
				var attributeType = attr.Constructor.DeclaringType;
				if (attributeType.__Name == "ExtensionAttribute" && attributeType.__Namespace == "System.Runtime.CompilerServices")
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
					m.IsOverride = (method.Attributes & MethodAttributes.NewSlot) == 0;
				} else if (method.IsFinal) {
					if ((method.Attributes & MethodAttributes.NewSlot) == 0) {
						m.IsSealed = true;
						m.IsOverride = true;
					}
				} else if (method.IsVirtual) {
					if ((method.Attributes & MethodAttributes.NewSlot) != 0)
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
		public IUnresolvedMethod ReadConstructor(MethodBase method, IUnresolvedTypeDefinition parentType, SymbolKind methodType = SymbolKind.Method)
		{
			return ReadConstructor((ConstructorInfo)method, parentType, methodType, null);
		}

		IUnresolvedMethod ReadConstructor(ConstructorInfo method, IUnresolvedTypeDefinition parentType, SymbolKind methodType, IUnresolvedMember accessorOwner)
		{
			if (method == null)
				return null;
			var m = new DefaultUnresolvedMethod(parentType, method.Name);
			m.SymbolKind = methodType;
			m.AccessorOwner = accessorOwner;
			m.HasBody = !method.DeclaringType.IsInterface && (method.GetMethodImplementationFlags () & MethodImplAttributes.CodeTypeMask) == MethodImplAttributes.IL;
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
			var customAttributes = parameter.CustomAttributes;
			var parameterType = parameter.ParameterType;
			var type = ReadTypeReference(parameterType, typeAttributes: customAttributes);
			var p = new DefaultUnresolvedParameter(type, interningProvider.Intern(parameter.Name ?? "index"));

			if (parameterType.IsByRef) {
				if (!parameter.IsIn && parameter.IsOut)
					p.IsOut = true;
				else
					p.IsRef = true;
			}
			AddAttributes(parameter, p, customAttributes);

			if (parameter.IsOptional) {
				p.DefaultValue = CreateSimpleConstantValue(type, parameter.RawDefaultValue);
			}

			if (parameterType.IsArray) {
				foreach (var att in customAttributes) {
					var dt = att.Constructor.DeclaringType;
					if (dt.__Name == "ParamArrayAttribute" && dt.Namespace == "System") {
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

			if ((field.Attributes & FieldAttributes.HasDefault) != 0) {
				f.ConstantValue = CreateSimpleConstantValue(f.ReturnType, field.GetRawConstantValue ());
			}
			else {
				var decConstant = field.CustomAttributes.FirstOrDefault(a => {
					var dt = a.Constructor.DeclaringType;
					return dt.__Name == "DecimalConstantAttribute" && dt.__Namespace == "System.Runtime.CompilerServices";
				});
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
			var attr = g.GenericParameterAttributes;
			if ((attr & GenericParameterAttributes.Contravariant) != 0) {
				tp.Variance = VarianceModifier.Contravariant;
			} else if ((attr & GenericParameterAttributes.Covariant) != 0) {
				tp.Variance = VarianceModifier.Covariant;
			}

			tp.HasReferenceTypeConstraint = (attr & GenericParameterAttributes.ReferenceTypeConstraint) != 0;
			tp.HasValueTypeConstraint = (attr & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0;
			tp.HasDefaultConstructorConstraint = (attr & GenericParameterAttributes.DefaultConstructorConstraint) != 0;

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
