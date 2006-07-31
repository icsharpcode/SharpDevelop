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
		class NewStringEval: Eval
		{
			string textToCreate;
			
			public NewStringEval(NDebugger debugger, string textToCreate):base(debugger, false, new IExpirable[] {})
			{
				this.textToCreate = textToCreate;
			}
			
			internal override bool SetupEvaluation(Thread targetThread)
			{
				debugger.AssertPaused();
				
				if (targetThread.IsLastFunctionNative) {
					OnError("Can not evaluate because native frame is on top of stack");
					return false;
				}
				
				// TODO: What if this thread is not suitable?
				corEval = targetThread.CorThread.CreateEval();
				
				try {
					corEval.NewString(textToCreate);
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
