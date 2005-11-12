// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Interop.CorDebug;

namespace Debugger
{
	public partial class NDebugger
	{
		List<Eval> pendingEvalsCollection = new List<Eval>();
		
		public event EventHandler<EvalEventArgs> EvalAdded;
		public event EventHandler<EvalEventArgs> EvalCompleted;
		public event EventHandler<DebuggerEventArgs> AllEvelsCompleted;
		
		protected virtual void OnEvalAdded(EvalEventArgs e)
		{
			if (EvalAdded != null) {
				EvalAdded(this, e);
			}
		}
		
		protected virtual void OnEvalCompleted(EvalEventArgs e)
		{
			if (EvalCompleted != null) {
				EvalCompleted(this, e);
			}
		}
		
		protected virtual void OnAllEvelsCompleted(DebuggerEventArgs e)
		{
			if (AllEvelsCompleted != null) {
				AllEvelsCompleted(this, e);
			}
		}
		
		public IList<Eval> PendingEvals {
			get {
				return pendingEvalsCollection.AsReadOnly();
			}
		}
		
		internal Eval GetEval(ICorDebugEval corEval) {
			return pendingEvalsCollection.Find(delegate (Eval eval) {return eval.CorEval == corEval;});
		}
		
		/// <summary>
		/// Adds eval to a list of pending evals.
		/// The evaluation of pending evals should be started by calling StartEvaluation in paused stated.
		/// The the debugger will continue until all evals are done and will stop with PausedReason.AllEvalsComplete
		/// </summary>
		internal Eval AddEval(Eval eval)
		{
			pendingEvalsCollection.Add(eval);
			
			eval.EvalComplete += EvalComplete;
			
			OnEvalAdded(new EvalEventArgs(eval));
			
			return eval;
		}
		
		// Removes eval from collection and fires events for the eval
		void EvalComplete(object sender, EvalEventArgs e)
		{
			pendingEvalsCollection.Remove(e.Eval);
			
			OnEvalCompleted(e);
			
			if (pendingEvalsCollection.Count == 0) {
				OnAllEvelsCompleted(new DebuggerEventArgs(this));
			}
		}
		
		// return true if there was eval to setup and it was setup
		internal bool SetupNextEvaluation()
		{
			if (pendingEvalsCollection.Count > 0) {
				return pendingEvalsCollection[0].SetupEvaluation();
			} else {
				return false;
			}
		}
		
		/// <summary>
		/// Starts evaluation of pending evals.
		/// </summary>
		public bool StartEvaluation()
		{
			this.AssertPaused();
			
			// TODO: Investigate other threads, are they alowed to run?
			if (SetupNextEvaluation()) {
				this.Continue();
				return true;
			} else {
				return false;
			}
		}
	}
}
