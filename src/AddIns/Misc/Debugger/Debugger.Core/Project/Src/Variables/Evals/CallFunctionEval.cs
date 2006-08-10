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
	class CallFunctionEval: Eval
	{
		ICorDebugFunction corFunction;
		Variable          thisValue;
		Variable[]        args;
		
		public CallFunctionEval(Process process, string name, Flags flags, IExpirable[] expireDependencies, IMutable[] mutateDependencies, ICorDebugFunction corFunction, Variable thisValue, Variable[] args)
			:base(process, name, flags, expireDependencies, mutateDependencies)
		{
			this.corFunction = corFunction;
			this.thisValue = thisValue;
			this.args = args;
		}
		
		protected override void StartEvaluation()
		{
			List<ICorDebugValue> corArgs = new List<ICorDebugValue>();
			try {
				if (thisValue != null) {
					Value val = thisValue.Value;
					if (!(val is ObjectValue)) {
						throw new EvalSetupException("Can not evaluate on a value which is not an object");
					}
					if (!((ObjectValue)val).IsInClassHierarchy(corFunction.Class)) {
						throw new EvalSetupException("Can not evaluate because the object does not contain specified function");
					}
					corArgs.Add(thisValue.SoftReference);
				}
				foreach(Variable arg in args) {
					corArgs.Add(arg.SoftReference);
				}
			} catch (CannotGetValueException e) {
				throw new EvalSetupException(e.Message);
			}
			
			corEval.CallFunction(corFunction, (uint)corArgs.Count, corArgs.ToArray());
		}
	}
}
