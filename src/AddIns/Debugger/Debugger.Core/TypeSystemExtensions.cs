// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Debugger.Interop.CorDebug;
using Debugger.MetaData;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.Cecil.Metadata;

namespace Debugger
{
	/// <summary>
	/// Cecil-based metadata loader.
	/// </summary>
	public static class TypeSystemExtensions
	{
		#region Module Loading
		static ConditionalWeakTable<IUnresolvedAssembly, ModuleMetadataInfo> weakTable = new ConditionalWeakTable<IUnresolvedAssembly, ModuleMetadataInfo>();
		
		internal class ModuleMetadataInfo
		{
			public readonly Module Module;
			public readonly Mono.Cecil.ModuleDefinition CecilModule;
			Dictionary<IUnresolvedEntity, uint> metadataTokens = new Dictionary<IUnresolvedEntity, uint>();
			Dictionary<uint, IUnresolvedMethod> tokenToMethod = new Dictionary<uint, IUnresolvedMethod>();
			Dictionary<IUnresolvedMember, ITypeReference[]> localVariableTypes = new Dictionary<IUnresolvedMember, ITypeReference[]>();
			readonly CecilLoader typeRefLoader;
			readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
			
			public ModuleMetadataInfo(Module module, Mono.Cecil.ModuleDefinition cecilModule)
			{
				this.Module = module;
				this.CecilModule = cecilModule;
				typeRefLoader = new CecilLoader();
				typeRefLoader.SetCurrentModule(cecilModule);
			}
			
			public void AddMember(IUnresolvedEntity entity, Mono.Cecil.MemberReference cecilObject)
			{
				rwLock.EnterWriteLock();
				try {
					uint token = cecilObject.MetadataToken.ToUInt32();
					metadataTokens[entity] = token;
					
					var cecilMethod = cecilObject as Mono.Cecil.MethodDefinition;
					if (cecilMethod != null) {
						IUnresolvedMethod method = (IUnresolvedMethod)entity;
						tokenToMethod[token] = method;
						if (cecilMethod.HasBody) {
							var locals = cecilMethod.Body.Variables;
							if (locals.Count > 0) {
								localVariableTypes[method] = locals.Select(v => typeRefLoader.ReadTypeReference(v.VariableType)).ToArray();
							}
							if (cecilMethod.RVA != 0) {
								// The method was loaded from image - we can free the memory for the body
								// because Cecil will re-initialize it on demand
								cecilMethod.Body = null;
							}
						}
					}
				} finally {
					rwLock.ExitWriteLock();
				}
			}
			
			public IReadOnlyList<ITypeReference> GetLocalVariableTypes(IUnresolvedMember member)
			{
				rwLock.EnterReadLock();
				try {
					ITypeReference[] result;
					if (localVariableTypes.TryGetValue(member, out result))
						return result;
					else
						return EmptyList<ITypeReference>.Instance;
				} finally {
					rwLock.ExitReadLock();
				}
			}
			
			public uint GetMetadataToken(IUnresolvedEntity entity)
			{
				rwLock.EnterReadLock();
				try {
					return metadataTokens[entity];
				} finally {
					rwLock.ExitReadLock();
				}
			}
			
			public IUnresolvedMethod GetMethodFromToken(uint functionToken)
			{
				rwLock.EnterReadLock();
				try {
					IUnresolvedMethod method;
					if (tokenToMethod.TryGetValue(functionToken, out method))
						return method;
					else
						return null;
				} finally {
					rwLock.ExitReadLock();
				}
			}
		}
		
		// used to prevent Cecil from loading referenced assemblies
		sealed class DummyAssemblyResolver : Mono.Cecil.IAssemblyResolver
		{
			public Mono.Cecil.AssemblyDefinition Resolve(Mono.Cecil.AssemblyNameReference name)
			{
				return null;
			}
			
			public Mono.Cecil.AssemblyDefinition Resolve(string fullName)
			{
				return null;
			}
			
			public Mono.Cecil.AssemblyDefinition Resolve(Mono.Cecil.AssemblyNameReference name, Mono.Cecil.ReaderParameters parameters)
			{
				return null;
			}
			
			public Mono.Cecil.AssemblyDefinition Resolve(string fullName, Mono.Cecil.ReaderParameters parameters)
			{
				return null;
			}
		}
		
		internal static Task<IUnresolvedAssembly> LoadModuleAsync(Module module, ICorDebugModule corModule)
		{
			string name = corModule.GetName();
			if (corModule.IsDynamic() == 1 || corModule.IsInMemory() == 1) {
				var defaultUnresolvedAssembly = new DefaultUnresolvedAssembly(name);
				var defaultUnresolvedTypeDefinition = new DefaultUnresolvedTypeDefinition("UnknownDynamicType");
				var defaultUnresolvedMethod = new DefaultUnresolvedMethod(defaultUnresolvedTypeDefinition, "UnknownMethod");
				var defaultUnresolvedField = new DefaultUnresolvedField(defaultUnresolvedTypeDefinition, "UnknownField");
				defaultUnresolvedTypeDefinition.Members.Add(defaultUnresolvedMethod);
				defaultUnresolvedTypeDefinition.Members.Add(defaultUnresolvedField);
				defaultUnresolvedAssembly.AddTypeDefinition(defaultUnresolvedTypeDefinition);
				weakTable.Add(defaultUnresolvedAssembly, new ModuleMetadataInfo(module, null));
				return Task.FromResult<IUnresolvedAssembly>(defaultUnresolvedAssembly);
			}
			
			//return Task.FromResult(LoadModule(module, name));
			return Task.Run(() => LoadModule(module, name));
		}
		
		static IUnresolvedAssembly LoadModule(Module module, string fileName)
		{
			var param = new Mono.Cecil.ReaderParameters { AssemblyResolver = new DummyAssemblyResolver() };
			var cecilModule = Mono.Cecil.ModuleDefinition.ReadModule(fileName, param);
			
			var moduleMetadataInfo = new ModuleMetadataInfo(module, cecilModule);
			var loader = new CecilLoader();
			loader.IncludeInternalMembers = true;
			loader.LazyLoad = true;
			loader.OnEntityLoaded = moduleMetadataInfo.AddMember;
			
			var asm = loader.LoadModule(cecilModule);
			weakTable.Add(asm, moduleMetadataInfo);
			return asm;
		}
		
		internal static ModuleMetadataInfo GetInfo(IAssembly assembly)
		{
			ModuleMetadataInfo info;
			if (!weakTable.TryGetValue(assembly.UnresolvedAssembly, out info))
				throw new ArgumentException("The assembly was not from the debugger type system");
			return info;
		}
		#endregion
		
		#region GetModule / GetMetadataToken / GetVariableType
		public static Module GetModule(this IAssembly assembly)
		{
			return GetInfo(assembly).Module;
		}
		
		public static IEnumerable<string> GetReferences(this Module module)
		{
			ModuleMetadataInfo info;
			if (!weakTable.TryGetValue(module.UnresolvedAssembly, out info))
				throw new ArgumentException("The assembly was not from the debugger type system");
			if (info.CecilModule == null)
				return EmptyList<string>.Instance;
			return info.CecilModule.AssemblyReferences.Select(r => r.FullName);
		}
		
		public static uint GetMetadataToken(this ITypeDefinition typeDefinition)
		{
			var info = GetInfo(typeDefinition.ParentAssembly);
			return info.GetMetadataToken(typeDefinition.Parts[0]);
		}
		
		public static uint GetMetadataToken(this IField field)
		{
			var info = GetInfo(field.ParentAssembly);
			return info.GetMetadataToken(field.UnresolvedMember);
		}
		
		public static uint GetMetadataToken(this IMethod method)
		{
			var info = GetInfo(method.ParentAssembly);
			return info.GetMetadataToken(method.UnresolvedMember);
		}
		
		public static IType GetLocalVariableType(this IMethod method, int index)
		{
			var info = GetInfo(method.ParentAssembly);
			var variableTypes = info.GetLocalVariableTypes(method.UnresolvedMember);
			return variableTypes[index]
				.Resolve(new SimpleTypeResolveContext(method))
				.AcceptVisitor(method.Substitution);
		}
		#endregion
		
		#region IType -> ICorDebugType
		public static ICorDebugType ToCorDebug(this IType type)
		{
			AppDomain appDomain;
			return ToCorDebug(type, out appDomain);
		}
		
		static ICorDebugType ToCorDebug(IType type, out AppDomain appDomain)
		{
			switch (type.Kind) {
				case TypeKind.Class:
				case TypeKind.Interface:
				case TypeKind.Struct:
				case TypeKind.Delegate:
				case TypeKind.Enum:
				case TypeKind.Module:
				case TypeKind.Void:
					return ConvertTypeDefOrParameterizedType(type, out appDomain);
				case TypeKind.Array:
					{
						var arrayType = (ArrayType)type;
						var elementType = ToCorDebug(arrayType.ElementType, out appDomain);
						return appDomain.CorAppDomain2.GetArrayOrPointerType(
							(uint)(arrayType.Dimensions == 1 ? CorElementType.SZARRAY : CorElementType.ARRAY),
							(uint)arrayType.Dimensions, elementType);
					}
				case TypeKind.Pointer:
				case TypeKind.ByReference:
					{
						var pointerType = (TypeWithElementType)type;
						var elementType = ToCorDebug(pointerType.ElementType, out appDomain);
						return appDomain.CorAppDomain2.GetArrayOrPointerType(
							(uint)(type.Kind == TypeKind.Pointer ? CorElementType.PTR : CorElementType.BYREF),
							0, elementType);
					}
				default:
					throw new System.Exception("Invalid value for TypeKind: " + type.Kind);
			}
		}
		
		static ICorDebugType ConvertTypeDefOrParameterizedType(IType type, out AppDomain appDomain)
		{
			ITypeDefinition typeDef = type.GetDefinition();
			appDomain = GetAppDomain(typeDef.Compilation);
			var info = GetInfo(typeDef.ParentAssembly);
			uint token = info.GetMetadataToken(typeDef.Parts[0]);
			ICorDebugClass corClass = info.Module.CorModule.GetClassFromToken(token);
			List<ICorDebugType> corGenArgs = new List<ICorDebugType>();
			ParameterizedType pt = type as ParameterizedType;
			if (pt != null) {
				foreach (var typeArg in pt.TypeArguments) {
					corGenArgs.Add(typeArg.ToCorDebug());
				}
			}
			return ((ICorDebugClass2)corClass).GetParameterizedType((uint)(type.IsReferenceType == false ? CorElementType.VALUETYPE : CorElementType.CLASS), corGenArgs.ToArray());
		}
		#endregion
		
		#region Compilation
		class DebugCompilation : SimpleCompilation
		{
			public readonly AppDomain AppDomain;
			
			public DebugCompilation(AppDomain appDomain, IUnresolvedAssembly mainAssembly, IEnumerable<IAssemblyReference> assemblyReferences)
				: base(mainAssembly, assemblyReferences)
			{
				this.AppDomain = appDomain;
			}
		}
		
		internal static ICompilation CreateCompilation(AppDomain appDomain, IList<IUnresolvedAssembly> assemblies)
		{
			if (assemblies.Count == 0)
				return new DebugCompilation(appDomain, MinimalCorlib.Instance, Enumerable.Empty<IAssemblyReference>());
			else
				return new DebugCompilation(appDomain, assemblies[0], assemblies.Skip(1));
		}
		
		public static AppDomain GetAppDomain(this ICompilation compilation)
		{
			DebugCompilation dc = compilation as DebugCompilation;
			if (dc != null)
				return dc.AppDomain;
			else
				throw new InvalidOperationException("The compilation is not a debugger type system");
		}
		
		public static IType Import(this ICompilation compilation, ICorDebugType corType)
		{
			return ToTypeReference(corType, GetAppDomain(compilation).Process).Resolve(compilation.TypeResolveContext);
		}
		#endregion
		
		#region ICorDebugType -> IType
		public static ITypeReference ToTypeReference(this ICorDebugType corType, Process process)
		{
			switch ((CorElementType)corType.GetTheType()) {
				case CorElementType.VOID:
					return KnownTypeReference.Void;
				case CorElementType.BOOLEAN:
					return KnownTypeReference.Boolean;
				case CorElementType.CHAR:
					return KnownTypeReference.Char;
				case CorElementType.I1:
					return KnownTypeReference.SByte;
				case CorElementType.U1:
					return KnownTypeReference.Byte;
				case CorElementType.I2:
					return KnownTypeReference.Int16;
				case CorElementType.U2:
					return KnownTypeReference.UInt16;
				case CorElementType.I4:
					return KnownTypeReference.Int32;
				case CorElementType.U4:
					return KnownTypeReference.UInt32;
				case CorElementType.I8:
					return KnownTypeReference.Int64;
				case CorElementType.U8:
					return KnownTypeReference.UInt64;
				case CorElementType.R4:
					return KnownTypeReference.Single;
				case CorElementType.R8:
					return KnownTypeReference.Double;
				case CorElementType.STRING:
					return KnownTypeReference.String;
				case CorElementType.PTR:
					return new PointerTypeReference(corType.GetFirstTypeParameter().ToTypeReference(process));
				case CorElementType.BYREF:
					return new ByReferenceTypeReference(corType.GetFirstTypeParameter().ToTypeReference(process));
				case CorElementType.VALUETYPE:
				case CorElementType.CLASS:
					// Get generic arguments
					List<ITypeReference> genericArguments = new List<ITypeReference>();
					foreach (ICorDebugType t in corType.EnumerateTypeParameters().GetEnumerator()) {
						genericArguments.Add(t.ToTypeReference(process));
					}
					var module = process.GetModule(corType.GetClass().GetModule());
					ITypeReference typeDefinitionReference = ToTypeDefinitionReference(module, corType.GetClass().GetToken());
					if (genericArguments.Count > 0)
						return new ParameterizedTypeReference(typeDefinitionReference, genericArguments);
					else
						return typeDefinitionReference;
				case CorElementType.ARRAY:
					return new ArrayTypeReference(corType.GetFirstTypeParameter().ToTypeReference(process),
					                              (int)corType.GetRank());
				case CorElementType.GENERICINST:
					throw new NotSupportedException();
				case CorElementType.I:
					return KnownTypeReference.IntPtr;
				case CorElementType.U:
					return KnownTypeReference.UIntPtr;
				case CorElementType.OBJECT:
					return KnownTypeReference.Object;
				case CorElementType.SZARRAY:
					return new ArrayTypeReference(corType.GetFirstTypeParameter().ToTypeReference(process));
				case CorElementType.CMOD_REQD:
				case CorElementType.CMOD_OPT:
					return corType.GetFirstTypeParameter().ToTypeReference(process);
				default:
					throw new InvalidOperationException("Invalid value for CorElementType");
			}
		}
		
		static ITypeReference ToTypeDefinitionReference(Module module, uint classToken)
		{
			var props = module.MetaData.GetTypeDefProps(classToken);
			var visibility = (TypeAttributes)props.Flags & TypeAttributes.VisibilityMask;
			if (visibility == TypeAttributes.Public || visibility == TypeAttributes.NotPublic) {
				// top-level type
				int dot = props.Name.LastIndexOf('.');
				int tick = props.Name.LastIndexOf('`');
				string ns = dot > 0 ? props.Name.Substring(0, dot) : string.Empty;
				string name;
				int typeParameterCount;
				if (tick < 0) {
					name = props.Name.Substring(dot + 1);
					typeParameterCount = 0;
				} else {
					name = props.Name.Substring(dot + 1, tick - (dot + 1));
					int.TryParse(props.Name.Substring(tick + 1), out typeParameterCount);
				}
				return new GetClassTypeReference(ns, name, typeParameterCount);
			} else {
				// nested type
				uint enclosingTk = module.MetaData.GetNestedClassProps(classToken).EnclosingClass;
				var declaringTypeReference = ToTypeDefinitionReference(module, enclosingTk);
				int typeParameterCount;
				string name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(props.Name, out typeParameterCount);
				return new NestedTypeReference(declaringTypeReference, name, typeParameterCount);
			}
		}
		#endregion
		
		public static bool IsPrimitiveType(this IType type)
		{
			var def = type.GetDefinition();
			if (def != null) {
				switch (def.KnownTypeCode) {
					case KnownTypeCode.Boolean:
					case KnownTypeCode.Char:
					case KnownTypeCode.SByte:
					case KnownTypeCode.Byte:
					case KnownTypeCode.Int16:
					case KnownTypeCode.UInt16:
					case KnownTypeCode.Int32:
					case KnownTypeCode.UInt32:
					case KnownTypeCode.Int64:
					case KnownTypeCode.UInt64:
					case KnownTypeCode.Single:
					case KnownTypeCode.Double:
						return true;
				}
			}
			return false;
		}
		
		public static bool IsInteger(this IType type)
		{
			var def = type.GetDefinition();
			if (def != null) {
				switch (def.KnownTypeCode) {
					case KnownTypeCode.SByte:
					case KnownTypeCode.Byte:
					case KnownTypeCode.Int16:
					case KnownTypeCode.UInt16:
					case KnownTypeCode.Int32:
					case KnownTypeCode.UInt32:
					case KnownTypeCode.Int64:
					case KnownTypeCode.UInt64:
						return true;
				}
			}
			return false;
		}
		
		public static bool IsKnownType(this IType type, Type knownType)
		{
			var def = type.GetDefinition();
			if (knownType.IsGenericTypeDefinition) {
				return def != null && def.Compilation.FindType(knownType).Equals(def);
			} else {
				return def != null && def.Compilation.FindType(knownType).Equals(type);
			}
		}
		
		public static ICorDebugType[] GetTypeArguments(this IMethod method)
		{
			List<ICorDebugType> typeArgs = new List<ICorDebugType>();
			var substitution = method.Substitution;
			if (substitution.ClassTypeArguments != null)
				typeArgs.AddRange(substitution.ClassTypeArguments.Select(t => t.ToCorDebug()));
			if (substitution.MethodTypeArguments != null)
				typeArgs.AddRange(substitution.MethodTypeArguments.Select(t => t.ToCorDebug()));
			return typeArgs.ToArray();
		}
		
		public static ICorDebugFunction ToCorFunction(this IMethod method)
		{
			Module module = method.ParentAssembly.GetModule();
			return module.CorModule.GetFunctionFromToken(method.GetMetadataToken());
		}
		
		public static bool IsDisplayClass(this IType type)
		{
			if (type.Name.StartsWith("<>", StringComparison.Ordinal) && type.Name.Contains("__DisplayClass")) {
				return true; // Anonymous method or lambda
			}
			
			if (type.GetDefinition() != null && type.GetDefinition().Attributes.Any(a => a.AttributeType.IsKnownType(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)))) {
				if (type.GetAllBaseTypeDefinitions().Any(t => t.FullName == typeof(System.Collections.IEnumerator).FullName)) {
					return true; // yield
				}
				if (type.GetAllBaseTypeDefinitions().Any(t => t.FullName == "System.Runtime.CompilerServices.IAsyncStateMachine")) {
					return true; // async
				}
			}
			
			return false;
		}
		
		public static IMethod Import(this ICompilation compilation, ICorDebugFunction corFunction, List<ICorDebugType> typeArgs)
		{
			IMethod definition = Import(compilation, corFunction);
			if (typeArgs == null || typeArgs.Count == 0) {
				return definition;
			}
			int classTPC = definition.DeclaringTypeDefinition.TypeParameterCount;
			IType[] classTPs = typeArgs.Take(classTPC).Select(t => compilation.Import(t)).ToArray();
			IType[] methodTPs = null;
			if (definition.TypeParameters.Count > 0)
				methodTPs = typeArgs.Skip(classTPC).Select(t => compilation.Import(t)).ToArray();
			return new SpecializedMethod(definition, new TypeParameterSubstitution(classTPs, methodTPs));
		}
		
		public static IMethod Import(this ICompilation compilation, ICorDebugFunction corFunction)
		{
			Module module = compilation.GetAppDomain().Process.GetModule(corFunction.GetModule());
			if (module.IsDynamic || module.IsInMemory) {
				return module.Assembly.GetTypeDefinition("", "UnknownDynamicType").Methods.First();
			}
			var info = GetInfo(module.Assembly);
			uint functionToken = corFunction.GetToken();
			var unresolvedMethod = info.GetMethodFromToken(functionToken);
			if (unresolvedMethod == null) {
				// The type containing this function wasn't loaded yet
				uint classToken = corFunction.GetClass().GetToken();
				var definition = ToTypeDefinitionReference(module, classToken).Resolve(new SimpleTypeResolveContext(module.Assembly)).GetDefinition();
				if (definition == null)
					throw new InvalidOperationException("Could not find class for token " + classToken);
				definition.Methods.ToList(); // enforce loading the methods so that they get added to the dictionary
				unresolvedMethod = info.GetMethodFromToken(functionToken);
				if (unresolvedMethod == null)
					throw new InvalidOperationException("Could not find function with token " + functionToken);
			}
			return unresolvedMethod.Resolve(new SimpleTypeResolveContext(module.Assembly));
		}
		
		public static IField ImportField(this IType declaringType, uint fieldToken)
		{
			var module = declaringType.GetDefinition().ParentAssembly.GetModule();
			if (module.IsDynamic || module.IsInMemory) {
				return module.Assembly.GetTypeDefinition("", "UnknownDynamicType").Fields.First();
			}
			var info = GetInfo(module.Assembly);
			return declaringType.GetFields(f => info.GetMetadataToken(f) == fieldToken, GetMemberOptions.IgnoreInheritedMembers).SingleOrDefault();
		}
	}
}
