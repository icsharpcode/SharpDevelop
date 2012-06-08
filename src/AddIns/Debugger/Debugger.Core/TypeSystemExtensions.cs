// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Debugger.Interop.CorDebug;
using Debugger.MetaData;
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
		static ConditionalWeakTable<IUnresolvedAssembly, ModuleMetadataInfo> weakTable = new ConditionalWeakTable<IUnresolvedAssembly, ModuleMetadataInfo>();
		
		class ModuleMetadataInfo
		{
			public readonly Module Module;
			public Dictionary<IUnresolvedTypeDefinition, uint> MetadataTokens = new Dictionary<IUnresolvedTypeDefinition, uint>();
			
			public ModuleMetadataInfo(Module module)
			{
				this.Module = module;
			}
		}
		
		internal static IUnresolvedAssembly LoadModule(Module module, ICorDebugModule corModule)
		{
			string name = corModule.GetName();
			if (corModule.IsDynamic() == 1 || corModule.IsInMemory() == 1)
				return new DefaultUnresolvedAssembly(name);
			CecilLoader loader = new CecilLoader(true);
			loader.IncludeInternalMembers = true;
			var asm = loader.LoadAssemblyFile(name);
			var moduleMetadataInfo = new ModuleMetadataInfo(module);
			foreach (var typeDef in asm.GetAllTypeDefinitions()) {
				var cecilTypeDef = loader.GetCecilObject(typeDef);
				moduleMetadataInfo.MetadataTokens[typeDef] = cecilTypeDef.MetadataToken.ToUInt32();
			}
			weakTable.Add(asm, moduleMetadataInfo);
			return asm;
		}
		
		static ModuleMetadataInfo GetInfo(IAssembly assembly)
		{
			ModuleMetadataInfo info;
			if (!weakTable.TryGetValue(assembly.UnresolvedAssembly, out info))
				throw new ArgumentException("The assembly was not from the debugger type system");
			return info;
		}
		
		public static Module GetModule(this IAssembly assembly)
		{
			return GetInfo(assembly).Module;
		}
		
		public static ICorDebugType ToCorDebug(this IType type)
		{
			switch (type.Kind) {
				case TypeKind.Class:
				case TypeKind.Interface:
				case TypeKind.Struct:
				case TypeKind.Delegate:
				case TypeKind.Enum:
				case TypeKind.Module:
				case TypeKind.Void:
					return ConvertTypeDefOrParameterizedType(type);
				case TypeKind.Array:
				case TypeKind.Pointer:
				case TypeKind.ByReference:
					throw new NotImplementedException();
				default:
					throw new System.Exception("Invalid value for TypeKind");
			}
		}
		
		static ICorDebugType ConvertTypeDefOrParameterizedType(IType type)
		{
			ITypeDefinition typeDef = type.GetDefinition();
			var info = GetInfo(typeDef.ParentAssembly);
			uint token = info.MetadataTokens[typeDef.Parts[0]];
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
	}
}
