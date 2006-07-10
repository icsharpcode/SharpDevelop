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
	public abstract partial class Eval 
	{
		NDebugger debugger;
		
		PersistentValue   pValue;
		
		ICorDebugEval     corEval;
		bool              reevaluateAfterDebuggeeStateChange;
		EvalState         evalState = EvalState.WaitingForRequest;
		ICorDebugValue    result;
		DebugeeState      debugeeStateOfResult;
		string            error;
		
		public event EventHandler<EvalEventArgs> EvalStarted;
		public event EventHandler<EvalEventArgs> EvalComplete;
		
		public NDebugger Debugger {
			get {
				return debugger;
			}
		}
		
		public EvalState EvalState {
			get {
				return evalState;
			}
			protected set {
				evalState = value;
				if (Evaluated) {
					debugeeStateOfResult = debugger.DebugeeState;
					OnEvalComplete(new EvalEventArgs(this));
				}
				pValue.NotifyValueChange();
			}
		}
		
		public bool Evaluated {
			get {
				return evalState == EvalState.EvaluatedSuccessfully ||
				       evalState == EvalState.EvaluatedException ||
				       evalState == EvalState.EvaluatedNoResult ||
				       evalState == EvalState.EvaluatedError;
			}
		}
		
		public PersistentValue Result {
			get {
				return pValue;
			}
		}
		
		internal ICorDebugEval CorEval {
			get {
				return corEval;
			}
		}
		
		protected Eval(NDebugger debugger, bool reevaluateAfterDebuggeeStateChange, IExpirable[] dependencies)
		{
			this.debugger = debugger;
			this.reevaluateAfterDebuggeeStateChange = reevaluateAfterDebuggeeStateChange;
			
			pValue = new PersistentValue(debugger,
			                             dependencies,
			                             delegate { return GetCorValue(); });
			
			foreach(IExpirable dependency in dependencies) {
				if (dependency is PersistentValue) {
					((PersistentValue)dependency).ValueChanged += delegate { EvalState = EvalState.WaitingForRequest; };
				}
			}
			if (reevaluateAfterDebuggeeStateChange) {
				debugger.DebuggeeStateChanged += delegate { EvalState = EvalState.WaitingForRequest; };
			}
		}
		
		public static Eval CallFunction(NDebugger debugger, string moduleName, string containgType, string functionName, bool reevaluateAfterDebuggeeStateChange, PersistentValue thisValue, PersistentValue[] args)
		{
			return new CallFunctionEval(debugger, moduleName, containgType, functionName, reevaluateAfterDebuggeeStateChange, thisValue, args);
		}
		
		public static Eval CallFunction(NDebugger debugger, ICorDebugFunction corFunction, bool reevaluateAfterDebuggeeStateChange, PersistentValue thisValue, PersistentValue[] args)
		{
			return new CallFunctionEval(debugger, corFunction, reevaluateAfterDebuggeeStateChange, thisValue, args);
		}
		
		public static Eval NewString(NDebugger debugger, string textToCreate)
		{
			return new NewStringEval(debugger, textToCreate);
		}
		
		ICorDebugValue GetCorValue()
		{
			switch(this.EvalState) {
				case EvalState.WaitingForRequest: ScheduleEvaluation(); goto case EvalState.EvaluationScheduled;
				case EvalState.EvaluationScheduled: throw new CannotGetValueException("Evaluation pending");
				case EvalState.Evaluating: throw new CannotGetValueException("Evaluating...");
				case EvalState.EvaluatedSuccessfully: return result;
				case EvalState.EvaluatedException: return result;
				case EvalState.EvaluatedNoResult: throw new CannotGetValueException("No return value");
				case EvalState.EvaluatedError: throw new CannotGetValueException(error);
				default: throw new DebuggerException("Unknown state");
			}
		}
		
		public void ScheduleEvaluation()
		{
			if (Evaluated || EvalState == EvalState.WaitingForRequest) {
				debugger.AddEval(this);
				debugger.MTA2STA.AsyncCall(delegate {
				                           	if (debugger.IsPaused) debugger.StartEvaluation();
				                           });
				EvalState = EvalState.EvaluationScheduled;
			}
		}
		
		/// <returns>True if setup was successful</returns>
		internal abstract bool SetupEvaluation(Thread targetThread);
		
		public PersistentValue EvaluateNow()
		{
			while (EvalState == EvalState.WaitingForRequest) {
				ScheduleEvaluation();
				debugger.WaitForPause();
				debugger.WaitForPause();
			}
			return Result;
		}
		
		protected void OnError(string msg)
		{
			error = msg;
			EvalState = EvalState.EvaluatedError;
		}
		
		protected virtual void OnEvalStarted(EvalEventArgs e)
		{
			if (EvalStarted != null) {
				EvalStarted(this, e);
			}
		}
		
		protected virtual void OnEvalComplete(EvalEventArgs e)
		{
			if (EvalComplete != null) {
				EvalComplete(this, e);
			}
		}
		
		internal void NotifyEvaluationComplete(bool successful) 
		{
			// Eval result should be ICorDebugHandleValue so it should survive Continue()
			result = corEval.Result;
			
			if (result == null) {
				EvalState = EvalState.EvaluatedNoResult;
			} else {
				if (successful) {
					EvalState = EvalState.EvaluatedSuccessfully;
				} else {
					EvalState = EvalState.EvaluatedException;
				}
			}
		}
	}
}
