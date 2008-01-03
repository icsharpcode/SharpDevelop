// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.CorSym;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	/// <summary>
	/// Provides information about a method in a class
	/// </summary>
	public class MethodInfo: MemberInfo
	{
		MethodProps methodProps;
		
		/// <summary> Gets the name of this method </summary>
		public override string Name {
			get {
				return methodProps.Name;
			}
		}
		
		/// <summary> Gets name of the method including the full name of the declaring type </summary>
		public string FullName {
			get {
				return this.DeclaringType.FullName + "." + methodProps.Name;
			}
		}
		
		/// <summary> Gets a value indicating whether this method is private </summary>
		public override bool IsPrivate {
			get {
				return !methodProps.IsPublic;
			}
		}
		
		/// <summary> Gets a value indicating whether this method is public </summary>
		public override bool IsPublic {
			get {
				return methodProps.IsPublic;
			}
		}
		
		/// <summary> Gets a value indicating whether the name of this method
		/// is marked as specail.</summary>
		/// <remarks> For example, property accessors are marked as special </remarks>
		public bool IsSpecialName {
			get {
				return methodProps.HasSpecialName;
			}
		}
		
		/// <summary> Gets a value indicating whether this method is static </summary>
		public override bool IsStatic {
			get {
				return methodProps.IsStatic;
			}
		}
		
		/// <summary> Gets the metadata token associated with this method </summary>
		[Debugger.Tests.Ignore]
		public override uint MetadataToken {
			get {
				return methodProps.Token;
			}
		}
		
		internal ICorDebugFunction CorFunction {
			get {
				return this.Module.CorModule.GetFunctionFromToken(this.MetadataToken);
			}
		}
		
		internal MethodInfo(DebugType declaringType, MethodProps methodProps):base (declaringType)
		{
			this.methodProps = methodProps;
		}
		
		/// <summary>
		/// Get a method from a managed type, method name and argument count
		/// </summary>
		public static MethodInfo GetFromName(Process process, System.Type type, string name, int paramCount)
		{
			if (type.IsNested) throw new DebuggerException("Not implemented for nested types");
			if (type.IsGenericType) throw new DebuggerException("Not implemented for generic types");
			if (type.IsGenericParameter) throw new DebuggerException("Type can not be generic parameter");
			
			foreach(Module module in process.Modules) {
				TypeDefProps typeDefProps = module.MetaData.FindTypeDefByName(type.FullName, 0 /* enclosing class for nested */);
				foreach(MethodProps methodProps in module.MetaData.EnumMethodsWithName(typeDefProps.Token, name)) {
					if (module.MetaData.GetParamCount(methodProps.Token) == paramCount) {
						ICorDebugFunction corFunction = module.CorModule.GetFunctionFromToken(methodProps.Token);
						ICorDebugClass2 corClass = corFunction.Class.As<ICorDebugClass2>();
						ICorDebugType corType = corClass.GetParameterizedType(type.IsValueType ? (uint)CorElementType.VALUETYPE : (uint)CorElementType.CLASS,
						                                                      0,
						                                                      new ICorDebugType[] {});
						return new MethodInfo(DebugType.Create(process, corType), methodProps);
					}
				}
			}
			throw new DebuggerException("Not found");
		}
		
		internal ISymUnmanagedMethod SymMethod {
			get {
				if (this.Module.SymReader == null) return null;
				try {
					return this.Module.SymReader.GetMethod(this.MetadataToken);
				} catch {
					return null;
				}
			}
		}
		
		[Debugger.Tests.Ignore]
		public int ParameterCount {
			get {
				return this.Module.MetaData.GetParamCount(this.MetadataToken);
			}
		}
		
		/// <summary> Gets the name of given parameter </summary>
		/// <param name="index"> Zero-based index </param>
		public string GetParameterName(int index)
		{
			// index = 0 is return parameter
			try {
				return this.Module.MetaData.GetParamForMethodIndex(this.MetadataToken, (uint)index + 1).Name;
			} catch {
				return String.Empty;
			}
		}
		
		[Debugger.Tests.Ignore]
		public List<ISymUnmanagedVariable> LocalVariables {
			get {
				if (this.SymMethod != null) { // TODO: Is this needed?
					return GetLocalVariablesInScope(this.SymMethod.RootScope);
				} else {
					return new List<ISymUnmanagedVariable>();
				}
			}
		}
		
		List<ISymUnmanagedVariable> GetLocalVariablesInScope(ISymUnmanagedScope symScope)
		{
			List<ISymUnmanagedVariable> vars = new List<ISymUnmanagedVariable>();
			foreach (ISymUnmanagedVariable symVar in symScope.Locals) {
				if (!symVar.Name.StartsWith("CS$")) { // TODO: Generalize
					vars.Add(symVar);
				}
			}
			foreach(ISymUnmanagedScope childScope in symScope.Children) {
				vars.AddRange(GetLocalVariablesInScope(childScope));
			}
			return vars;
		}
	}
}
