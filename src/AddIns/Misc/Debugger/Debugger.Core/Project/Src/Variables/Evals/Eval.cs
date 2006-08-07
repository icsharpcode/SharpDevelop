// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public enum EvalState {WaitingForRequest, EvaluationScheduled, Evaluating, EvaluatedSuccessfully, EvaluatedException, EvaluatedNoResult, EvaluatedError};
	
	/// <summary>
	/// This class holds information about function evaluation.
	/// </summary>
	public abstract partial class Eval: Variable
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
		
		protected Eval(NDebugger debugger, string name, Flags flags, IExpirable[] expireDependencies, IMutable[] mutateDependencies)
			:base(debugger, name, flags, expireDependencies, mutateDependencies, null)
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
			this.currentCorValuePauseSession = debugger.PauseSession;
			OnChanged(new EvalEventArgs(this));
		}
		
		public static Eval CallFunction(NDebugger debugger, Type type, string functionName, Variable thisValue, Variable[] args)
		{
			string moduleName = System.IO.Path.GetFileName(type.Assembly.Location);
			Module module = debugger.GetModule(moduleName);
			string containgType = type.FullName;
			ICorDebugFunction corFunction = module.GetMethod(containgType, functionName, args.Length);
			return new CallFunctionEval(debugger,
			                            "Function call: " + containgType + "." + functionName,
			                            Variable.Flags.Default,
			                            new IExpirable[] {},
			                            new IMutable[] {},
			                            corFunction,
			                            thisValue,
			                            args);
		}
		
		public static Eval NewString(NDebugger debugger, string textToCreate)
		{
			return new NewStringEval(debugger,
			                         "New string: " + textToCreate,
			                         Variable.Flags.Default,
			                         new IExpirable[] {},
			                         new IMutable[] {},
			                         textToCreate);
		}
		
		public static Eval NewObject(NDebugger debugger, ICorDebugClass classToCreate)
		{
			return new NewObjectEval(debugger,
			                         "New object: " + classToCreate.Token,
			                         Variable.Flags.Default,
			                         new IExpirable[] {},
			                         new IMutable[] {},
			                         classToCreate);
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
				debugger.PerformEval(this);
				SetState(EvalState.EvaluationScheduled, null, null);
			}
		}
		
		/// <returns>True if setup was successful</returns>
		internal bool SetupEvaluation(Thread targetThread)
		{
			debugger.AssertPaused();
			
			try {
				if (targetThread.IsLastFunctionNative) {
					throw new EvalSetupException("Can not evaluate because native frame is on top of stack");
				}
				
				// TODO: What if this thread is not suitable?
				corEval = targetThread.CorThread.CreateEval();
				
				try {
					StartEvaluation();
				} catch (COMException e) {
					if ((uint)e.ErrorCode == 0x80131C26) {
						throw new EvalSetupException("Can not evaluate in optimized code");
					} else {
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
		
		public Variable EvaluateNow()
		{
			while (State == EvalState.WaitingForRequest) {
				ScheduleEvaluation();
				debugger.WaitForPause();
				debugger.WaitForPause();
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
