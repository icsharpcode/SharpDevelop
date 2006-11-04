// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Runtime.InteropServices;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public enum EvalState {WaitingForRequest, EvaluationScheduled, Evaluating, EvaluatedSuccessfully, EvaluatedException, EvaluatedNoResult, EvaluatedError};
	
	/// <summary>
	/// This class holds information about function evaluation.
	/// </summary>
	public abstract partial class Eval: Value
	{
		protected class EvalSetupException: System.Exception
		{
			public EvalSetupException(string msg):base(msg)
			{
			}
		}
		
		protected ICorDebugEval corEval;
		EvalState currentState = EvalState.WaitingForRequest;
		string    currentErrorMsg;
		
		string description = String.Empty;
		
		public EvalState State {
			get {
				return currentState;
			}
		}
		
		public bool Evaluated {
			get {
				return currentState == EvalState.EvaluatedSuccessfully ||
				       currentState == EvalState.EvaluatedException ||
				       currentState == EvalState.EvaluatedNoResult ||
				       currentState == EvalState.EvaluatedError;
			}
		}
		
		internal ICorDebugEval CorEval {
			get {
				return corEval;
			}
		}
		
		public string Description {
			get { return description; }
			set { description = value; }
		}
		
		protected Eval(Process process,
		               IExpirable[] expireDependencies,
		               IMutable[] mutateDependencies)
			:base(process, expireDependencies, mutateDependencies, null)
		{
		}
		
		protected override void ClearCurrentValue()
		{
			currentState = EvalState.WaitingForRequest;
			currentErrorMsg = null;
			base.ClearCurrentValue();
		}
		
		public void SetState(EvalState currentState, string currentErrorMsg, ICorDebugValue currentCorValue)
		{
			ClearCurrentValue();
			this.currentState = currentState;
			this.currentErrorMsg = currentErrorMsg;
			this.currentCorValue = currentCorValue;
			this.currentCorValuePauseSession = process.PauseSession;
			OnChanged(new EvalEventArgs(this));
		}
		
		/// <summary>
		/// Synchronously calls a function and returns its return value
		/// </summary>
		public static Value CallFunction(Process process, Type type, string functionName, Value thisValue, Value[] args)
		{
			string moduleName = System.IO.Path.GetFileName(type.Assembly.Location);
			Module module = process.GetModule(moduleName);
			string containgType = type.FullName;
			ICorDebugFunction corFunction = module.GetMethod(containgType, functionName, args.Length);
			Eval eval = new CallFunctionEval(
				process,
				new IExpirable[] {},
				new IMutable[] {},
				corFunction,
				thisValue,
				args
			);
			eval.Description = "Function call: " + containgType + "." + functionName;
			return eval.EvaluateNow();
		}
		
		/// <summary>
		/// Synchronously creates a new string
		/// </summary>
		public static Value NewString(Process process, string textToCreate)
		{
			Eval eval = new NewStringEval(
				process,
				new IExpirable[] {},
				new IMutable[] {},
				textToCreate
			);
			eval.Description = "New string: " + textToCreate;
			return eval.EvaluateNow();
		}
		
		/// <summary>
		/// Synchronously creates a new object
		/// </summary>
		public static Value NewObject(Process process, ICorDebugClass classToCreate)
		{
			Eval eval = new NewObjectEval(
				process,
				new IExpirable[] {},
				new IMutable[] {},
				classToCreate
			);
			eval.Description = "New object: " + classToCreate.Token;
			return eval.EvaluateNow();
		}
		
		protected override ICorDebugValue RawCorValue {
			get {
				switch(this.State) {
					case EvalState.WaitingForRequest: ScheduleEvaluation(); goto case EvalState.EvaluationScheduled;
					case EvalState.EvaluationScheduled: throw new CannotGetValueException("Evaluation pending");
					case EvalState.Evaluating: throw new CannotGetValueException("Evaluating...");
					case EvalState.EvaluatedSuccessfully: return currentCorValue;
					case EvalState.EvaluatedException: return currentCorValue;
					case EvalState.EvaluatedNoResult: throw new CannotGetValueException("No return value");
					case EvalState.EvaluatedError: throw new CannotGetValueException(currentErrorMsg);
					default: throw new DebuggerException("Unknown state");
				}
			}
		}
		
		public void ScheduleEvaluation()
		{
			if (Evaluated || State == EvalState.WaitingForRequest) {
				process.ScheduleEval(this);
				process.Debugger.MTA2STA.AsyncCall(delegate { process.StartEvaluation(); });
				SetState(EvalState.EvaluationScheduled, null, null);
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
					StartEvaluation();
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
				
				SetState(EvalState.Evaluating, null, null);
				return true;
			} catch (EvalSetupException e) {
				SetState(EvalState.EvaluatedError, e.Message, null);
				return false;
			}
		}
		
		protected abstract void StartEvaluation();
		
		public Value EvaluateNow()
		{
			while (State == EvalState.WaitingForRequest) {
				ScheduleEvaluation();
				process.WaitForPause();
				process.WaitForPause();
			}
			return this;
		}
		
		internal void NotifyEvaluationComplete(bool successful) 
		{
			// Eval result should be ICorDebugHandleValue so it should survive Continue()
			
			if (corEval.Result == null) {
				SetState(EvalState.EvaluatedNoResult, null, null);
			} else {
				if (successful) {
					SetState(EvalState.EvaluatedSuccessfully, null, corEval.Result);
				} else {
					SetState(EvalState.EvaluatedException, null, corEval.Result);
				}
			}
		}
	}
}
