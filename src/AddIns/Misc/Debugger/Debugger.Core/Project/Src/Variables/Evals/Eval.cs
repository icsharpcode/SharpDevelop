// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public enum EvalState {WaitingForRequest, EvaluationScheduled, Evaluating, EvaluatedSuccessfully, EvaluatedException, EvaluatedNoResult, EvaluatedError};
	
	/// <summary>
	/// This class holds information about function evaluation.
	/// </summary>
	public class Eval: RemotingObjectBase
	{
		delegate void EvaluationInvoker(ICorDebugEval corEval);
		
		class EvalSetupException: System.Exception
		{
			public EvalSetupException(string msg):base(msg)
			{
			}
		}
		
		Process process;
		Value val;
		string description;
		EvaluationInvoker evaluationInvoker;
		
		EvalState      state = EvalState.WaitingForRequest;
		ICorDebugEval  corEval;
		string         errorMsg;
		ICorDebugValue result;
		
		public Process Process {
			get {
				return process;
			}
		}
		
		public Value Result {
			get {
				return val;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public EvalState State {
			get {
				return state;
			}
		}
		
		public bool Evaluated {
			get {
				return state == EvalState.EvaluatedSuccessfully ||
				       state == EvalState.EvaluatedException ||
				       state == EvalState.EvaluatedNoResult ||
				       state == EvalState.EvaluatedError;
			}
		}
		
		Eval(Process process,
		     string description,
		     IExpirable[] expireDependencies,
		     IMutable[] mutateDependencies,
		     EvaluationInvoker evaluationInvoker)
		{
			this.process = process;
			this.description = description;
			this.val = new Value(process,
			                     expireDependencies,
			                     mutateDependencies,
			                     delegate { return GetCorValue(); });
			this.val.Changed += delegate { SetState(EvalState.WaitingForRequest, null, null); };
			this.evaluationInvoker = evaluationInvoker;
		}
		
		internal bool IsCorEval(ICorDebugEval corEval)
		{
			return this.corEval == corEval;
		}
		
		public ICorDebugValue GetCorValue()
		{
			switch(this.State) {
				case EvalState.WaitingForRequest:     RequestEvaluation(); goto case EvalState.EvaluationScheduled;
				case EvalState.EvaluationScheduled:   throw new CannotGetValueException("Evaluation pending");
				case EvalState.Evaluating:            throw new CannotGetValueException("Evaluating...");
				case EvalState.EvaluatedSuccessfully: return result;
				case EvalState.EvaluatedException:    return result;
				case EvalState.EvaluatedNoResult:     throw new CannotGetValueException("No return value");
				case EvalState.EvaluatedError:        throw new CannotGetValueException(errorMsg);
				default: throw new DebuggerException("Unknown state");
			}
		}
		
		void SetState(EvalState state, string errorMsg, ICorDebugValue result)
		{
			this.state    = state;
			this.errorMsg = errorMsg;
			this.result   = result;
		}
		
		void ChangeState(EvalState state, string errorMsg, ICorDebugValue result)
		{
			SetState(state, errorMsg, result);
			this.Result.NotifyChange();
		}
		
		/// <summary> Synchronously calls a function and returns its return value </summary>
		public static Value InvokeMethod(Process process, System.Type type, string name, Value thisValue, Value[] args)
		{
			return InvokeMethod(MethodInfo.GetFromName(process, type, name, args.Length), thisValue, args);
		}
		
		/// <summary> Synchronously calls a function and returns its return value </summary>
		public static Value InvokeMethod(MethodInfo method, Value thisValue, Value[] args)
		{
			return AsyncInvokeMethod(method, thisValue, args).EvaluateNow();
		}
		
		public static Eval AsyncInvokeMethod(MethodInfo method, Value thisValue, Value[] args)
		{
//			string moduleName = System.IO.Path.GetFileName(type.Assembly.Location);
//			Module module = process.GetModule(moduleName);
//			string containgType = type.FullName;
//			ICorDebugFunction corFunction = module.GetMethod(containgType, functionName, args.Length);
			return new Eval(
				method.Process,
				"Function call: " + method.DeclaringType.Name + "." + method.Name,
				new IExpirable[] {}, // TODO
				new IMutable[] {},
				delegate(ICorDebugEval corEval) { StartMethodInvoke(corEval, method, thisValue, args); }
			);
		}
		
		static void StartMethodInvoke(ICorDebugEval corEval, MethodInfo method, Value thisValue, Value[] args)
		{
			List<ICorDebugValue> corArgs = new List<ICorDebugValue>();
			args = args ?? new Value[0];
			try {
				if (thisValue != null) {
					if (!(thisValue.IsObject)) {
						throw new EvalSetupException("Can not evaluate on a value which is not an object");
					}
					if (!method.DeclaringType.IsInstanceOfType(thisValue)) {
						throw new EvalSetupException("Can not evaluate because the object is not of proper type");
					}
					corArgs.Add(thisValue.SoftReference);
				}
				foreach(Value arg in args) {
					corArgs.Add(arg.SoftReference);
				}
			} catch (CannotGetValueException e) {
				throw new EvalSetupException(e.Message);
			}
			
			corEval.CallFunction(method.CorFunction, (uint)corArgs.Count, corArgs.ToArray());
		}
		
		public static Value NewString(Process process, string textToCreate)
		{
			return AsyncNewString(process, textToCreate).EvaluateNow();
		}
		
		public static Eval AsyncNewString(Process process, string textToCreate)
		{
			return new Eval(
				process,
				"New string: " + textToCreate,
				new IExpirable[] {},
				new IMutable[] {},
				delegate(ICorDebugEval corEval) { corEval.NewString(textToCreate); }
			);
		}
		
		public static Value NewObject(Process process, ICorDebugClass classToCreate)
		{
			return AsyncNewObject(process, classToCreate).EvaluateNow();
		}
		
		public static Eval AsyncNewObject(Process process, ICorDebugClass classToCreate)
		{
			return new Eval(
				process,
				"New object: " + classToCreate.Token,
				new IExpirable[] {},
				new IMutable[] {},
				delegate(ICorDebugEval corEval) { corEval.NewObjectNoConstructor(classToCreate); }
			);
		}
		
		
		public void RequestEvaluation()
		{
			if (Evaluated || State == EvalState.WaitingForRequest) {
				process.ScheduleEval(this);
				process.Debugger.MTA2STA.AsyncCall(delegate { process.StartEvaluation(); });
				ChangeState(EvalState.EvaluationScheduled, null, null);
			}
		}
		
		/// <returns>True if setup was successful</returns>
		internal bool SetupEvaluation(Thread targetThread)
		{
			process.AssertPaused();
			
			try {
				if (targetThread.IsLastFunctionNative) {
					throw new EvalSetupException("Can not evaluate because native frame is on top of stack");
				}
				if (!targetThread.IsAtSafePoint) {
					throw new EvalSetupException("Can not evaluate because thread is not at a safe point");
				}
				
				// TODO: What if this thread is not suitable?
				corEval = targetThread.CorThread.CreateEval();
				
				try {
					evaluationInvoker(corEval);
				} catch (COMException e) {
					if ((uint)e.ErrorCode == 0x80131C26) {
						throw new EvalSetupException("Can not evaluate in optimized code");
					} else if ((uint)e.ErrorCode == 0x80131C28) {
						throw new EvalSetupException("Object is in wrong AppDomain");
					} else if ((uint)e.ErrorCode == 0x8013130A) {
						// Happens on getting of Sytem.Threading.Thread.ManagedThreadId; See SD2-1116
						throw new EvalSetupException("Function does not have IL code");
					}else {
						throw;
					}
				}
				
				ChangeState(EvalState.Evaluating, null, null);
				return true;
			} catch (EvalSetupException e) {
				ChangeState(EvalState.EvaluatedError, e.Message, null);
				return false;
			}
		}
		
		public Value EvaluateNow()
		{
			while (State == EvalState.WaitingForRequest) {
				RequestEvaluation();
				process.WaitForPause();
				process.WaitForPause();
			}
			return this.Result;
		}
		
		internal void NotifyEvaluationComplete(bool successful) 
		{
			// Eval result should be ICorDebugHandleValue so it should survive Continue()
			
			if (corEval.Result == null) {
				ChangeState(EvalState.EvaluatedNoResult, null, null);
			} else {
				if (successful) {
					ChangeState(EvalState.EvaluatedSuccessfully, null, corEval.Result);
				} else {
					ChangeState(EvalState.EvaluatedException, null, corEval.Result);
				}
			}
		}
	}
}
