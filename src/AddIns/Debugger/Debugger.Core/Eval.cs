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
using System.Linq;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Debugger.MetaData;
using Debugger.Interop.CorDebug;
using ICSharpCode.NRefactory.TypeSystem;

namespace Debugger
{
	public enum EvalState {
		Evaluating,
		EvaluatedSuccessfully,
		EvaluatedException,
		EvaluatedNoResult,
		EvaluatedTimeOut,
	};
	
	/// <summary>
	/// This class holds information about function evaluation.
	/// </summary>
	public class Eval: DebuggerObject
	{
		delegate void EvalStarter(Eval eval);
		
		AppDomain     appDomain;
		Process       process;
		
		string        description;
		ICorDebugEval corEval;
		Thread        thread;
		Value         result;
		EvalState     state;
		
		public AppDomain AppDomain {
			get { return appDomain; }
		}
		
		public Process Process {
			get { return process; }
		}
		
		public string Description {
			get { return description; }
		}
		
		internal ICorDebugEval CorEval {
			get { return corEval; }
		}
		
		internal ICorDebugEval2 CorEval2 {
			get { return (ICorDebugEval2)corEval; }
		}

		/// <exception cref="GetValueException">Evaluating...</exception>
		public Value Result {
			get {
				switch(this.State) {
						case EvalState.Evaluating:            throw new GetValueException("Evaluating...");
						case EvalState.EvaluatedSuccessfully: return result;
						case EvalState.EvaluatedException:    return result;
						case EvalState.EvaluatedNoResult: 	  throw new GetValueException("no result");
						case EvalState.EvaluatedTimeOut:      throw new GetValueException("Timeout");
						default: throw new DebuggerException("Unknown state");
				}
			}
		}
		
		public EvalState State {
			get { return state; }
		}
		
		public bool Evaluated {
			get {
				return state == EvalState.EvaluatedSuccessfully ||
					state == EvalState.EvaluatedException ||
					state == EvalState.EvaluatedNoResult ||
					state == EvalState.EvaluatedTimeOut;
			}
		}
		
		Eval(Thread evalThread, string description, EvalStarter evalStarter)
		{
			if (evalThread == null)
				throw new DebuggerException("No evaluation thread was provided");
			
			this.appDomain = evalThread.AppDomain;
			this.process = appDomain.Process;
			this.description = description;
			this.state = EvalState.Evaluating;
			this.thread = evalThread;
			
			if (evalThread.Suspended)
				throw new GetValueException("Can not evaluate because thread is suspended");
			if (evalThread.IsInNativeCode)
				throw new GetValueException("Can not evaluate because thread is in native code");
			if (!evalThread.IsAtSafePoint)
				throw new GetValueException("Can not evaluate because thread is not at safe point");
			
			this.corEval = evalThread.CorThread.CreateEval();
			
			try {
				evalStarter(this);
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131C26) {
					throw new GetValueException("Can not evaluate in optimized code");
				} else if ((uint)e.ErrorCode == 0x80131C28) {
					throw new GetValueException("Object is in wrong AppDomain");
				} else if ((uint)e.ErrorCode == 0x8013130A) {
					// Happens on getting of Sytem.Threading.Thread.ManagedThreadId; See SD2-1116
					throw new GetValueException("Function does not have IL code");
				} else if ((uint)e.ErrorCode == 0x80131C23) {
					// The operation failed because it is a GC unsafe point. (Exception from HRESULT: 0x80131C23)
					// This can probably happen when we break and the thread is in native code
					throw new GetValueException("Thread is in GC unsafe point");
				} else if ((uint)e.ErrorCode == 0x80131C22) {
					// The operation is illegal because of a stack overflow.
					throw new GetValueException("Can not evaluate after stack overflow");
				} else if ((uint)e.ErrorCode == 0x80131313) {
					// Func eval cannot work. Bad starting point.
					// Reproduction circumstancess are unknown
					throw new GetValueException("Func eval cannot work. Bad starting point.");
				} else {
					#if DEBUG
					throw; // Expose for more diagnostics
					#else
					throw new GetValueException(e.Message);
					#endif
				}
			}
			
			appDomain.Process.activeEvals.Add(this);
			
			appDomain.Process.AsyncContinue(DebuggeeStateAction.Keep, evalThread);
		}

		/// <exception cref="DebuggerException">Evaluation can not be stopped</exception>
		/// <exception cref="GetValueException">Process exited</exception>
		Value WaitForResult()
		{
			// Note that aborting is not supported for suspended threads
			try {
				process.WaitForPause(TimeSpan.FromMilliseconds(500));
				if (!Evaluated) {
					process.TraceMessage("Aborting eval: " + Description);
					this.CorEval.Abort();
					process.WaitForPause(TimeSpan.FromMilliseconds(2500));
					if (!Evaluated) {
						process.TraceMessage("Rude aborting eval: " + Description);
						this.CorEval2.RudeAbort();
						process.WaitForPause(TimeSpan.FromMilliseconds(5000));
						if (!Evaluated) {
							throw new DebuggerException("Evaluation can not be stopped");
						}
					}
					// Note that this sets Evaluated to true
					state = EvalState.EvaluatedTimeOut;
				}
				process.AssertPaused();
				return this.Result;
			} catch (ProcessExitedException) {
				throw new GetValueException("Process exited");
			}
		}
		
		internal void NotifyEvaluationComplete(bool successful)
		{
			// Eval result should be ICorDebugHandleValue so it should survive Continue()
			if (state == EvalState.EvaluatedTimeOut) {
				return;
			}
			if (corEval.GetResult() == null) {
				state = EvalState.EvaluatedNoResult;
			} else {
				if (successful) {
					state = EvalState.EvaluatedSuccessfully;
				} else {
					state = EvalState.EvaluatedException;
				}
				result = new Value(AppDomain, corEval.GetResult());
			}
		}
		
		/// <summary> Synchronously calls a function and returns its return value </summary>
		public static Value InvokeMethod(Thread evalThread, IMethod method, Value thisValue, Value[] args)
		{
			uint fieldToken = method.GetBackingFieldToken();
			if (fieldToken != 0) {
				var field = method.DeclaringType.ImportField(fieldToken);
				if (field != null) {
					evalThread.Process.TraceMessage("Using backing field for " + method.FullName);
					return Value.GetMemberValue(evalThread, thisValue, field, args);
				}
			}
			return AsyncInvokeMethod(evalThread, method, thisValue, args).WaitForResult();
		}
		
		public static Eval AsyncInvokeMethod(Thread evalThread, IMethod method, Value thisValue, Value[] args)
		{
			return new Eval(
				evalThread,
				"Function call: " + method.FullName,
				delegate(Eval eval) {
					MethodInvokeStarter(eval, method, thisValue, args);
				}
			);
		}

		/// <exception cref="GetValueException"><c>GetValueException</c>.</exception>
		static void MethodInvokeStarter(Eval eval, IMethod method, Value thisValue, Value[] args)
		{
			List<ICorDebugValue> corArgs = new List<ICorDebugValue>();
			args = args ?? new Value[0];
			if (args.Length != method.Parameters.Count) {
				throw new GetValueException("Invalid parameter count");
			}
			if (!method.IsStatic) {
				if (thisValue == null)
					throw new GetValueException("'this' is null");
				if (thisValue.IsNull)
					throw new GetValueException("Null reference");
				corArgs.Add(thisValue.CorValue);
			}
			for(int i = 0; i < args.Length; i++) {
				Value arg = args[i];
				IType paramType = method.Parameters[i].Type;
				if (!arg.IsNull &&
				    arg.Type.GetDefinition() != null &&
				    paramType.GetDefinition() != null &&
				    !arg.Type.GetDefinition().IsDerivedFrom(paramType.GetDefinition())) {
					throw new GetValueException("Incorrect parameter type. Expected " + paramType.ToString());
				}
				// It is importatnt to pass the parameter in the correct form (boxed/unboxed)
				if (paramType.IsReferenceType == true) {
					if (!arg.IsReference)
						throw new DebuggerException("Reference expected as method argument");
					corArgs.Add(arg.CorValue);
				} else {
					corArgs.Add(arg.CorGenericValue); // Unbox
				}
			}
			
			ICorDebugType[] genericArgs = method.GetTypeArguments();
			
			eval.CorEval2.CallParameterizedFunction(
				method.ToCorFunction(),
				(uint)genericArgs.Length, genericArgs,
				(uint)corArgs.Count, corArgs.ToArray()
			);
		}
		
		public static Value CreateValue(Thread evalThread, object value, IType type = null)
		{
			if (value == null) {
				ICorDebugClass corClass = evalThread.AppDomain.ObjectType.ToCorDebug().GetClass();
				ICorDebugEval corEval = evalThread.CorThread.CreateEval();
				ICorDebugValue corValue = corEval.CreateValue((uint)CorElementType.CLASS, corClass);
				return new Value(evalThread.AppDomain, corValue);
			} else if (value is string) {
				return Eval.NewString(evalThread, (string)value);
			} else {
				if (type == null)
					type = evalThread.AppDomain.Compilation.FindType(value.GetType());
				if (!type.IsPrimitiveType() && type.Kind != TypeKind.Enum)
					throw new DebuggerException("Value must be primitve type.  Seen " + value.GetType());
				Value val = Eval.NewObjectNoConstructor(evalThread, type);
				val.SetPrimitiveValue(evalThread, value);
				return val;
			}
		}
		
		public static Value NewString(Thread evalThread, string textToCreate)
		{
			return AsyncNewString(evalThread, textToCreate).WaitForResult();
		}
		
		public static Eval AsyncNewString(Thread evalThread, string textToCreate)
		{
			return new Eval(
				evalThread,
				"New string: " + textToCreate,
				delegate(Eval eval) {
					eval.CorEval2.NewStringWithLength(textToCreate, (uint)textToCreate.Length);
				}
			);
		}
		
		public static Value NewArray(Thread evalThread, IType type, uint length, uint? lowerBound)
		{
			return AsyncNewArray(evalThread, type, length, lowerBound).WaitForResult();
		}
		
		public static Eval AsyncNewArray(Thread evalThread, IType type, uint length, uint? lowerBound)
		{
			lowerBound = lowerBound ?? 0;
			return new Eval(
				evalThread,
				"New array: " + type + "[" + length + "]",
				delegate(Eval eval) {
					// Multi-dimensional arrays not supported in .NET 2.0
					eval.CorEval2.NewParameterizedArray(type.ToCorDebug(), 1, new uint[] { length }, new uint[] { lowerBound.Value });
				}
			);
		}
		
		public static Value NewObject(Thread evalThread, IMethod constructor, Value[] constructorArguments)
		{
			return AsyncNewObject(evalThread, constructor, constructorArguments).WaitForResult();
		}
		
		public static Eval AsyncNewObject(Thread evalThread, IMethod constructor, Value[] constructorArguments)
		{
			ICorDebugType[] typeArgs = constructor.GetTypeArguments();
			ICorDebugValue[] ctorArgs = ValuesAsCorDebug(constructorArguments);
			return new Eval(
				evalThread,
				"New object: " + constructor.FullName,
				delegate(Eval eval) {
					eval.CorEval2.NewParameterizedObject(
						constructor.ToCorFunction(),
						(uint)typeArgs.Length, typeArgs,
						(uint)ctorArgs.Length, ctorArgs);
				}
			);
		}
		
		public static Value NewObjectNoConstructor(Thread evalThread, IType type)
		{
			return AsyncNewObjectNoConstructor(evalThread, type).WaitForResult();
		}
		
		public static Eval AsyncNewObjectNoConstructor(Thread evalThread, IType type)
		{
			ICorDebugType[] typeArgs = new ICorDebugType[0];
			var genType = type as ParameterizedType;
			if (genType != null) {
				typeArgs = genType.TypeArguments.Select(t => t.ToCorDebug()).ToArray();
			}
			return new Eval(
				evalThread,
				"New object: " + type.FullName,
				delegate(Eval eval) {
					eval.CorEval2.NewParameterizedObjectNoConstructor(type.ToCorDebug().GetClass(), (uint)typeArgs.Length, typeArgs);
				}
			);
		}
		
		static ICorDebugValue[] ValuesAsCorDebug(Value[] values)
		{
			ICorDebugValue[] valuesAsCorDebug = new ICorDebugValue[values.Length];
			for(int i = 0; i < values.Length; i++) {
				valuesAsCorDebug[i] = values[i].CorValue;
			}
			return valuesAsCorDebug;
		}
		
		public static Value TypeOf(Thread evalThread, IType type)
		{
			switch (type.Kind) {
				case TypeKind.Unknown:
					throw new GetValueException("Cannot find type '{0}'", type.FullName);
				case TypeKind.Class:
				case TypeKind.Interface:
				case TypeKind.Struct:
				case TypeKind.Delegate:
				case TypeKind.Enum:
				case TypeKind.Module:
				case TypeKind.Void:
					return ConvertTypeDefOrParameterizedType(evalThread, type);
				case TypeKind.Array:
					{
						var arrayType = (ArrayType)type;
						var elementType = TypeOf(evalThread, arrayType.ElementType);
						return InvokeMakeArrayType(evalThread, elementType, arrayType.Dimensions);
					}
				case TypeKind.Pointer:
					{
						var pointerType = (TypeWithElementType)type;
						var elementType = TypeOf(evalThread, pointerType.ElementType);
						return InvokeMakePointerType(evalThread, elementType);
					}
				case TypeKind.ByReference:
					{
						var pointerType = (TypeWithElementType)type;
						var elementType = TypeOf(evalThread, pointerType.ElementType);
						return InvokeMakeByRefType(evalThread, elementType);
					}
				default:
					throw new System.Exception("Invalid value for TypeKind: " + type.Kind);
			}
		}
		
		static Value ConvertTypeDefOrParameterizedType(Thread evalThread, IType type)
		{
			var definition = type.GetDefinition();
			if (definition == null)
				throw new GetValueException("Cannot find type '{0}'", type.FullName);
			var foundType = InvokeGetType(evalThread, new AssemblyQualifiedTypeName(definition));
			ParameterizedType pt = type as ParameterizedType;
			if (pt != null) {
				var typeParams = new List<Value>();
				foreach (var typeArg in pt.TypeArguments) {
					typeParams.Add(TypeOf(evalThread, typeArg));
				}
				return InvokeMakeGenericType(evalThread, foundType, typeParams.ToArray());
			}
			return foundType;
		}
		
		static Value InvokeGetType(Thread evalThread, AssemblyQualifiedTypeName name)
		{
			var sysType = evalThread.AppDomain.Compilation.FindType(KnownTypeCode.Type);
			var getType = sysType.GetMethods(m => m.Name == "GetType" && m.Parameters.Count == 2).FirstOrDefault();
			return InvokeMethod(evalThread, getType, null, new[] { NewString(evalThread, name.ToString()), CreateValue(evalThread, false) });
		}
		
		static Value InvokeMakeArrayType(Thread evalThread, Value type, int rank = 1)
		{
			var sysType = evalThread.AppDomain.Compilation.FindType(KnownTypeCode.Type);
			var makeArrayType = sysType.GetMethods(m => m.Name == "MakeArrayType" && m.Parameters.Count == 1).FirstOrDefault();
			return InvokeMethod(evalThread, makeArrayType, type, new[] { CreateValue(evalThread, rank) });
		}
		
		static Value InvokeMakePointerType(Thread evalThread, Value type)
		{
			var sysType = evalThread.AppDomain.Compilation.FindType(KnownTypeCode.Type);
			var makePointerType = sysType.GetMethods(m => m.Name == "MakePointerType" && m.Parameters.Count == 0).FirstOrDefault();
			return InvokeMethod(evalThread, makePointerType, type, new Value[0]);
		}
		
		static Value InvokeMakeByRefType(Thread evalThread, Value type)
		{
			var sysType = evalThread.AppDomain.Compilation.FindType(KnownTypeCode.Type);
			var makeByRefType = sysType.GetMethods(m => m.Name == "MakeByRefType" && m.Parameters.Count == 0).FirstOrDefault();
			return InvokeMethod(evalThread, makeByRefType, type, new Value[0]);
		}
		
		static Value InvokeMakeGenericType(Thread evalThread, Value type, Value[] typeParams)
		{
			var sysType = evalThread.AppDomain.Compilation.FindType(KnownTypeCode.Type);
			var makeByRefType = sysType.GetMethods(m => m.Name == "MakeGenericType" && m.Parameters.Count == 1).FirstOrDefault();
			var tp = Eval.NewArray(evalThread, sysType, (uint)typeParams.Length, 0);
			for (int i = 0; i < typeParams.Length; i++) {
				tp.SetArrayElement(evalThread, new[] { (uint)i }, typeParams[i]);
			}
			return InvokeMethod(evalThread, makeByRefType, type, new[] { tp });
		}
	}
}
