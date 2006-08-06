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
		protected class EvalSetartException: System.Exception
		{
			public EvalSetartException(string msg):base(msg)
			{
			}
		}
		
		protected ICorDebugEval corEval;
		EvalState         evalState = EvalState.WaitingForRequest;
		ICorDebugValue    result;
		DebugeeState      debugeeStateOfResult;
		string            error;
		
		public EvalState EvalState {
			get {
				return evalState;
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
		
		internal ICorDebugEval CorEval {
			get {
				return corEval;
			}
		}
		
		protected Eval(NDebugger debugger, string name, Flags flags, IExpirable[] expireDependencies, IMutable[] mutateDependencies)
			:base(debugger, name, flags, expireDependencies, mutateDependencies, null)
		{
		}
		
		internal override void NotifyChange()
		{
			evalState = EvalState.WaitingForRequest;
			base.NotifyChange();
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
		}
		
		public void ScheduleEvaluation()
		{
			if (Evaluated || EvalState == EvalState.WaitingForRequest) {
				debugger.PerformEval(this);
				evalState = EvalState.EvaluationScheduled;
				OnChanged(new EvalEventArgs(this));
			}
		}
		
		/// <returns>True if setup was successful</returns>
		internal bool SetupEvaluation(Thread targetThread)
		{
			debugger.AssertPaused();
			
			if (targetThread.IsLastFunctionNative) {
				OnError("Can not evaluate because native frame is on top of stack");
			}
			
			// TODO: What if this thread is not suitable?
			corEval = targetThread.CorThread.CreateEval();
			
			try {
				StartEvaluation();
			} catch (EvalSetartException e) {
				OnError(e.Message);
				return false;
			} catch (COMException e) {
				if ((uint)e.ErrorCode == 0x80131C26) {
					OnError("Can not evaluate in optimized code");
					return false;
				} else {
					throw;
				}
			}
			
			evalState = EvalState.Evaluating;
			
			return true;
		}
		
		protected abstract void StartEvaluation();
		
		public Variable EvaluateNow()
		{
			while (EvalState == EvalState.WaitingForRequest) {
				ScheduleEvaluation();
				debugger.WaitForPause();
				debugger.WaitForPause();
			}
			return this;
		}
		
		protected void OnError(string msg)
		{
			error = msg;
			result = null;
			debugeeStateOfResult = debugger.DebugeeState;
			evalState = EvalState.EvaluatedError;
			OnChanged(new EvalEventArgs(this));
		}
		
		internal void NotifyEvaluationComplete(bool successful) 
		{
			error = null;
			// Eval result should be ICorDebugHandleValue so it should survive Continue()
			result = corEval.Result;
			debugeeStateOfResult = debugger.DebugeeState;
			
			if (result == null) {
				evalState = EvalState.EvaluatedNoResult;
			} else {
				if (successful) {
					evalState = EvalState.EvaluatedSuccessfully;
				} else {
					evalState = EvalState.EvaluatedException;
				}
			}
			OnChanged(new EvalEventArgs(this));
		}
	}
}
