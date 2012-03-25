// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.MetaData;
using Debugger.Interop.CorDebug;

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
					case EvalState.EvaluatedNoResult:     return null;
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
			
			if (appDomain.Process.Options.SuspendOtherThreads) {
				appDomain.Process.AsyncContinue(DebuggeeStateAction.Keep, new Thread[] { evalThread }, CorDebugThreadState.THREAD_SUSPEND);
			} else {
				appDomain.Process.AsyncContinue(DebuggeeStateAction.Keep, this.Process.UnsuspendedThreads, CorDebugThreadState.THREAD_RUN);
			}
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
		public static Value InvokeMethod(Thread evalThread, DebugMethodInfo method, Value thisValue, Value[] args)
		{
			if (method.BackingField != null) {
				method.Process.TraceMessage("Using backing field for " + method.FullName);
				return Value.GetMemberValue(evalThread, thisValue, method.BackingField, args);
			}
			return AsyncInvokeMethod(evalThread, method, thisValue, args).WaitForResult();
		}
		
		public static Eval AsyncInvokeMethod(Thread evalThread, DebugMethodInfo method, Value thisValue, Value[] args)
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
		static void MethodInvokeStarter(Eval eval, DebugMethodInfo method, Value thisValue, Value[] args)
		{
			List<ICorDebugValue> corArgs = new List<ICorDebugValue>();
			args = args ?? new Value[0];
			if (args.Length != method.ParameterCount) {
				throw new GetValueException("Invalid parameter count");
			}
			if (!method.IsStatic) {
				if (thisValue == null)
					throw new GetValueException("'this' is null");
				if (thisValue.IsNull)
					throw new GetValueException("Null reference");
				// if (!(thisValue.IsObject)) // eg Can evaluate on array
				if (!method.DeclaringType.IsInstanceOfType(thisValue)) {
					throw new GetValueException(
						"Can not evaluate because the object is not of proper type.  " + 
						"Expected: " + method.DeclaringType.FullName + "  Seen: " + thisValue.Type.FullName
					);
				}
				corArgs.Add(thisValue.CorValue);
			}
			for(int i = 0; i < args.Length; i++) {
				Value arg = args[i];
				DebugType paramType = (DebugType)method.GetParameters()[i].ParameterType;
				if (!arg.Type.CanImplicitelyConvertTo(paramType))
					throw new GetValueException("Inncorrect parameter type. Expected " + paramType.ToString());
				// Implicitely convert to correct primitve type
				if (paramType.IsPrimitive && args[i].Type != paramType) {
					object oldPrimVal = arg.PrimitiveValue;
					object newPrimVal = Convert.ChangeType(oldPrimVal, paramType.PrimitiveType);
					// Eval - TODO: Is this dangerous?
					arg = CreateValue(eval.thread, newPrimVal);
				}
				// It is importatnt to pass the parameted in the correct form (boxed/unboxed)
				if (paramType.IsValueType) {
					corArgs.Add(arg.CorGenericValue);
				} else {
					if (args[i].Type.IsValueType) {
						// Eval - TODO: Is this dangerous?
						corArgs.Add(arg.Box(eval.thread).CorValue);
					} else {
						corArgs.Add(arg.CorValue);
					}
				}
			}
			
			ICorDebugType[] genericArgs = ((DebugType)method.DeclaringType).GenericArgumentsAsCorDebugType;
			eval.CorEval2.CallParameterizedFunction(
				method.CorFunction,
				(uint)genericArgs.Length, genericArgs,
				(uint)corArgs.Count, corArgs.ToArray()
			);
		}
		
		public static Value CreateValue(Thread evalThread, object value)
		{
			if (value == null) {
				ICorDebugClass corClass = evalThread.AppDomain.ObjectType.CorType.GetClass();
				ICorDebugEval corEval = evalThread.CorThread.CreateEval();
				ICorDebugValue corValue = corEval.CreateValue((uint)CorElementType.CLASS, corClass);
				return new Value(evalThread.AppDomain, corValue);
			} else if (value is string) {
				return Eval.NewString(evalThread, (string)value);
			} else {
				if (!value.GetType().IsPrimitive)
					throw new DebuggerException("Value must be primitve type.  Seen " + value.GetType());
				Value val = Eval.NewObjectNoConstructor(evalThread, DebugType.CreateFromType(evalThread.AppDomain.Mscorlib, value.GetType()));
				val.SetPrimitiveValue(evalThread, value);
				return val;
			}
		}
		
	    /*
		// The following function create values only for the purpuse of evalutaion
		// They actually do not allocate memory on the managed heap
		// The advantage is that it does not continue the process
	    /// <exception cref="DebuggerException">Can not create string this way</exception>
	    public static Value CreateValue(Process process, object value)
		{
			if (value is string) throw new DebuggerException("Can not create string this way");
			CorElementType corElemType;
			ICorDebugClass corClass = null;
			if (value != null) {
				corElemType = DebugType.TypeNameToCorElementType(value.GetType().FullName);
			} else {
				corElemType = CorElementType.CLASS;
				corClass = DebugType.Create(process, null, typeof(object).FullName).CorType.Class;
			}
			ICorDebugEval corEval = CreateCorEval(process);
			ICorDebugValue corValue = corEval.CreateValue((uint)corElemType, corClass);
			Value v = new Value(process, new Expressions.PrimitiveExpression(value), corValue);
			if (value != null) {
				v.PrimitiveValue = value;
			}
			return v;
		}
		*/
		
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
		
		public static Value NewArray(Thread evalThread, DebugType type, uint length, uint? lowerBound)
		{
			return AsyncNewArray(evalThread, type, length, lowerBound).WaitForResult();
		}
		
		public static Eval AsyncNewArray(Thread evalThread, DebugType type, uint length, uint? lowerBound)
		{
			lowerBound = lowerBound ?? 0;
			return new Eval(
				evalThread,
				"New array: " + type + "[" + length + "]",
				delegate(Eval eval) {
					// Multi-dimensional arrays not supported in .NET 2.0
					eval.CorEval2.NewParameterizedArray(type.CorType, 1, new uint[] { length }, new uint[] { lowerBound.Value });
				}
			);
		}
		
		public static Value NewObject(Thread evalThread, DebugMethodInfo constructor, Value[] constructorArguments)
		{
			return AsyncNewObject(evalThread, constructor, constructorArguments).WaitForResult();
		}
		
		public static Eval AsyncNewObject(Thread evalThread, DebugMethodInfo constructor, Value[] constructorArguments)
		{
			ICorDebugValue[] constructorArgsCorDebug = ValuesAsCorDebug(constructorArguments);
			return new Eval(
				evalThread,
				"New object: " + constructor.FullName,
				delegate(Eval eval) {
					eval.CorEval2.NewParameterizedObject(
						constructor.CorFunction,
						(uint)constructor.DeclaringType.GetGenericArguments().Length,
						((DebugType)constructor.DeclaringType).GenericArgumentsAsCorDebugType,
						(uint)constructorArgsCorDebug.Length,
						constructorArgsCorDebug);
				}
			);
		}
		
		public static Value NewObjectNoConstructor(Thread evalThread, DebugType debugType)
		{
			return AsyncNewObjectNoConstructor(evalThread, debugType).WaitForResult();
		}
		
		public static Eval AsyncNewObjectNoConstructor(Thread evalThread, DebugType debugType)
		{
			return new Eval(
				evalThread,
				"New object: " + debugType.FullName,
				delegate(Eval eval) {
					eval.CorEval2.NewParameterizedObjectNoConstructor(debugType.CorType.GetClass(), (uint)debugType.GetGenericArguments().Length, debugType.GenericArgumentsAsCorDebugType);
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
	}
}
