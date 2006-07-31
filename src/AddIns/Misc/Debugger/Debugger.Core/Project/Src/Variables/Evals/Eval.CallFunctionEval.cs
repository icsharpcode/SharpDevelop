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
	public partial class Eval 
	{
		class CallFunctionEval: Eval
		{
			string moduleName;
			string containgType;
			string functionName;
			ICorDebugFunction corFunction;
			Variable          thisValue;
			Variable[]        args;
			
			public CallFunctionEval(NDebugger debugger, string moduleName, string containgType, string functionName, bool reevaluateAfterDebuggeeStateChange, Variable thisValue, Variable[] args)
				:this(debugger, null, reevaluateAfterDebuggeeStateChange, thisValue, args)
			{
				this.moduleName = moduleName;
				this.containgType = containgType;
				this.functionName = functionName;
			}
			
			public CallFunctionEval(NDebugger debugger, ICorDebugFunction corFunction, bool reevaluateAfterDebuggeeStateChange, Variable thisValue, Variable[] args)
				:base(debugger, reevaluateAfterDebuggeeStateChange, thisValue == null? args : Util.MergeLists(new Variable[] {thisValue}, args).ToArray())
			{
				this.corFunction = corFunction;
				this.thisValue = thisValue;
				this.args = args;
			}
			
			internal override bool SetupEvaluation(Thread targetThread)
			{
				debugger.AssertPaused();
				
				if (targetThread.IsLastFunctionNative) {
					OnError("Can not evaluate because native frame is on top of stack");
					return false;
				}
				
				if (corFunction == null) {
					try {
						Module module = debugger.GetModule(moduleName);
						corFunction = module.GetMethod(containgType, functionName, args.Length);
					} catch (DebuggerException) {
						// Error thrown later on
					}
				}
				
				List<ICorDebugValue> corArgs = new List<ICorDebugValue>();
				try {
					if (thisValue != null) {
						Value val = thisValue.Value;
						if (!(val is ObjectValue)) {
							OnError("Can not evaluate on a value which is not an object");
							return false;
						}
						if (corFunction != null && !((ObjectValue)val).IsInClassHierarchy(corFunction.Class)) {
							OnError("Can not evaluate because the object does not contain specified function");
							return false;
						}
						corArgs.Add(thisValue.SoftReference);
					}
					foreach(Variable arg in args) {
						corArgs.Add(arg.SoftReference);
					}
				} catch (CannotGetValueException e) {
					OnError(e.Message);
					return false;
				}
				
				if (corFunction == null) {
					OnError("Can not find or load function " + functionName);
					return false;
				}
				
				// TODO: What if this thread is not suitable?
				corEval = targetThread.CorThread.CreateEval();
				
				try {
					corEval.CallFunction(corFunction, (uint)corArgs.Count, corArgs.ToArray());
				} catch (COMException e) {
					if ((uint)e.ErrorCode == 0x80131C26) {
						OnError("Can not evaluate in optimized code");
						return false;
					}
				}
				
				EvalState = EvalState.Evaluating;
				
				OnEvalStarted(new EvalEventArgs(this));
				
				return true;
			}
		}
	}
}
