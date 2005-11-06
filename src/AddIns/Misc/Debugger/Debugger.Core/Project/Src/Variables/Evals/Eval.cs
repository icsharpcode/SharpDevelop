// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace Debugger
{
	class Eval 
	{
		NDebugger debugger;

		ICorDebugEval     corEval;
		ICorDebugFunction corFunction;
		ICorDebugValue[]  args;
		bool              complete = false;
		
		public event EventHandler<DebuggerEventArgs> EvalComplete;

		protected virtual void OnEvalComplete(DebuggerEventArgs e) 
		{
			if (EvalComplete != null) {
				EvalComplete(this, e);
			}
		}
		
		public Eval(NDebugger debugger, ICorDebugFunction corFunction, ICorDebugValue[] args)
		{
			this.debugger = debugger;
			this.corFunction = corFunction;
			this.args = args;
			debugger.ManagedCallback.CorDebugEvalCompleted += new EventHandler<CorDebugEvalEventArgs>(CorDebugEvalCompletedInManagedCallback);
		}
		
		/// <summary>
		/// Executes the evaluation and doesn't return until value is evaluated.
		/// 
		/// 
		/// </summary>
		public void PerformEval()
		{
			AsyncPerformEval();
			while (!complete) {
				System.Windows.Forms.Application.DoEvents();
			}
		}

		/// <summary>
		/// Executes the evaluation and resumes debugger.
		/// </summary>
		public void AsyncPerformEval()
		{
			debugger.AssertPaused();

			debugger.CurrentThread.CorThread.CreateEval(out corEval);
			
			corEval.CallFunction(corFunction, (uint)args.Length, args);

			debugger.Continue();
		}
		
		public ICorDebugValue GetResult()
		{
			ICorDebugValue corValue;
			corEval.GetResult(out corValue);
			return corValue;
		}
		
		void CorDebugEvalCompletedInManagedCallback(object sender, CorDebugEvalEventArgs e) 
		{
			if (e.CorDebugEval == corEval) {
				complete = true;
				OnEvalComplete(new DebuggerEventArgs(debugger));
			}
		}
	}
}
