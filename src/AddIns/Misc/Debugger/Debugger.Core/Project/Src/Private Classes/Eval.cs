// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;

using DebuggerInterop.Core;
using DebuggerInterop.Symbols;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{
	class Eval 
	{
		ICorDebugEval     corEval;
		ICorDebugFunction corFunction;
		ICorDebugValue[]  args;
		bool              complete = false;
		
		public event EventHandler EvalComplete;
		
		void OnEvalComplete()
		{
			if (EvalComplete != null) {
				EvalComplete(this, EventArgs.Empty);
			}
		}		
		
		public Eval(ICorDebugFunction corFunction, ICorDebugValue[] args)
		{
			this.corFunction = corFunction;
			this.args = args;
			NDebugger.ManagedCallback.CorDebugEvalCompleted += new CorDebugEvalEventHandler(CorDebugEvalCompleted);
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
			if (NDebugger.IsProcessRunning) {
				throw new DebuggerException("Debugger must be paused");
			}

			NDebugger.CurrentThread.CorThread.CreateEval(out corEval);
			
			corEval.CallFunction(corFunction, (uint)args.Length, args);
			
			NDebugger.Continue();
		}
		
		public ICorDebugValue GetResult()
		{
			ICorDebugValue corValue;
			corEval.GetResult(out corValue);
			return corValue;
		}
		
		protected virtual void OnEvalComplete(EventArgs e) 
		{
			if (EvalComplete != null) {
				EvalComplete(this, e);
			}
		}
		
		void CorDebugEvalCompleted(object sender, CorDebugEvalEventArgs e) 
		{
			if (e.CorDebugEval == corEval) {
				complete = true;
				OnEvalComplete();
			}
		}
	}
}
