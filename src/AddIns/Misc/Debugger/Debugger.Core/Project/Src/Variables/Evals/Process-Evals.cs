// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class Process
	{
		List<Eval> activeEvals = new List<Eval>();
		Queue<Eval> pendingEvalsCollection = new Queue<Eval>();
		
		public bool Evaluating {
			get {
				return activeEvals.Count > 0;
			}
		}
		
		internal Eval GetEval(ICorDebugEval corEval)
		{
			return activeEvals.Find(delegate (Eval eval) {return eval.CorEval == corEval;});
		}
		
		/// <summary>
		/// Adds eval to a list of pending evals.
		/// </summary>
		internal void ScheduleEval(Eval eval)
		{
			pendingEvalsCollection.Enqueue(eval);
		}
		
		public void StartEvaluation()
		{
			if (this.IsPaused) {
				if (this.SetupNextEvaluation()) {
					this.Continue();
				}
			}
		}
		
		internal void NotifyEvaluationComplete(Eval eval)
		{
			activeEvals.Remove(eval);
		}
		
		// return true if there was eval to setup and it was setup
		internal bool SetupNextEvaluation()
		{
			this.AssertPaused();
			
			while (pendingEvalsCollection.Count > 0) {
				Eval nextEval = pendingEvalsCollection.Dequeue();
				if (nextEval.SetupEvaluation(this.SelectedThread)) {
					activeEvals.Add(nextEval);
					return true;
				}
			}
			return false;
		}
	}
}
