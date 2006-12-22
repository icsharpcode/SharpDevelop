// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2023 $</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	/// <summary>
	/// Provides information about a method in a class
	/// </summary>
	public class MethodInfo: MemberInfo
	{
		MethodProps methodProps;
		
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
		public override uint MetadataToken {
			get {
				return methodProps.Token;
			}
		}
		
		/// <summary> Gets the name of this method </summary>
		public override string Name {
			get {
				return methodProps.Name;
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
		
		/// <summary>
		/// Synchronously invoke the method of an a given object
		/// </summary>
		public Value Invoke(Value objectInstance, Value[] arguments)
		{
			return Eval.InvokeMethod(
				this,
				this.IsStatic ? null : objectInstance,
				arguments ?? new Value[0]
			);
		}
		
		/// <summary>
		/// Asynchronously invoke the method of an a given object
		/// </summary>
		public Eval AsyncInvoke(Value objectInstance, Value[] arguments)
		{
			return Eval.AsyncInvokeMethod(
				this,
				this.IsStatic ? null : objectInstance,
				arguments ?? new Value[0]
			);
		}
	}
}
